using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TrackingUnko : MonoBehaviour
{
    [SerializeField] private Transform enemy;
    [SerializeField] private ParticleSystem explosion;
    
    // Tracking Unko
    private Vector3 position;
    private Vector3 velocity;
    private float initializePeriod = 1.5f;
    private Vector3 initializePosition;
    private float trackingPeriod = 0.3f;

    // Enemy
    private Renderer enemyRenderer;

    private float elapsedTime = 0;

    void Start()
    {
        position = transform.position;
        velocity = Vector3.zero;
        
        var x = (Random.value - 0.5f) * 2f;
        var z = (Random.value - 0.5f) * 2f;
        initializePosition = transform.position + new Vector3(x, 2f, z);
        
        enemyRenderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private bool isExplosion = false;
    void Update()
    {
        if (elapsedTime < initializePeriod)
        {
            MoveToInit();
        }
        else if(elapsedTime < trackingPeriod)
        {
            TrackEnemy();
        }
        else if(!isExplosion)
        {
            Explose();

            var toDisappear = 1; // sec
            DestructAndFadeOut(toDisappear);
            Destroy(enemy.gameObject, toDisappear);
            
            Destroy(gameObject);
        }
    }

    public void SetEnemy(Transform t)
    {
        enemy = t;
    }

    private void MoveToInit()
    {
        initializePeriod = Track(initializePosition, initializePeriod);
    }

    private void TrackEnemy()
    {
        var targetPosition = enemy.position + new Vector3(0, 1f, 0);
        trackingPeriod = Track(targetPosition, trackingPeriod);
    }

    private float Track(Vector3 targetPosition, float period)
    {
        var diff = targetPosition - position;
        var acceleration = (diff - velocity * period) * 2f / (period * period);

        period -= Time.deltaTime;
        if(period < 0) return 0f;
        
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
        return period;
    }

    private void Explose()
    {
        var effect = Instantiate(explosion, transform.position, transform.rotation);
        effect.Play();
        Observable
            .Timer(TimeSpan.FromSeconds(3)) // SPEC: 3秒後にStop
            .Subscribe(_ => effect.Stop());
    }

    /// <param name="fadeOutPeriod">sec</param>
    /// <param name="updateFrequencyMilliSec">milli sec</param>
    private void DestructAndFadeOut(float fadeOutPeriod, float updateFrequencyMilliSec = 50f)
    {
        var fadOutPeriodMilliSec = fadeOutPeriod * 1000;
        var updateRatePerTime = 1 / ( fadOutPeriodMilliSec / updateFrequencyMilliSec);
        Observable.Interval(TimeSpan.FromMilliseconds(updateFrequencyMilliSec)).Subscribe(count =>
        {
            var percentage = updateRatePerTime * count;
            enemyRenderer.material.SetFloat("_Destruction", percentage);
            enemyRenderer.material.SetFloat("_AlphaFactor", 1.0f - percentage);
        }).AddTo(enemy);
    }
}
