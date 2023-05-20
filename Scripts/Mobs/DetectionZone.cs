using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Collider2D> detectedObj;
    public Collider2D col;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player")
        {
            detectedObj.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        detectedObj.Remove(other);
    }
}

