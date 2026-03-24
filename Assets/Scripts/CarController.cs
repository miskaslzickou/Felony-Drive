
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

    [Header("Nastavení náprav(Gripu)")]
    public float frontGrip = 5f;
    public float rearGrip = 2f;
    public float axleDistance = 1f;

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
            if (forwardSpeed >= -0.1f)
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

        // --- SIMULACE PNEUMATIK A DRIFTU ---

        // 1. Zjistíme, kde pøesnì ve 2D svìtì se nachází naše pøední a zadní náprava
        Vector2 frontAxlePos = (Vector2)transform.position + (Vector2)transform.up * axleDistance;
        Vector2 rearAxlePos = (Vector2)transform.position - (Vector2)transform.up * axleDistance;

        // 2. Zeptáme se enginu: "Jakou rychlostí letí tyto konkrétní body?"
        Vector2 frontVelocity = rb.GetPointVelocity(frontAxlePos);
        Vector2 rearVelocity = rb.GetPointVelocity(rearAxlePos);

        // 3. Vytáhneme z toho jen to klouzání DO BOKU (ignorujeme jízdu dopøedu)
        float frontLateralSpeed = Vector2.Dot(frontVelocity, transform.right);
        float rearLateralSpeed = Vector2.Dot(rearVelocity, transform.right);

        // 4. Spoèítáme protisílu (tøení pneumatik), která to klouzání zastaví
        // Vynásobíme to hmotností auta (rb.mass), aby to fungovalo i pro tìžký Charger
        Vector2 frontFriction = -transform.right * frontLateralSpeed * frontGrip * rb.mass;
        Vector2 rearFriction = -transform.right * rearLateralSpeed * rearGrip * rb.mass;

        // 5. APLIKUJEME SÍLU! Neviditelná ruka tlaèí nápravy zpátky do stopy.
        rb.AddForceAtPosition(frontFriction, frontAxlePos, ForceMode2D.Force);
        rb.AddForceAtPosition(rearFriction, rearAxlePos, ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
           
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
       
    }
}
