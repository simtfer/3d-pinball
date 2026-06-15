using UnityEngine;

namespace DouyinPinball.Platform
{
    public sealed class DouyinPlatform : MonoBehaviour
    {
        [SerializeField] private bool logCallsInEditor = true;

        public void VibrateShort()
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#else
            LogEditorCall(nameof(VibrateShort));
#endif
        }

        public void VibrateLong()
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#else
            LogEditorCall(nameof(VibrateLong));
#endif
        }

        public void SubmitScore(int score)
        {
            LogEditorCall($"{nameof(SubmitScore)}: {score}");
        }

        public void ShareGame()
        {
            LogEditorCall(nameof(ShareGame));
        }

        private void LogEditorCall(string message)
        {
            if (logCallsInEditor)
            {
                Debug.Log($"[DouyinPlatform] {message}");
            }
        }
    }
}
