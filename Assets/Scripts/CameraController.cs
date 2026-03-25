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
    private void Awake()
    {
      rb = target.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraController: Target player not assigned!");
            return;
        }


        Vector3 desiredPosition = target.position + offset;
        float speedPerc = Vector2.Dot(rb.linearVelocity, transform.up) / 64;

        float targetZoom = Mathf.Lerp(maxZoomIn, maxZoomOut, speedPerc);

        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, cameraMoveSpeed * Time.deltaTime);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSmoothness);

    
    }
}