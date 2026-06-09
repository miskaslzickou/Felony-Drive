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
    public bool engineStarted = false;
    public bool isHonking = false;
    public bool autoShifting = false;
    private float forwardSpeed => Vector2.Dot(rb.linearVelocity, transform.up);
    public float normalizedSpeed => (rb != null) ? (Mathf.Abs(forwardSpeed / maxSpeed)) : 0f;
    public float optimalSteeringSpeed = 9.8f;//rychlost v m/s, při které je zatáčení nejefektivnější

    //public AnimationCurve steeringCurve=AnimationCurve.Linear(0,1,1,0.5f); // křivka pro úpravu síly řízení v závislosti na rychlosti
    [Header("Nastavení náprav(Gripu)")]
    public float frontGrip = 10f;
    public float rearGrip = 2.5f;
    private float currRearGrip;
    public float axleDistance = 0.75f;
    public float rearLateralSpeed;
    private bool lightsOn = false;
    [Header("Komponenty")]
    private CarEffects carEffects; // Reference na skript pro efekty
    private CarGearBox carGearBox; // Reference na skript pro převodovku
    public CarNitro carNitro; // Reference na skript pro nitro
    public CarFuel carFuel;

    public Vector2 CarCoords => transform.position;
    public float Heading => transform.eulerAngles.z;
    public void SetHeading(float heading) => transform.eulerAngles = new Vector3(0, 0, heading);
    public void SetCoords(Vector2 coords) => transform.position = coords;
    private void Awake()
    {

        playerActions = new PlayerActions(); //importování ovládání
        rb = GetComponent<Rigidbody2D>();
        carCollider = GetComponent<Collider2D>();
        carEffects = GetComponent<CarEffects>();
        carGearBox = GetComponent<CarGearBox>();
        carNitro = GetComponent<CarNitro>();
        carFuel = GetComponent<CarFuel>();


        rb.mass = weight; //nastavení hmotnosti auta
        currRearGrip = rearGrip;

        playerActions.Car.Handbrake.performed += ctx =>
        {
            isHandbrake = ctx.ReadValueAsButton();
            currRearGrip = isHandbrake ? rearGrip * 0.5f : rearGrip;
            rb.linearDamping = brakeForce;
        };
        playerActions.Car.Handbrake.canceled += ctx =>
        {
            currRearGrip = rearGrip;
            isHandbrake = false;
            rb.linearDamping = 0f;
        };
        playerActions.Car.EngineStartStop.performed += ctx => EngineStart();
        playerActions.Car.Honk.started += ctx =>
        {
            carEffects.Honk();
            isHonking = true;
        };

        // dodělat troubení s držením tlačítka, aby se přehrávalo dokud je člověk drží


        playerActions.Car.Honk.canceled += ctx =>
        {
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

        engineStarted = !engineStarted;
        carEffects.StartEngineSound();
    }
    private float GetSteeringMultiplier()
    {

        float absSpeed = Mathf.Abs(forwardSpeed);

       
        if (absSpeed <= optimalSteeringSpeed)
        {
            float t = Mathf.InverseLerp(0f, optimalSteeringSpeed, absSpeed);
            return Mathf.Lerp(0.1f, 1f, t);
        }

        else
        {
            float t = Mathf.InverseLerp(optimalSteeringSpeed, maxSpeed, absSpeed);
            return Mathf.Lerp(1f, 0.6f, t);
        }
    }

  
    void AutoShift()
    {
        if (!engineStarted) return;

        Gear gear = carGearBox.CurrentGear;

       
        if (throttleInput > 0.1f)
        {
         
            if (forwardSpeed < -0.5f) return;

         
            if (carGearBox.currentGear < 2)
            {
                carGearBox.ShiftUp();
                return;
            }

        
            if (forwardSpeed > gear.maxSpeed * 0.9f)
            {
                carGearBox.ShiftUp();
                return;
            }
        }
    
        else if (throttleInput < -0.1f)
        {
       
            if (forwardSpeed > 0.2f) return;

         
            if (carGearBox.currentGear > 0)
            {
                carGearBox.ShiftDown();
                return;
            }
        }

    
        if (carGearBox.currentGear > 2 &&
            forwardSpeed < carGearBox.CurrentGear.maxSpeed * 0.4f)
        {
            carGearBox.ShiftDown();
        }
    }

    
    void ApplyGearForce(Gear gear)
    {
        float gearAccel = gear.gearAcceleration;       
        if (Mathf.Abs(gearAccel) < 0.001f) return;     

        float gearDir = Mathf.Sign(gearAccel);          
        float maxSpeedAbs = Mathf.Abs(gear.maxSpeed);

   
        float speedInDir = forwardSpeed * gearDir;
        float speedFactor = (maxSpeedAbs > 0f) ? 1f - (speedInDir / maxSpeedAbs) : 0f;
        speedFactor = Mathf.Clamp01(speedFactor);

   
        float boost = (gearDir > 0f && carNitro != null && carNitro.nitroActive)
            ? carNitro.nitroBoost
            : 1f;

        rb.AddForce(transform.up * gearAccel * speedFactor * boost);
    }

   
    void ApplyManualDrive(Gear gear)
    {
        if (throttleInput > 0.1f)
        {
            rb.linearDamping = 0f;
            isBraking = false;
            ApplyGearForce(gear);
        }
        else if (throttleInput < -0.1f)
        {
       
            rb.linearDamping = brakeForce;
            isBraking = true;
        }
        else
        {
            rb.linearDamping = cruiseDamping;
            isBraking = false;
        }
    }

 
    void ApplyAutoDrive(Gear gear)
    {
        if (throttleInput > 0.1f)
        {
            if (forwardSpeed < -0.2f)
            {
              
                rb.linearDamping = brakeForce;
                isBraking = true;
            }
            else
            {
                rb.linearDamping = 0f;
                isBraking = false;
                ApplyGearForce(gear); 
            }
        }
        else if (throttleInput < -0.1f)
        {
            if (forwardSpeed > 0.2f)
            {
               
                rb.linearDamping = brakeForce;
                isBraking = true;
            }
            else
            {
               
                rb.linearDamping = 0f;
                isBraking = false;
                ApplyGearForce(gear);
            }
        }
        else
        {
            rb.linearDamping = cruiseDamping;
            isBraking = false;
        }
    }

    void ApplySteeringAndGrip()
    {
        if (Mathf.Abs(forwardSpeed) > 0.13f)
            rb.AddTorque(steeringInput * steeringPower * GetSteeringMultiplier());

        Vector2 frontAxlePos = (Vector2)transform.position + (Vector2)transform.up * axleDistance;
        Vector2 rearAxlePos = (Vector2)transform.position - (Vector2)transform.up * axleDistance;

        Debug.DrawLine(transform.position, frontAxlePos, Color.red);
        Debug.DrawLine(transform.position, rearAxlePos, Color.blue);

        Vector2 frontVelocity = rb.GetPointVelocity(frontAxlePos);
        Vector2 rearVelocity = rb.GetPointVelocity(rearAxlePos);

        float frontLateralSpeed = Vector2.Dot(frontVelocity, transform.right);
        rearLateralSpeed = Vector2.Dot(rearVelocity, transform.right);

        Vector2 frontFriction = -transform.right * frontLateralSpeed * frontGrip * rb.mass;
        Vector2 rearFriction = -transform.right * rearLateralSpeed * currRearGrip * rb.mass;

        rb.AddForceAtPosition(frontFriction, frontAxlePos, ForceMode2D.Force);
        rb.AddForceAtPosition(rearFriction, rearAxlePos, ForceMode2D.Force);

        if (Mathf.Abs(steeringInput) < 0.05f && Mathf.Abs(rb.angularVelocity) < 0.5f)
            rb.angularVelocity = 0f;
    }

    void UpdateSpeed()
    {
      
        if (autoShifting)
            AutoShift();
        if (carFuel.currentFuel < 0.2f)
        {
            engineStarted = false;
            carEffects.StartEngineSound();
        }

      
        Gear currentGear = carGearBox.CurrentGear;

      
        if (engineStarted)
        {
            if (autoShifting)
                ApplyAutoDrive(currentGear);
            else
                ApplyManualDrive(currentGear);
        }
        else
        {
            rb.linearDamping = cruiseDamping;
            isBraking = false;
        }

      
        ApplySteeringAndGrip();
    }

    void GetInputs()
    {
        throttleInput = playerActions.Car.Throttle.ReadValue<float>();
        steeringInput = playerActions.Car.Turning.ReadValue<float>();
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