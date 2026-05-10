using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Gauge {
    public GaugeNeedle Needle;
    public VisualElement GaugeElement;
    public void Initialize(string elementName, UIDocument uiDocument)
    {
        var root = uiDocument.rootVisualElement;
        GaugeElement = root.Q<VisualElement>(elementName);
    }
    public void SetTint(Color tintColor)
    {
        GaugeElement.style.unityBackgroundImageTintColor = tintColor;
    }
    [System.Serializable]
    public class GaugeNeedle
    {
        public void Initialize(string elementName, UIDocument uiDocument)
        {
            var root = uiDocument.rootVisualElement;
            NeedleElement = root.Q<VisualElement>(elementName);
        }
      
        
        private VisualElement NeedleElement;
        public float MinAngle;
        public float MaxAngle;
        public void UpdateNeedle(float normalizedValue)
        {
            float angle = Mathf.Lerp(MinAngle, MaxAngle, normalizedValue);
            NeedleElement.style.rotate = new Rotate(new Angle(angle, AngleUnit.Degree));
        }

    }
}
public class UI : MonoBehaviour
{
    public UIDocument uiDocument;
    public string needleSpeedElementName = "NeedleSpeed";
    public string speedometerElementName ="Speedometer";
    public string needleRPMElementName = "NeedleRPM";
    public string rpmElementName = "RPM";
    public string gearElementName = "Gear";
    public string needleFuelElementName = "NeedleFuel";
    public Transform target;
    public Gauge speedometer;
    public Gauge tachometer;
    public Gauge fuel;
    private CarControllerV2 carController;
    private CarFuel carFuel;
    private CarGearBox carGearBox;
    private VisualElement gear;


     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        
        var root = uiDocument.rootVisualElement;
        speedometer.Initialize(speedometerElementName, uiDocument);
        tachometer.Initialize(rpmElementName, uiDocument);
        speedometer.Needle.Initialize(needleSpeedElementName, uiDocument);
        tachometer.Needle.Initialize(needleRPMElementName, uiDocument);
        gear =root.Q<VisualElement>(gearElementName);
        carController = target.GetComponent<CarControllerV2>();
        carFuel = target.GetComponent<CarFuel>();
        carGearBox = target.GetComponent<CarGearBox>();
        fuel.Needle.Initialize(needleFuelElementName, uiDocument);

    }

    // Update is called once per frame
    public void ChangeSpeedometerTint(bool state)
    {
        if (state) {
          
            gear.style.color = new Color(184f / 255f, 252f / 255f, 242f / 249f);
            speedometer.SetTint(new Color(184f / 255f, 252f / 255f, 242f / 249f));
            tachometer.SetTint(new Color(184f / 255f, 252f / 255f, 242f / 249f));
        }
        else
        {
           
            gear.style.color =  Color.white;
            speedometer.SetTint(Color.white);
            tachometer.SetTint(Color.white);
        }
    }
    void Update()
    {
        float normalizedSpeed = carController.normalizedSpeed;
        float normalizedRPM= carGearBox.rpm /6000f;
        float normalizedFuel = carFuel.currentFuel / carFuel.maxFuel;
        speedometer.Needle.UpdateNeedle(normalizedSpeed);
        tachometer.Needle.UpdateNeedle(normalizedRPM);
        fuel.Needle.UpdateNeedle(normalizedFuel);

    }
}
