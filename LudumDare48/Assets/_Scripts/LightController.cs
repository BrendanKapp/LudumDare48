using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField]
    private float maxTime = 0.15f;
    [SerializeField]
    private float minTime = 0.05f;
    [SerializeField]
    private LightStats[] lights;
    private Coroutine lightRoutine;

    private void OnEnable()
    {
        lightRoutine = StartCoroutine(LightRoutine());
    }
    private void OnDisable()
    {
        if (lightRoutine != null) StopCoroutine(lightRoutine);
    }
    private IEnumerator LightRoutine()
    {
        while (true)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].light.intensity = Random.Range(lights[i].minIntensity, lights[i].maxIntensity);
            }
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}
[System.Serializable]
public struct LightStats {
    public float maxIntensity;
    public float minIntensity;
    public Light light;
    public LightStats(float maxIntensity, float minIntensity)
    {
        this.maxIntensity = maxIntensity;
        this.minIntensity = minIntensity;
        this.light = null;
    }
}
