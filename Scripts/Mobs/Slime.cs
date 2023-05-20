using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slime : MonoBehaviour, IDamageHandler
{
    public Animator animator;
    public float health;
    public float damage;
    public float moveSpeed;
    public DetectionZone detectionZone;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public GameObject healthText;
    public float kbMagnitude;

    private void Start() =>  rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (detectionZone.detectedObj.Count >= 1)
        {
            Collider2D col = detectionZone.detectedObj[0];
            float step = moveSpeed * Time.fixedDeltaTime;
            this.transform.position = Vector2.MoveTowards(this.transform.position, col.gameObject.transform.position, step);
            if (col.transform.position.x < this.transform.position.x)
                sprite.flipX = true;
            else    
                sprite.flipX = false;
        }
    }

    void IDamageHandler.TakeDamage(float damage, Vector2 knockback)
    {
        this.health -= damage;

        RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
        textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        textTransform.SetParent(canvas.transform);
        
        TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "" + damage;
        text.color = new Color(1, 0, 0, 1);

        if (this.health <= 0)
        {
            Killed();
        }

        rb.AddForce(knockback);

        animator.SetBool("gettingHit", true);
        print(knockback);
    }

    void IDamageHandler.TakeDamage(float dmg) 
    {
        throw new System.NotImplementedException();
    }

    private void Killed() 
    {
        animator.SetTrigger("dead");
    }

    public void StopDmgAnim() => animator.SetBool("gettingHit", false);

    public void DestroyMob() 
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        Collider2D col = other.collider;
        IDamageHandler player = col.GetComponent<IDamageHandler>();

        if (player != null && other.gameObject.tag == "Player")
        {
            Vector3 slimePos = gameObject.GetComponentInParent<Transform>().position;

            Vector2 direction = (Vector2) (other.transform.position - slimePos).normalized;
            Vector2 kb = direction * kbMagnitude;
        
            player.TakeDamage(damage, kb);
        }
    }
}
