using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public Sprite sprite;
    public GameObject instance; //! Ranged weapons only
    public float damage;
    public float knockBack;
    public float range; //! Ranged weapons only (Range = time to live in sec)
    public float speed; //! Ranged weapons only
    public Type type;

    public enum Type
    {
        Melee,
        Ranged,
        Magic
    }
}
