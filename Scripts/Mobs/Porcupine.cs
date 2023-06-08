using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Porcupine : MonoBehaviour, IDamageHandler
{
    public Animator animator;
    public Weapon weapon;
    public float health;
    public int expReward;
    public float moveSpeed;
    public float shootInterval;
    public DetectionZone detectionZone;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public GameObject healthText;
    public GameObject tileDropPF;
    public Sprite drop;
    public AudioManager audioManager;
    public PlayerController playerController;
    public RoomFirstDungeonGenerator waveManager;
    public Color originalColor;
    
    public float invincibilityTime = 0.25f;
    public bool canTurnInvincible = false;
    public bool _invincible = false;
    private float invincibleTimeElapsed = 0f;
    private bool canShoot = true;
    

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
        audioManager = FindObjectOfType<AudioManager>();
        rb = GetComponent<Rigidbody2D>();
        playerController = FindObjectOfType<PlayerController>();
        waveManager = FindObjectOfType<RoomFirstDungeonGenerator>();
        originalColor = GetComponent<SpriteRenderer>().color;
    } 

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

            if (canShoot)
            {
                ShootSpike(col);
            }            
        }

        if (Invincible)
        {
            invincibleTimeElapsed += Time.deltaTime;
            if (invincibleTimeElapsed > invincibilityTime)
                Invincible = false;

            StartCoroutine(FlashColor(0.2f));
        }

        if (waveManager.wave > 1 && waveManager.wave < 3)
            moveSpeed = 1.5f;
        else if (waveManager.wave >= 3 && waveManager.wave < 7)
            moveSpeed = 1.7f;
        else if (waveManager.wave >= 7 && waveManager.wave <= 10)
        {
            moveSpeed = 2f;
            weapon.damage = 7f;
        }
    }

    private void ShootSpike(Collider2D col)
    {
        Vector2 targetPos = col.gameObject.transform.position;
        Vector2 targetDir = targetPos - rb.position;
        targetDir.Normalize();

        GameObject spikeInstance = null;
        if (targetPos.x < transform.position.x && targetPos.y < transform.position.y)
        {
            spikeInstance = Instantiate(weapon.instance, new Vector2(transform.position.x - 0.3f, transform.position.y), Quaternion.identity);
        }
        else if (targetPos.x > transform.position.x && targetPos.y < transform.position.y)
        {
            spikeInstance = Instantiate(weapon.instance, new Vector2(transform.position.x + 0.3f, transform.position.y), Quaternion.identity);
        }
        else if (targetPos.y < transform.position.y)
        {
            spikeInstance = Instantiate(weapon.instance, new Vector2(transform.position.x + 0.3f, transform.position.y - 2), Quaternion.identity);
        }
        else if (targetPos.y > transform.position.y)
        {
            spikeInstance = Instantiate(weapon.instance, new Vector2(transform.position.x, transform.position.y + 0.4f), Quaternion.identity);
        }
        else

        spikeInstance.GetComponent<SpriteRenderer>().flipY = (targetPos.x < transform.position.x);
        spikeInstance.GetComponent<Rigidbody2D>().velocity = targetDir * weapon.speed;
        spikeInstance.transform.Rotate(0f, 0f, Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg);
        Destroy(spikeInstance, weapon.range); 

        StartCoroutine(SpikeCooldown());
    }

    private IEnumerator SpikeCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }

    void IDamageHandler.TakeDamage(float damage, Vector2 knockback)
    {
        if (!Invincible)
        {
            // if (!audioManager.GetSound("PorcupineDamage").source.isPlaying)
            // audioManager.Play("PorcupineDamage");

            #region Damage numbers
            RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
            textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            textTransform.SetParent(canvas.transform);
            
            TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
            float rand = Random.Range(0f, 1f);

            if (rand > 0.7)
            {
                damage++;
                text.text = damage + " CRITICAL HIT!";
                text.color = new Color(1, 0.451f, 0.075f, 1);
            }
            else
            {
                text.text = "" + damage;
                text.color = new Color(1, 0, 0, 1);
            }
            #endregion
            
            this.health -= damage;
            rb.AddForce(knockback);

            if (canTurnInvincible)
            {
                // activate timer
                Invincible = true;
            }

            if (this.health <= 0)
            {
                Killed();
                CoinDrop();
                rb.simulated = false;
                playerController.exp += expReward;
                playerController.CheckLevelUp();
            }
        }
        
    }

    void IDamageHandler.TakeDamage(float dmg) 
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator FlashColor(float interval)
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(interval);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    private void Killed() => animator.SetTrigger("dead");

    private void CoinDrop()
    {
        GameObject td = Instantiate(tileDropPF, transform.position, Quaternion.identity);
        td.name = "coin";
        td.GetComponent<SpriteRenderer>().sprite = drop;
        td.AddComponent<TileDropController>();
    }
    public void DestroyMob() => Destroy(gameObject);
}
