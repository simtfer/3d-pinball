using DouyinPinball.Core;
using UnityEngine;

namespace DouyinPinball.Pinball
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float minSpeedWhilePlaying = 1.5f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float failZ = -5.4f;
        [SerializeField] private Vector3 tableGravity = new Vector3(0f, 0f, -7f);
        [SerializeField] private GameManager gameManager;

        private Rigidbody ballRigidbody;

        private void Awake()
        {
            ballRigidbody = GetComponent<Rigidbody>();

            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
        }

        private void FixedUpdate()
        {
            if (gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            ballRigidbody.AddForce(tableGravity, ForceMode.Acceleration);
            ClampVelocity();

            if (transform.position.z < failZ)
            {
                gameManager.GameOver();
            }
        }

        public void ResetBall()
        {
            Transform target = spawnPoint != null ? spawnPoint : transform;
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            ballRigidbody.position = target.position;
            ballRigidbody.rotation = Quaternion.identity;
        }

        private void ClampVelocity()
        {
            Vector3 velocity = ballRigidbody.velocity;

            if (velocity.magnitude > maxSpeed)
            {
                ballRigidbody.velocity = velocity.normalized * maxSpeed;
                return;
            }

            if (velocity.sqrMagnitude > 0.01f && velocity.magnitude < minSpeedWhilePlaying)
            {
                ballRigidbody.velocity = velocity.normalized * minSpeedWhilePlaying;
            }
        }
    }
}
