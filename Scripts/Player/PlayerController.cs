using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float collisionOffset;
    public SwordAttack swordAttack;
    public ContactFilter2D movementFilter;
    public TextMeshProUGUI coinText;

    private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();
    private Vector2 _movementInput;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private int _coins;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate() 
    {
        if (_movementInput != Vector2.zero)
        {
            int count = _rb.Cast(_movementInput, movementFilter, _castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
                _rb.MovePosition(_rb.position + _movementInput * moveSpeed * Time.fixedDeltaTime);

            _animator.SetBool("IsMoving", true);
        } 
        else
            _animator.SetBool("IsMoving", false);

        if (_movementInput.x < 0)
            _spriteRenderer.flipX = true;
        else if (_movementInput.x > 0)
            _spriteRenderer.flipX = false;
    }

    public void SwordAttack()
    {
        if (_spriteRenderer.flipX)
            swordAttack.AttackLeft();
        else
            swordAttack.AttackRight();

        print("attack");
    }

    public void UpdateCoinCount()
    {
        _coins++;
        coinText.text = "" + _coins;
    }

    public void StopSwordAttack() => swordAttack.StopAttack();

    void OnMove(InputValue movementValue) => _movementInput = movementValue.Get<Vector2>();

    private void OnFire() => _animator.SetTrigger("swordAttack");
}
