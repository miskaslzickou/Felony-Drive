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
    public float cruiseDamping = 1.5f; // odpor při jízdě bez plynu
    private bool isHandbrake = false;
    public AnimationCurve steeringCurve; // k�ivka pro �pravu s�ly ��zen� v z�vislosti na rychlosti

    [Header("Nastaven� n�prav(Gripu)")]
    public  float frontGrip = 5f;
    public float rearGrip = 2f;
    private float currRearGrip;
    public float axleDistance = 1f;

    [Header("Nastavení zvukových efektů")]
    public AudioClip engineAudioClip;
    private AudioSource audioSrc;
 
    [Header("Nastavení vizuálních efektů")]
    public float driftThreshold = 2f; // Rychlost, při které se spustí efekt driftu
    public TrailRenderer[] trailRenderers;
    public ParticleSystem[] driftParticles;
    public Animator animator;

    private void Awake()
    {
       playerActions = new PlayerActions(); //importov�n� ovl�d�n� 
       rb = GetComponent<Rigidbody2D>();
       carCollider = GetComponent<Collider2D>();
       rb.mass = weight; //nastaven� hmotnosti auta
       audioSrc= GetComponent<AudioSource>();
       audioSrc.clip = engineAudioClip;
       audioSrc.Play();
       currRearGrip = rearGrip;
        // předělat ruční brzdu aby byla trochu arkádová
       playerActions.Car.Handbrake.performed += ctx =>
        {
            isHandbrake = ctx.ReadValueAsButton();
            currRearGrip = isHandbrake ? rearGrip * 0.5f : rearGrip; 
            rb.linearDamping=brakeForce;
            animator.SetBool("isBraking", true);
        };
        playerActions.Car.Handbrake.canceled += ctx => { 
            currRearGrip=rearGrip;
            isHandbrake=false;
            animator.SetBool("isBraking", false);
           
        };
        

    }
    private void OnEnable()
    {
      
        playerActions.Car.Enable();
    }

    private void OnDisable()
    {
        playerActions.Car.Disable();
    }
    
    [System.Serializable]
    public class Gear
    {
        public string gearName;
        public float gearSpeed;
        public float accelerationPower;
        public float minRpm;
        public float maxRpm;
    }

    private void FixedUpdate()
    {
       
        float throttleInput=playerActions.Car.Throttle.ReadValue<float>(); 
        float steeringInput = playerActions.Car.Turning.ReadValue<float>();

        float forwardSpeed = Vector2.Dot(rb.linearVelocity, transform.up);
        //potřeba překopat tyto ify aby byli více smysluplné 
        if (throttleInput > 0.1f)
        {
            if (forwardSpeed >= -0.1f)
            {
                rb.linearDamping = 0f; // Jedeme - vyp�n�me odpor
                rb.AddForce(transform.up * throttleInput * acceleration, ForceMode2D.Force);
              
            }
            else
            {
                rb.linearDamping = 5f;
            } 
        }
        else if (throttleInput < -0.1)
        {

            if (forwardSpeed > 0.1f)
            {
              
                rb.linearDamping = brakeForce;
            }
            else
            {
             
                rb.AddForce(transform.up * throttleInput * acceleration, ForceMode2D.Force);
            }
        }
        else
        {
            rb.linearDamping = cruiseDamping; // Pustili jsme plyn - brzd�me motorem
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

        // 1. Zjištění lokace přední a zadní nápravy
        Vector2 frontAxlePos = (Vector2)transform.position + (Vector2)transform.up * axleDistance;
        Vector2 rearAxlePos = (Vector2)transform.position - (Vector2)transform.up * axleDistance;

        Debug.DrawLine(transform.position, frontAxlePos, Color.red);
        Debug.DrawLine(transform.position, rearAxlePos, Color.blue);
        // 2. Zjištění rychlosti náprav
        Vector2 frontVelocity = rb.GetPointVelocity(frontAxlePos);
        Vector2 rearVelocity = rb.GetPointVelocity(rearAxlePos);

        // 3. Zjištění boční rychlosti náprav 
        float frontLateralSpeed = Vector2.Dot(frontVelocity, transform.right);
        float rearLateralSpeed = Vector2.Dot(rearVelocity, transform.right);

        // 4. Spočítáme protisílu pneumatik
        Vector2 frontFriction = -transform.right * frontLateralSpeed * frontGrip * rb.mass;
        Vector2 rearFriction = -transform.right * rearLateralSpeed * currRearGrip * rb.mass;
        speed = forwardSpeed;
        rb.AddForceAtPosition(frontFriction, frontAxlePos, ForceMode2D.Force);
        rb.AddForceAtPosition(rearFriction, rearAxlePos, ForceMode2D.Force);
        
        if (Mathf.Abs(rearLateralSpeed) > driftThreshold || isHandbrake)
        {
            for (int i = 0; i < 2; i++)
            {
                trailRenderers[i].emitting = true;
                if (!driftParticles[i].isPlaying) driftParticles[i].Play();
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                trailRenderers[i].emitting = false;
                if (driftParticles[i].isPlaying) driftParticles[i].Stop();
            }
        }

        if (rb.linearVelocity.magnitude > maxSpeed)   
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        
        if(forwardSpeed < -maxReverseSpeed)
            rb.linearVelocity= rb.linearVelocity.normalized * maxReverseSpeed;  
    }
}
