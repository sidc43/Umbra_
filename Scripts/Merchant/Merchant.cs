using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Merchant : MonoBehaviour
{
    public Sprite promptSprite;
    public GameObject promptContainer;
    public Canvas shop;
    public Weapon throwingAxe;

    PlayerController player;
    PlayerHealth playerHealth;
    private bool inRange;
    private const int healthPrice = 10;
    private const int axePrice = 50;
    
    private void Update()
    {
        bool eKeyPressed = Keyboard.current.eKey.wasPressedThisFrame;

        if (eKeyPressed && inRange)
        {
            if (shop.gameObject.activeSelf)
            {
                ResumeGame();
                Cursor.visible = false;
                shop.gameObject.SetActive(false);
            }
            else    
            {
                PauseGame();
                Cursor.visible = true;
                shop.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player")
        {
            promptContainer.SetActive(true);
            promptContainer.GetComponent<SpriteRenderer>().sprite = promptSprite;
            player = other.GetComponent<PlayerController>();
            playerHealth = other.GetComponent<PlayerHealth>();
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        promptContainer.SetActive(false);
        if (other.tag == "Player")
            inRange = false;    
    }

    private void PauseGame() => Time.timeScale = 0;
    private void ResumeGame() => Time.timeScale = 1;

    public void BuyHealth()
    {
        if (player.Coins() >= healthPrice)
        {
            playerHealth.health = playerHealth.maxHealth;
            playerHealth.DrawHearts();
            player.UpdateCoinCountDec(healthPrice);
        }
    }

    public void BuyThrowingAxe()
    {
        List<Transform> slots = new List<Transform>();
        if (player.Coins() >= axePrice)
        {
            for (int i = 0; i < player.weaponsMenu.transform.childCount; i++)
            {
                var slot = player.weaponsMenu.transform.GetChild(i);
                if (slot.GetChild(0).GetComponent<Image>().sprite == null)
                {
                    slots.Add(slot);
                }
            }
            player.weapons.Add(throwingAxe);
            slots[0].GetChild(0).GetComponent<Image>().sprite = throwingAxe.sprite;
            slots[0].GetChild(0).GetComponent<WeaponInSlot>().weapon = throwingAxe;
            slots[0].GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = new Vector2(40, 90);   
            slots[0].GetChild(0).GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, 11);
            player.UpdateCoinCountDec(axePrice);
            player.UpdateCurrentWeapon(throwingAxe);
        }
    }
}
