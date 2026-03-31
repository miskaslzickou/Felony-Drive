using UnityEngine;
[System.Serializable]
public class Gear
{
    public string name;
    public float gearAcceleration;
    public float maxSpeed;

}
public class CarGearBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Gear[] gears;
    public int currentGear;
    public Gear CurrenGear=>gears[currentGear];
    void Start()
    {
        

    }
   
  
    // Update is called once per frame
    void Update()
    {
        
    }
}
