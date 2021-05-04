using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
public class SoundManager : MonoBehaviour
{
    public static AudioSource PlaySoundForPlayer (string soundID)
    {
        AudioSource audioSource = ObjectPooler.PoolObject("Sounds", soundID).GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.transform.position = PlayerController.main.transform.position;
        return audioSource;
    }
    public static AudioSource GetSound(string soundID)
    {
        return ObjectPooler.PoolObject("Sounds", soundID).GetComponent<AudioSource>();
    }
    public static AudioSource GetRandomizedSound(string soundID, float pitchLow, float pitchHigh, float volumeDiv, float volumeAdd)
    {
        AudioSource audioSource = ObjectPooler.PoolObject("Sounds", soundID).GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(pitchLow, pitchHigh);
        audioSource.volume = Random.value / volumeDiv + volumeAdd;
        return audioSource;
    }
}
