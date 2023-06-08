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
    public float invincibilityTime = 0.25f;
    public bool canTurnInvincible = false;
    public bool _invincible = false;
    public RoomFirstDungeonGenerator waveManager;


    private float invincibleTimeElapsed = 0f;
    private Color originalColor;
    private int numCoinsDropped;

    public bool Invincible { get {
        return _invincible;
    }
    set {
        _invincible = value;

        if (_invincible == true)
        {
            invincibleTimeElapsed = 0f;
        }
    } }

    private void Start()  
    {
        numCoinsDropped = 3;
        audioManager = FindObjectOfType<AudioManager>(); 
        playerController = FindObjectOfType<PlayerController>(); 
        originalColor = GetComponent<SpriteRenderer>().color;
        waveManager = FindObjectOfType<RoomFirstDungeonGenerator>();
    }

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

        if (Invincible)
        {
            invincibleTimeElapsed += Time.deltaTime;
            if (invincibleTimeElapsed > invincibilityTime)
                Invincible = false;

            StartCoroutine(FlashColor(0.2f));
        }

        if (waveManager.wave > 1 && waveManager.wave < 3)
        {
            numCoinsDropped = 5;
            moveSpeed = 1.3f;
        }
        else if (waveManager.wave >= 3 && waveManager.wave < 7)
        {
            numCoinsDropped = 7;
            moveSpeed = 1.4f;
        }
        else if (waveManager.wave >= 7 && waveManager.wave <= 10)
        {
            numCoinsDropped = 9;
            moveSpeed = 1.6f;
            swordAttack.damage = 10f;
        }
    }

    private IEnumerator FlashColor(float interval)
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(interval);
        GetComponent<SpriteRenderer>().color = originalColor;
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
        if (!Invincible)
        {
            if (!audioManager.GetSound("KnightDamage").source.isPlaying)
            audioManager.Play("KnightDamage");

            rb.AddForce(kb);
            this.health -= dmg;

            if (canTurnInvincible)
            {
                // activate timer
                Invincible = true;
            }

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
                CoinDrop(numCoinsDropped);
                rb.simulated = false;
                playerController.exp += expReward;
                playerController.CheckLevelUp();
            }
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
