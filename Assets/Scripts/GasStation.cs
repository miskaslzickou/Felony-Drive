//Michal Mikuš, 3C, PVA, Felony Drive
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GasStation : MonoBehaviour
{
    public CarFuel carFuel;
    public CarNitro carNitro;
    public static bool playerInRange = false;
    public Collider2D[] triggers;
    public float flowRate = 0.5f; // množství paliva doplňovaného za sekundu
    public TextMeshPro text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var col in triggers)
        {
            var checker = col.gameObject.AddComponent<TriggerChecker>();
            checker.onTriggered += () => { playerInRange = true;text.transform.position = col.transform.position+  new Vector3(2,0.5f,0); };
            checker.onExit += () => { playerInRange = false;text.text = ""; };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (PlayerWallet.cash > 2 && Mathf.Approximately(carFuel.currentFuel, carFuel.maxFuel) == false && Keyboard.current.iKey.isPressed)
            {
                carFuel.AddFuel(flowRate * Time.deltaTime);
                PlayerWallet.Subtract(flowRate * 3 * Time.deltaTime);
                text.text = $"Fuel: {carFuel.currentFuel:F1}/{carFuel.maxFuel}";
                carNitro.AddNitro(flowRate * Time.deltaTime * 0.5f);
            }
            else
                text.text = $"HOLD I to refuel\nFuel: {carFuel.currentFuel:F1}/{carFuel.maxFuel}";
        }
        

    }
}
