using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> environment;
    public List<GameObject> coinsBig;
    public List<GameObject> coinsSmall;
    public List<GameObject> coinsTiny;
    public List<GameObject> trapsMedium;
    public List<GameObject> trapsSmall;
    public List<GameObject> trapsTiny;
    public List<GameObject> trapsExpanding;
    public List<GameObject> zombies;
    public List<GameObject> upgrades;

    public GameObject trapBig;
    public GameObject trapRotating;
    public GameObject player;
    public GameObject coinsUpgrade;

    private PlayerController playerController;
    private GameManager gameManager;

    private float environmentSpawnRate;
    private float coinsSpawnRate;
    private float trapsSpawnRate;
    private float zombiesSpawnRate;
    private float expandingTrapSpawnRate = 6;
    private float expandingTrapDestroyTime = 5;
    private float upgradesSpawnRate = 25;
    private float spawnRangeX = 20;
    private float spawnRangeMinX = 15;
    private float spawnRangeMaxX = 50;
    private float coinsBigSpawnRangeMinY = 0;
    private float coinsBigSpawnRangeMaxY = 1.4f;
    private float coinsSmallSpawnRangeMinY = -1.7f;
    private float coinsSmallSpawnRangeMaxY = 3;
    private float coinsTinySpawnRangeMinY = -3.5f;
    private float trapsSpawnRangeMaxY = 4;
    private float trapsBigSpawnRangeMinY = -1.5f;
    private float trapsSmallSpawnRangeMinY = -2;
    private float trapsTinySpawnRangeMinY = -3.2f;
    private float trapsRotatingSpawnRangeMinY = -1.7f;
    private float trapsRotatingSpawnRangeMaxY = 2.8f;
    private float upgradesSpawnRangeMaxY = 4.5f;

    private int startPos = -5;
    private int swapTime = 0;
    private int swapTimeEnd = 3;
    private int expandingTrapSwap = 0;
    private int expandingTrapSwapEnd = 4;
    private int resetSwap = 0;
    private int incrementSwap = 1;

    private bool isCorutineActive = false;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        SpawnBoostSpeed();

        if (!isCorutineActive && player.transform.position.x > startPos)
        {
            isCorutineActive = true;

            StartCoroutine(SpawnEnvironment());
            StartCoroutine(SpawnCoinsAndTraps());
            StartCoroutine(SpawnZombies());
            StartCoroutine(SpawnUpgrades());
        }
    }

    private void SpawnBoostSpeed()
    {
        if (gameManager.isBoostActive || gameManager.isBoost2XActive)
        {
            environmentSpawnRate = 0.5f;
            zombiesSpawnRate = 0.5f;
            coinsSpawnRate = 1;
            trapsSpawnRate = 0.5f;
        }
        else if (gameManager.is2xSpeedActive)
        {
            environmentSpawnRate = 1;
            zombiesSpawnRate = 2.5f;
            coinsSpawnRate = 1.5f;
            trapsSpawnRate = 1;
        }
        else if (gameManager.distance > gameManager.fastMoveDistance)
        {
            environmentSpawnRate = 0.8f;
            zombiesSpawnRate = 2.5f;
            coinsSpawnRate = 1.2f;
            trapsSpawnRate = 1;
        }
        else
        {
            environmentSpawnRate = 1.5f;
            zombiesSpawnRate = 5;
            coinsSpawnRate = 3;
            trapsSpawnRate = 2;
        }
    }

    IEnumerator SpawnEnvironment()
    {
        while (!playerController.isDead)
        {
            int index = Random.Range(0, environment.Count);

            Vector2 spawnPos = new Vector2(Random.Range(spawnRangeMinX, spawnRangeMaxX), environment[index].transform.position.y);

            yield return new WaitForSeconds(environmentSpawnRate);

            Instantiate(environment[index], spawnPos, environment[index].transform.rotation);
        }
    }

    IEnumerator SpawnCoinsAndTraps()
    {
        while (!playerController.isDead)
        {
            int indexCoinsBig = Random.Range(0, coinsBig.Count);
            int indexCoinsSmall = Random.Range(0, coinsSmall.Count);
            int indexCoinsTiny = Random.Range(0, coinsTiny.Count);

            int indexTrapsMedium = Random.Range(0, trapsMedium.Count);
            int indexTrapsSmall = Random.Range(0, trapsSmall.Count);
            int indexTrapsTiny = Random.Range(0, trapsTiny.Count);
            int indexTrapExpanding = Random.Range(0, trapsExpanding.Count);

            Vector2 spawnPosCoinsBig = new Vector2(spawnRangeX, Random.Range(coinsBigSpawnRangeMinY, coinsBigSpawnRangeMaxY));
            Vector2 spawnPosCoinsSmall = new Vector2(spawnRangeX, Random.Range(coinsSmallSpawnRangeMinY, coinsSmallSpawnRangeMaxY));
            Vector2 spawnPosCoinsTiny = new Vector2(spawnRangeX, Random.Range(coinsTinySpawnRangeMinY, coinsSmallSpawnRangeMaxY));

            Vector2 spawnPosTrapsBig = new Vector2(spawnRangeX, Random.Range(trapsBigSpawnRangeMinY, trapsSpawnRangeMaxY));
            Vector2 spawnPosTrapsMedium = new Vector2(spawnRangeX, Random.Range(trapsSmallSpawnRangeMinY, trapsSpawnRangeMaxY));
            Vector2 spawnPosTrapsSmall = new Vector2(spawnRangeX, Random.Range(trapsSmallSpawnRangeMinY, trapsSpawnRangeMaxY));
            Vector2 spawnPosTrapsTiny = new Vector2(spawnRangeX, Random.Range(trapsTinySpawnRangeMinY, trapsSpawnRangeMaxY));
            Vector2 spawnPosTrapsRotating = new Vector2(spawnRangeX, Random.Range(trapsRotatingSpawnRangeMinY, trapsRotatingSpawnRangeMaxY));

            if (swapTime == resetSwap)
            {
                yield return new WaitForSeconds(coinsSpawnRate);

                Instantiate(coinsBig[indexCoinsBig], spawnPosCoinsBig, coinsBig[indexCoinsBig].transform.rotation);

                yield return new WaitForSeconds(coinsSpawnRate);

                Instantiate(coinsSmall[indexCoinsSmall], spawnPosCoinsSmall, coinsSmall[indexCoinsSmall].transform.rotation);

                yield return new WaitForSeconds(coinsSpawnRate);

                Instantiate(coinsTiny[indexCoinsTiny], spawnPosCoinsTiny, coinsTiny[indexCoinsTiny].transform.rotation);

                swapTime += incrementSwap;
            }
            else if (swapTime < swapTimeEnd)
            {
                yield return new WaitForSeconds(trapsSpawnRate);

                Instantiate(trapBig, spawnPosTrapsBig, trapBig.transform.rotation);

                yield return new WaitForSeconds(trapsSpawnRate);

                Instantiate(trapsMedium[indexTrapsMedium], spawnPosTrapsMedium, trapsMedium[indexTrapsMedium].transform.rotation);

                yield return new WaitForSeconds(trapsSpawnRate);

                Instantiate(trapsSmall[indexTrapsSmall], spawnPosTrapsSmall, trapsSmall[indexTrapsSmall].transform.rotation);

                yield return new WaitForSeconds(trapsSpawnRate);

                Instantiate(trapsTiny[indexTrapsTiny], spawnPosTrapsTiny, trapsTiny[indexTrapsTiny].transform.rotation);

                yield return new WaitForSeconds(trapsSpawnRate);

                Instantiate(trapRotating, spawnPosTrapsRotating, trapRotating.transform.rotation);

                swapTime += incrementSwap;
                expandingTrapSwap += incrementSwap;

                if (swapTime == swapTimeEnd)
                {
                    swapTime = resetSwap;
                }

                if (expandingTrapSwap == expandingTrapSwapEnd && !gameManager.isWaveformActive && !playerController.isShieldActive)
                {
                    yield return new WaitForSeconds(expandingTrapSpawnRate);

                    Instantiate(trapsExpanding[indexTrapExpanding], trapsExpanding[indexTrapExpanding].transform.position, trapsExpanding[indexTrapExpanding].transform.rotation);

                    expandingTrapSwap = resetSwap;

                    yield return new WaitForSeconds(expandingTrapDestroyTime);

                    Destroy(GameObject.FindWithTag("Expanding Trap"));
                }
            }
        }
    }

    IEnumerator SpawnZombies()
    {
        while (!playerController.isDead)
        {
            int index = Random.Range(0, zombies.Count);

            Vector2 spawnPos = new Vector2(Random.Range(spawnRangeMinX, spawnRangeMaxX), zombies[index].transform.position.y);

            yield return new WaitForSeconds(zombiesSpawnRate);

            Instantiate(zombies[index], spawnPos, zombies[index].transform.rotation);
        }
    }

    public IEnumerator SpawnUpgrades()
    {
        while (true)
        {
            if (!playerController.isShieldActive)
            {
                int index = Random.Range(0, upgrades.Count);

                Vector2 spawnPos = new Vector2(Random.Range(spawnRangeMinX, spawnRangeMaxX), Random.Range(coinsBigSpawnRangeMinY, upgradesSpawnRangeMaxY));

                yield return new WaitForSeconds(upgradesSpawnRate);

                Instantiate(upgrades[index], spawnPos, upgrades[index].transform.rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void SpawnUpgardeCoins()
    {
        Instantiate(coinsUpgrade, coinsUpgrade.transform.position, coinsUpgrade.transform.rotation);
    }
}