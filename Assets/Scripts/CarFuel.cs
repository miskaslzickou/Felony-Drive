using UnityEngine;

public class CarFuel : MonoBehaviour
{   
    public float maxFuel = 75f; // Maximum fuel capacity
    public float currentFuel { get; private set; } // Initial fuel level
    public float fuelConsumptionRate = 0.1f; // Fuel consumed per second
    private CarGearBox carGearBox;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentFuel = maxFuel;
        carGearBox = GetComponent<CarGearBox>();
    }

    // Update is called once per frame
    void Update()
    {
        float engineLoad = carGearBox.rpm / carGearBox.maxRPM;
        float fuelBurned = fuelConsumptionRate * engineLoad * Time.deltaTime;
        currentFuel -= fuelBurned;
        currentFuel = Mathf.Max(currentFuel, 0f);
    }
}
