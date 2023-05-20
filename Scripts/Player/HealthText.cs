using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthText : MonoBehaviour
{
    public float timeToLive = 0.5f;
    public float floatSpeed = 50f;
    public TextMeshProUGUI text;
    public RectTransform rect;
    public Vector3 floatDir = new Vector3(0, 1, 0);

    private Color _starting;
    private float _timeElapsed = 0f;

    private void Start() => _starting = text.color;

    void Update()
    {
        _timeElapsed += Time.deltaTime;

        rect.position += floatDir * floatSpeed * Time.deltaTime;

        text.color = new Color(_starting.r, _starting.g, _starting.b, 1 - (_timeElapsed / timeToLive));

        if (_timeElapsed > timeToLive)
            Destroy(gameObject);
    }
}
