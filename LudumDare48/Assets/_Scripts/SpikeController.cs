using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [SerializeField]
    private string[] damageLines;
    [SerializeField]
    private string hitEffectName = "HitEffect";
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }
    private bool isActivated = false;
    public void Activate()
    {
        boxCollider.enabled = false;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        StartCoroutine(Fade(10, true));
        //transform.parent = null;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!isActivated) return;
        isActivated = false;
        rb.isKinematic = true;
        //transform.parent = other.transform;
        ParticleSystem hitEffect = ObjectPooler.PoolObject(hitEffectName).GetComponent<ParticleSystem>();
        hitEffect.transform.position = transform.localPosition;
        hitEffect.Play();
        float distance = (PlayerController.main.transform.position - transform.position).sqrMagnitude;
        if (distance < 400)
        {
            AudioSource rockSmashSound = Sound.GetSound("RockSmash" + Random.Range(1, 4));
            rockSmashSound.pitch = Random.Range(0.2f, 0.5f);
            rockSmashSound.volume = Random.value / 4f + 0.3f;
            rockSmashSound.Play();
        }
        if (distance < 100)
        {
            CameraRotate.screenShakeAmount += (int)(500 / distance);
        }
        FadeInstant();
        if (distance < 10)
        {
            PlayerController.main.TakeDamage(damageLines[Random.Range(0, damageLines.Length - 1)]);
        }
    }
    private IEnumerator Fade(int time, bool setActive)
    {
        for (int i = 0; i < time; i++)
        {
            transform.Translate(0, -0.2f, 0);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(setActive);
        rb.isKinematic = false;
        boxCollider.enabled = true;
        isActivated = true;
    }
    private void FadeInstant()
    {
        gameObject.SetActive(false);
        rb.isKinematic = false;
        boxCollider.enabled = true;
        isActivated = true;
    }
}
