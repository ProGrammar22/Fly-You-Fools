using UnityEngine;

public class CoinsMoveLeft : MonoBehaviour
{
    private float speed = 5;

    private bool isOnGround = false;

    void Update()
    {
        if (isOnGround)
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed, Space.World);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
}