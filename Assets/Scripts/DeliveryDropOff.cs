//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;

public class DeliveryDropOff : MonoBehaviour
{
    public float reward;
   
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        DeliveryMission.isDelivering = false;
        PlayerWallet.Add(reward);
        Destroy(gameObject);
    }

}
