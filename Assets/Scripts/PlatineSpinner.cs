using UnityEngine;

public class PlatineSpinner : MonoBehaviour
{
    public float rotationSpeed = 30f;
    private bool shouldSpin = false;

    void Update()
    {
        if (shouldSpin)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void StartSpin()
    {
        shouldSpin = true;
    }

    public void StopSpin()
    {
        shouldSpin = false;
    }
}
