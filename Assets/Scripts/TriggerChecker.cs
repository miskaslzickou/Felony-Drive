//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;

public class TriggerChecker : MonoBehaviour
{
  
    public System.Action onTriggered;
    public System.Action onExit;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        onTriggered?.Invoke();
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        onExit?.Invoke();
    }
}