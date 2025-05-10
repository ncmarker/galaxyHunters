using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public Text countdownText;
    private int countdownTime = 15;

    void Start()
    {
        StartCoroutine(CountdownAndLoad());
    }

    void Update()
    {
        // press space to skip countdown
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySound("toggle");
            StopAllCoroutines();
            SoundManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene("Game");
        }
    }

    IEnumerator CountdownAndLoad()
    {
        while (countdownTime > 0)
        {
            countdownText.text = "Game starting in " + countdownTime + "s";
            yield return new WaitForSeconds(1);
            countdownTime--;
        }

        SoundManager.Instance.StopBackgroundMusic();
        SceneManager.LoadScene("Game");
    }
}
