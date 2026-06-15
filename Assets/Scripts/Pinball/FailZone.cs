using DouyinPinball.Core;
using UnityEngine;

namespace DouyinPinball.Pinball
{
    [RequireComponent(typeof(Collider))]
    public sealed class FailZone : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private void Awake()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BallController _))
            {
                gameManager.GameOver();
            }
        }
    }
}
