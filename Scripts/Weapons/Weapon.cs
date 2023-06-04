using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public Sprite sprite;
    public float damage;
    public float knockBack;
    public float range;
    public Type type;

    public enum Type
    {
        Melee,
        Ranged,
        Magic
    }
}
