using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PonPonCreater : MonoBehaviour
{
    [FormerlySerializedAs("creature")] [SerializeField] private GameObject ponponUnko;
    [SerializeField] private bool isAuto = false;

    void Start()
    {
        Observable
            .Interval(TimeSpan.FromSeconds(0.1))
            .Where(_ => isAuto)
            .Subscribe(_ => Ponpon())
            .AddTo(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Ponpon();
        }
    }
    
    private void Ponpon()
    {
        var pos = transform.position + new Vector3(-0.8f, 1f, 1.5f);
        var obj = Instantiate(ponponUnko, pos, new Quaternion());

        var x = (Random.value - 0.5f) * 2f;
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(x, 1f, -1f) * 5, ForceMode.Impulse);

        obj.OnCollisionEnterAsObservable().Subscribe(_ =>
        {
            Debug.Log($"Hit to {_.transform.name}");
            if (_.transform.name.ToLower() == "plane") return;

            Destroy(obj, 1);
        });
    }
}