 using UnityEngine;


public class CarControllerV2 : MonoBehaviour
{
    private PlayerActions playerActions;
    private Rigidbody2D rb;
    private Collider2D carCollider;
    [Header("Fyzikální hodnoty")]
    public float maxSpeed = 61f;
    public float maxReverseSpeed = 10f;
    //public float acceleration = 10f;
    public float weight = 1f;
    public float speed => rb.linearVelocity.magnitude;
    public float steeringPower = 5f;
    public float brakeForce = 2f;
    public float cruiseDamping = 1.5f; // odpor při jízdě bez plynu
    public bool isHandbrake = false;
    public bool isBraking = false;
    private float throttleInput;
    private float steeringInput;
    private float steerAngle;
    private bool engineStarted=false;
    private float forwardSpeed => Vector2.Dot(rb.linearVelocity, transform.up);
    public float normalizedSpeed => (rb != null) ? (Mathf.Abs(forwardSpeed/ maxSpeed)) : 0f;
    //public AnimationCurve steeringCurve; // k�ivka pro �pravu s�ly ��zen� v z�vislosti na rychlosti
    [Header("Nastaven� n�prav(Gripu)")]
    public float frontGrip = 10f;
    public float rearGrip = 2.5f;
    private float currRearGrip;
    public float axleDistance = 0.75f;
    public float rearLateralSpeed;
    private bool lightsOn=false;
    [Header("Komponenty")]
    private CarEffects carEffects; // Reference na skript pro efekty
    private CarGearBox carGearBox; // Reference na skript pro převodovku



    private void Awake()
    {
        playerActions = new PlayerActions(); //importov�n� ovl�d�n� 
        rb = GetComponent<Rigidbody2D>();
        carCollider = GetComponent<Collider2D>();
        carEffects = GetComponent<CarEffects>();
        carGearBox = GetComponent<CarGearBox>();
        rb.mass = weight; //nastaven� hmotnosti auta
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
        playerActions.Car.EngineStartStop.performed += ctx =>EngineStart();
        playerActions.Car.Honk.performed += ctx => {
            carEffects.Honk(true);
        };

        // dodělat troubení s držením tlačítka, aby se přehrávalo dokud je držíš
        // Když tlačítko pustíš, přestane troubit
        playerActions.Car.Honk.canceled += ctx => {
           carEffects.Honk(false);
        };
        playerActions.Car.LightsOnOff.performed += ctx =>
        {
            lightsOn = !lightsOn;
            carEffects.Lights(lightsOn);
        };

        playerActions.Car.ShiftUp.performed += ctx =>
        {
            carGearBox.ShiftUp();
        };
        playerActions.Car.ShiftDown.performed += ctx =>
        {
            carGearBox.ShiftDown();
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
    private void EngineStart()
    {
        
        engineStarted=!engineStarted;
        carEffects.StartEngineSound(engineStarted);
    }
    void UpdateSpeed()
    {
       Gear currentGear = carGearBox.CurrenGear;
        Debug.Log($"Current Gear: {currentGear.name} | Speed: {forwardSpeed}");
        if (throttleInput == 1 && engineStarted)
        {
            rb.linearDamping = 0f;
            float speedFactor = 1 - (forwardSpeed / currentGear.maxSpeed);
            speedFactor = Mathf.Clamp01(speedFactor);
            float finalForce=throttleInput * currentGear.gearAcceleration*speedFactor;
            rb.AddForce(transform.up *finalForce);
        }
        else if (throttleInput == -1 )
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
            rb.AddTorque(steeringInput * steeringPower  * Mathf.Sign(forwardSpeed));
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

        if (Mathf.Abs(steeringInput) < 0.05f && Mathf.Abs(rb.angularVelocity) < 0.5f)
        {
            rb.angularVelocity = 0f;
        }
        //Debug.Log($"Speed: {rb.linearVelocity.magnitude} | Angular: {rb.angularVelocity}");
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
    }
    void FixedUpdate()
    {   
        UpdateSpeed();
    }
}
