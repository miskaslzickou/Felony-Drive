using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public UIDocument uiDocument;
    public string needleElement = "Needle";
    public string speedometerElement ="Speedometer";
    public string gearElement = "Gear";
    public Transform target;
    private float normalizedSpeed;
    [Header("Úhly (Ve stupních)")]
    public float minAngle = 0f; 
    public float maxAngle = 271f;
    private VisualElement needle;
    private VisualElement speedometer;
    private VisualElement gear;
    public Color lightsOnColour; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        needle = root.Q<VisualElement>(needleElement);
        speedometer = root.Q<VisualElement>(speedometerElement);
        gear=root.Q<VisualElement>(gearElement);
    }

    // Update is called once per frame
    public void ChangeSpeedometerTint(bool state)
    {
        if (state) {
            speedometer.style.unityBackgroundImageTintColor =new Color(76f / 255f, 231f / 255f, 222f / 255f);
            gear.style.color = new Color(76f / 255f, 231f / 255f, 222f / 255f);
        }
        else
        {
            speedometer.style.unityBackgroundImageTintColor = Color.white;
            gear.style.color =  Color.white;

        }
    }
    void Update()
    {
        normalizedSpeed = target.GetComponent<CarControllerV2>().normalizedSpeed;
        needle.style.rotate = new Rotate(new Angle(Mathf.Lerp(minAngle, maxAngle, normalizedSpeed), AngleUnit.Degree));
        
    }
}
