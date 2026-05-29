//Michal Mikuš, 3C, PVA, Felony Drive
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
   
    public float throttleInput { get; private set; }
    public float steeringInput { get; private set; }
    public bool engineStarted=false;
    public bool isHonking=false;
    public bool autoShifting = false;
    private float forwardSpeed => Vector2.Dot(rb.linearVelocity, transform.up);
    public float normalizedSpeed => (rb != null) ? (Mathf.Abs(forwardSpeed/ maxSpeed)) : 0f;
    public float optimalSteeringSpeed = 9.8f;//rychlost v m/s, při které je zatáčení nejefektivnější
   
    //public AnimationCurve steeringCurve=AnimationCurve.Linear(0,1,1,0.5f); // křivka pro úpravu síly řízení v závislosti na rychlosti
    [Header("Nastavení náprav(Gripu)")]
    public float frontGrip = 10f;
    public float rearGrip = 2.5f;
    private float currRearGrip;
    public float axleDistance = 0.75f;
    public float rearLateralSpeed;
    private bool lightsOn=false;
    [Header("Komponenty")]
    private CarEffects carEffects; // Reference na skript pro efekty
    private CarGearBox carGearBox; // Reference na skript pro převodovku
    private CarNitro carNitro; // Reference na skript pro nitro


    private void Awake()
    {
        playerActions = new PlayerActions(); //importov�n� ovl�d�n� 
        rb = GetComponent<Rigidbody2D>();
        carCollider = GetComponent<Collider2D>();
        carEffects = GetComponent<CarEffects>();
        carGearBox = GetComponent<CarGearBox>();
        carNitro = GetComponent<CarNitro>();
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
            carEffects.Honk();
            isHonking = true;
        };

        // dodělat troubení s držením tlačítka, aby se přehrávalo dokud je člověk drží

        // Když tlačítko pustíš, přestane troubit
        playerActions.Car.Honk.canceled += ctx => {
           carEffects.Honk();
            isHonking = false;
        };
        playerActions.Car.LightsOnOff.performed += ctx =>
        {
            lightsOn = !lightsOn;
            carEffects.Lights(lightsOn);
        };

        playerActions.Car.ShiftUp.performed += ctx =>
        {
            if (autoShifting) return;
            carGearBox.ShiftUp();
        };
        playerActions.Car.ShiftDown.performed += ctx =>
        {
            if (autoShifting) return;
            carGearBox.ShiftDown();
        };
        playerActions.Car.Nitro.performed += ctx =>
        {
            carNitro.ToggleNitro();
        };
        playerActions.Car.AutoShift.performed += ctx =>
        {
            autoShifting = !autoShifting;
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
        carEffects.StartEngineSound();
    }
    private float GetSteeringMultiplier()
    {
        
        float absSpeed = Mathf.Abs(forwardSpeed);

        // zatáčení se zlepšuje z 0.1 na 1.0 při nižších rychlostech, pak tuhne z 1.0 na 0.4 při vyšších rychlostech
        if (absSpeed <= optimalSteeringSpeed)
        {
            float t = Mathf.InverseLerp(0f, optimalSteeringSpeed, absSpeed);
            return Mathf.Lerp(0.1f, 1f, t); // Výsledek je 0.1 až 1.0
        }
      
        else
        {
            float t = Mathf.InverseLerp(optimalSteeringSpeed, maxSpeed, absSpeed);
            return Mathf.Lerp(1f, 0.6f, t); // Výsledek je 1.0 až 0.4
        }
    }
    void UpdateSpeed()
    {
       Gear currentGear = carGearBox.CurrentGear;
        if(carGearBox.rpm>2000 && (carGearBox.currentGear != 1 || carGearBox.currentGear != 0)){
            engineStarted = false;
            carEffects.StartEngineSound();
        }
        if (autoShifting) {

            AutoShift();
        }


        if (throttleInput == 1 && engineStarted||(throttleInput==-1 && carGearBox.currentGear==0 && autoShifting))
        {
            float direction = carGearBox.currentGear==0 && autoShifting ? -1f : 1f;
            rb.linearDamping = 0f;
            float speedFactor = 1 - (forwardSpeed / currentGear.maxSpeed);
            speedFactor = Mathf.Clamp01(speedFactor);
            float finalForce=throttleInput * currentGear.gearAcceleration*direction*speedFactor*(carNitro.nitroActive? carNitro.nitroBoost : 1f);
            rb.AddForce(transform.up *finalForce);
        }
        else if (throttleInput == -1||(throttleInput==1&&carGearBox.currentGear==0) && autoShifting)
        {            
            rb.linearDamping = brakeForce;
            isBraking = true;   
        }
        else
        {
            rb.linearDamping = cruiseDamping;
            isBraking = false;
        }
        
        if (Mathf.Abs(forwardSpeed) > 0.13f)
        {
            rb.AddTorque(steeringInput * steeringPower * GetSteeringMultiplier());
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
    void AutoShift()
    {
        if (forwardSpeed > carGearBox.CurrentGear.maxSpeed * 0.9f && forwardSpeed>0)
            carGearBox.ShiftUp();
        else if (forwardSpeed < carGearBox.CurrentGear.maxSpeed  * 0.5f && carGearBox.currentGear != 2)
            carGearBox.ShiftDown();
        else if (throttleInput == -1 && forwardSpeed < 0.1 && carGearBox.currentGear!=0)
        {
            carGearBox.ShiftDown();
        }
        else if(throttleInput == 1 && forwardSpeed < -0.1 && carGearBox.currentGear==0)
        {
            carGearBox.ShiftUp();
                  
        }
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
