using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject upgradePopup; 
    public GameObject minimap;
    private bool isPaused = false;

    // for game time 
    private float timeElapsed = 0.0f;
    public Text gameTimeText;
    public Text uiMessages;
    private int completedCheckpoints = 0;

    // for end game popup 
    private bool isGameOver = false;
    public GameObject endGamePopup;
    private Button playAgainButton;
    private Button backToHomeButton;
    private Text endGameText;
    private Text lifetimeText;

    // for singal weapon tracking 
    public Slider p1SignalBar;
    public Slider p2SignalBar;

    // for checkpoint tracking
    private float checkpointCooldown = 5.0f;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // preserves between scenes (if needed)
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
        SoundManager.Instance.PlayBackgroundMusic("background_music");

        // find items for end game popup
        endGameText = endGamePopup.transform.Find("Popup/EndGameText").GetComponent<Text>();
        lifetimeText = endGamePopup.transform.Find("Popup/LifetimeText").GetComponent<Text>();
        playAgainButton = endGamePopup.transform.Find("Popup/PlayAgainBtn").GetComponent<Button>();
        backToHomeButton = endGamePopup.transform.Find("Popup/BackToHomeBtn").GetComponent<Button>();

        playAgainButton.onClick.AddListener(PlayAgain);
        backToHomeButton.onClick.AddListener(BackToHome);

        endGamePopup.SetActive(false);
    }
    

    private void Update()
    {
        if (!isGameOver && !isPaused)
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerUI();
        }

        // restart
        if (Input.GetKeyDown(KeyCode.R)) {
            EndGame(false);
        }

        // handle minimap
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            isPaused = true;
            Time.timeScale = 0; 
            AudioListener.pause = true;
            minimap.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Escape)) 
        {
            isPaused = false;
            Time.timeScale = 1; 
            minimap.SetActive(false);
            AudioListener.pause = false; 
        }

        if (checkpointCooldown >= 0)
        {
            checkpointCooldown -= Time.deltaTime;
        }
    }

    private void UpdateTimerUI()
    {
        if (gameTimeText != null)
        {
            int minutes = Mathf.FloorToInt(timeElapsed / 60);
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; 
        AudioListener.pause = true;
        upgradePopup.SetActive(true); 
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1; 
        upgradePopup.SetActive(false);
        AudioListener.pause = false; 
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
        UpdateTimerUI();
    }

    public float GetCurrentTimeElapsed()
    {
        return timeElapsed;
    }


    // helper functions to assist with displaying temporary UI messages
    public void ShowTemporaryMessage(string message, float duration)
    {
        StartCoroutine(DisplayMessageCoroutine(message, duration));
    }

    private IEnumerator DisplayMessageCoroutine(string message, float duration)
    {
        uiMessages.text = message;
        yield return new WaitForSeconds(duration);
        
        // prevents overwriting other messages if it has changed
        if (uiMessages.text == message)
        {
            uiMessages.text = "";
        }
    }


    // displays end of game popup, player chooses to play again or return home
    public void EndGame(bool won)
    {
        isGameOver = true;
        Time.timeScale = 0;

        endGamePopup.SetActive(true);
        endGameText.text = won ? "You Win!" : "You Lose!";
        SoundManager.Instance.PlaySound(won ? "win" : "lose");

        int minutes = Mathf.FloorToInt(timeElapsed / 60);
        int seconds = Mathf.FloorToInt(timeElapsed % 60);
        lifetimeText.text = string.Format("You survived {0:00}:{1:00} minutes", minutes, seconds);

        playAgainButton.interactable = true;
        backToHomeButton.interactable = true;
        
        // for logging data
        GameLogger.LogGameData(timeElapsed, completedCheckpoints);
    }


    // destroys everything, then restarts the current scene 
    public void PlayAgain()
    {
        SoundManager.Instance.PlaySound("toggle");
        Time.timeScale = 1;

        // restart scene
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Persistent")) 
                continue;

            Destroy(obj);
        }

        Instance = null;
        SceneManager.LoadScene("Game");
    }

    public void BackToHome()
    {
        Time.timeScale = 1;
        SoundManager.Instance.PlaySound("toggle");
        SceneManager.LoadScene("Home"); 
    }

    public Slider GetPlayer1SignalBar()
    {
        return p1SignalBar;
    }

    public Slider GetPlayer2SignalBar()
    {
        return p2SignalBar;
    }

    public int GetNumCheckpointsCompleted()
    {
        return completedCheckpoints;
    }

    public void CheckpointCompleted()
    {
        if (checkpointCooldown <= 0)
        {
            Debug.Log("PICKED UP CHECKPOINT");
            completedCheckpoints++;
            Debug.Log("COMPLETED CHECKPOINTS:" + completedCheckpoints);
            checkpointCooldown = 5.0f;
        }

        if (completedCheckpoints >= 3)
        {
            EndGame(true);
        }
    }
}
