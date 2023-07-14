using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour, IDamageHandler
{
    #region Public
    public  float maxHealth = 20f;
    public float maxMana = 20f;
    public float mana;
    public float health;
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject healthText;
    public Sprite heartFull;
    public Sprite heartEmpty;
    public Sprite manaFull;
    public Sprite manaEmpty;
    public GameObject heartContainer;
    public GameObject manaContainer;
    public AudioManager audioManager;
    public float invincibilityTime = 0.25f;
    public bool canTurnInvincible = false;
    public bool _invincible = false;
    public RoomFirstDungeonGenerator waveManager;
    public TextMeshProUGUI buffText;
    public PlayerController playerController;
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
    #endregion
    #region Private
    private int _numHearts;
    private int _numMana;
    private float invincibleTimeElapsed = 0f;
    private Color originalColor;
    private bool healthAndManaInc = true;
    #endregion
    void Start()
    {
        this.health = maxHealth;
        this.mana = maxMana;
        originalColor = GetComponent<SpriteRenderer>().color;
        playerController = FindObjectOfType<PlayerController>();
        DrawHearts();
        DrawMana();
    }
    void IDamageHandler.TakeDamage(float dmg, Vector2 kb)
    {
        if (!Invincible)
        {
           audioManager.Play("PlayerDamage");
            rb.AddForce(kb);
            this.health -= dmg;
            ClearHearts();
            DrawHearts();

            #region Damage numbers
            RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
            textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            textTransform.SetParent(canvas.transform);
            
            TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "" + dmg;
            text.color = new Color(1, 0, 0, 1);
            #endregion

            if (canTurnInvincible)
                Invincible = true;

            if (this.health <= 0)
            {
                rb.simulated = false;
                animator.SetTrigger("dead");
                audioManager.Play("PlayerDeath");
            } 
        }
        
    }
    public void DrawHearts()
    {
        _numHearts = (int) health / 5;

        for (int i = 0; i < (int) (maxHealth / 5); i++)
        {
           GameObject newHeart = new GameObject("" + i);
           newHeart.transform.position = heartContainer.transform.position;

           Image img = newHeart.AddComponent<Image>();
           img.sprite = heartEmpty;

           newHeart.transform.SetParent(heartContainer.transform);

           RectTransform rt = newHeart.GetComponent<RectTransform>();
           rt.sizeDelta = new Vector2(35, 35);
           rt.position = rt.position + new Vector3((45 * i), 0, 0);
        }

        for (int i = 0; i < _numHearts; i++)
        {
           GameObject newHeart = new GameObject("" + i);
           newHeart.transform.position = heartContainer.transform.position;

           Image img = newHeart.AddComponent<Image>();
           img.sprite = heartFull;

           newHeart.transform.SetParent(heartContainer.transform);

           RectTransform rt = newHeart.GetComponent<RectTransform>();
           rt.sizeDelta = new Vector2(35, 35);
           rt.position = rt.position + new Vector3((45 * i), 0, 0);
        }
    }
    public void DrawMana()
    {
        _numMana = (int) mana / 5;

        for (int i = 0; i < (int) (maxMana / 5); i++)
        {
           GameObject newMana = new GameObject("" + i);
           newMana.transform.position = manaContainer.transform.position;

           Image img = newMana.AddComponent<Image>();
           img.sprite = manaEmpty;

           newMana.transform.SetParent(manaContainer.transform);

           RectTransform rt = newMana.GetComponent<RectTransform>();
           rt.sizeDelta = new Vector2(35, 35);
           rt.position = rt.position + new Vector3((45 * i), -50, 0);
        }

        for (int i = 0; i < _numMana; i++)
        {
           GameObject newMana = new GameObject("" + i);
           newMana.transform.position = manaContainer.transform.position;

           Image img = newMana.AddComponent<Image>();
           img.sprite = manaFull;

           newMana.transform.SetParent(manaContainer.transform);

           RectTransform rt = newMana.GetComponent<RectTransform>();
           rt.sizeDelta = new Vector2(35, 35);
           rt.position = rt.position + new Vector3((45 * i), -50, 0);
        }
    }
    public void ClearHearts() 
    {
        foreach (Transform child in heartContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public void ClearMana() 
    {
        foreach (Transform child in manaContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    void IDamageHandler.TakeDamage(float dmg)
    {
        throw new System.NotImplementedException();
    }
    public void Update()
    {
        if (Invincible)
        {
            invincibleTimeElapsed += Time.deltaTime;

            if (invincibleTimeElapsed >= invincibilityTime)
            {
                Invincible = false;
            }

            StartCoroutine(FlashColor(0.2f));
        }

        if (waveManager.wave >= 6)
        {
            if (healthAndManaInc)
            {
                playerController.UpdateCoinCountInc(50);
                StartCoroutine(ShowBuffText(2));
                healthAndManaInc = false;
                maxHealth = 40;
                health = maxHealth;
                RedrawHearts();

                maxMana = 40;
                mana = maxMana;
                RedrawMana();
            }
            
        }
    }
    public void RedrawHearts()
    {
        ClearHearts();
        DrawHearts();
    }
    public void RedrawMana()
    {
        ClearMana();
        DrawMana();
    }
    private IEnumerator ShowBuffText(int delay)
    {
        buffText.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        buffText.gameObject.SetActive(false);
    }
    private IEnumerator FlashColor(float interval)
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(interval);
        GetComponent<SpriteRenderer>().color = originalColor;
    }
    public void UpdateMana(float manaCost)
    {
        mana -= manaCost;
        ClearMana();
        DrawMana();
    }
    public void GameOver() {Time.timeScale = 0; Thread.Sleep(500); SceneManager.LoadScene("GameOver"); }
}
