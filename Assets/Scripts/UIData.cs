using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(fileName = "UIData", menuName = "Scriptable Objects/UIData")]
public class UIData : ScriptableObject
{
    [CreateProperty] public string gear;
    [CreateProperty] public float fuel;

}
