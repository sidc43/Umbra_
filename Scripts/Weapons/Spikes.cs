using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Weapon spikes;
    [SerializeField] private CircleCollider2D col;
    private float damage;
    private float kb;

    private void Start()
    {
        damage = spikes.damage;
        kb = spikes.knockBack;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag is not "Mob" or "Player")
            Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player")
        {
            IDamageHandler player = other.GetComponent<IDamageHandler>();

            if (player != null)
            {
                Vector3 playerPos = gameObject.GetComponentInParent<Transform>().position;

                Vector2 direction = (Vector2) (other.transform.position - playerPos).normalized;
                Vector2 _kb = direction * kb;
         
                player.TakeDamage(damage, _kb);
                Destroy(this.gameObject);
            }
            else
                print("Other doesn't implement IplayerDamageHandler");
        }
    }
}
