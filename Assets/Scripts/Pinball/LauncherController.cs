using DouyinPinball.Core;
using UnityEngine;

namespace DouyinPinball.Pinball
{
    public sealed class LauncherController : MonoBehaviour
    {
        [SerializeField] private Rigidbody ball;
        [SerializeField] private Transform launchDirection;
        [SerializeField] private KeyCode keyboardKey = KeyCode.Space;
        [SerializeField] private float minImpulse = 6f;
        [SerializeField] private float maxImpulse = 18f;
        [SerializeField] private float chargePerSecond = 14f;
        [SerializeField] private bool logLaunches;
        [SerializeField] private GameManager gameManager;

        private float charge;
        private bool charging;

        public float Charge01 => Mathf.InverseLerp(minImpulse, maxImpulse, charge);

        private void Awake()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            ResetLauncher();
        }

        private void Update()
        {
            if (gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            if (Input.GetKey(keyboardKey))
            {
                if (!charging)
                {
                    BeginCharge();
                }

                ContinueCharge();
            }

            if (Input.GetKeyUp(keyboardKey))
            {
                Release();
            }
        }

        public void BeginCharge()
        {
            charging = true;
            charge = minImpulse;
        }

        public void ContinueCharge()
        {
            if (!charging)
            {
                return;
            }

            charge = Mathf.Min(maxImpulse, charge + chargePerSecond * Time.deltaTime);
        }

        public void Release()
        {
            if (ball == null)
            {
                ResetLauncher();
                return;
            }

            if (!charging)
            {
                charge = minImpulse;
            }

            Vector3 direction = launchDirection != null ? launchDirection.forward : transform.forward;
            ball.WakeUp();
            ball.AddForce(direction.normalized * charge, ForceMode.Impulse);

            if (logLaunches)
            {
                Debug.Log($"Launcher released with impulse {charge:0.00} toward {direction.normalized}.");
            }

            ResetLauncher();
        }

        public void ResetLauncher()
        {
            charging = false;
            charge = minImpulse;
        }
    }
}
