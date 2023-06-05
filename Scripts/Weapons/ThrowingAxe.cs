using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingAxe : MonoBehaviour
{
    [SerializeField] private Weapon throwingAxe;
    [SerializeField] private BoxCollider2D col;
    private float damage;
    private float kb;

    private void Start()
    {
        damage = throwingAxe.damage;
        kb = throwingAxe.knockBack;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name != "Player")
            Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Mob")
        {
            
            IDamageHandler mob = other.GetComponent<IDamageHandler>();

            if (mob != null)
            {
                Vector3 playerPos = gameObject.GetComponentInParent<Transform>().position;

                Vector2 direction = (Vector2) (other.transform.position - playerPos).normalized;
                Vector2 _kb = direction * kb;
            
                mob.TakeDamage(damage, _kb);
                Destroy(this.gameObject);
            }
            else
                print("Other doesn't implement IMobDamageHandler");
        }
    }
}
