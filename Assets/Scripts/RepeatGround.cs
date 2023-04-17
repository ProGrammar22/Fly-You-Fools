using UnityEngine;

public class RepeatGround : MonoBehaviour
{
    private Vector2 startPos;

    private float repeatWidth = 39;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Repeat();
    }

    private void Repeat()
    {
        if (transform.position.x < startPos.x - repeatWidth)
        {
            transform.position = startPos;
        }
    }
}