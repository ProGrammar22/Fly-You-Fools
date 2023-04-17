using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUIHandler : MonoBehaviour
{
    public GameObject settings;
    public GameObject startMenu;
    public GameObject instructionsMenu;

    public MainManager mainManager;

    private Scene currentScene;

    private string sceneName;

    private int mainScene = 1;

    private float exitAppWait = 0.5f;

    private void Awake()
    {
        mainManager.LoadSoundState();

        if (mainManager.isSoundOn)
        {
            AudioListener.pause = false;
        }
        else if (!mainManager.isSoundOn)
        {
            AudioListener.pause = true;
        }
    }

    private void Start()
    {
        mainManager.LoadSoundState();

        FindObjectOfType<AudioManager>().Play("Start Menu BGM");
    }

    void Update()
    {
        StartKey();

        currentScene = SceneManager.GetActiveScene();

        sceneName = currentScene.name;

        if (sceneName == "Start Menu")
        {
            mainManager = GameObject.Find("Main Manager").GetComponent<MainManager>();
        }
    }

    public void StartGame()
    {
        FindObjectOfType<AudioManager>().Play("Game Start Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Start Menu BGM");

        SceneManager.LoadScene(mainScene);
    }

    private void StartKey()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !mainManager.isSettingActive) 
        {
            FindObjectOfType<AudioManager>().Play("Game Start Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Start Menu BGM");

            SceneManager.LoadScene(mainScene);
        }
    }

    public void Settings()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        mainManager.isSettingActive = true;

        settings.SetActive(true);
        startMenu.SetActive(false);
    }

    public void Instrucions()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        mainManager.isSettingActive = true;

        instructionsMenu.SetActive(true);
        startMenu.SetActive(false);
    }
    public void Home()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        mainManager.isSettingActive = false;

        settings.SetActive(false);
        instructionsMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void SoundOn()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        AudioListener.pause = false;
        mainManager.isSoundOn = true;

        mainManager.SaveSoundState();
    }

    public void SoundOff()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        AudioListener.pause = true;
        mainManager.isSoundOn = false;

        mainManager.SaveSoundState();
    }

    public void Exit()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        StartCoroutine(ExitApp());
    }

    IEnumerator ExitApp()
    {
        yield return new WaitForSeconds(exitAppWait);

        mainManager.SaveBestDistance();
        MainManager.Instance.SaveSoundState();

        Application.Quit();
    }
}