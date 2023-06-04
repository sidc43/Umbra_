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
    public  float maxHealth = 20f;
    public float health;
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject healthText;
    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;
    public GameObject heartContainer;
    public AudioManager audioManager;

    private int _numHearts;

    void Start()
    {
        this.health = maxHealth;
        DrawHearts();
    }
    void IDamageHandler.TakeDamage(float dmg, Vector2 kb)
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

        if (this.health <= 0)
        {
            rb.simulated = false;
            animator.SetTrigger("dead");
            audioManager.Play("PlayerDeath");
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
           rt.sizeDelta = new Vector2(40, 40);
           rt.position = rt.position + new Vector3((55 * i), 0, 0);
        }

        for (int i = 0; i < _numHearts; i++)
        {
           GameObject newHeart = new GameObject("" + i);
           newHeart.transform.position = heartContainer.transform.position;

           Image img = newHeart.AddComponent<Image>();
           img.sprite = heartFull;

           newHeart.transform.SetParent(heartContainer.transform);

           RectTransform rt = newHeart.GetComponent<RectTransform>();
           rt.sizeDelta = new Vector2(40, 40);
           rt.position = rt.position + new Vector3((55 * i), 0, 0);
        }
    }

    void ClearHearts() 
    {
        foreach (Transform child in heartContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void IDamageHandler.TakeDamage(float dmg)
    {
        throw new System.NotImplementedException();
    }

    public void GameOver() {Time.timeScale = 0; Thread.Sleep(500); SceneManager.LoadScene("GameOver"); }
}
