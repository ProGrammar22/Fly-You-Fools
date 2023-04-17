using UnityEngine;

public class WaveformController : MonoBehaviour
{
    private Rigidbody2D waveformRb;

    private float waveFlySpeed = 5;

    void Start()
    {
        waveformRb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            waveformRb.AddForce(Vector2.up * waveFlySpeed);
        }
    }
}