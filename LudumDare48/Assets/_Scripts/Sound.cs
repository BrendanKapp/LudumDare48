using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public static GameObject PlaySound (string soundID)
    {
        AudioSource audioSource = ObjectPooler.PoolObject("Sounds", soundID).GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.transform.position = PlayerController.main.transform.position;
        return audioSource.gameObject;
    }
    public static AudioSource GetSound(string soundID)
    {
        return ObjectPooler.PoolObject("Sounds", soundID).GetComponent<AudioSource>();
    }
}
