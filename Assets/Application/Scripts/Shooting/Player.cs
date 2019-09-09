using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject head;

    [Header("Unko")] 
    [SerializeField] private GameObject bullet;

    public ReactiveProperty<int> HitPoint = new ReactiveProperty<int>(3);
    
    private List<GameObject> shootBullets = new List<GameObject>();
    private Queue<Transform> targets = new Queue<Transform>();

    private LineRenderer lineRenderer;

    void Start()
    {
        var target = GameObject.Find("Enemy");
        LockOn(target.transform);

        lineRenderer = GetComponent<LineRenderer>();

        HitPoint.Value = 3;
    }

    void Update()
    {
        SetControllerOperation();
    }

    private void LockOn(Transform target)
    {
        targets.Enqueue(target);
    }

    private void Shoot()
    {
        if (targets.Count > 0)
        {
            for (var i = targets.Count; i > 0; i--)
            {
                var target = targets.Dequeue();
                
                var pos = new Vector3(Random.Range(-1f, 1f), Random.Range(1f, 2f), Random.Range(-1f, 1f));
                var c = Instantiate(bullet, transform.position + pos, transform.rotation);
                var trackingUnko = c.GetComponent<TrackingUnko>();
                trackingUnko.SetEnemy(target);

                shootBullets.Add(c);
            }
        }
        else
        {
            // はずれの演出
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.name == "PonPonUnko(Clone)")
        {
            HitPoint.Value--;
        }
    }
    
    private void SetControllerOperation()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            var target = GameObject.Find("Enemy(Clone)");
            LockOn(target.transform);
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.L))
        {
            var leftHand = GameObject.Find("LeftHandAnchor").transform;
            var ray = new Ray(leftHand.position, leftHand.forward);
            var hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                targets.Enqueue(hit.transform);
            }
            
            lineRenderer.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out hit))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, ray.origin + ray.direction * 100);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                targets.Enqueue(hit.transform);
            }
        }
    }
}