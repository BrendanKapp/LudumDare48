using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController main;
    [SerializeField]
    private float hookPower = 50;
    [SerializeField]
    private float pullPower = 50;
    [SerializeField]
    private HookController hook;
    [SerializeField]
    private Transform shootTarget;

    private bool hookActive = false;
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
    private void PullTowards()
    {
        Vector3 dir = (hook.transform.position - transform.position);
        characterController.Move(dir * pullPower * Time.fixedDeltaTime);
    }
    private void ShootHook()
    {
        hook.rb.position = shootTarget.position;
        Vector3 dir = shootTarget.transform.forward.normalized * hookPower;
        hook.rb.velocity = dir;
        hook.Shoot();
    }
    private void RetractHook()
    {

    }
    public void TakeDamage()
    {
        firstPersonController.enabled = false;
        GameController.main.StopGame();
        UIManager.main.ShowEndGame();
        firstPersonController.EnableCursor();
    }
    public void Activate()
    {
        firstPersonController.enabled = true;
        firstPersonController.DisableCursor();
    }
    private IEnumerator RetrackHookRoutine()
    {
        float distance = (hook.transform.position - shootTarget.transform.position).sqrMagnitude;
        //while (hook.transform.position )
        yield return null;
    }
}
