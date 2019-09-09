using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject bullet;

    private Vector3 nextPosition;
    private float movePeriod = 1.0f;

    void Start()
    {
        SetNextMovePosition();

        Observable.Interval(TimeSpan.FromSeconds(movePeriod))
            .Subscribe(_ => SetNextMovePosition())
            .AddTo(this);

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(_ => Shoot())
            .AddTo(this);
    }

    void Update()
    {
        transform.position += nextPosition * (Time.deltaTime / movePeriod);
    }

    private void SetNextMovePosition()
    {
        var x = (Random.value - 0.5f) * 2;
        var z = (Random.value - 0.5f) * 2;
        nextPosition = new Vector3(x, 0f, z);
    }
    
    private void Shoot()
    {
        var pos = transform.position + new Vector3(-0.8f, 1f, -0.1f);
        var obj = Instantiate(bullet, pos, new Quaternion());

        var x = (Random.value - 0.5f) * 2f;
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(x, 1f, -1f) * 5, ForceMode.Impulse);

        obj.OnCollisionEnterAsObservable().Subscribe(_ => Destroy(obj, 1)).AddTo(obj);
    }
}
