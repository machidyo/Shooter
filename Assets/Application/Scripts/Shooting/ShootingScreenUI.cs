using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unkoman.Shooting
{
    public class ShootingScreenUI : MonoBehaviour
    {
        [Header("UI Parts")]
        [SerializeField] private Text Hp;
        [SerializeField] private Text Score;
        [SerializeField] private Text Timer;
        [SerializeField] private Text Message;

        private ShootingManager manager;
        private Player player;
        
        void Start()
        {
            manager = FindObjectOfType<ShootingManager>();
            manager.Timer.DistinctUntilChanged().Subscribe(timer => Timer.text = $"TIME: {timer}").AddTo(this);
            manager.Score.DistinctUntilChanged().Subscribe(score => Score.text = $"SCORE: {score}").AddTo(this);
            manager.Message.DistinctUntilChanged().Subscribe(message => Message.text = message).AddTo(this);

            player = GameObject.Find("Unkoman").GetComponent<Player>();
            player.HitPoint.DistinctUntilChanged().Subscribe(hp => Hp.text = $"HP: {ShowStars(hp)}");
        }
        
        void Update()
        {
        
        }

        private string ShowStars(int hp)
        {
            var star = "";
            for (var i = 0; i < hp; i++)
            {
                star += "☆";
            }
            return star;
        }
    }
}
