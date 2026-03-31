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
    public int currentGear=1;
    public Gear CurrenGear=>gears[currentGear];
    void Start()
    {
        

    }
    public void ShiftUp()
    {
        if (currentGear < gears.Length - 1) currentGear++;
    }

    public void ShiftDown()
    {
        if (currentGear > 0) currentGear--;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
