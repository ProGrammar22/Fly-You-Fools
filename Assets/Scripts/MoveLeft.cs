using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float speed;

    private GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        MoveLeftBoostSpeed();

        if (!playerController.isDead)
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed, Space.World);
        }
    }

    private void MoveLeftBoostSpeed()
    {
        if (gameManager.isBoostActive || gameManager.isBoost2XActive)
        {
            speed = 40;
        }
        else if (gameManager.is2xSpeedActive)
        {
            speed = 16;
        }
        else if (gameManager.distance > gameManager.fastMoveDistance)
        {
            speed = 10;
        }
        else
        {
            speed = 5;
        }
    }
}