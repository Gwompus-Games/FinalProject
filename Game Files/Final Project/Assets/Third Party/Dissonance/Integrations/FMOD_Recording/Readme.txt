Dissonance FMOD Integration
===========================

This package integrates Dissonance Voice Chat (https://assetstore.unity.com/packages/tools/audio/dissonance-voice-chat-70078?aid=1100lJDF) with
the FMOD audio system (https://assetstore.unity.com/packages/tools/audio/fmod-for-unity-161631?aid=1100lJDF) to replace the standard Unity
`Microphone` class with the FMOD microphone system.

To use this package you must have purchased and installed Dissonance and FMOD - **you will encounter build errors until you have done this**!

Setup (Recording)
=================

Add an FMODMicrophoneInput script to the same object as the DissonanceComms script. Dissonance will now use FMOD to record audio.