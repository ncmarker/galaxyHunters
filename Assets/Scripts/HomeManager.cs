using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    public GameObject playButton;
    public GameObject weaponInfoButton;
    public GameObject weaponInfoPopup;
    public GameObject directionsButton;
    public GameObject directionsPopup;
    public GameObject logoCanvas;
    private float countdownTime = 5f;
    private static bool isFirstLaunch = true;

    void Start()
    {
        // only show logo on game launch 
        if (isFirstLaunch)
        {
            logoCanvas.SetActive(true);
            isFirstLaunch = false;  
            SoundManager.Instance.PlaySound("logoIntro");
            StartCoroutine(CountdownAndLoad());  
        }
        else
        {
            logoCanvas.SetActive(false); 
            countdownTime = 0;
        }
        
        if (!playButton) Debug.Log("NO PLAY BTN");
        playButton.GetComponent<Button>().onClick.AddListener(PlayCharacterSelection);
        weaponInfoButton.GetComponent<Button>().onClick.AddListener(ShowWeaponInfo);
        directionsButton.GetComponent<Button>().onClick.AddListener(ShowDirections);
    }

    void Update()
    {
        // wait for logo to go away
        if (countdownTime > 0) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySound("toggle");
            SceneManager.LoadScene("CharacterSelection");
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SoundManager.Instance.PlaySound("toggle");
            weaponInfoPopup.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SoundManager.Instance.PlaySound("toggle");
            directionsPopup.SetActive(true);
        }
    }

    public void CloseWeaponInfo()
    {
        weaponInfoPopup.SetActive(false);
    }

    public void CloseDirections()
    {
        directionsPopup.SetActive(false);
    }

    private void PlayCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    private void ShowWeaponInfo()
    {
        weaponInfoPopup.SetActive(true);
    }

    private void ShowDirections()
    {
        directionsPopup.SetActive(true);
    }

    private IEnumerator CountdownAndLoad()
    {
        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1);
            countdownTime--;
        }

        logoCanvas.SetActive(false);
    }
}
