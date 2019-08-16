using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Random = UnityEngine.Random;

public class PonPonCreater : MonoBehaviour
{
    [SerializeField] private GameObject creature;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            var pos = transform.position + new Vector3(0f, 1f, 0f);
            var obj = Instantiate(creature, pos, new Quaternion());

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
}