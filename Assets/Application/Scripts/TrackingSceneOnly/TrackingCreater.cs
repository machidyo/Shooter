using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TrackingCreater : MonoBehaviour
{
    [SerializeField] private GameObject creature;
    [SerializeField] private List<Transform> targets = new List<Transform>();

    private List<GameObject> creatures = new List<GameObject>();

    private int index = 0;

    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.3)).Subscribe(_ =>
        {
            if (index >= targets.Count) return;

            var target = targets[index++];

            var c = Instantiate(creature, transform.position + new Vector3(0, 1f, 0), transform.rotation);
            var trackingUnko = c.GetComponent<TrackingUnko>();
            trackingUnko.SetEnemy(target);

            creatures.Add(c);
        });
    }
}