//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;


public class DeliveryMission : MonoBehaviour
{
    public GameObject dropoff;
    public static bool isDelivering = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDelivering) return;
        isDelivering = true;
        MinimapArrow.target = dropoff.transform;
        dropoff.SetActive(true);
        Destroy(gameObject);
    }
}
