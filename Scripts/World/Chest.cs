using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    public Animator animator;
    public Canvas rewardsCanvas;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        print(other.name);
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
            
            if (hit2D.collider != null)
            {
                animator.SetTrigger("open");
                PauseGame();
            }
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ResumeGame();
    }

    private void OnTriggerExit2D(Collider2D other) => ResumeGame();
        
    private void PauseGame()
    {
        Cursor.visible = true;
        rewardsCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    private void ResumeGame() 
    {
        rewardsCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
    }
}
