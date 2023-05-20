using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    public Slime slimePF;
    public PlayerController player;

    public void SpawnSlime() => Instantiate(slimePF, player.transform.position + new Vector3(1f, 1f, 0), Quaternion.identity);
}
