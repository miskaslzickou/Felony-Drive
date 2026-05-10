using UnityEngine;

public class CarNitro : MonoBehaviour
{
    public float maxNitro = 100f;
    public float nitroBoost = 5f;
    public float nitroConsumptionRate = 10f;
    private CarControllerV2 carController;
    public float currentNitro { get; private set; }
    public bool nitroActive { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentNitro = maxNitro;
        carController = GetComponent<CarControllerV2>();
    }
    public void ToggleNitro()
    {
        if (currentNitro > 0)
        {
            nitroActive = !nitroActive;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (nitroActive&&carController.normalizedSpeed>0.1f)
        {
            currentNitro -= nitroConsumptionRate * Time.deltaTime;
            currentNitro = Mathf.Clamp(currentNitro, 0, maxNitro);
                if (currentNitro <= 0)
                    nitroActive = false;
            
        }
        
    }
}
