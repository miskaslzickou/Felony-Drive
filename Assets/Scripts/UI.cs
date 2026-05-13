//Michal Mikuš, 3C, PVA, Felony Drive

using UnityEngine;
using System;
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
public class ButtonHandler
{   
    private Button button;
    
    public ButtonHandler(string elementName, UIDocument uiDocument,Action onClickFunc,Action onHoverEnter,Action onHoverLeave) { 
        var root = uiDocument.rootVisualElement;
        button = root.Q<Button>(elementName);
        button.clicked +=onClickFunc;
        button.RegisterCallback<PointerEnterEvent>(e => onHoverEnter());
        button.RegisterCallback<PointerLeaveEvent>(e => onHoverLeave());
    }
    public ButtonHandler(string elementName, UIDocument uiDocument, Action onClickFunc)
    {
        var root = uiDocument.rootVisualElement;
        button = root.Q<Button>(elementName);
        button.clicked += onClickFunc;
       
    }
}
public class UI : MonoBehaviour
{
    [Header("Nastavení UI")]
    public UIDocument uiDocument;
    public string needleSpeedElementName = "NeedleSpeed";
    public string speedometerElementName ="Speedometer";
    public string needleRPMElementName = "NeedleRPM";
    public string rpmElementName = "RPM";
    public string gearElementName = "Gear";
    public string needleFuelElementName = "NeedleFuel";
    public string nextStationButtonElementName = "Right";
    public string prevStationButtonElementName = "Left";
    public string nextSongButtonElementName = "NextSong";
    public string prevSongButtonElementName = "PrevSong";
    public string playPauseButtonElementName = "PlayPause";
    public UIData UIData;
    public CarRadio carRadio;
    public Transform target;
    [Header("Nastavení budíků")]
    public Gauge speedometer;
    public Gauge tachometer;
    public Gauge fuel;
    private CarControllerV2 carController;
    private CarFuel carFuel;
    private CarGearBox carGearBox;
    private CarNitro carNitro;
    private VisualElement gear;
    [Header("Nastavení nitro")]
    public string nitroFillElementName = "LiquidFill";
    private VisualElement nitroFill;
    public float maxSloshAngle = 20f;
    public float maxSloshYOffset = 15f;
    public float sloshSpeed = 5f;
    private float currentSloshRotation = 0f;
    private float currentSloshY = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        
        var root = uiDocument.rootVisualElement;
        
        speedometer.Initialize(speedometerElementName, uiDocument);
        tachometer.Initialize(rpmElementName, uiDocument);
        speedometer.Needle.Initialize(needleSpeedElementName, uiDocument);
        tachometer.Needle.Initialize(needleRPMElementName, uiDocument);
        gear =root.Q<VisualElement>(gearElementName);
        nitroFill = root.Q<VisualElement>(nitroFillElementName);

        carController = target.GetComponent<CarControllerV2>();
        carFuel = target.GetComponent<CarFuel>();
        carGearBox = target.GetComponent<CarGearBox>();
        carNitro = target.GetComponent<CarNitro>();
        fuel.Needle.Initialize(needleFuelElementName, uiDocument);
        
        ButtonHandler nextStationButton = new ButtonHandler(nextStationButtonElementName, uiDocument, () => carRadio.NextStation(1));
        ButtonHandler prevStationButton = new ButtonHandler(prevStationButtonElementName, uiDocument, () => carRadio.NextStation(-1));
        ButtonHandler nextSongButton= new ButtonHandler(nextSongButtonElementName, uiDocument, () => carRadio.NextSong(1));
        ButtonHandler prevSongButton= new ButtonHandler(prevSongButtonElementName, uiDocument, () => carRadio.NextSong(-1));
        ButtonHandler playPauseButton= new ButtonHandler(playPauseButtonElementName, uiDocument, () => carRadio.PlayPause());
    }

    public void UpdateNitroUI()
    {
        float ratio =carNitro.currentNitro / carNitro.maxNitro;
        float targetRotation = -carController.steeringInput * maxSloshAngle*carController.normalizedSpeed;
        currentSloshRotation = Mathf.Lerp(currentSloshRotation, targetRotation, Time.deltaTime * sloshSpeed);

        float targetYOffset = -carController.normalizedSpeed * maxSloshYOffset;
        currentSloshY = Mathf.Lerp(currentSloshY, targetYOffset, Time.deltaTime * sloshSpeed);

        nitroFill.style.rotate = new Rotate(new Angle(currentSloshRotation, AngleUnit.Degree));

        nitroFill.style.translate = new Translate(0, Mathf.Lerp(99,0,Mathf.Clamp01(ratio -currentSloshY)), 0);
       
    }

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
        UpdateNitroUI();
    }
}
