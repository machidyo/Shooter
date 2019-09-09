using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Random = UnityEngine.Random;

public class PonPonCreater : MonoBehaviour
{
    [SerializeField] private GameObject ponponUnko;
    [SerializeField] private bool isAuto = false;

    void Start()
    {
        Observable
            .Interval(TimeSpan.FromSeconds(5.5))
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
        var pos = transform.position + new Vector3(-0.8f, 1f, -0.1f);
        var obj = Instantiate(ponponUnko, pos, new Quaternion());

        var x = (Random.value - 0.5f) * 2f;
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(x, 1f, -1f) * 5, ForceMode.Impulse);

        obj.OnCollisionEnterAsObservable().Subscribe(_ =>
        {
            if (_.transform.name.ToLower() != "unkoman") return;

            Debug.Log($"Hit to {_.transform.name}");
            Destroy(obj, 1);
        });
    }
}