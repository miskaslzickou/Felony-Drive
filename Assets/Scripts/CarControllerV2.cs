 using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]

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
    public float steeringPower = 3f;
    public float brakeForce = 2f;
    public float cruiseDamping = 1.5f; // odpor při jízdě bez plynu
    private bool isHandbrake = false;
    private bool isBraking = false;

    private float forwardSpeed => Vector2.Dot(rb.linearVelocity, transform.up);
    private float normalizedSpeed => Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);

    //public AnimationCurve steeringCurve; // k�ivka pro �pravu s�ly ��zen� v z�vislosti na rychlosti

    [Header("Nastaven� n�prav(Gripu)")]
    public float frontGrip = 10f;
    public float rearGrip = 2.5f;
    private float currRearGrip;
    public float axleDistance = 0.75f;

    [Header("Nastavení zvukových efektů")]
    public AudioClip engineAudioClip;
    private AudioSource audioSrc;
    
    [Header("Nastavení vizuálních efektů")]
    public float driftThreshold = 2f; // Rychlost, při které se spustí efekt driftu
    public TrailRenderer[] trailRenderers;
    public ParticleSystem[] driftParticles;
    public Animator animator;
    
    private float throttleInput;
    private float steeringInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerActions = new PlayerActions(); //importov�n� ovl�d�n� 
        rb = GetComponent<Rigidbody2D>();
        carCollider = GetComponent<Collider2D>();
        rb.mass = weight; //nastaven� hmotnosti auta
        audioSrc = GetComponent<AudioSource>();
        audioSrc.clip = engineAudioClip;
        audioSrc.Play();
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
        if(throttleInput==1)
        rb.AddForce(transform.up * throttleInput * acceleration, ForceMode2D.Force);
         else if(throttleInput==-1)
        {
            rb.linearDamping = brakeForce;
            isBraking = true;
        }
        else
        {
            rb.linearDamping = cruiseDamping;
            isBraking = false;
        }




        if (speed > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        if (forwardSpeed < -maxReverseSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxReverseSpeed;

    }
    void UpdateSteering() {
        if(Mathf.Abs(forwardSpeed) > 0.1f)
        {
            rb.AddTorque(steeringInput * steeringPower * Mathf.Sign(forwardSpeed));
        }
        
    }
    void UpdateVisuals()
    {
        if (isHandbrake)
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
        audioSrc.pitch = 0.5f + normalizedSpeed * 1.5f; // Základní pitch 0.5, který se zvyšuje s rychlostí
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
        UpdateSteering();
    }
}
