//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;


public class DeliveryMission : MonoBehaviour
{
    public GameObject dropoff;
   
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        dropoff.SetActive(true);
        Destroy(gameObject);
    }
}
