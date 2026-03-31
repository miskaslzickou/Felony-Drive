using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public UIDocument uiDocument;
    public string needleElement = "Needle";
    public Transform target;
    private float normalizedSpeed;
    [Header("Úhly (Ve stupních)")]
    public float minAngle = 0f; 
    public float maxAngle = 271f;
    private VisualElement needle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        needle = root.Q<VisualElement>(needleElement);
        
    }

    // Update is called once per frame
    void Update()
    {
        normalizedSpeed = target.GetComponent<CarControllerV2>().normalizedSpeed;
        needle.style.rotate = new Rotate(new Angle(Mathf.Lerp(minAngle, maxAngle, normalizedSpeed), AngleUnit.Degree));
        
    }
}
