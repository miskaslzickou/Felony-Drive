//

using UnityEngine;

public class CarController : MonoBehaviour
{
    private PlayerActions playerActions;
    private Rigidbody2D rb;
    private Collider2D carCollider;
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float weight = 1f;
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
      
        rb.AddForce(transform.up * throttleInput *acceleration); //pøidání síly pro pohyb auta
        float forwardSpeed = Vector2.Dot(rb.linearVelocity, transform.up);
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);
        float speedFactor = steeringCurve.Evaluate(Mathf.Abs(normalizedSpeed));
        if (Mathf.Abs(forwardSpeed) > 0.1f)
        {
            rb.AddTorque(steeringInput * steeringPower* speedFactor * Mathf.Sign(forwardSpeed));
        }



    }
}
