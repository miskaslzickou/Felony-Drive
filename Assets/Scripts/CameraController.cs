using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float cameraMoveSpeed = 10f;
    public float maxZoomIn = 0.9f;
    public float maxZoomOut = 4.9f;

    private void Awake()
    {
      
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraController: Target player not assigned!");
            return;
        }


        Vector3 desiredPosition = target.position + offset;


        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, cameraMoveSpeed * Time.deltaTime);



    }
}