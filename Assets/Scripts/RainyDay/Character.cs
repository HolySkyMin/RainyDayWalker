using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace RainyDay
{
    public class Character : MonoBehaviour
    {
        [System.Serializable]
        public class FloodRateChangedEvent : UnityEvent<float> { }

        public string CurrentState => _stateMachine.CurrentState.Name;

        public float FloodRate => _floodRate;

        public Vector2 LookDirection => new Vector2(
            Mathf.Cos((transform.eulerAngles.y - 90) * Mathf.Deg2Rad),
            -Mathf.Sin((transform.eulerAngles.y - 90) * Mathf.Deg2Rad));

        public event System.Action ActionKeyPressed;

        public event System.Action ActionKeyReleased;

        public event System.Action JumpFinished;

        [Header("Player")]
        [SerializeField, Tooltip("Sprint speed of the character in m/s")] float sprintSpeed = 5.333f;
        [SerializeField, Tooltip("Acceleration and deceleration")] float speedChangeRate = 10f;
        [SerializeField, Tooltip("The character uses its own gravity value. The engine default is -9.81f")] float gravity = -15f;
        [SerializeField, Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")] float fallTimeout = 0.15f;
        [Header("Player Grounded")]
        [SerializeField, Tooltip("Useful for rough ground")] float groundedOffset = -0.14f;
        [SerializeField, Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")] float groundedRadius = 0.28f;
        [SerializeField, Tooltip("What layers the character uses as ground")] LayerMask groundLayers;
        [Header("Cinemachine")]
        [SerializeField] GameObject cinemachineCameraTarget;
        [SerializeField] float topClamp = 70f;
        [SerializeField] float bottomClamp = -30f;
        [SerializeField] float cameraAngleOverride;
        [Header("Rainy Day Walker")]
        [SerializeField] GameObject jumpIndicator;
        [SerializeField] GameObject indicatorMaxRange, indicatorPoint;
        [SerializeField] FloodRateChangedEvent onFloodRateChange;

        // cinemachine
        float _cinemachineTargetYaw, _cinemachineTargetPitch;

        // animation IDs
        int _animIDSpeed, _animIDGrounded, _animIDJump, _animIDFreeFall, _animIDMotionSpeed;

        float _fallTimeoutDelta, _nextJumpAngle, _nextJumpRange;
        bool _hasAnimator;
        Vector3 _moveDirection = Vector3.zero;
        Animator _animator;
        CharacterController _controller;
        StateMachine _stateMachine;

        // 게임 로직
        float _floodRate, _waterClock;
        bool _isOnWater;

        #region States

        public class IdleState : State<Character>
        {
            public IdleState(Character character) : base("Idle", character._stateMachine, character) { }

            public override void OnEnter()
            {
                Parent._waterClock = 0;
                Parent._isOnWater = Parent.IsStandingOnWater();

                Parent.ActionKeyPressed += OnActionKeyPress;
            }

            public override void OnExit()
            {
                Parent.ActionKeyPressed -= OnActionKeyPress;
            }

            public void OnActionKeyPress()
            {
                Machine.ChangeState("Jump Angle Select");
            }
        }

        public class JumpAngleSelectState : State<Character>
        {
            public JumpAngleSelectState(Character character) : base("Jump Angle Select", character._stateMachine, character) { }

            public override void OnEnter()
            {
                Parent.jumpIndicator.transform.SetParent(null);
                Parent.jumpIndicator.transform.position = new Vector3(
                    Parent.transform.position.x,
                    0f,
                    Parent.transform.position.z
                );
                Parent.jumpIndicator.transform.eulerAngles = Parent.transform.eulerAngles - new Vector3(0, 90, 0);
                Parent.jumpIndicator.SetActive(true);
                Parent.indicatorMaxRange.SetActive(true);

                Parent.ActionKeyPressed += OnActionKeyPress;
            }

            public override void Update()
            {
                Parent.jumpIndicator.transform.RotateAround(Parent.transform.position, Vector3.up, 360 * Time.deltaTime);
            }

            public override void OnExit()
            {
                Parent.ActionKeyPressed -= OnActionKeyPress;

                Parent._nextJumpAngle = Parent.jumpIndicator.transform.eulerAngles.y + 90;
                Parent.indicatorMaxRange.SetActive(false);
            }

            public void OnActionKeyPress()
            {
                Machine.ChangeState("Jump Range Select");
            }
        }

        public class JumpRangeSelectState : State<Character>
        {
            float _clock;
            bool _reverse;

            public JumpRangeSelectState(Character character) : base("Jump Range Select", character._stateMachine, character) { }

            public override void OnEnter()
            {
                _clock = 0;
                _reverse = false;

                Parent._moveDirection = new Vector3(
                    Mathf.Sin(Parent._nextJumpAngle * Mathf.Deg2Rad),
                    0,
                    Mathf.Cos(Parent._nextJumpAngle * Mathf.Deg2Rad)
                ).normalized;
                var targetRotation = Mathf.Atan2(Parent._moveDirection.x, Parent._moveDirection.z) * Mathf.Rad2Deg;
                Parent.transform.DORotateQuaternion(Quaternion.Euler(0, targetRotation, 0), 0.35f);

                Parent.indicatorPoint.transform.localPosition = Vector3.zero;
                Parent.indicatorPoint.transform.localScale = new Vector3(
                    0.3f / Parent.jumpIndicator.transform.localScale.x,
                    2,
                    0.3f / Parent.jumpIndicator.transform.localScale.z
                );
                Parent.indicatorPoint.SetActive(true);

                Parent.ActionKeyPressed += OnActionKeyPress;
            }

            public override void Update()
            {
                if (_clock > 0.5f)
                {
                    _clock -= 0.5f;
                    _reverse = !_reverse;
                }

                Parent.indicatorPoint.transform.localPosition = Vector3.Lerp(
                    _reverse ? new Vector3(0.5f, 0, 0) : Vector3.zero,
                    _reverse ? Vector3.zero : new Vector3(0.5f, 0, 0),
                    _clock * 2
                );

                _clock += Time.deltaTime;
            }

            public override void OnExit()
            {
                Parent.ActionKeyPressed -= OnActionKeyPress;

                Parent._nextJumpRange = Parent.indicatorPoint.transform.localPosition.x * Parent.jumpIndicator.transform.localScale.x;
                Parent.indicatorPoint.SetActive(false);
                Parent._isOnWater = false;
            }

            public void OnActionKeyPress()
            {
                Machine.ChangeState("Jumping");
            }
        }

        public class JumpingState : State<Character>
        {
            float _speed, _verticalSpeed, _motionSpeed;
            bool _released;

            public JumpingState(Character character) : base("Jumping", character._stateMachine, character) { }

            public override void OnEnter()
            {
                // var duration = Mathf.Lerp(0.5f, 1f, Parent._nextJumpRange / Parent.jumpIndicator.transform.localScale.x);
                var duration = 1;
                _speed = Parent._nextJumpRange / duration / 2;
                _motionSpeed = 1 / duration;
                _released = false;

                Parent.jumpIndicator.transform.SetParent(Parent.transform);
                Parent.jumpIndicator.SetActive(false);
                // Parent.JumpTo(Parent.transform.position + Parent._nextJumpRange * Parent._moveDirection);
                Parent.Jump().Forget();

                Parent.ActionKeyReleased += OnActionKeyRelease;
                Parent.JumpFinished += OnJumpFinish;
            }

            public override void OnExit()
            {
                Parent.ActionKeyReleased -= OnActionKeyRelease;
                Parent.JumpFinished -= OnJumpFinish;
            }

            public void OnActionKeyRelease()
            {
                _released = true;
            }

            public void OnJumpFinish()
            {
                if (_released)
                    Machine.ChangeState("Idle");
                else
                    Machine.ChangeState("Running");
            }
        }

        public class RunningState : State<Character>
        {
            float _speed;

            public RunningState(Character character) : base("Running", character._stateMachine, character) { }

            public override void OnEnter()
            {
                _speed = Parent.sprintSpeed;

                Parent.ActionKeyReleased += OnActionKeyRelease;
            }

            public override void Update()
            {
                Parent.MoveForward(_speed, 0);
            }

            public override void OnExit()
            {
                Parent.ActionKeyReleased -= OnActionKeyRelease;

                if (Parent._hasAnimator)
                    Parent.Stop(_speed).Forget();
            }

            public void OnActionKeyRelease()
            {
                Machine.ChangeState("Idle");
            }
        }

        public class OutOfControlState : State<Character>
        {
            public OutOfControlState(Character character) : base("Out Of Control", character._stateMachine, character) { }
        }

        #endregion

        #region Unity messages

        private void Awake()
        {
            _stateMachine = new StateMachine();
            _stateMachine.SetStates("Idle",
                new IdleState(this),
                new JumpAngleSelectState(this),
                new JumpRangeSelectState(this),
                new JumpingState(this),
                new RunningState(this),
                new OutOfControlState(this));
        }

        void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();

            AssignAnimationIDs();

            _fallTimeoutDelta = fallTimeout;
        }

        void Update()
        {
            _stateMachine.OnUpdate();

            if(_isOnWater)
            {
                if(_waterClock > 1f)
                {
                    _waterClock -= 1f;
                    IncreaseFloodRate(5);
                }
                _waterClock += Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            _stateMachine.OnFixedUpdate();   
        }

        private void LateUpdate()
        {
            _stateMachine.OnLateUpdate();

            CameraRotation();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(LookDirection.x, 0, LookDirection.y));
        }

        public void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                if (IsStandingOnWater())
                {
                    IncreaseFloodRate(10);
                    API.Sound.PlayOneShotEffect($"water_step_{Random.Range(0, 2) + 1}");
                }
        }

        #endregion

        #region Character moving

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void CameraRotation()
        {
            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public void MoveForward(float frontSpeed, float verticalSpeed, float motionSpeed = 1f)
        {
            if (_stateMachine.CurrentState.Name != "Jumping" && IsGrounded())
                verticalSpeed += gravity;
            
            // move the player
            _controller.Move(_moveDirection * (frontSpeed * Time.deltaTime) + new Vector3(0.0f, verticalSpeed, 0.0f) * Time.deltaTime);
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, frontSpeed);
                _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
            }
        }

        bool _grounded = true;

        public void JumpTo(Vector3 position)
        {
            Physics.Raycast(position + new Vector3(0, 0.5f, 0), Vector3.down, out RaycastHit hit, 1f, 1 << 0);

            _animator.SetBool(_animIDGrounded, false);
            _animator.SetBool(_animIDJump, true);

            DOTween.Sequence()
                .AppendInterval(0.1f)
                .Append(transform.DOJump(new Vector3(position.x, hit.point.y, position.z), 0.3f, 1, 0.8f).SetEase(Ease.InOutCubic))
                .AppendCallback(() => {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDGrounded, true);
                    JumpFinished?.Invoke();
                })
                .Play();
        }

        public async UniTask Jump()
        {
            // _moveDirection, _nextJumpRange
            var from = transform.position;
            Physics.Raycast(transform.position + _nextJumpRange * _moveDirection + new Vector3(0, 0.5f, 0), Vector3.down, out RaycastHit dest, 1f, 1 << 0);

            float clock = 0f, prevClock = 0f;

            _animator.SetBool(_animIDGrounded, false);
            _animator.SetBool(_animIDJump, true);

            while(prevClock <= 1f)
            {
                prevClock = clock;
                clock += Time.deltaTime;

                var frontSpeed = DOVirtual.EasedValue(0, _nextJumpRange, clock, Ease.InOutCubic)
                    - DOVirtual.EasedValue(0, _nextJumpRange, prevClock, Ease.InOutCubic);
                var verticalSpeed = 
                    (clock < 0.5f ? DOVirtual.EasedValue(from.y, 0.4f, clock * 2, Ease.InOutCubic) : DOVirtual.EasedValue(0.4f, dest.point.y, (clock - 0.5f) * 2, Ease.InOutCubic)) -
                    (prevClock < 0.5f ? DOVirtual.EasedValue(from.y, 0.4f, prevClock * 2, Ease.InOutCubic) : DOVirtual.EasedValue(0.4f, dest.point.y, (prevClock - 0.5f) * 2, Ease.InOutCubic));
                _controller.Move(_moveDirection * frontSpeed + new Vector3(0.0f, verticalSpeed, 0.0f));

                if (prevClock >= 0.5f && IsGrounded())
                    break;

                await UniTask.Yield();
            }

            Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out RaycastHit hit, 1f, 1 << 0);
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDGrounded, true);
            transform.position = hit.point;
            JumpFinished?.Invoke();

            if (IsStandingOnWater())
            {
                IncreaseFloodRate(20);
                API.Sound.PlayOneShotEffect("water_land");
            }
        }

        public async UniTaskVoid Stop(float initialSpeed)
        {
            _animator.SetFloat(_animIDMotionSpeed, 1);

            var blendedSpeed = initialSpeed;
            while (blendedSpeed > 0.01f)
            {
                blendedSpeed = Mathf.Lerp(blendedSpeed, 0, Time.deltaTime * speedChangeRate);
                _animator.SetFloat(_animIDSpeed, blendedSpeed);
                await UniTask.Yield();
            }
            
            _animator.SetFloat(_animIDSpeed, 0);
        }

        #endregion

        #region Game logic

        public void OnAction(InputValue value)
        {
            if (value.isPressed)
                ActionKeyPressed?.Invoke();
            else
                ActionKeyReleased?.Invoke();
        }

        public bool IsGrounded()
        {
            // set sphere position, with offset
            var spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            return Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        public bool IsStandingOnWater()
        {
            Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out RaycastHit hit);
            if (hit.collider.gameObject.layer == 4)
                return true;
            else 
                return false;
        }

        void IncreaseFloodRate(float delta)
        {
            _floodRate += delta;
            onFloodRateChange.Invoke(_floodRate);
        }

        public void SetOutOfControl()
        {
            _stateMachine.ChangeState("Out Of Control");
        }

        public void SetIdle()
        {
            _stateMachine.ChangeState("Idle");
        }

        #endregion
    }
}
