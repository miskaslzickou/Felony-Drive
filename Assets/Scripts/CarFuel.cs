//Michal Mikuš, 3C, PVA, Felony Drive
using System.Security.Policy;
using UnityEngine;

public class CarFuel : MonoBehaviour
{   
    public float maxFuel = 75f; // Maximum fuel capacity
    public float currentFuel { get; private set; } // Initial fuel level
    public float fuelConsumptionRate = 0.1f; // Fuel consumed per second
    private CarGearBox carGearBox;
    public void SetFuel(float amount) => currentFuel= amount;
    public void AddFuel(float amount) => currentFuel = Mathf.Min(currentFuel + amount, maxFuel);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentFuel = maxFuel;
        carGearBox = GetComponent<CarGearBox>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GasStation.playerInRange)
            return;
        float engineLoad = carGearBox.rpm / carGearBox.maxRPM;
        float fuelBurned = fuelConsumptionRate * engineLoad * Time.deltaTime;
        currentFuel -= fuelBurned;
        currentFuel = Mathf.Max(currentFuel, 0f);
    }
}
