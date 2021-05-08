using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController main;
    private Rigidbody rb;
    private CharacterController characterController;
    private FirstPersonController firstPersonController;

    private void Awake()
    {
        main = this;
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();
    }
    private void Start()
    {
        firstPersonController.enabled = false;
        firstPersonController.EnableCursor();
    }
    public void TakeDamage(string byWho)
    {
        firstPersonController.enabled = false;
        GameController.main.StopGame();
        UIManager.main.ShowEndGame(byWho);
        firstPersonController.EnableCursor();
    }
    public void Activate()
    {
        firstPersonController.enabled = true;
        firstPersonController.DisableCursor();
    }
}
