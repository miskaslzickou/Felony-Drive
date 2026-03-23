
using UnityEngine;

public class CarController : MonoBehaviour
{
    private PlayerActions playerActions;
    private Rigidbody2D rb;
    private Collider2D carCollider;
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float weight = 1f;
    public float speed = 0f;
    public float steeringPower = 1f;
    public AnimationCurve steeringCurve; // køivka pro úpravu síly øízení v závislosti na rychlosti
    private void Awake()
    {
       playerActions = new PlayerActions(); //importování ovládání 
       rb = GetComponent<Rigidbody2D>();
       carCollider = GetComponent<Collider2D>();
        rb.mass = weight; //nastavení hmotnosti auta
    }
    private void OnEnable()
    {
      
        playerActions.Car.Enable();
    }

    private void OnDisable()
    {
        playerActions.Car.Disable();
    }

    private void FixedUpdate()
    {
        float throttleInput=playerActions.Car.Throttle.ReadValue<float>();
        float steeringInput = playerActions.Car.Turning.ReadValue<float>();
        // TODO  pøidat zde smart linear drag pro actually good akceleraci a brždìní, aby se auto chovalo lépe 
        // když hráè drží plyn linear drag =0 , když hráè nepouští plyn linear drag = 1, když hráè brzdí linear drag = 2

        rb.AddForce(transform.up * throttleInput *acceleration); //pøidání síly pro pohyb auta
        float forwardSpeed = Vector2.Dot(rb.linearVelocity, transform.up);
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);
        float speedFactor = steeringCurve.Evaluate(Mathf.Abs(normalizedSpeed));
        
        if (Mathf.Abs(forwardSpeed) > 0.1f)
        {
            rb.AddTorque(steeringInput * steeringPower* speedFactor * Mathf.Sign(forwardSpeed));
        }
        
        Vector2 lateralVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        rb.linearVelocity = forwardVelocity + (lateralVelocity * 0.4f);
        speed = rb.linearVelocity.magnitude;
        
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
           
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
       
    }
}
