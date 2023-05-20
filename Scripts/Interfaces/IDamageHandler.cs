using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageHandler
{
    void TakeDamage(float dmg, Vector2 kb);
    void TakeDamage(float dmg);
}
