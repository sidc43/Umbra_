using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{
    
    private Vector2 _mousePos;
    private Vector2 _mouseWorldPos;
    private void FixedUpdate()
    {
        GetMousePos();
        this.transform.position = _mouseWorldPos;
    }

    public void GetMousePos()
    {
        float distance = Camera.main.nearClipPlane;
        _mousePos = Mouse.current.position.ReadValue();
        _mouseWorldPos = Camera.main.ScreenToWorldPoint(_mousePos);
    }
}
