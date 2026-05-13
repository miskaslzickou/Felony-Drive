//Michal Mikuš, 3C, PVA, Felony Drive
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
    public UIData UIData;
    public float rpm;
    private CarControllerV2 car;

    void Start()
    {
        car = GetComponent<CarControllerV2>();
        UIData.gear=gears[currentGear].name;
        
    }
    public void ShiftUp()
    {
        if (!(currentGear < gears.Length - 1))
            return;
        currentGear++;
        UIData.gear = gears[currentGear].name;

    }

    public void ShiftDown()
    {
        if (!(currentGear > 0))
            return;
        currentGear--;
        UIData.gear = gears[currentGear].name;
    }


    // Update is called once per frame
    void Update()
    {
        if (car.engineStarted)
        {
            if (currentGear == 1 )//pokud je auto v neutrálu a drží plyn, tak se otáčky zvyšují, ale auto se nepohybuje
            {
                rpm += (car.throttleInput > 0 ? 1f : -1f) * 2000f * Time.deltaTime;
                rpm= Mathf.Clamp(rpm, 1000f, maxRPM);
                return;
            }
           
            rpm = 1000f;
            rpm +=  rpmCurve.Evaluate(Mathf.Abs(car.speed / gears[currentGear].maxSpeed  ))*4000f;
            
        }
        else
            rpm = 0f;
       
    }
}
