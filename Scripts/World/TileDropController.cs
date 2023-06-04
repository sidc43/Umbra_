using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDropController : MonoBehaviour
{
    PlayerController player;
    private void OnTriggerEnter2D(Collider2D other) {
        player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.UpdateCoinCount();
            Destroy(gameObject);
        }
    }
}
