using Dissonance.Editor;
using UnityEditor;
using UnityEngine;

namespace Dissonance.Integrations.FMOD_Recording.Editor
{
    [CustomEditor(typeof(FMODMicrophoneInput))]
    public class FMODMicrophoneInputEditor
        : BaseIMicrophoneCaptureEditor<FMODMicrophoneInput>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var capture = (FMODMicrophoneInput)target;

            // The `RuntimeManager` cannot be accessed in edit mode! The mic selector tries to access this
            // to get the list of devices
            if (Application.isPlaying)
                DrawMicSelectorGui(capture);
        }
    }
}