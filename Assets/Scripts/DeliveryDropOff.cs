//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;

public class DeliveryDropOff : MonoBehaviour
{
    public float reward;
   
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerWallet.Add(reward);
        Destroy(gameObject);
    }

}
