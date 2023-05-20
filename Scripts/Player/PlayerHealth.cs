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
    public const float _MaxHealth = 20f;
    public float health = _MaxHealth;
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject healthText;
    public Sprite heartFull;
    public Sprite heartHalf;
    public GameObject heartContainer;

    private int _numHearts;

    void Start()
    {
        this.health = _MaxHealth;
        DrawHearts();
    }
    void IDamageHandler.TakeDamage(float dmg, Vector2 kb)
    {
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
        }
        print(this.health);

    }

    void DrawHearts()
    {
        _numHearts = (int) health / 5;

        for (int i = 0; i < _numHearts; i++)
        {
           GameObject newHeart = new GameObject("" + i);
           newHeart.transform.position = heartContainer.transform.position;

           Image img = newHeart.AddComponent<Image>();
           img.sprite = heartFull;

           newHeart.transform.SetParent(heartContainer.transform);

           RectTransform rt = newHeart.GetComponent<RectTransform>();
           rt.sizeDelta = new Vector2(25, 25);
           rt.position = rt.position + new Vector3((i) - (50 * i), 0, 0);
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
