using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Knight : MonoBehaviour, IDamageHandler
{
    public Animator animator;
    public float health;
    public float moveSpeed;
    public int expReward;
    public DetectionZone detectionZone;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public HealthText healthText;
    public GameObject tileDropPF;
    public Sprite drop;
    public SwordAttack swordAttack;
    public AudioManager audioManager;
    public PlayerController playerController;

    private void Start()  {audioManager = FindObjectOfType<AudioManager>(); playerController = FindObjectOfType<PlayerController>();}

    private void FixedUpdate()
    {
        if (detectionZone.detectedObj.Count >= 1)
        {
            Collider2D col = detectionZone.detectedObj[0];
            float step = moveSpeed * Time.fixedDeltaTime;
            transform.position = Vector2.MoveTowards(transform.position, col.gameObject.transform.position, step);

            sprite.flipX = col.transform.position.x < transform.position.x;
            animator.SetBool("isMoving", true);
            animator.SetBool("swordAttack", Vector2.Distance(transform.position, col.gameObject.transform.position) < 1.6f);
        }
        else
            animator.SetBool("isMoving", false);
    }

    public void SwordAttack()
    {
        audioManager.Play("PlayerAttack");
        if (sprite.flipX)
        {
            swordAttack.AttackLeft();
        }
        else
            swordAttack.AttackRight();
    }

    public void StopSwordAttack() => swordAttack.StopAttack();

    void IDamageHandler.TakeDamage(float dmg, Vector2 kb)
    {
        if (!audioManager.GetSound("KnightDamage").source.isPlaying)
            audioManager.Play("KnightDamage");

        rb.AddForce(kb);
        this.health -= dmg;

        #region Damage numbers
        RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
        textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        textTransform.SetParent(canvas.transform);
        
        TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "" + dmg;
        text.color = new Color(1, 0, 0, 1);
        #endregion

        if (this.health <= 0)
        {
            animator.SetTrigger("dead");
            CoinDrop(3);
            rb.simulated = false;
            playerController.exp += expReward;
            playerController.CheckLevelUp();
        }
    }

    private void CoinDrop()
    {
        GameObject td = Instantiate(tileDropPF, transform.position, Quaternion.identity);
        td.name = "coin";
        td.GetComponent<SpriteRenderer>().sprite = drop;
        td.AddComponent<TileDropController>();
    }

    private void CoinDrop(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject td = Instantiate(tileDropPF, new Vector3(transform.position.x - (i / 2), transform.position.y), Quaternion.identity);
            td.name = "coin" + i;
            td.GetComponent<SpriteRenderer>().sprite = drop;
            td.AddComponent<TileDropController>();
        }
        
    }

    public void DestroyMob() => Destroy(gameObject);

    void IDamageHandler.TakeDamage(float dmg)
    {
        throw new System.NotImplementedException();
    }
}
