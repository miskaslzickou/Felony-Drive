using System;
using JetBrains.Annotations;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float cameraMoveSpeed = 10f;
    private Rigidbody2D rb;
    public float maxZoomOut=7.9f;
    public float maxZoomIn = 5f;
    public float zoomSmoothness = 3f;
    public float maxForwardOffset = 3f; // Maximální posun kamery dopředu při vysoké rychlosti
    private float maxSpeed; 
    private void Awake()
    {
      rb = target.GetComponent<Rigidbody2D>();
      maxSpeed=target.GetComponent<CarController>().maxSpeed; //Získání maxSpeed z CarControlleru pro správné nastavení zoomu
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraController: Target player not assigned!");
            return;
        }

        float speedPerc = Vector2.Dot(rb.linearVelocity, transform.up) / maxSpeed;

        float targetZoom = Mathf.Lerp(maxZoomIn, maxZoomOut, speedPerc);
        float currOffsetDist =Mathf.Lerp(0f,maxForwardOffset,speedPerc);
        Vector3 currOffset =target.transform.up * currOffsetDist+offset; //Sečtení nastaveného offsetu s dynamickým posunem dopředu
        Vector3 desiredPosition = target.position + currOffset;
       
  

        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, cameraMoveSpeed * Time.deltaTime);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSmoothness);

    
    }
}