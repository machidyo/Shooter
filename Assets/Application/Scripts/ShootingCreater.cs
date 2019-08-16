using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ShootingCreater : MonoBehaviour
{
    [SerializeField] private MagicOnionClient magicOnionClient;
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
        
        SetControllerOperation();
    }

    void Update()
    {
    }
    
    private void SetControllerOperation()
    {
        this.UpdateAsObservable()
            .Where(_ => OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            .Subscribe(_ =>
            {
                var message = "Input by SecondaryIndexTrigger.";
                
                if (magicOnionClient.IsJoin.Value)
                {
//                    magicOnionClient.Leave();
//                    message += Environment.NewLine + "Action to Leave";

                    magicOnionClient.Shoot();
                }
                else
                {
                    magicOnionClient.Join();
                    message += Environment.NewLine + "Action to Join";
                }

                Debug.Log(message);
            });

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    magicOnionClient.Join();
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    magicOnionClient.Leave();
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    magicOnionClient.Shoot();
                }
            });
    }
}