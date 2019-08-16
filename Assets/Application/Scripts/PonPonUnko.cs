using UnityEngine;

public class PonPonUnko : MonoBehaviour
{
    [SerializeField] private GameObject[] skins;

    void Start()
    {
        SetUnkoSkinByRandom();
    }

    private int index = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"うんこ変身（index = {index}）");
            SetModel(index);
            index = ++index % skins.Length;
        }
    }

    private void SetUnkoSkinByRandom()
    {
        // SPEC: 5 が rocket なので、rocket 以外を random で設定する
        var rand = 5;
        while (rand == 5)
        {
            rand = Mathf.FloorToInt(Random.value * 10) % skins.Length;
        }
        SetModel(rand);
    }
    
    private void SetModel(int num)
    {
        foreach (var skin in skins)
        {
            skin.SetActive(false);
        }
        skins[num].SetActive(true);
    }
}
