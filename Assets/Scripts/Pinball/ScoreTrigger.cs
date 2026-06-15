using DouyinPinball.Core;
using UnityEngine;

namespace DouyinPinball.Pinball
{
    [RequireComponent(typeof(Collider))]
    public sealed class ScoreTrigger : MonoBehaviour
    {
        [SerializeField] private int scoreValue = 100;
        [SerializeField] private float impulse = 7f;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private ScoreManager scoreManager;

        private void Awake()
        {
            if (scoreManager == null)
            {
                scoreManager = FindObjectOfType<ScoreManager>();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.TryGetComponent(out BallController _))
            {
                return;
            }

            scoreManager.AddScore(scoreValue);
            scoreManager.IncreaseCombo();

            Rigidbody ballBody = collision.rigidbody;
            if (ballBody != null && collision.contactCount > 0)
            {
                Vector3 direction = collision.GetContact(0).normal * -1f;
                ballBody.AddForce(direction.normalized * impulse, ForceMode.Impulse);
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }

            if (hitEffect != null)
            {
                hitEffect.Play();
            }
        }
    }
}
