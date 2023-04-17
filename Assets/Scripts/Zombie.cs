using UnityEngine;
using UnityEngine.SceneManagement;

public class Zombie : MonoBehaviour
{
    private Animator zombieAnim;

    private Collider2D zombieCollider;

    private GameObject player;

    private GameManager gameManager;
    private PlayerController playerController;

    private Vector3 rotation;

    private Scene currentScene;

    private float runPosition = 4;
    private float speed;
    private float runSpeed;
    private float faceDirection = 180;
    private float rotationScale = 0;

    private string sceneName;

    private bool isRunOver = false;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        sceneName = currentScene.name;

        if (sceneName == "Main")
        {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        player = GameObject.Find("Player");

        zombieAnim = GetComponent<Animator>();
        zombieCollider = GetComponent<Collider2D>();

        rotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        if (sceneName == "Main")
        {
            ZombieBoostSpeed();
        }

        ZombieAnim();
    }

    private void ZombieAnim()
    {
        if (sceneName == "Main")
        {
            if (transform.position.x < runPosition && !isRunOver && !playerController.isDead)
            {
                zombieAnim.SetTrigger("runTrigger");

                transform.Translate(Vector2.left * Time.deltaTime * runSpeed, Space.World);

                if (transform.rotation.eulerAngles.y == faceDirection)
                {
                    rotation.y = rotationScale;

                    transform.rotation = Quaternion.Euler(rotation);
                }
            }
            else if (player.transform.position.x > -runPosition && !playerController.isDead)
            {
                transform.Translate(Vector2.left * Time.deltaTime * speed, Space.World);
            }
            if (isRunOver)
            {
                zombieAnim.SetTrigger("deathTrigger");
            }
        }
    }

    private void ZombieBoostSpeed()
    {
        if (gameManager.isBoostActive || gameManager.isBoost2XActive)
        {
            runSpeed = 20;
            speed = 40;
        }
        else if (gameManager.is2xSpeedActive)
        {
            runSpeed = 10;
            speed = 20;
        }
        else if (gameManager.distance > gameManager.fastMoveDistance)
        {
            runSpeed = 5;
            speed = 10;
        }
        else
        {
            runSpeed = 2.5f;
            speed = 5;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Shield"))
        {
            isRunOver = true;
            zombieCollider.enabled = false;

            FindObjectOfType<AudioManager>().Play("Zombie Death Sound");
        }
    }
}