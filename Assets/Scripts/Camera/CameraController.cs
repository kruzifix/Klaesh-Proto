using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game;
using Klaesh.Game.Message;
using UnityEngine;

public class CameraController : ManagerBehaviour
{
    public int mouseButton = 1;
    public Transform root;

    [Header("Sensitivity")]
    public float rotationSensivity = 0f;
    public float zoomSensitivity = 0f;

    [Header("Lerping")]
    public float rotationLerpConst = 0.9f;
    public float moveLerpConst = 0.1f;
    public float zoomLerpConst = 0.3f;

    [Header("Zoom")]
    public float minDistance = 1f;
    public float maxDistance = 20f;

    [Header("Yaw")]
    public float minYaw = 0f;
    public float maxYaw = 360f;

    [Header("Pitch")]
    public float minPitch = 20f;
    public float maxPitch = 70f;

    [Header("Start Orientation")]
    public float startYaw;
    public float startPitch;

    public float Yaw => _yaw;
    public float Pitch => _pitch;
    public float Distance => _distance;
    public Vector3 TargetPos => _rootTargetPos;

    private Vector3 _lastMousePos;
    private float _yaw = -30f;
    private float _yawTarget = -30f;
    private float _pitch = 50f;
    private float _pitchTarget = 50f;
    private float _distance = 10f;
    private float _targetDistance = 10f;

    private float _yawMin;
    private float _yawMax;

    private Vector3 _rootTargetPos;

    private void Start()
    {
        _rootTargetPos = root.transform.position;
        _targetDistance = Mathf.Lerp(minDistance, maxDistance, 0.5f);

        _yaw = _yawTarget = startYaw;
        _pitch = _pitchTarget = startPitch;

        _yawMin = minYaw;
        _yawMax = maxYaw;

        _bus.Subscribe<FocusCameraMessage>(OnFocusCamera);
        _bus.Subscribe<GameStartedMessage>(OnGameStarted);
    }

    private void LateUpdate()
    {
        if (root == null)
            return;

        if (Input.GetMouseButtonDown(mouseButton))
        {
            _lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(mouseButton))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 delta = mousePos - _lastMousePos;

            _yawTarget = _yawTarget + delta.x * rotationSensivity;
            _yawTarget = Mathf.Clamp(_yawTarget, _yawMin, _yawMax);
            _pitchTarget = Mathf.Clamp(_pitchTarget - delta.y * rotationSensivity, minPitch, maxPitch);

            _lastMousePos = mousePos;
        }

        float zoomDelta = Input.mouseScrollDelta.y * zoomSensitivity;
        _targetDistance = Mathf.Clamp(_targetDistance - zoomDelta, minDistance, maxDistance);

        _yaw = Mathf.Lerp(_yaw, _yawTarget, rotationLerpConst);
        _pitch = Mathf.Lerp(_pitch, _pitchTarget, rotationLerpConst);
        _distance = Mathf.Lerp(_distance, _targetDistance, zoomLerpConst);
        root.transform.position = Vector3.Lerp(root.transform.position, _rootTargetPos, moveLerpConst);
        root.transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

        transform.localPosition = (Quaternion.Euler(_pitch, 0f, 0f) * Vector3.back) * _distance;
        transform.LookAt(root);
    }

    private void OnFocusCamera(FocusCameraMessage msg)
    {
        _rootTargetPos = msg.Position;
        if (!msg.InterpolatePosition)
            root.transform.position = msg.Position;
    }

    private void OnGameStarted(GameStartedMessage msg)
    {
        bool homeTeam = _locator.GetService<IGameManager>().GameConfig.HomeSquadId == 0;

        _yawMin = minYaw;
        _yawMax = maxYaw;

        if (!homeTeam)
        {
            _yawMin += 180f;
            _yawMax += 180f;
        }
        float yaw = startYaw + (homeTeam? 0f : 180f);
        _yaw = _yawTarget = yaw;
    }
}
