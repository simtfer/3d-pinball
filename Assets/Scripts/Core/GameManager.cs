using DouyinPinball.Pinball;
using DouyinPinball.Platform;
using DouyinPinball.UI;
using UnityEngine;

namespace DouyinPinball.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private BallController ball;
        [SerializeField] private LauncherController launcher;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private HudController hud;
        [SerializeField] private DouyinPlatform douyinPlatform;

        public GameState State { get; private set; } = GameState.Ready;

        private void Awake()
        {
            if (scoreManager == null)
            {
                scoreManager = FindObjectOfType<ScoreManager>();
            }

            if (hud == null)
            {
                hud = FindObjectOfType<HudController>();
            }

            if (douyinPlatform == null)
            {
                douyinPlatform = FindObjectOfType<DouyinPlatform>();
            }
        }

        private void Start()
        {
            SetReady();
        }

        private void Update()
        {
            if (State == GameState.Ready && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
            {
                StartGame();
            }

            if (State == GameState.GameOver && Input.GetKeyDown(KeyCode.Return))
            {
                RestartGame();
            }
        }

        public bool IsPlaying => State == GameState.Playing;

        public void StartGame()
        {
            if (State == GameState.Playing)
            {
                return;
            }

            State = GameState.Playing;
            scoreManager.ResetScore();
            ball.ResetBall();
            launcher.ResetLauncher();
            hud.ShowPlaying();
            douyinPlatform.VibrateShort();
        }

        public void GameOver()
        {
            if (State == GameState.GameOver)
            {
                return;
            }

            State = GameState.GameOver;
            scoreManager.ResetCombo();
            hud.ShowGameOver(scoreManager.Score);
            douyinPlatform.SubmitScore(scoreManager.Score);
            douyinPlatform.VibrateLong();
        }

        public void RestartGame()
        {
            StartGame();
        }

        public void PauseGame()
        {
            if (State != GameState.Playing)
            {
                return;
            }

            State = GameState.Paused;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (State != GameState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            State = GameState.Playing;
        }

        private void SetReady()
        {
            Time.timeScale = 1f;
            State = GameState.Ready;
            scoreManager.ResetScore();
            ball.ResetBall();
            launcher.ResetLauncher();
            hud.ShowReady();
        }
    }
}
