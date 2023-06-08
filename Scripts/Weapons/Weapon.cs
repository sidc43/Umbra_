using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Global")]
    public Sprite sprite;
    public float damage;
    public float knockBack;
    public Type type;

    [Header("Ranged and Magic")]
    public GameObject instance; //% Ranged and Magic weapons only
    public float range; //% Ranged and Magic weapons only (Range = time to live in sec)
    public float speed; //% Ranged and Magic weapons only
    public float cooldown;

    [Header("Magic")]
    public float manaCost; //@ Magic weapons only

    public enum Type
    {
        Melee,
        Ranged,
        Magic
    }
}
