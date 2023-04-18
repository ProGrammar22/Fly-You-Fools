using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float flySpeed;
    private float normalFlySpeed = 80;
    private float normalMass = 1;
    private float normalGravityScale = 2;
    private float upgradeFlySpeed = 2000;
    private float upgradeMass = 20;
    private float upgradeGravityScale = 4;
    private float walkSpeed = 1;
    private float startPos = -4;
    private float muzzleWait = 0;
    private float muzzleWaitEnd = 0.1f;

    public int coins;

    public Rigidbody2D playerRb;

    private Animator playerAnim;

    private MoveLeft groundMoveScript;
    private MoveLeft backgroundMoveScript;
    public SpawnManager spawnManager;
    public GameManager gameManager;

    public GameObject gameOver;
    public GameObject mainUI;
    public GameObject muzzleFlash;
    public GameObject shield;

    public bool isDead = false;
    public bool isOnGround = true;
    public bool isShieldActive = false;
    public bool isOnStartPoint = false;

    private float stopTimeWait = 0.5f;
    private float playerAnimSpeedSlow = 0.5f;
    private float playerAnimSpeedFast = 1;

    void Start()
    {
        coins = 0;

        playerRb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();

        FindObjectOfType<AudioManager>().Play("Slow Footsteps Sound");

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        groundMoveScript = GameObject.Find("Tilemap").GetComponent<MoveLeft>();
        backgroundMoveScript = GameObject.Find("Background").GetComponent<MoveLeft>();
    }

    void FixedUpdate()
    {
        MovePlayer();
        UpgradeFlySpeed();
        StartAnimation();
        MuzzleAnim();
    }

    void MovePlayer()
    {
        if (Input.GetKey(KeyCode.Space) && !isDead && isOnStartPoint)
        {
            if (!isOnGround && !gameManager.isWaveformActive)
            {
                FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");
                FindObjectOfType<AudioManager>().Play("Jetpack Sound");
            }

            isOnGround = true;

            playerAnim.SetTrigger("flyTrigger");
            playerAnim.ResetTrigger("walkTrigger");
            playerRb.AddForce(Vector2.up * flySpeed);
        }
    }

    void UpgradeFlySpeed()
    {
        if (isShieldActive)
        {
            flySpeed = upgradeFlySpeed;
            playerRb.mass = upgradeMass;
            playerRb.gravityScale = upgradeGravityScale;
        }
        else
        {
            flySpeed = normalFlySpeed;
            playerRb.mass = normalMass;
            playerRb.gravityScale = normalGravityScale;
        }
    }

    public void StartAnimation()
    {
        if (transform.position.x < startPos)
        {
            playerAnim.SetFloat("animSpeed", playerAnimSpeedSlow);
            transform.Translate(Vector2.right * Time.deltaTime * walkSpeed);
        }
        else if (!isDead)
        {
            groundMoveScript.enabled = true;
            backgroundMoveScript.enabled = true;

            playerAnim.SetFloat("animSpeed", playerAnimSpeedFast);

            if (!isOnStartPoint && !gameManager.isWaveformActive)
            {
                FindObjectOfType<AudioManager>().StopPlaying("Slow Footsteps Sound");
                FindObjectOfType<AudioManager>().Play("Fast Footsteps Sound");

                isOnStartPoint = true;
            }
        }
        else if (isDead)
        {
            groundMoveScript.enabled = false;
            backgroundMoveScript.enabled = false;
        }
    }

    void MuzzleAnim()
    {
        muzzleWait += Time.deltaTime;

        if (muzzleWait > muzzleWaitEnd)
        {
            muzzleFlash.SetActive(false);

            muzzleWait = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isOnGround && isOnStartPoint)
            {
                FindObjectOfType<AudioManager>().StopPlaying("Jetpack Sound");
                FindObjectOfType<AudioManager>().Play("Fast Footsteps Sound");
            }

            isOnGround = false;

            playerAnim.SetTrigger("walkTrigger");
            playerAnim.ResetTrigger("flyTrigger");
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coins"))
        {
            coins++;

            FindObjectOfType<AudioManager>().Play("Coin Collect Sound");

            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Zombie") && !isOnGround && !gameManager.isWaveformActive)
        {
            muzzleFlash.SetActive(true);

            FindObjectOfType<AudioManager>().Play("Gun Sound");
        }

        if (other.gameObject.CompareTag("Trap") && !gameManager.isWaveformActive && !isShieldActive && gameManager.isKillable)
        {
            isDead = true;
            MainManager.Instance.isSettingActive = true;
            playerRb.simulated = false;

            playerAnim.SetTrigger("explosionTrigger");

            FindObjectOfType<AudioManager>().Play("Explosion Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Jetpack Sound");
            FindObjectOfType<AudioManager>().StopPlaying("Fast Footsteps Sound");

            gameManager.totalCoins += coins;

            playerAnim.ResetTrigger("walkTrigger");
            playerAnim.ResetTrigger("flyTrigger");

            spawnManager.StopAllCoroutines();
            gameManager.BestDistance();
            gameManager.SaveDistanceAndCoins();

            gameManager.gameOverTotalCoins.text = $"{gameManager.totalCoins}  <sprite=\"Coin\" name=\"Coin\">";

            StartCoroutine(StopTime());
        }

        if (other.gameObject.CompareTag("Upgrade1") && !gameManager.isWaveformActive)
        {
            Destroy(other.gameObject);

            FindObjectOfType<AudioManager>().Play("Coins Upgrade Activate Sound");

            spawnManager.SpawnUpgardeCoins();
        }

        if (other.gameObject.CompareTag("Upgrade2") && !gameManager.isWaveformActive)
        {
            Destroy(other.gameObject);

            FindObjectOfType<AudioManager>().Play("Shield Upgrade Activate Sound");

            gameManager.Flash();

            gameManager.playerCapsuleCollider.enabled = false;

            shield.SetActive(true);

            isShieldActive = true;
            gameManager.isKillable = false;

            gameManager.fastSpeed = gameManager.distance;
        }
    }

    IEnumerator StopTime()
    {
        yield return new WaitForSeconds(stopTimeWait);

        Time.timeScale = 0;

        mainUI.SetActive(false);
        gameOver.SetActive(true);
    }
}