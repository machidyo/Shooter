using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UIElements;

public class ShootingCreater : MonoBehaviour
{
    [SerializeField] private MagicOnionClient magicOnionClient;
    [SerializeField] private GameObject head;
    
    [Header("Unko")]
    [SerializeField] private GameObject creature;
    [SerializeField] private List<Transform> targets = new List<Transform>();

    private List<GameObject> creatures = new List<GameObject>();

    private int index = 0;

    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.3)).Subscribe(_ =>
        {
        });
        
        SetControllerOperation();
    }

    void Update()
    {
        if (magicOnionClient.IsJoin.Value)
        {
            var pos = head.transform.position;
            magicOnionClient.SendPosition(pos.x, pos.y, pos.z);
        }
    }
    
    private void SetControllerOperation()
    {
        this.UpdateAsObservable()
            .Where(_ => OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            .Subscribe(_ =>
            {
                if (magicOnionClient.IsJoin.Value)
                {
//                    magicOnionClient.Leave();
//                    message += Environment.NewLine + "Action to Leave";

                    magicOnionClient.Shoot();
                }
                else
                {
                    magicOnionClient.Join();
                    magicOnionClient.ShootAction = Shoot;
                }
            });

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    magicOnionClient.Join();
                    magicOnionClient.ShootAction = Shoot;
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

    private void Shoot()
    {
        if (index >= targets.Count) return;

        var target = targets[index++];

        var c = Instantiate(creature, transform.position + new Vector3(0, 1f, 0), transform.rotation);
        var trackingUnko = c.GetComponent<TrackingUnko>();
        trackingUnko.SetEnemy(target);

        creatures.Add(c);
    }
}