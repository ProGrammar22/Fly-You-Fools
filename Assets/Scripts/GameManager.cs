using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI mainBestDistanceText;
    public TextMeshProUGUI meters;
    public TextMeshProUGUI mainCoinsCollected;
    public TextMeshProUGUI gameOverCoinsCollected;
    public TextMeshProUGUI gameOverTotalCoins;

    private PlayerController playerController;
    public Shield shield;

    public GameObject pauseMenu;
    public GameObject player;
    public GameObject mainUI;
    public GameObject gameOver;
    public GameObject boost;
    public GameObject boost2X;
    public GameObject flash;
    public GameObject waveform;

    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer shieldSpriteRenderer;
    public SpriteRenderer circleSpriteRenderer;

    private Collider2D playerFeetCollider;
    public Collider2D playerCapsuleCollider;

    private float startPos = -4;
    private float boostWait = 0;
    private float boostWaitEnd = 5;
    private float flashEnd = 0;
    private float shieldBlinkWait = 0.5f;
    public float fastSpeed;
    public float flashStart = 5;
    public float distance;
    public float bestDistance;

    private int distanceSpeed = 10;
    private int shieldDistanceSpeed = 50;
    private int boostDistanceSpeed = 100;
    private int boost2XDistanceSpeed = 200;
    private int boostEndDistance = 700;
    private int boost2XEndDistance = 1500;
    private int boostCost = 500;
    private int boost2XCost = 1000;
    private int stopTime = 0;
    private int resumeTime = 1;
    private int mainScene = 1;
    private int startMenuScene = 0;
    public int totalCoins;
    public int fastMoveDistance;

    public bool isBoostActive = false;
    public bool isBoost2XActive = false;
    public bool isWaveformActive = false;
    public bool is2xSpeedActive = false;
    public bool wasBoostActivated = false;
    public bool wasBoost2XActivated = false;
    public bool isKillable;

    private void Awake()
    {
        MainManager.Instance.LoadSoundState();

        if (MainManager.Instance.isSoundOn)
        {
            AudioListener.pause = false;
        }
        else if (!MainManager.Instance.isSoundOn)
        {
            AudioListener.pause = true;
        }
    }

    void Start()
    {
        LoadDistanceAndCoins();

        distance = 0;

        isKillable = true;

        FindObjectOfType<AudioManager>().Play("BGM");

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerSpriteRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>();
        playerCapsuleCollider = GameObject.Find("Player").GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GameObject.Find("Feet Collider").GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        IncreaseDifficulty();
        PauseKey();

        mainBestDistanceText.text = $"Best: {Mathf.RoundToInt(bestDistance)}";

        if (player.transform.position.x > startPos && !playerController.isDead)
        {
            DistanceCovered();
            CoinsCollected();
            Boost();
            Boost2X();
            DeactivateBoost();
        }

        if (distance - fastSpeed > 200 && playerController.isShieldActive)
        {
            is2xSpeedActive = true;
        }

        if (shield.isHit)
        {
            StartCoroutine(DisableShield());
        }
        else if (!shield.isHit)
        {
            StopCoroutine(DisableShield());
        }
    }

    private void IncreaseDifficulty()
    {
        if (wasBoostActivated)
        {
            fastMoveDistance = 1500;
        }
        else if (wasBoost2XActivated)
        {
            fastMoveDistance = 2300;
        }
        else
        {
            fastMoveDistance = 1000;
        }
    }

    public void DistanceCovered()
    {
        if (isBoostActive)
        {
            distance += Time.deltaTime * boostDistanceSpeed;
        }
        else if (isBoost2XActive)
        {
            distance += Time.deltaTime * boost2XDistanceSpeed;
        }
        else if (is2xSpeedActive)
        {
            distance += Time.deltaTime * shieldDistanceSpeed;
        }
        else
        {
            distance += Time.deltaTime * distanceSpeed;
        }

        distanceText.text = $"{Mathf.RoundToInt(distance)}m";
        meters.text = distanceText.text;
    }

    public void CoinsCollected()
    {
        mainCoinsCollected.text = $"{playerController.coins}  <sprite=\"Coin\" name=\"Coin\">";
        gameOverCoinsCollected.text = $"And Collected\n    Coins :          {playerController.coins}  <sprite=\"Coin\" name=\"Coin\">";
    }

    public void BestDistance()
    {
        if (distance > bestDistance)
        {
            bestDistance = distance;
        }
    }

    private void Boost()
    {
        boostWait += Time.deltaTime;

        if (totalCoins > boostCost && boostWait < boostWaitEnd && !isBoost2XActive && !isBoostActive)
        {
            boost.SetActive(true);
        }
        else
        {
            boost.SetActive(false);
        }
    }

    private void Boost2X()
    {
        if (totalCoins > boost2XCost && boostWait < boostWaitEnd && !isBoost2XActive && !isBoostActive)
        {
            boost2X.SetActive(true);
        }
        else
        {
            boost2X.SetActive(false);
        }
    }

    public void ActivateBoost()
    {
        isBoostActive = true;
        isWaveformActive = true;
        wasBoostActivated = true;

        totalCoins -= boostCost;

        FindObjectOfType<AudioManager>().Play("Boost Activate Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Jetpack Sound");
        FindObjectOfType<AudioManager>().StopPlaying("BGM");

        Flash();

        waveform.SetActive(true);

        playerSpriteRenderer.enabled = false;
        playerCapsuleCollider.enabled = false;
        playerFeetCollider.enabled = false;

        FindObjectOfType<AudioManager>().Play("Boost BGM");
        FindObjectOfType<AudioManager>().Play("Waveform Sound");
    }

    public void ActivateBoost2X()
    {
        isBoost2XActive = true;
        isWaveformActive = true;
        wasBoost2XActivated = true;

        totalCoins -= boost2XCost;

        FindObjectOfType<AudioManager>().Play("Boost Activate Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Jetpack Sound");
        FindObjectOfType<AudioManager>().StopPlaying("BGM");

        Flash();

        waveform.SetActive(true);

        playerSpriteRenderer.enabled = false;
        playerCapsuleCollider.enabled = false;
        playerFeetCollider.enabled = false;

        FindObjectOfType<AudioManager>().Play("Boost BGM");
        FindObjectOfType<AudioManager>().Play("Waveform Sound");
    }

    private void DeactivateBoost()
    {
        if (distance > boost2XEndDistance)
        {
            flashStart -= Time.deltaTime;
        }

        if (flashStart <= flashEnd)
        {
            Destroy(GameObject.FindWithTag("Flash"));
        }

        if (distance > boostEndDistance && !isBoost2XActive && isWaveformActive)
        {
            isBoostActive = false;
            isWaveformActive = false;

            FindObjectOfType<AudioManager>().Play("Boost Activate Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Boost BGM");
            FindObjectOfType<AudioManager>().StopPlaying("Waveform Sound");

            Flash();

            player.transform.position = waveform.transform.position;

            waveform.SetActive(false);

            playerSpriteRenderer.enabled = true;
            playerCapsuleCollider.enabled = true;
            playerFeetCollider.enabled = true;

            FindObjectOfType<AudioManager>().Play("BGM");

            if (!playerController.isOnGround && playerController.isOnStartPoint)
            {
                FindObjectOfType<AudioManager>().Play("Fast Footsteps Sound");
            }
            else if (playerController.isOnGround)
            {
                FindObjectOfType<AudioManager>().Play("Jetpack Sound");
            }

        }
        else if (distance > boost2XEndDistance && isWaveformActive)
        {
            isBoost2XActive = false;
            isWaveformActive = false;

            FindObjectOfType<AudioManager>().Play("Boost Activate Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Boost BGM");
            FindObjectOfType<AudioManager>().StopPlaying("Waveform Sound");

            Flash();

            player.transform.position = waveform.transform.position;

            waveform.SetActive(false);

            playerSpriteRenderer.enabled = true;
            playerCapsuleCollider.enabled = true;
            playerFeetCollider.enabled = true;

            FindObjectOfType<AudioManager>().Play("BGM");

            if (!playerController.isOnGround && playerController.isOnStartPoint)
            {
                FindObjectOfType<AudioManager>().Play("Fast Footsteps Sound");
            }
            else if (playerController.isOnGround)
            {
                FindObjectOfType<AudioManager>().Play("Jetpack Sound");
            }
        }
    }

    public void Flash()
    {
        Instantiate(flash, flash.transform.position, flash.transform.rotation);
    }

    IEnumerator DisableShield()
    {
        shield.isHit = false;
        playerController.isShieldActive = false;
        is2xSpeedActive = false;

        yield return new WaitForSeconds(shieldBlinkWait);

        shieldSpriteRenderer.enabled = false;
        circleSpriteRenderer.enabled = false;

        yield return new WaitForSeconds(shieldBlinkWait);

        shieldSpriteRenderer.enabled = true;
        circleSpriteRenderer.enabled = true;

        yield return new WaitForSeconds(shieldBlinkWait);

        shieldSpriteRenderer.enabled = false;
        circleSpriteRenderer.enabled = false;

        yield return new WaitForSeconds(shieldBlinkWait);

        shieldSpriteRenderer.enabled = true;
        circleSpriteRenderer.enabled = true;

        yield return new WaitForSeconds(shieldBlinkWait);

        playerController.shield.SetActive(false);

        isKillable = true;
        playerCapsuleCollider.enabled = true;
    }

    public void Pause()
    {
        MainManager.Instance.isSettingActive = true;

        FindObjectOfType<AudioManager>().Play("Button Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Slow Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Jetpack Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Waveform Sound");

        pauseMenu.SetActive(true);
        mainUI.SetActive(false);

        Time.timeScale = stopTime;
    }

    public void Resume()
    {
        MainManager.Instance.isSettingActive = false;

        FindObjectOfType<AudioManager>().Play("Button Sound");

        if (!playerController.isOnGround && !isWaveformActive && playerController.isOnStartPoint)
        {
            FindObjectOfType<AudioManager>().Play("Fast Footsteps Sound");
        }
        else if (playerController.isOnGround && !isWaveformActive)
        {
            FindObjectOfType<AudioManager>().Play("Jetpack Sound");
        }
        else if (!playerController.isOnGround && !playerController.isOnStartPoint)
        {
            FindObjectOfType<AudioManager>().Play("Slow Footsteps Sound");
        }
        else if (isWaveformActive)
        {
            FindObjectOfType<AudioManager>().Play("Waveform Sound");
        }

        pauseMenu.SetActive(false);
        mainUI.SetActive(true);

        Time.timeScale = resumeTime;
    }

    private void PauseKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !MainManager.Instance.isSettingActive)
        {
            MainManager.Instance.isSettingActive = true;

            FindObjectOfType<AudioManager>().Play("Button Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Slow Footsteps Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Jetpack Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Waveform Sound");

            pauseMenu.SetActive(true);
            mainUI.SetActive(false);

            Time.timeScale = stopTime;
        }
    }

    public void Quit()
    {
        MainManager.Instance.isSettingActive = false;

        FindObjectOfType<AudioManager>().Play("Button Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Slow Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Boost BGM");
        FindObjectOfType<AudioManager>().StopPlaying("Waveform Sound");
        FindObjectOfType<AudioManager>().StopPlaying("BGM");

        SaveDistanceAndCoins();
        SceneManager.LoadScene(startMenuScene);

        Time.timeScale = resumeTime;
    }

    public void Restart()
    {
        MainManager.Instance.isSettingActive = false;

        FindObjectOfType<AudioManager>().Play("Button Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Slow Footsteps Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Waveform Sound");
        FindObjectOfType<AudioManager>().StopPlaying("Boost BGM");

        SceneManager.LoadScene(mainScene);

        Time.timeScale = resumeTime;
    }

    public void Next()
    {
        MainManager.Instance.isSettingActive = false;

        FindObjectOfType<AudioManager>().Play("Button Sound");
        FindObjectOfType<AudioManager>().StopPlaying("BGM");

        SceneManager.LoadScene(startMenuScene);

        Time.timeScale = resumeTime;
    }

    public void SoundOn()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        AudioListener.pause = false;
        MainManager.Instance.isSoundOn = true;

        MainManager.Instance.SaveSoundState();
    }

    public void SoundOff()
    {
        FindObjectOfType<AudioManager>().Play("Button Sound");

        AudioListener.pause = true;
        MainManager.Instance.isSoundOn = false;

        MainManager.Instance.SaveSoundState();
    }

    [System.Serializable]
    class SaveData
    {
        public float bestDistance;

        public int totalCoins;
    }

    public void SaveDistanceAndCoins()
    {
        SaveData data = new SaveData();

        data.bestDistance = bestDistance;
        data.totalCoins = totalCoins;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/distanceSave1.json", json);
    }

    public void LoadDistanceAndCoins()
    {
        string path = Application.persistentDataPath + "/distanceSave1.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestDistance = data.bestDistance;
            totalCoins = data.totalCoins;
        }
    }
}