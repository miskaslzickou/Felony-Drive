using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float cameraMoveSpeed = 10f;
    public float maxZoomIn = 0.9f;
    public float maxZoomOut = 4.9f;
    private PlayerActions playerActions;
    private void Awake()
    {
        playerActions = new PlayerActions();
        playerActions.Camera.Zoom.performed += ctx =>
        {
            float zoomInput = ctx.ReadValue<float>();
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomInput, maxZoomIn, maxZoomOut);
        };
    }
    private void OnEnable()
    {
        playerActions.Camera.Enable();
    }

    private void OnDisable()
    {
        playerActions.Camera.Disable();
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