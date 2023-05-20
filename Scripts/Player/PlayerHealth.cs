using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
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

    void Start() => this.health = _MaxHealth;
    void IDamageHandler.TakeDamage(float dmg, Vector2 kb)
    {
        rb.AddForce(kb);
        this.health -= dmg;

        RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
        textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        textTransform.SetParent(canvas.transform);
        
        TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "" + dmg;
        text.color = new Color(1, 0, 0, 1);


        if (this.health <= 0)
        {
            rb.simulated = false;
            animator.SetTrigger("dead");
        }
        print(this.health);

    }

    void IDamageHandler.TakeDamage(float dmg)
    {
        throw new System.NotImplementedException();
    }

    public void GameOver() {Time.timeScale = 0; Thread.Sleep(500); SceneManager.LoadScene("GameOver"); }
}
