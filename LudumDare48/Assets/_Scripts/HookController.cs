using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Shoot()
    {
        rb.isKinematic = false;
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") return;
        rb.isKinematic = true;
    }
}
