using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class Merchant : MonoBehaviour
{
    public Sprite promptSprite;
    public Sprite defaultSlotSprite;
    public GameObject promptContainer;
    public Canvas shop;
    public Weapon throwingAxe;
    public Weapon fireball;
    public TextMeshProUGUI healthPricetext;
    public TextMeshProUGUI manaPricetext;
    public TextMeshProUGUI axePriceText;
    public TextMeshProUGUI fireballPriceText;
    [SerializeField] private int healthPrice;
    [SerializeField] private int manaPrice;
    [SerializeField] private int axePrice;
    [SerializeField] private int fireballPrice;
    [SerializeField] private TextMeshProUGUI errorText;
    

    PlayerController player;
    PlayerHealth playerHealth;
    private bool inRange;
    private void Start() 
    {
        healthPricetext.text = "" + healthPrice;
        manaPricetext.text = "" + manaPrice;
        axePriceText.text = "" + axePrice;
        fireballPriceText.text = "" + fireballPrice;
    }
    private void Update()
    {
        bool eKeyPressed = Keyboard.current.eKey.wasPressedThisFrame;

        if (eKeyPressed && inRange)
        {
            if (shop.gameObject.activeSelf)
            {
                ResumeGame();
                player.canAttack = true;
            }
            else    
            {
                player.canAttack = false;
                PauseGame();
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
    private void PauseGame() 
    {
        Cursor.visible = true;
        shop.gameObject.SetActive(true);
        player.GetComponent<PlayerInput>().DeactivateInput();
    }
    private void ResumeGame() 
    {
        Cursor.visible = false;
        shop.gameObject.SetActive(false);
        player.GetComponent<PlayerInput>().ActivateInput();
    }
    public void BuyHealth()
    {
        if (player.Coins() >= healthPrice)
        {
            playerHealth.health = playerHealth.maxHealth;
            playerHealth.RedrawHearts();
            player.UpdateCoinCountDec(healthPrice);
        }
        else
            StartCoroutine(ShowErrorText(1));
    }
    public void BuyMana()
    {
        if (player.Coins() >= manaPrice)
        {
            playerHealth.mana = playerHealth.maxMana;
            playerHealth.RedrawMana();
            player.UpdateCoinCountDec(manaPrice);
        }
        else
            StartCoroutine(ShowErrorText(1));
    }
    public void BuyThrowingAxe() => BuyWeapon(throwingAxe, axePrice);
    public void BuyFireball() => BuyWeapon(fireball, fireballPrice);
    private void BuyWeapon(Weapon weapon, int weaponPrice)
    {
        if (player.Coins() >= weaponPrice && !player.weapons.Contains(weapon))
        {
            for (int i = 0; i < player.slots.Length; i++)
            {
                if (player.slots[i].GetComponent<Image>().sprite == defaultSlotSprite)
                {
                    player.AddWeapon(weapon, i);
                    break;
                }
            }
            player.UpdateCoinCountDec(weaponPrice);
        }
        else
            StartCoroutine(ShowErrorText(1));
    }
    private IEnumerator ShowErrorText(float delay)
    {
        errorText.gameObject.SetActive(true);
        errorText.text = "You don't have enough Coins!";
        yield return new WaitForSeconds(delay);
        errorText.gameObject.SetActive(false);
    }
}
