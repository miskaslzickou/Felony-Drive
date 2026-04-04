using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public UIDocument uiDocument;
    public string needleSpeedElement = "NeedleSpeed";
    public string speedometerElement ="Speedometer";
    public string needleRPMElement = "RPMNeedle";
    public string rpmElement = "RPM";
    public string gearElement = "Gear";
    public Transform target;
    private float normalizedSpeed;
    [Header("Úhly (Ve stupních)")]
    public float speedMinAngle = -2f; 
    public float speedMaxAngle = 271f;
    public float rpmMinAngle = 1f;
    public float rpmMaxAngle = 263f;
    private VisualElement needleSpeed;
    private VisualElement needleRPM;
    private VisualElement rpm;
    private VisualElement speedometer;
    private VisualElement gear;
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        needleSpeed = root.Q<VisualElement>(needleSpeedElement);
        speedometer = root.Q<VisualElement>(speedometerElement);
        needleRPM= root.Q<VisualElement>(needleRPMElement);
        rpm = root.Q<VisualElement>(rpmElement);
        gear =root.Q<VisualElement>(gearElement);
    }

    // Update is called once per frame
    public void ChangeSpeedometerTint(bool state)
    {
        if (state) {
            speedometer.style.unityBackgroundImageTintColor =new Color(184f / 255f, 252f / 255f, 242f / 249f);
            rpm.style.unityBackgroundImageTintColor = new Color(184f / 255f, 252f / 255f, 242f / 249f);
            gear.style.color = new Color(184f / 255f, 252f / 255f, 242f / 249f);
        }
        else
        {
            speedometer.style.unityBackgroundImageTintColor = Color.white;
            rpm.style.unityBackgroundImageTintColor= Color.white;
            gear.style.color =  Color.white;

        }
    }
    void Update()
    {
        normalizedSpeed = target.GetComponent<CarControllerV2>().normalizedSpeed;
        needleSpeed.style.rotate = new Rotate(new Angle(Mathf.Lerp(speedMinAngle, speedMaxAngle, normalizedSpeed), AngleUnit.Degree));
        
    }
}
