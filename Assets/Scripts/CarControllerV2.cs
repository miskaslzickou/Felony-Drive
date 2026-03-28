 using UnityEngine;


public class CarControllerV2 : MonoBehaviour
{
    private PlayerActions playerActions;
   
    private Rigidbody2D rb;
    private Collider2D carCollider;
    [Header("Fyzikální hodnoty")]
    public float maxSpeed = 61f;
    public float maxReverseSpeed = 10f;
    public float acceleration = 10f;
    public float weight = 1f;
    public float speed => rb.linearVelocity.magnitude;
    public float steeringPower = 5f;
    public float brakeForce = 2f;
    public float cruiseDamping = 1.5f; // odpor při jízdě bez plynu
    private bool isHandbrake = false;
    private bool isBraking = false;
    private float throttleInput;
    private float steeringInput;
    private float steerAngle;
    private float forwardSpeed => Vector2.Dot(rb.linearVelocity, transform.up);
    private float normalizedSpeed => Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);

    //public AnimationCurve steeringCurve; // k�ivka pro �pravu s�ly ��zen� v z�vislosti na rychlosti

    [Header("Nastaven� n�prav(Gripu)")]
    public float frontGrip = 10f;
    public float rearGrip = 2.5f;
    private float currRearGrip;
    public float axleDistance = 0.75f;
    private float rearLateralSpeed;

    [Header("Nastavení zvukových efektů")]
    public AudioClip engineAudioClip;
    [SerializeField]private AudioSource engineAudioSrc;
    public AudioClip honkAudioClip;
    [SerializeField]private AudioSource honkAudioSrc;
    private bool isHonking = false;

    [Header("Nastavení vizuálních efektů")]
    public float driftThreshold = 2f; // Rychlost, při které se spustí efekt driftu
    public TrailRenderer[] trailRenderers;
    public ParticleSystem[] driftParticles;
    public Animator animator;
    
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerActions = new PlayerActions(); //importov�n� ovl�d�n� 
        rb = GetComponent<Rigidbody2D>();
        carCollider = GetComponent<Collider2D>();
        rb.mass = weight; //nastaven� hmotnosti auta
     
        engineAudioSrc.clip = engineAudioClip;
        engineAudioSrc.Play();
        honkAudioSrc.clip = honkAudioClip;


        currRearGrip = rearGrip;
      
        playerActions.Car.Handbrake.performed += ctx =>
        {
            isHandbrake = ctx.ReadValueAsButton();
            currRearGrip = isHandbrake ? rearGrip * 0.5f : rearGrip;
            rb.linearDamping = brakeForce;
           
        };
        playerActions.Car.Handbrake.canceled += ctx => {
            currRearGrip = rearGrip;
            isHandbrake = false;
            rb.linearDamping = 0f;


        };
        // dodělat troubení s držením tlačítka, aby se přehrávalo dokud je držíš
        playerActions.Car.Honk.performed += ctx => {
            isHonking = true;
            if (!honkAudioSrc.isPlaying) honkAudioSrc.Play();
            
        };

        // Když tlačítko pustíš, přestane troubit
        playerActions.Car.Honk.canceled += ctx => {
            isHonking = false;
            honkAudioSrc.Stop();
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
    
    void UpdateSpeed()
    {


        if (throttleInput == 1)
        {
            rb.linearDamping = 0f;
            rb.AddForce(transform.up * throttleInput * acceleration, ForceMode2D.Force);
        }
        else if (throttleInput == -1)
        {
            rb.linearDamping = brakeForce;
            isBraking = true;
        }
        else
        {
            rb.linearDamping = cruiseDamping;
            isBraking = false;
        }
        
        if (Mathf.Abs(forwardSpeed) > 0.1f)
        {
            rb.AddTorque(steeringInput * steeringPower *normalizedSpeed * Mathf.Sign(forwardSpeed));
        }
        // testoval jsem různé fyzikální způsoby magic formula, ale toto i když to je daleko od dokonalého má nejvíc konzistentní chování 
        
        Vector2 frontAxlePos = (Vector2)transform.position + (Vector2)transform.up * axleDistance;
        Vector2 rearAxlePos = (Vector2)transform.position - (Vector2)transform.up * axleDistance;

        Debug.DrawLine(transform.position, frontAxlePos, Color.red);
        Debug.DrawLine(transform.position, rearAxlePos, Color.blue);
        // 2. Zjištění rychlosti náprav
        Vector2 frontVelocity = rb.GetPointVelocity(frontAxlePos);
        Vector2 rearVelocity = rb.GetPointVelocity(rearAxlePos);

        // 3. Zjištění boční rychlosti náprav 
        float frontLateralSpeed = Vector2.Dot(frontVelocity, transform.right);
        rearLateralSpeed = Vector2.Dot(rearVelocity, transform.right);

        // 4. Spočítáme protisílu pneumatik
        Vector2 frontFriction = -transform.right * frontLateralSpeed * frontGrip * rb.mass;
        Vector2 rearFriction = -transform.right * rearLateralSpeed * currRearGrip * rb.mass;
     
        rb.AddForceAtPosition(frontFriction, frontAxlePos, ForceMode2D.Force);
        rb.AddForceAtPosition(rearFriction, rearAxlePos, ForceMode2D.Force);

        if (speed > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        if (forwardSpeed < -maxReverseSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxReverseSpeed;
        if (Mathf.Abs(steeringInput) < 0.05f && Mathf.Abs(rb.angularVelocity) < 0.5f)
        {
            rb.angularVelocity = 0f;
        }
        //Debug.Log($"Speed: {rb.linearVelocity.magnitude} | Angular: {rb.angularVelocity}");
    }
   
    void UpdateVisuals()
    {
        if (isHandbrake|| Mathf.Abs(rearLateralSpeed) >driftThreshold)
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
        animator.SetBool("isBraking", (isBraking || isHandbrake));

    }
    void UpdateAudio()
    {
        engineAudioSrc.pitch = 1f + Mathf.Clamp01(normalizedSpeed); // Základní pitch 0.5, který se zvyšuje s rychlostí
    
    }
    void GetInputs() 
    {   
        throttleInput = playerActions.Car.Throttle.ReadValue<float>();
        steeringInput= playerActions.Car.Turning.ReadValue<float>();
    }
    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateVisuals();
        UpdateAudio();
    }
    void FixedUpdate()
    {   
        UpdateSpeed();
        
    
    }
}
