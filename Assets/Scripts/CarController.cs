
using UnityEngine;

public class CarController : MonoBehaviour
{
    private PlayerActions playerActions;
    private Rigidbody2D rb;
    private Collider2D carCollider;
    [Header("Fyzikální hodnoty")]
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float weight = 1f;
    public float speed = 0f;
    public float steeringPower = 1f;
    public float brakeForce = 2f;
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


        float forwardSpeed = Vector2.Dot(rb.linearVelocity, transform.up);
        if (throttleInput > 0.1f)
        {
            if (forwardSpeed >= 0f)
            {
                rb.linearDamping = 0f; // Jedeme - vypínáme odpor
                rb.AddForce(transform.up * throttleInput * acceleration, ForceMode2D.Force);
            }
            else
            rb.linearDamping = 5f;
        }
        else if (throttleInput < -0.1)
        {
            if(forwardSpeed>0.1f)
            rb.linearDamping = brakeForce;
            else
            rb.AddForce(transform.up * throttleInput * acceleration, ForceMode2D.Force);
        }
        else
        {
            rb.linearDamping = 2.5f; // Pustili jsme plyn - brzdíme motorem
        }

        
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
