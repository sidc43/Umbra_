using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Rendering.Universal;
using System;

public class PlayerController : MonoBehaviour
{
    #region Public
    [Header("Attributes")]
    public float moveSpeed;
    public float collisionOffset;
    public SwordAttack swordAttack;
    public ContactFilter2D movementFilter;
    public PlayerHealth playerHealth;
    public int level;
    public int exp;
    public int expForNextLvl;
    public bool canAttack;

    [Header("Weapons")]
    public Weapon startingWeapon;
    public List<Weapon> weapons = new List<Weapon>(3);
    public Weapon currentWeapon;
    public GameObject[] slots;

    [Header("UI")]
    public TextMeshProUGUI coinText;
    public Slider levelSlider;
    public TextMeshProUGUI levelText;

    [Header("Sounds")]
    public AudioManager audioManager;
    #endregion
    #region Private
    [SerializeField] TextMeshProUGUI errorText;
    private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();
    private Vector2 _movementInput;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private int _coins;
    private bool weaponMenuOn;
    private float rangedCooldown;
    private float lastRangeTime;

    [Header("Room")]
    [SerializeField] private RoomFirstDungeonGenerator room;
    #endregion
    private void Start()
    {
        Application.targetFrameRate = 300;
        exp = 0;
        level = 0;
        expForNextLvl = 10;
        levelSlider.maxValue = expForNextLvl;
        levelSlider.value = exp;
        levelText.text = "" + level;
        canAttack = true;

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();  

        audioManager.Play("Theme"); 
        UpdateCoinCountInc(100);

        AddWeapon(startingWeapon, 0);
        currentWeapon = startingWeapon;

        Cursor.visible = false;
        room.GenerateDungeon();
        transform.position = room.centers[UnityEngine.Random.Range(0, room.centers.Count - 1)];
    }
    private void FixedUpdate() 
    {
        if (_movementInput != Vector2.zero)
        {
            int count = _rb.Cast(_movementInput, movementFilter, _castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
                _rb.MovePosition(_rb.position + _movementInput * moveSpeed * Time.fixedDeltaTime);

            _animator.SetBool("IsMoving", true);
            if (!audioManager.GetSound("PlayerWalk").source.isPlaying)
                audioManager.Play("PlayerWalk");
        } 
        else
        {
            _animator.SetBool("IsMoving", false);
        }

        if (_movementInput.x < 0)
            _spriteRenderer.flipX = true;
        else if (_movementInput.x > 0)
            _spriteRenderer.flipX = false;

    }
    private void Update()
    {
        if (Mouse.current.scroll.ReadValue().y == 120) // forward
        {
            CycleForward();
        }

        if (Mouse.current.scroll.ReadValue().y == -120) // backward
        {
            CycleBackward();
        }
    }
    private void CycleBackward()
    {
        int slotCount = 0;
        foreach (GameObject slot in slots)
        {
            if (slot.GetComponent<WeaponInSlot>() != null)
                slotCount++;
        }
        Weapon[] currentItems = new Weapon[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            currentItems[i] = slots[i].GetComponent<WeaponInSlot>().weapon;
        }

        Weapon lastItem = currentItems[slotCount - 1];
        Array.Copy(currentItems, 0, currentItems, 1, slotCount - 1);
        currentItems[0] = lastItem;

        for (int i = 0; i < slotCount; i++)
        {
            slots[i].GetComponent<WeaponInSlot>().weapon = currentItems[i];
            slots[i].GetComponent<Image>().sprite = currentItems[i].sprite;
        }

        currentWeapon = slots[0].GetComponent<WeaponInSlot>().weapon;
    }
    private void CycleForward()
    {
        int slotCount = 0;
        foreach (GameObject slot in slots)
        {
            if (slot.GetComponent<WeaponInSlot>() != null)
                slotCount++;
        }
        Weapon[] currentItems = new Weapon[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            currentItems[i] = slots[i].GetComponent<WeaponInSlot>().weapon;
        }

        Weapon firstItem = currentItems[0];
        Array.Copy(currentItems, 1, currentItems, 0, slotCount - 1);
        currentItems[slotCount - 1] = firstItem;

        for (int i = 0; i < slotCount; i++)
        {
            slots[i].GetComponent<WeaponInSlot>().weapon = currentItems[i];
            slots[i].GetComponent<Image>().sprite = currentItems[i].sprite;
        }

        currentWeapon = slots[0].GetComponent<WeaponInSlot>().weapon;
    }
    public void AddWeapon(Weapon weapon, int slotNum)
    {
        weapons.Add(weapon);
        slots[slotNum].GetComponent<Image>().sprite = weapon.sprite;
        if (slots[slotNum].GetComponent<WeaponInSlot>() == null)
            slots[slotNum].AddComponent<WeaponInSlot>().weapon = weapon;

        slots[slotNum].GetComponent<Image>().sprite = weapon.sprite;
    }
    public void CheckLevelUp()
    {
        if (exp >= expForNextLvl)
        {
            level++;
            expForNextLvl *= 3;
            levelSlider.minValue = exp;
        }
        levelSlider.maxValue = expForNextLvl;
        levelSlider.value = exp;
        levelText.text = "" + level;
    }
    public void SwordAttack()
    {
        audioManager.Play("PlayerAttack");
        if (_spriteRenderer.flipX)
            swordAttack.AttackLeft();
        else
            swordAttack.AttackRight();
    }
    public void UpdateCoinCount()
    {
        audioManager.Play("Coin");
        _coins++;
        coinText.text = "" + _coins;
    }
    public void UpdateCoinCountInc(int coins)
    {
        _coins += coins;
        coinText.text = "" + _coins;
    }
    public void UpdateCoinCountDec(int coins)
    {
        _coins -= coins;
        coinText.text = "" + _coins;
    }
    public void StopSwordAttack() => swordAttack.StopAttack();
    void OnMove(InputValue movementValue) => _movementInput = movementValue.Get<Vector2>();
    private void OnFire()
    {
        if (canAttack)
        {
            switch (currentWeapon.type)
            {
                case Weapon.Type.Melee:
                    _animator.SetTrigger("swordAttack");
                    break;
                case Weapon.Type.Ranged:
                    ThrowAxe();
                    break;
                case Weapon.Type.Magic:
                    ThrowFireball();
                    break;
            }
        }
    }
    private void ThrowAxe()
    {
        rangedCooldown = currentWeapon.cooldown;
        if (Time.time - lastRangeTime >= rangedCooldown)
        {
            Vector2 mouseCursorePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = mouseCursorePos - _rb.position;
            lookDir.Normalize();

            GameObject throwingAxe = null;
            if (mouseCursorePos.x < transform.position.x && mouseCursorePos.y < transform.position.y)
            {
                throwingAxe = Instantiate(currentWeapon.instance, new Vector2(transform.position.x - 1, transform.position.y), Quaternion.identity);
            }
            else if (mouseCursorePos.x > transform.position.x && mouseCursorePos.y < transform.position.y)
            {
                throwingAxe = Instantiate(currentWeapon.instance, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity);
            }
            else if (mouseCursorePos.y < transform.position.y)
            {
                throwingAxe = Instantiate(currentWeapon.instance, new Vector2(transform.position.x, transform.position.y - 1), Quaternion.identity);
            }
            else if (mouseCursorePos.y > transform.position.y)
            {
                throwingAxe = Instantiate(currentWeapon.instance, new Vector2(transform.position.x, transform.position.y + 0.4f), Quaternion.identity);
            }
            else
                throwingAxe = Instantiate(currentWeapon.instance, transform.position, Quaternion.identity);

            throwingAxe.GetComponent<SpriteRenderer>().flipY = (mouseCursorePos.x < transform.position.x);
            throwingAxe.GetComponent<Rigidbody2D>().velocity = lookDir * currentWeapon.speed;
            throwingAxe.transform.Rotate(0f, 0f, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);

            audioManager.Play("ThrowingAxe");
            lastRangeTime = Time.time;

            Destroy(throwingAxe, currentWeapon.range); 
        }
    }
    private void ThrowFireball()
    {
        rangedCooldown = currentWeapon.cooldown;
        if (Time.time - lastRangeTime >= rangedCooldown)
        {
            Vector2 mouseCursorePos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir1 = mouseCursorePos1 - _rb.position;
            lookDir1.Normalize();

            if (playerHealth.mana >= currentWeapon.manaCost)
            {
                GameObject fireball = null;

                if (mouseCursorePos1.x < transform.position.x && mouseCursorePos1.y < transform.position.y)
                {
                    fireball = Instantiate(currentWeapon.instance, new Vector2(transform.position.x - .3f, transform.position.y), Quaternion.identity);
                }
                else if (mouseCursorePos1.x > transform.position.x && mouseCursorePos1.y < transform.position.y)
                {
                    fireball = Instantiate(currentWeapon.instance, new Vector2(transform.position.x + .3f, transform.position.y), Quaternion.identity);
                }
                else if (mouseCursorePos1.y < transform.position.y)
                {
                    fireball = Instantiate(currentWeapon.instance, new Vector2(transform.position.x, transform.position.y - 2), Quaternion.identity);
                }
                else if (mouseCursorePos1.y > transform.position.y)
                {
                    fireball = Instantiate(currentWeapon.instance, new Vector2(transform.position.x, transform.position.y + 0.4f), Quaternion.identity);
                }
                else
                    fireball = Instantiate(currentWeapon.instance, transform.position, Quaternion.identity);

                
                fireball.GetComponent<SpriteRenderer>().flipY = (mouseCursorePos1.x < transform.position.x);
                fireball.GetComponent<Rigidbody2D>().velocity = lookDir1 * currentWeapon.speed;
                fireball.transform.Rotate(0f, 0f, Mathf.Atan2(lookDir1.y, lookDir1.x) * Mathf.Rad2Deg);

                audioManager.Play("Fireball");

                playerHealth.UpdateMana(currentWeapon.manaCost);
                lastRangeTime = Time.time;

                Destroy(fireball, currentWeapon.range);
            }
            else
                StartCoroutine(ShowErrorText(1));
        }
        
    }
    private IEnumerator ShowErrorText(float delay)
    {
        errorText.gameObject.SetActive(true);
        errorText.text = "You don't have enough Mana!";
        yield return new WaitForSeconds(delay);
        errorText.gameObject.SetActive(false);
    }
    public int Coins() => _coins;
}