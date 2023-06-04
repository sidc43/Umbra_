using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region Public
    [Header("Attributes")]
    public float moveSpeed;
    public float collisionOffset;
    public SwordAttack swordAttack;
    public ContactFilter2D movementFilter;
    public int level;
    public int exp;
    public int expForNextLvl;

    [Header("Weapons")]
    public GameObject weaponsMenu;
    public Weapon startingWeapon;
    public List<Weapon> weapons = new List<Weapon>(3);
    public Weapon currentWeapon;

    [Header("UI")]
    public Image currWeaponImage;
    public TextMeshProUGUI coinText;
    public Slider levelSlider;
    public TextMeshProUGUI levelText;

    [Header("Sounds")]
    public AudioManager audioManager;
    #endregion

    #region Private
    private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();
    private Vector2 _movementInput;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private int _coins;
    private bool weaponMenuOn;
    [Header("Room")]
    [SerializeField] private RoomFirstDungeonGenerator room;
    #endregion
    
    private void Start()
    {
        exp = 0;
        level = 0;
        expForNextLvl = 10;
        levelSlider.maxValue = expForNextLvl;
        levelSlider.value = exp;
        levelText.text = "" + level;

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();  

        audioManager.Play("Theme"); 

        weapons.Add(startingWeapon);
        UpdateCurrentWeapon(startingWeapon);
        UpdateCoinCountInc(50);

        Cursor.visible = false;
        room.GenerateDungeon();
        transform.position = room.centers[Random.Range(0, room.centers.Count - 1)];
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
        if (Keyboard.current.qKey.wasPressedThisFrame) 
        {
            if (weaponMenuOn)
                CloseWeaponMenu();
            else    
                OpenWeaponsMenu();
        }
    }
    public void UpdateCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        currWeaponImage.sprite = weapon.sprite;
    }
    private void CloseWeaponMenu()
    {
        weaponMenuOn = false;
        Cursor.visible = false;
        weaponsMenu.SetActive(false);
        Time.timeScale = 1;
    }
    private void OpenWeaponsMenu()
    {
        weaponMenuOn = true;
        Cursor.visible = true;
        weaponsMenu.SetActive(true);
        Time.timeScale = 0;
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

    public void SelectWeapon(GameObject child) 
    {
        this.currentWeapon = child.GetComponent<WeaponInSlot>().weapon;
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
        if (currentWeapon.type == Weapon.Type.Melee)
            _animator.SetTrigger("swordAttack");
    } 
    public int Coins() => _coins;
}
