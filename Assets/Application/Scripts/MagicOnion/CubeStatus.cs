using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

public class CubeStatus : MonoBehaviour
{
    [SerializeField] private MagicOnionClient magicOnionClient;

    [SerializeField] private Material red;
    [SerializeField] private Material green;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();

        if (name == "IsJoin")
        {
            magicOnionClient.IsJoin.DistinctUntilChanged().Subscribe(isJoin => renderer.material = isJoin ? green : red);
        }
        else
        {
            this.UpdateAsObservable()
                .Where(_ => OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                .Subscribe(_ => renderer.material = green);
        }
    }
}