using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSpawner : MonoBehaviour
{
    public Knight knight;
    public PlayerController player;

    public void SpawnKnight() => Instantiate(knight, player.transform.position + new Vector3(1f, 1f, 0), Quaternion.identity);
}
