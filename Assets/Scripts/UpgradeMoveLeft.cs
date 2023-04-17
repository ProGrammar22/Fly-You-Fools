using UnityEngine;

public class UpgradeMoveLeft : MonoBehaviour
{
    public Rigidbody2D upgrade1Rb;
    public Rigidbody2D upgrade2Rb;

    private PlayerController playerController;

    private float speed = 5;
    private float moveInterval = 1;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (!playerController.isDead)
        {
            upgrade1Rb.MovePosition(new Vector2(transform.position.x - moveInterval * Time.fixedDeltaTime * speed, transform.position.y));
            upgrade2Rb.MovePosition(new Vector2(transform.position.x - moveInterval * Time.fixedDeltaTime * speed, transform.position.y));
        }
    }
}