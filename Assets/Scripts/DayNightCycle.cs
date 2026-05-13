//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("Nastavení")]
    public Light2D globalLight;
    public Gradient dayCycle; 
    public float dayLength = 60f;

    
    public float timeNow = 0;
   
    void Start()
    {
        
    }

  
    void Update()
    {
        timeNow += Time.deltaTime / dayLength;

       
        if (timeNow >= 1f) timeNow = 0f;

      
        globalLight.color = dayCycle.Evaluate(timeNow);
        globalLight.intensity = Mathf.Lerp(0.2f, 1.2f, Mathf.Sin(timeNow * Mathf.PI * 2) * 0.5f + 0.5f);
    }
}
