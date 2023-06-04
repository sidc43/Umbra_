using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float smoothing = 9f; // Adjust the smoothing factor as needed

    private Vector2 _mousePos;
    private Vector2 _mouseWorldPos;
    private Vector2 _targetPos;

    private void Start()
    {
        _targetPos = transform.position;
    }

    private void FixedUpdate()
    {
        GetMousePos();
        UpdateCrosshairPosition();
    }

    public void GetMousePos()
    {
        float distance = Camera.main.nearClipPlane;
        _mousePos = Mouse.current.position.ReadValue();
        _mouseWorldPos = Camera.main.ScreenToWorldPoint(_mousePos);
    }

    private void UpdateCrosshairPosition()
    {
        _targetPos = Vector2.Lerp(_targetPos, _mouseWorldPos, smoothing * Time.fixedDeltaTime);
        transform.position = _targetPos;
    }
}
