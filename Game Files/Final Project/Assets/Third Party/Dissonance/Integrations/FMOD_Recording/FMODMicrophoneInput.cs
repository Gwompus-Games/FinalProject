using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dissonance.Audio.Capture;
using FMOD;
using FMODUnity;
using JetBrains.Annotations;
using NAudio.Wave;
using UnityEngine;

namespace Dissonance.Integrations.FMOD_Recording
{
    public class FMODMicrophoneInput
        : MonoBehaviour, IMicrophoneCapture, IMicrophoneDeviceList
    {
        private static readonly Log Log = Logs.Create(LogCategory.Recording, nameof(FMODMicrophoneInput));

        private readonly float[] _buffer = new float[48000];

        private readonly List<IMicrophoneSubscriber> _subscribers = new List<IMicrophoneSubscriber>();

        private int _deviceID;
        private string _deviceName;
        private WaveFormat _format;
        private Sound _sound;
        private uint _soundLength;
        private uint _lastRecordPos;

        public bool IsRecording { get; private set; }

        public string Device => IsRecording ? _deviceName : null;

        public TimeSpan Latency => TimeSpan.Zero;

        private static bool Check(RESULT result, string message)
        {
            if (result != RESULT.OK)
            {
                Log.Warn(message + $" FMOD Result: {result}");
                return false;
            }

            return true;
        }

        // ReSharper disable once ParameterHidesMember
        public WaveFormat StartCapture(string name)
        {
            // Just in case the device is already running, stop it first. This will only happen if the Start/Stop methods are improperly used.
            StopCapture();

            // Choose an input device, if we can't find one we can't record audio.
            var deviceID = ChooseAudioDevice(name, out _deviceName);
            if (deviceID == null)
                return null;

            // Store the device we're using
            var (deviceSampleRate, deviceChannels) = GetDeviceInfo(deviceID.Value);
            _deviceID = deviceID.Value;
            _format = new WaveFormat(deviceSampleRate, 1);

            // Reset subscribers to prepare them for another stream of data
            lock (_subscribers)
                for (var i = 0; i < _subscribers.Count; i++)
                    _subscribers[i].Reset();

            // Create a sound with a one second buffer
            CREATESOUNDEXINFO exinfo = default;
            exinfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
            exinfo.numchannels = 1;
            exinfo.format = SOUND_FORMAT.PCMFLOAT;
            exinfo.defaultfrequency = deviceSampleRate;
            exinfo.length = (uint)(deviceSampleRate * sizeof(float));
            var sys = RuntimeManager.CoreSystem;
            if (!Check(sys.createSound("recording", MODE.LOOP_NORMAL | MODE.OPENUSER, ref exinfo, out _sound), "Failed to call `createSound`"))
                return null;

            if (!Check(_sound.getLength(out _soundLength, TIMEUNIT.PCM), "Failed to call `getLength`"))
                return null;

            if (!Check(_sound.getFormat(out var st, out var sf, out var sc, out var sb), "Failed to call `getFormat`"))
                return null;
            Log.Debug($"FMOD Recording SoundType:{st} Format:{sf} Channels:{sc} Bits:{sb} freq:{deviceSampleRate}");

            // Start recording into the sound buffer
            if (!Check(sys.recordStart(_deviceID, _sound, true), "Failed to call `recordStart`"))
                return null;
            IsRecording = true;
            _lastRecordPos = 0;

            // Return WaveFormat to indicate that recording has started
            Log.Info("Began mic capture (SampleRate:{0}Hz, ChannelCount:{1}, Device:'{2}')", deviceSampleRate, deviceChannels, _deviceName);
            return _format;
        }

        private static int? ChooseAudioDevice([CanBeNull] string name, out string fullName)
        {
            var sys = RuntimeManager.CoreSystem;
            sys.getRecordNumDrivers(out var driverCount, out _);

            fullName = null;
            if (driverCount == 0)
                return null;

            (int, string)? defaultDevice = default;

            for (var i = 0; i < driverCount; i++)
            {
                if (sys.getRecordDriverInfo(
                        i,
                        out var driverName, 128,
                        out _,
                        out _,
                        out _,
                        out _,
                        out var driverState
                    ) != RESULT.OK)
                {
                    continue;
                }

                // Use the very first device as the default. This will be replaced by the actual default if one is found
                if (i == 0)
                    defaultDevice = (i, driverName);

                // Ignore unconnected devices, we can't record from those!
                if (!driverState.HasFlag(DRIVER_STATE.CONNECTED))
                    continue;

                // Exactly one of the devices will be marked as the default (hopefully).
                if (driverState.HasFlag(DRIVER_STATE.DEFAULT))
                {
                    // Note down which one it is when we find it.
                    defaultDevice = (i, driverName);

                    // A null name means we should use the default device.
                    if (name == null)
                    {
                        fullName = driverName;
                        return i;
                    }
                }

                // Check if this is the device we want (by name)
                if (driverName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    fullName = driverName;
                    return i;
                }
            }

            // We didn't find a suitable device, use the default.
            if (defaultDevice.HasValue)
            {
                fullName = defaultDevice.Value.Item2;
                return defaultDevice.Value.Item1;
            }

            // This should never happen!
            fullName = null;
            return null;
        }

        private static (int rate, int channels) GetDeviceInfo(int id)
        {
            RuntimeManager.CoreSystem.getRecordDriverInfo(
                id,
                out _, 0,
                out _,
                out var systemRate,
                out _,
                out var channels,
                out _
            );

            return (systemRate, channels);
        }

        public void StopCapture()
        {
            if (IsRecording)
                RuntimeManager.CoreSystem.recordStop(_deviceID);

            if (_sound.hasHandle())
                _sound.release();
            _sound = default;
        }

        private void OnDestroy()
        {
            if (_sound.hasHandle())
                _sound.release();
            _sound = default;
        }

        public void Subscribe(IMicrophoneSubscriber listener)
        {
            lock (_subscribers)
            {
                _subscribers.Add(listener);
            }
        }

        public bool Unsubscribe(IMicrophoneSubscriber listener)
        {
            lock (_subscribers)
            {
                return _subscribers.Remove(listener);
            }
        }

        public bool UpdateSubscribers()
        {
            // Returning `true` from this method indicates that reset of the capture pipeline is being requested.

            // If not recording something is wrong!
            if (!IsRecording)
                return true;

            // We think we're recording, does FMOD agree?
            if (RuntimeManager.CoreSystem.isRecording(_deviceID, out var recording) != RESULT.OK || !recording)
                return true;

            // Begin the process of pulling data from FMOD
            var sys = RuntimeManager.CoreSystem;
            if (sys.getRecordPosition(_deviceID, out var recordPosPCM) != RESULT.OK)
                return true;

            // Calculate how many samples are available. Accounting for wraparound.
            var recordDeltaPCM = recordPosPCM >= _lastRecordPos ? recordPosPCM - _lastRecordPos : recordPosPCM + _soundLength - _lastRecordPos;
            if (recordDeltaPCM == 0)
                return false;

            // Lock the buffer, getting back one or two pointers (the bits of the buffer that are relevant to us)
            if (_sound.@lock(_lastRecordPos * sizeof(float), recordDeltaPCM * sizeof(float), out var ptr1, out var ptr2, out var ptr1BytesLength, out var ptr2BytesLength) != RESULT.OK)
                return true;
            _lastRecordPos = recordPosPCM;

            bool finallyOk;
            try
            {
                if (ReadSamples(ptr1, ptr1BytesLength))
                    return true;
                if (ReadSamples(ptr2, ptr2BytesLength))
                    return true;
            }
            finally
            {
                finallyOk = _sound.unlock(ptr1, ptr2, ptr1BytesLength, ptr2BytesLength) == RESULT.OK;
            }

            // Cannot return inside a finally block, do it here instead
            return !finallyOk;
        }

        /// <summary>
        /// Read audio data from a pointer obtained to `Sound.lock`
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="ptrBytesLength"></param>
        /// <returns></returns>
        private bool ReadSamples(IntPtr ptr, uint ptrBytesLength)
        {
            if (ptrBytesLength == 0)
                return false;

            // This shouldn't ever happen because the buffer is massive (one full second).
            // If it does happen request a reset because everything must be horribly broken.
            var ptrSamples = ptrBytesLength / sizeof(float);
            if (ptrSamples > _buffer.Length)
            {
                Log.Error("Insufficient buffer space to pump microphone");
                return true;
            }

            unsafe
            {
                // Copy out data from the pointer
                fixed (float* bufferPtr = &_buffer[0])
                {
                    Buffer.MemoryCopy(ptr.ToPointer(), bufferPtr, sizeof(float) * _buffer.Length, ptrBytesLength);
                }
            }

            // Send the audio out to subscribers
            SendToSubscribers(new ArraySegment<float>(_buffer, 0, (int)ptrSamples));

            return false;
        }

        private void SendToSubscribers(ArraySegment<float> data)
        {
            lock (_subscribers)
                for (var i = 0; i < _subscribers.Count; i++)
                    _subscribers[i].ReceiveMicrophoneData(data, _format);
        }

        void IMicrophoneDeviceList.GetDevices(List<string> output)
        {
            GetDevices(output);
        }

        public static void GetDevices(List<string> output)
        {
            output.Clear();

            // The `RuntimeManager` cannot be accessed in edit mode! Just clear the list and
            // immediately return in edit mode.
            if (Application.isPlaying)
            {
                var sys = RuntimeManager.CoreSystem;
                sys.getRecordNumDrivers(out var driverCount, out _);

                if (driverCount == 0)
                    return;

                for (var i = 0; i < driverCount; i++)
                {
                    sys.getRecordDriverInfo(
                        i,
                        out var driverName, 128,
                        out _,
                        out _,
                        out _,
                        out _,
                        out var driverState
                    );

                    if (!driverState.HasFlag(DRIVER_STATE.CONNECTED))
                        continue;

                    if (driverState.HasFlag(DRIVER_STATE.DEFAULT))
                        output.Insert(0, driverName);
                    else
                        output.Add(driverName);
                }
            }
        }
    }
}
