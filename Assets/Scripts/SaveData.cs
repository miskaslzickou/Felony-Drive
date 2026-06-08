//Michal Mikuš, 3C, PVA, Felony Drive
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static SaveData;

public class SaveData : MonoBehaviour
{

   
    [SerializeField] private CarControllerV2 _carControllerV2;
    public static CarControllerV2 carControllerV2;
    public static CarNitro carNitro;
    public static CarFuel carFuel;
    [Serializable]
    public class SaveDataObj
    {
        public float cash;
        public float fuel;
        public float nitro;
        public Vector2 coords;
        public float heading;
       
       public static SaveDataObj GetData()
        {
            return new SaveDataObj
            {
                cash = PlayerWallet.cash,
                fuel = carFuel.currentFuel,
                nitro = carNitro.currentNitro,
                coords = carControllerV2.CarCoords,
                heading = carControllerV2.Heading,
                
            };

        }
        public void LoadData()
        {
            PlayerWallet.SetCash(cash);
            carFuel.SetFuel(fuel);
            carNitro.SetNitro(nitro);
            carControllerV2.SetCoords(coords);
            carControllerV2.SetHeading(heading);
          
        }
    }
    private void Start()
    {
        carControllerV2 = _carControllerV2;
        carFuel = carControllerV2.carFuel;
        carNitro = carControllerV2.carNitro;
        void OnApplicationQuit()=> SaveGameplayData();
        ReadGameplayData();
       
    }
    public static void SaveGameplayData()
    {   
        
        string json = JsonUtility.ToJson(SaveDataObj.GetData());
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }
    public static void ReadGameplayData() {
        if (!File.Exists(Application.persistentDataPath + "/save.json"))
            return;
        string json = File.ReadAllText(Application.persistentDataPath + "/save.json");
        SaveDataObj data=JsonUtility.FromJson<SaveDataObj>(json);
        data.LoadData();
    
    }
}
