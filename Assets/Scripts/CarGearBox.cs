using UnityEngine;
using UnityEngine.Rendering;

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
    public float maxRPM = 5000f;
    public int currentGear=1;
    public AnimationCurve rpmCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Gear CurrenGear=>gears[currentGear];
    public UIData uiData;
    public float rpm;
    public CarControllerV2 car;

    void Start()
    {
        uiData.gear=gears[currentGear].name;
      
    }
    public void ShiftUp()
    {
        if (!(currentGear < gears.Length - 1))
            return;
        currentGear++;
        uiData.gear = gears[currentGear].name;

    }

    public void ShiftDown()
    {
        if (!(currentGear > 0))
            return;
        currentGear--;
        uiData.gear = gears[currentGear].name;
    }


    // Update is called once per frame
    void Update()
    {
        if (car.engineStarted)
        {
            rpm = 1000f;
            rpm +=  rpmCurve.Evaluate(Mathf.Abs(car.speed / gears[currentGear].maxSpeed  ))*4000f;
            
        }
        else
            rpm = 0f;
       
    }
}
