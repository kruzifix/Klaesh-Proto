using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Core.Message;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int mouseButton = 1;
    public Transform root;
    public float rotationSensivity = 1f;
    public float zoomSensitivity = 0.6f;
    public float rotationLerpConst = 0.9f;
    public float moveLerpConst = 0.1f;
    public float zoomLerpConst = 0.3f;

    public float minDistance = 1f;
    public float maxDistance = 20f;

    public float minPitch = 20f;
    public float maxPitch = 70f;

    private Vector3 _lastMousePos;
    private float _yaw = 0f;
    private float _yawTarget = 0f;
    private float _pitch = 45f;
    private float _pitchTarget = 45f;
    private float _distance = 0f;
    private float _targetDistance = 0f;

    private Vector3 _rootTargetPos;

    private void Start()
    {
        _rootTargetPos = root.transform.position;
        _targetDistance = Vector3.Distance(transform.position, _rootTargetPos);

        var bus = ServiceLocator.Instance.GetService<IMessageBus>();
        bus.Subscribe<HexTileSelectedMessage>(OnHexTileSelected);
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
            _pitchTarget = Mathf.Clamp(_pitchTarget + delta.y * rotationSensivity, minPitch, maxPitch);

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

    private void OnHexTileSelected(HexTileSelectedMessage msg)
    {
        _rootTargetPos = msg.Content.transform.position + new Vector3(0f, msg.Content.height * 0.5f, 0f);
    }
}
