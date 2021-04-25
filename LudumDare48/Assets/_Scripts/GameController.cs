using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController main;
    private bool isPlaying = true;

    [SerializeField]
    private TerrainGenerator terrainGenerator;

    [SerializeField]
    private string spikeName;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private MinotaurController minotaurController;

    private void Start()
    {
        main = this;
    }
    private Coroutine gameRoutine;
    public void StartGame()
    {
        gameRoutine = StartCoroutine(GameRoutine());
        ObjectPooler.DePoolAll("main");
        startTime = Time.time;
        PlayerController.main.Activate();
        minotaurController.gameObject.SetActive(true);
        minotaurController.transform.position = Vector3.down * -100;
    }
    public void StopGame()
    {
        minotaurController.gameObject.SetActive(false);
        StopCoroutine(gameRoutine);
        UIManager.main.SetHighScore(Time.time - startTime);
    }
    private IEnumerator GameRoutine()
    {
        while (true)
        {
            DropSpikes();
            UpdateScore();
            yield return new WaitForSeconds(0.25f);
        }
    }
    private float range = 25f;
    private void DropSpikes()
    {
        if (Random.value > 0.7f) return;
        SpikeController spike = ObjectPooler.PoolObject(spikeName).GetComponent<SpikeController>();
        spike.transform.position = terrainGenerator.ConvertMapToWorld(2, 1, 2) + new Vector3(Random.Range(-range, range), 20, Random.Range(-range, range)); // lazy center
        spike.transform.localScale = Vector3.one * Random.Range(1f, 2f);
        spike.gameObject.SetActive(true);
        spike.Activate();
    }
    private float startTime;
    private void UpdateScore()
    {
        scoreText.text = "Time: " + (int)(Time.time - startTime);
    }
}
