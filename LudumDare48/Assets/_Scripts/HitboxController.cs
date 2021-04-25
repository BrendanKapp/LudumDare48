using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField]
    private float scale = 3f;
    public void Activate()
    {
        transform.localScale = Vector3.one * scale;
        StartCoroutine(ActivateRoutine());
    }
    private IEnumerator ActivateRoutine()
    {
        for (int i = 0; i < 30; i++)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, 4f * scale, 0.2f);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 30; i > 0; i--)
        {
            transform.localScale = Vector3.one * scale * 4f * (i / 30f);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().TakeDamage();
        }
    }
}
