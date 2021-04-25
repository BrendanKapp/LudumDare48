using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }
    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         if (!hookActive)
    //         {
    //             ShootHook();
    //         } else {
    //             RetractHook();
    //         }
    //         hookActive = !hookActive;
    //     }
    // }
    // private void FixedUpdate()
    // {
    //     if (Input.GetMouseButton(1))
    //     {
    //         PullTowards();
    //     }
    // }
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
        print("Player Lost");
    }
    private IEnumerator RetrackHookRoutine()
    {
        float distance = (hook.transform.position - shootTarget.transform.position).sqrMagnitude;
        //while (hook.transform.position )
        yield return null;
    }
}
