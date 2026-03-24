
using UnityEngine;

public class CarController : MonoBehaviour
{
    private PlayerActions playerActions;
    private Rigidbody2D rb;
    private Collider2D carCollider;
    [Header("Fyzik�ln� hodnoty")]
    public float maxSpeed = 10f;
    public float maxReverseSpeed=10f;
    public float acceleration = 5f;
    public float weight = 1f;
    public float speed = 0f;
    public float steeringPower = 1f;
    public float brakeForce = 2f;
    public AnimationCurve steeringCurve; // k�ivka pro �pravu s�ly ��zen� v z�vislosti na rychlosti

    [Header("Nastaven� n�prav(Gripu)")]
    public float frontGrip = 5f;
    public float rearGrip = 2f;
    public float axleDistance = 1f;

    [Header("Nastavení efektů")]
    private AudioSource audioSrc;
    public AudioClip audioClip;




    private void Awake()
    {
       playerActions = new PlayerActions(); //importov�n� ovl�d�n� 
       rb = GetComponent<Rigidbody2D>();
       carCollider = GetComponent<Collider2D>();
       rb.mass = weight; //nastaven� hmotnosti auta
       audioSrc= GetComponent<AudioSource>();

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
                rb.linearDamping = 0f; // Jedeme - vyp�n�me odpor
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
            rb.linearDamping = 2.5f; // Pustili jsme plyn - brzd�me motorem
        }

        
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);
        float speedFactor = steeringCurve.Evaluate(Mathf.Abs(normalizedSpeed));
        
        if (Mathf.Abs(forwardSpeed) > 0.1f)
        {
            rb.AddTorque(steeringInput * steeringPower* speedFactor * Mathf.Sign(forwardSpeed));
        }

        // --- SIMULACE ZVUKU ---
        audioSrc.pitch=1+normalizedSpeed;


        // --- SIMULACE PNEUMATIK A DRIFTU ---

        // 1. Zjist�me, kde p�esn� ve 2D sv�t� se nach�z� na�e p�edn� a zadn� n�prava
        Vector2 frontAxlePos = (Vector2)transform.position + (Vector2)transform.up * axleDistance;
        Vector2 rearAxlePos = (Vector2)transform.position - (Vector2)transform.up * axleDistance;

        // 2. Zept�me se enginu: "Jakou rychlost� let� tyto konkr�tn� body?"
        Vector2 frontVelocity = rb.GetPointVelocity(frontAxlePos);
        Vector2 rearVelocity = rb.GetPointVelocity(rearAxlePos);

        // 3. Vyt�hneme z toho jen to klouz�n� DO BOKU (ignorujeme j�zdu dop�edu)
        float frontLateralSpeed = Vector2.Dot(frontVelocity, transform.right);
        float rearLateralSpeed = Vector2.Dot(rearVelocity, transform.right);

        // 4. Spo��t�me protis�lu (t�en� pneumatik), kter� to klouz�n� zastav�
        // Vyn�sob�me to hmotnost� auta (rb.mass), aby to fungovalo i pro t�k� Charger
        Vector2 frontFriction = -transform.right * frontLateralSpeed * frontGrip * rb.mass;
        Vector2 rearFriction = -transform.right * rearLateralSpeed * rearGrip * rb.mass;

        // 5. APLIKUJEME S�LU! Neviditeln� ruka tla�� n�pravy zp�tky do stopy.
        rb.AddForceAtPosition(frontFriction, frontAxlePos, ForceMode2D.Force);
        rb.AddForceAtPosition(rearFriction, rearAxlePos, ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)   
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        
        if(forwardSpeed < -maxReverseSpeed)
            rb.linearVelocity= rb.linearVelocity.normalized * maxReverseSpeed;
       
    }
}
