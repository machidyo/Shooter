using UnityEngine;
using Random = UnityEngine.Random;

public class TrackingTarget : MonoBehaviour
{
    private Vector3 start;
    private Vector3 diff;
    private float elapsedTime = 0f;
    private float period = 0.1f;

    void Start()
    {
        Reset();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > period)
        {
            Reset();
        }
        else
        {
            var distance = diff * (elapsedTime / period);
            transform.position = start + distance;
        }
    }

    private void Reset()
    {
        start = transform.position;
        diff = GetGoal() - start;
        elapsedTime = 0.0f;
    }

    private Vector3 GetGoal()
    {
        var x = (Random.value - 0.5f) * 2;
        var z = (Random.value - 0.5f) * 2;
        return transform.position + new Vector3(x, 0f, z);
    }
}
