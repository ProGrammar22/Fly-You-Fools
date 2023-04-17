using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainManager : MonoBehaviour
{
    private TextMeshProUGUI startBestDistanceText;
    private TextMeshProUGUI startMenuCoinsText;

    public static MainManager Instance;

    private GameManager gameManager;

    private Scene currentScene;

    private string sceneName;
    private string startMenuBestDistanceText;
    private string startCoinsText;

    public bool isSettingActive = false;
    public bool isSoundOn = true;

    private float pauseVolume = 0.5f;
    private float normalVolume = 1;

    private void Update()
    {
        currentScene = SceneManager.GetActiveScene();

        sceneName = currentScene.name;

        if (sceneName == "Main")
        {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

            startMenuBestDistanceText = $"BEST\n{Mathf.RoundToInt(gameManager.bestDistance)}m";
            startCoinsText = $"{gameManager.totalCoins}  <sprite=\"Coin\" name=\"Coin\">";

            SaveBestDistance();
        }
        else if (sceneName == "Start Menu" && !isSettingActive)
        {
            startBestDistanceText = GameObject.Find("Start Menu Best Distance Text").GetComponent<TextMeshProUGUI>();
            startMenuCoinsText = GameObject.Find("Total Coins Text").GetComponent<TextMeshProUGUI>();

            LoadBestDistance();
        }

        if (isSettingActive)
        {
            AudioListener.volume = pauseVolume;
        }
        else
        {
            AudioListener.volume = normalVolume;
        }

        startBestDistanceText.text = startMenuBestDistanceText;
        startMenuCoinsText.text = startCoinsText;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        LoadBestDistance();
    }

    [System.Serializable]
    class SaveData
    {
        public string startMenuBestDistanceText;
        public string startCoinsText;

        public bool isSoundOn;
    }

    public void SaveBestDistance()
    {
        SaveData data = new SaveData();

        data.startMenuBestDistanceText = startMenuBestDistanceText;
        data.startCoinsText = startCoinsText;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/distanceSave.json", json);
    }

    public void LoadBestDistance()
    {
        string path = Application.persistentDataPath + "/distanceSave.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            startMenuBestDistanceText = data.startMenuBestDistanceText;
            startCoinsText = data.startCoinsText;
        }
    }

    public void SaveSoundState()
    {
        SaveData data = new SaveData();

        data.isSoundOn = isSoundOn;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/SoundStateSave.json", json);
    }

    public void LoadSoundState()
    {
        string path = Application.persistentDataPath + "/SoundStateSave.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            isSoundOn = data.isSoundOn;
        }
    }
}