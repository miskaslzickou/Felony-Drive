using UnityEngine;

public class CarNitro : MonoBehaviour
{
    public float maxNitro = 100f;
    public float nitroBoost = 5f;
    public float nitroConsumptionRate = 10f;
    public float currentNitro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentNitro = maxNitro;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
