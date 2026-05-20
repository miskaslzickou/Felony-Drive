//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;

public  class PlayerWallet : MonoBehaviour
{
    [SerializeField] public  UIData uiData;
    private static UIData _uiData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _uiData = uiData;
        _uiData.cash = cash;

    }
    public static float cash { get; private set; } = 250f;

    public static void Add(float amount)
    {
        cash += amount;
        _uiData.cash = cash;
    }
    public static void Subtract(float amount)
    {
        cash -= amount;
        _uiData.cash = cash;
    }
}
