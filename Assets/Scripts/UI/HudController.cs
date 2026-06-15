using DouyinPinball.Core;
using DouyinPinball.Pinball;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DouyinPinball.UI
{
    public sealed class HudController : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text comboText;
        [SerializeField] private Text messageText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button launchButton;
        [SerializeField] private LauncherController launcher;
        [SerializeField] private ScoreManager scoreManager;

        private void Awake()
        {
            if (scoreManager == null)
            {
                scoreManager = FindObjectOfType<ScoreManager>();
            }

            scoreManager.ScoreChanged += UpdateScore;
            scoreManager.ComboChanged += UpdateCombo;

            WireLaunchButton();
        }

        private void OnDestroy()
        {
            if (scoreManager == null)
            {
                return;
            }

            scoreManager.ScoreChanged -= UpdateScore;
            scoreManager.ComboChanged -= UpdateCombo;
        }

        public void ShowReady()
        {
            SetMessage("3D Pinball");
            startButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(false);
            launchButton.gameObject.SetActive(true);
            UpdateScore(scoreManager.Score);
            UpdateCombo(scoreManager.Combo);
        }

        public void ShowPlaying()
        {
            SetMessage(string.Empty);
            startButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            launchButton.gameObject.SetActive(true);

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void ShowGameOver(int score)
        {
            SetMessage($"Game Over\nScore {score}");
            startButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(true);
            launchButton.gameObject.SetActive(false);
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        private void UpdateCombo(int combo)
        {
            comboText.text = $"Combo x{combo}";
        }

        private void SetMessage(string message)
        {
            messageText.text = message;
        }

        private void WireLaunchButton()
        {
            if (launchButton == null || launcher == null)
            {
                return;
            }

            EventTrigger trigger = launchButton.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = launchButton.gameObject.AddComponent<EventTrigger>();
            }

            AddTrigger(trigger, EventTriggerType.PointerDown, _ => launcher.BeginCharge());
            AddTrigger(trigger, EventTriggerType.PointerUp, _ => launcher.Release());
        }

        private static void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }
    }
}
