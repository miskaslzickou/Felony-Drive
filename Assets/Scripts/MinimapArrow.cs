//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;

public class MinimapArrow : MonoBehaviour
{
    public static Transform target; // pickup nebo dropoff
    public Transform player;
    private SpriteRenderer spriteRenderer;  
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null){ 
            spriteRenderer.enabled = false;
            return;
        }
        spriteRenderer.enabled = true;
        Vector2 dir = (Vector2)target.position - (Vector2)player.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);




    }
}
