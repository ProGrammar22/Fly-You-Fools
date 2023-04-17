using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private float xBound = -17;

    void Update()
    {
        if (gameObject.transform.position.x < xBound)
        {
            Destroy(gameObject);
        }
    }
}