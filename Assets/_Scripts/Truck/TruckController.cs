using System;
using System.Collections;
using _Scripts.Managers;
using _Scripts.UI;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Truck
{
    public enum GearState
    {
        Neutral,
        Running,
        CheckingChange,
        Changing
    };

    public class TruckController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private EngineAudio _engineAudio;
        [field: SerializeField] public TruckUpgradeManager UpgradeManager { get; private set; }
        [SerializeField] private TruckEffects _truckEffects;
        [SerializeField] private WheelColliders _colliders;
        [SerializeField] private WheelMeshes _wheelMeshes;
        [SerializeField] private AnimationCurve _hpToRpmCurve;
        [SerializeField] private AnimationCurve _steeringCurve;
        [SerializeField] private float _redLine;
        [SerializeField] private float _idleRpm;
        [SerializeField] private float[] _gearRatios;
        [SerializeField] private float _differentialRatio;
        [SerializeField] private float _motorPower;
        [SerializeField] private float _brakePower;
        [SerializeField] private float _slipAngle;
        [SerializeField] private float _increaseGearRpm;
        [SerializeField] private float _decreaseGearRpm;
        [SerializeField] private float _changeGearTime = 0.5f;

        #region Drift

        private float _currentDriftScore;
        private bool _isDrifting;
        private DriftManager _driftManager;

        #endregion

        //
        [SerializeField] private int _currentGear;
        [SerializeField] private float _speed;
        [SerializeField] private int _isEngineRunning;
        [SerializeField] private GearState _gearState;
        private const float SteeringSpeed = 50f;
        private const float MaxSteerAngle = 60f;
        private float _rpm;
        private float _gasInput;
        private float _brakeInput;
        private float _steeringInput;
        private float _speedClamped;
        private float _currentTorque;
        private float _clutch;
        private float _wheelRpm;
        public PhotonView _photonView;
        private InputManager.InputManager _inputManager;
        public int TruckPrefabId { get; set; }
        public bool IsForceBrake { get; private set; } = false;

        public WheelColliders Colliders => _colliders;

        public Rigidbody PlayerRb { get; private set; }

        #region Network

        private bool _isLocalPlayer;

        public void SetPlayerId(int playerActorNumber)
        {
            var allViews = GetComponentsInChildren<PhotonView>();
            foreach (var view in allViews)
            {
                view.OwnerActorNr = playerActorNumber;
                view.ControllerActorNr = playerActorNumber;
            }

            _isLocalPlayer = _photonView.IsMine;
        }

        #endregion

        private void Start()
        {
            FinishPanel.OnFinishPanelOpened += OnGameEnd;
        }

        public void Initialize(DriftManager driftManager, InputManager.InputManager inputManager)
        {
            _inputManager = inputManager;
            _driftManager = driftManager;
            _photonView = GetComponent<PhotonView>();
            PlayerRb = gameObject.GetComponent<Rigidbody>();
        }

        private void OnGameEnd()
        {
            _isDrifting = false;
            IsForceBrake = true;
            _driftManager.AddDriftPoints(_photonView.OwnerActorNr, _currentDriftScore);
            _currentDriftScore = 0;
        }

        private void Update()
        {
            if (Pause.IsPaused || IsForceBrake)
            {
                ForceBreak();
                return;
            }

            _speed = Colliders._rrWheel.rpm * Colliders._rrWheel.radius * 2f * Mathf.PI / 10f;
            _speedClamped = Mathf.Lerp(_speedClamped, _speed, Time.deltaTime);
            if (_isLocalPlayer)
            {
                CheckLocalInput();
                ApplyMotor();
                ApplySteering();
                ApplyBrake();
                CheckDrift();
                ApplyWheelPositions();
                _truckEffects.CheckParticles();
            }
        }

        private void ForceBreak()
        {
            Colliders._frWheel.brakeTorque = 1000f;
            Colliders._flWheel.brakeTorque = 1000f;
            Colliders._rrWheel.brakeTorque = 1000f;
            Colliders._rlWheel.brakeTorque = 1000f;
        }

        private void CheckDrift()
        {
            if (_slipAngle > _driftManager.DriftAngleMin &&
                PlayerRb.velocity.magnitude >= DriftManager.MinMagnitudeForDrift)
            {
                if (!_isDrifting)
                {
                    _isDrifting = true;
                }

                var points = _slipAngle * _driftManager.DriftScoreFactor;
                _currentDriftScore += points;
            }
            else
            {
                _isDrifting = false;
                _driftManager.AddDriftPoints(_photonView.OwnerActorNr, _currentDriftScore);
                _currentDriftScore = 0;
            }
        }

        [PunRPC]
        private void ReceiveInput(float gasInput, float brakeInput, float steeringInput, int viewId)
        {
            if (_photonView.ViewID != viewId) return;
            _gasInput = gasInput;
            _brakeInput = brakeInput;
            _steeringInput = steeringInput;
            MoveTruck();
        }

        private void CheckLocalInput()
        {
            _gasInput = _inputManager.GetVerticalInput();
            _steeringInput = _inputManager.GetHorizontalInput();
            _photonView.RPC(nameof(ReceiveInput), RpcTarget.Others, _gasInput, _brakeInput, _steeringInput,
                _photonView.ViewID);
            MoveTruck();
        }

        private void MoveTruck()
        {
            if (Mathf.Abs(_gasInput) > 0 && _isEngineRunning == 0)
            {
                StartCoroutine(StartEngine());
                _gearState = GearState.Running;
            }

            _slipAngle = Vector3.Angle(transform.forward, PlayerRb.velocity - transform.forward);
            var movingDirection = Vector3.Dot(transform.forward, PlayerRb.velocity);
            if (_gearState != GearState.Changing)
            {
                if (_gearState == GearState.Neutral)
                {
                    _clutch = 0;
                    if (Mathf.Abs(_gasInput) > 0) _gearState = GearState.Running;
                }
                else
                {
                    _clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(_clutch, 1, Time.deltaTime);
                }
            }
            else
            {
                _clutch = 0;
            }

            if (movingDirection < -0.5f && _gasInput > 0)
            {
                _brakeInput = Mathf.Abs(_gasInput);
            }
            else if (movingDirection > 0.5f && _gasInput < 0)
            {
                _brakeInput = Mathf.Abs(_gasInput);
            }
            else
            {
                _brakeInput = 0;
            }
        }

        private IEnumerator StartEngine()
        {
            _isEngineRunning = 1;
            _engineAudio.PlayStartingSound();
            yield return new WaitForSeconds(0.6f);
            _engineAudio.IsEngineRunning = true;
            yield return new WaitForSeconds(0.4f);
            _isEngineRunning = 2;
        }

        private void ApplyBrake()
        {
            Colliders._frWheel.brakeTorque = _brakeInput * _brakePower * 0.7f;
            Colliders._flWheel.brakeTorque = _brakeInput * _brakePower * 0.7f;
            Colliders._rrWheel.brakeTorque = _brakeInput * _brakePower * 0.3f;
            Colliders._rlWheel.brakeTorque = _brakeInput * _brakePower * 0.3f;
        }

        private void ApplyMotor()
        {
            _currentTorque = CalculateTorque();
            Colliders._rrWheel.motorTorque = _currentTorque * _gasInput;
            Colliders._rlWheel.motorTorque = _currentTorque * _gasInput;
        }

        private float CalculateTorque()
        {
            float torque = 0;
            if (_rpm < _idleRpm + 200 && _gasInput == 0 && _currentGear == 0)
            {
                _gearState = GearState.Neutral;
            }

            if (_gearState == GearState.Running && _clutch > 0)
            {
                if (_rpm > _increaseGearRpm)
                {
                    StartCoroutine(ChangeGear(1));
                }
                else if (_rpm < _decreaseGearRpm)
                {
                    StartCoroutine(ChangeGear(-1));
                }
            }

            if (_isEngineRunning > 0)
            {
                if (_clutch < 0.1f)
                {
                    _rpm = Mathf.Lerp(_rpm, Mathf.Max(_idleRpm, _redLine * _gasInput) + Random.Range(-50, 50),
                        Time.deltaTime);
                }
                else
                {
                    _wheelRpm = Mathf.Abs((Colliders._rrWheel.rpm + Colliders._rlWheel.rpm) / 2f) *
                                _gearRatios[_currentGear] * _differentialRatio;
                    _rpm = Mathf.Lerp(_rpm, Mathf.Max(_idleRpm - 100, _wheelRpm), Time.deltaTime * 3f);
                    torque = _hpToRpmCurve.Evaluate(_rpm / _redLine) * _motorPower / _rpm *
                             _gearRatios[_currentGear] *
                             _differentialRatio * 5252f * _clutch;
                }
            }

            return torque;
        }

        private void ApplySteering()
        {
            var steeringAngle = _steeringInput * _steeringCurve.Evaluate(_speed);
            if (_slipAngle < 120f)
            {
                steeringAngle +=
                    Vector3.SignedAngle(transform.forward, PlayerRb.velocity + transform.forward, Vector3.up);
            }

            steeringAngle = Mathf.Clamp(steeringAngle, -MaxSteerAngle, MaxSteerAngle);
            //lerp
            Colliders._frWheel.steerAngle = Mathf.Lerp(Colliders._frWheel.steerAngle, steeringAngle,
                Time.deltaTime * SteeringSpeed);
            Colliders._flWheel.steerAngle = Mathf.Lerp(Colliders._flWheel.steerAngle, steeringAngle,
                Time.deltaTime * SteeringSpeed);
        }

        private void ApplyWheelPositions()
        {
            UpdateWheel(Colliders._frWheel, _wheelMeshes._frWheel);
            UpdateWheel(Colliders._flWheel, _wheelMeshes._flWheel);
            UpdateWheel(Colliders._rrWheel, _wheelMeshes._rrWheel);
            UpdateWheel(Colliders._rlWheel, _wheelMeshes._rlWheel);
        }

        private void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
        {
            Quaternion quat;
            Vector3 position;
            coll.GetWorldPose(out position, out quat);
            wheelMesh.transform.position = position;
            wheelMesh.transform.rotation = quat;
        }

        public float GetSpeedRatio()
        {
            var gas = Mathf.Clamp(Mathf.Abs(_gasInput), 0.5f, 1f);
            return _rpm * gas / _redLine;
        }

        private IEnumerator ChangeGear(int gearChange)
        {
            _gearState = GearState.CheckingChange;
            if (_currentGear + gearChange >= 0)
            {
                if (gearChange > 0)
                {
                    //increase the gear
                    yield return new WaitForSeconds(0.7f);
                    if (_rpm < _increaseGearRpm || _currentGear >= _gearRatios.Length - 1)
                    {
                        _gearState = GearState.Running;
                        yield break;
                    }
                }

                if (gearChange < 0)
                {
                    //decrease the gear
                    yield return new WaitForSeconds(0.1f);

                    if (_rpm > _decreaseGearRpm || _currentGear <= 0)
                    {
                        _gearState = GearState.Running;
                        yield break;
                    }
                }

                _gearState = GearState.Changing;
                yield return new WaitForSeconds(_changeGearTime);
                _currentGear += gearChange;
            }

            if (_gearState != GearState.Neutral)
                _gearState = GearState.Running;
        }

        public void DisableAllPhysics()
        {
            PlayerRb.isKinematic = true;
            PlayerRb.useGravity = false;
            Colliders._frWheel.gameObject.SetActive(false);
            Colliders._flWheel.gameObject.SetActive(false);
            Colliders._rrWheel.gameObject.SetActive(false);
            Colliders._rlWheel.gameObject.SetActive(false);
        }
    }

    [Serializable]
    public class WheelColliders
    {
        public WheelCollider _frWheel;
        public WheelCollider _flWheel;
        public WheelCollider _rrWheel;
        public WheelCollider _rlWheel;
    }

    [Serializable]
    public class WheelMeshes
    {
        public MeshRenderer _frWheel;
        public MeshRenderer _flWheel;
        public MeshRenderer _rrWheel;
        public MeshRenderer _rlWheel;
    }
}