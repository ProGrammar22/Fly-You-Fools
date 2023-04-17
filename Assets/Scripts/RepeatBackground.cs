using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private Vector2 startPos;

    private float repeatWidth = 37.8f;

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