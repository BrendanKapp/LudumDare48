using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> screens;
    /**
     * 0 is the intro screen
     * 1 is the main menu
     * 2 is the game active screen
     * 3 is the game over screen
     * **/
    private int currentScreen = 0;
    private SaveFloat highScore;
    private SaveFloat currentScreenSize;
    private bool isGamePaused = false;
    [SerializeField]
    private CanvasGroup fadeToBlackCanvasGroup;
    [SerializeField]
    private GameObject soundHolder; //holds all the sound objects
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text highScoreText;
    [SerializeField]
    private GameObject newHighScore;
    [SerializeField]
    private Text screenSizeText;
    [SerializeField]
    private Text byWhoText;
    [SerializeField]
    private Text soundText;

    public static bool soundActive = true;

    public delegate void SoundAction();
    public static SoundAction SoundEvent;
    public static UIManager main;

    private void Awake()
    {
        currentScreenSize = new SaveFloat("iCurrentScreenSize");
        highScore = new SaveFloat("iHighScore");
        for (int i = 0; i < screens.Count; i++)
        {
            screens[i].SetActive(false);
        }
        main = this;
    }
    private void Start()
    {
        StartCoroutine("IntroSequence");
        UpdateScreenSize();
    }
    /*
     * @desc sets the UI to the target screen
     * @param target, the screen to show
     */
    public void SetScreen(int target)
    {
        PlayMouseClick();
        screens[currentScreen].SetActive(false);
        screens[target].SetActive(true);
        currentScreen = target;
    }
    public void SetHighScore(float newScore)
    {
        scoreText.text = "Score: " + FloatWithTwoDecimals(newScore);
        highScoreText.text = "High Score: " + FloatWithTwoDecimals(highScore.value);
        if (newScore > highScore.value)
        {
            highScore.value = newScore;
            newHighScore.SetActive(true);
            highScoreText.text = "*High Score: " + FloatWithTwoDecimals(highScore.value) + "*";
        }
    }
    /*
     * @desc pauses the game
     * @param pauseGame, true = pauses game
     */
    public void Pause(bool pauseGame)
    {
        PlayMouseClick();
        isGamePaused = pauseGame;
        if (pauseGame)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }
    }
    /*
     * @desc the intro sequence that is played once the player clicks play
     */
    private IEnumerator IntroSequence()
    {
        SetScreen(0);
        InstantToBlack(true);
        FadeToBlack(false);
        #if UNITY_EDITOR
        yield return new WaitForSeconds(0.1f);
        #else
        yield return new WaitForSeconds(3);;
        #endif
        SetScreen(1);
    }
    /*
     * @desc used for when the player clicks the play button
     */
    public void Play()
    {
        Invoke("StartGame", 0.1f);
    }
    private void StartGame()
    {
        SetScreen(2);
        GameController.main.StartGame();
    }
    public void ReturnToMainMenu()
    {
        SetScreen(1);
    }
    public void ShowEndGame (string byWho)
    {   
        byWhoText.text = "\"" + byWho + "\"";
        SetScreen(3);
    }
    public void SetSound ()
    {
        PlayMouseClick();
        if (soundActive)
        {
            soundActive = false;
            soundHolder.SetActive(false);
            soundText.text = "Sound Off";
        } else
        {
            soundActive = true;
            soundHolder.SetActive(true);
            soundText.text = "Sound On";
        }
        SoundEvent?.Invoke();
    }
    private void PlayMouseClick()
    {
        AudioSource audioSource = ObjectPooler.PoolObject("Sounds", "MouseClick").GetComponent<AudioSource>();
        audioSource.Play();
    }
    public void SetScreenSize ()
    {
        PlayMouseClick();
        currentScreenSize.value++;
        currentScreenSize.value = currentScreenSize.value == 3 ? 0 : currentScreenSize.value;
        UpdateScreenSize();
    }
    private void UpdateScreenSize ()
    {
        switch (currentScreenSize.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
                screenSizeText.text = "1080p";
                break;
            case 1:
                Screen.SetResolution(1280, 720, FullScreenMode.ExclusiveFullScreen);
                screenSizeText.text = "720p";
                break;
            case 2:
                Screen.SetResolution(858, 480, FullScreenMode.ExclusiveFullScreen);
                screenSizeText.text = "480p";
                break;
        }
    }

    public void FadeToBlack(bool inOrOut)
    {
        StartCoroutine(FadeToBlackRoutine(inOrOut));
    }
    public void InstantToBlack(bool inOrOut)
    {
        fadeToBlackCanvasGroup.alpha = inOrOut ? 1 : 0;
    }
    private IEnumerator FadeToBlackRoutine(bool inOrOut)
    {
        for (int i = 0; i < 20; i++)
        {
            fadeToBlackCanvasGroup.alpha = inOrOut ? i / 20 : 1 - (i / 20);
            yield return new WaitForSeconds(0.05f);
        }
        InstantToBlack(inOrOut);
    }
    private float FloatWithTwoDecimals(float value)
    {
        return ((int)(value * 100) / 100f);
    }
    public void Quit()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
