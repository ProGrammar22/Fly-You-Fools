using UnityEngine;

public class Shield : MonoBehaviour
{
    private PlayerController playerController;
    private GameManager gameManager;

    public bool isHit = false;

    private float resetFlashDestroyTime;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coins"))
        {
            playerController.coins++;

            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Trap"))
        {
            isHit = true;

            FindObjectOfType<AudioManager>().Play("Shield Deactivate Sound");

            gameManager.Flash();

            gameManager.flashStart = resetFlashDestroyTime;

            Destroy(other.gameObject);
        }
    }
}