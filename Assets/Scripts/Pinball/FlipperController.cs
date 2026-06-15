using DouyinPinball.Core;
using UnityEngine;

namespace DouyinPinball.Pinball
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FlipperController : MonoBehaviour
    {
        [SerializeField] private bool leftFlipper = true;
        [SerializeField] private KeyCode keyboardKey = KeyCode.A;
        [SerializeField] private float restAngle = 0f;
        [SerializeField] private float pressedAngle = 45f;
        [SerializeField] private float spring = 12000f;
        [SerializeField] private float damper = 180f;
        [SerializeField] private GameManager gameManager;

        private HingeJoint hingeJoint;
        private bool pointerHeld;

        private void Awake()
        {
            hingeJoint = GetComponent<HingeJoint>();

            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            ConfigureJoint();
        }

        private void Update()
        {
            if (gameManager == null || !gameManager.IsPlaying)
            {
                SetPressed(false);
                return;
            }

            bool keyboardHeld = Input.GetKey(keyboardKey)
                || (leftFlipper && Input.GetKey(KeyCode.LeftArrow))
                || (!leftFlipper && Input.GetKey(KeyCode.RightArrow));

            SetPressed(pointerHeld || keyboardHeld || IsTouchHeldOnSide());
        }

        public void SetPointerHeld(bool held)
        {
            pointerHeld = held;
        }

        private void ConfigureJoint()
        {
            if (hingeJoint == null)
            {
                return;
            }

            hingeJoint.useSpring = true;
            JointSpring jointSpring = hingeJoint.spring;
            jointSpring.spring = spring;
            jointSpring.damper = damper;
            hingeJoint.spring = jointSpring;
        }

        private void SetPressed(bool pressed)
        {
            if (hingeJoint == null)
            {
                return;
            }

            JointSpring jointSpring = hingeJoint.spring;
            float angle = pressed ? pressedAngle : restAngle;
            jointSpring.targetPosition = leftFlipper ? angle : -angle;
            hingeJoint.spring = jointSpring;
        }

        private bool IsTouchHeldOnSide()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    continue;
                }

                bool onLeft = touch.position.x < Screen.width * 0.5f;
                if (onLeft == leftFlipper)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
