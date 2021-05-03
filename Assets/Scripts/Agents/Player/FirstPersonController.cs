using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] FirearmController firearm = null;
    [SerializeField] ExplosiveItemController grenades = null;
    [SerializeField] ExplosiveItemController mines = null;

    [SerializeField] SphereCollider groundCheckArea = null;
    [SerializeField] LayerMask groundLayerMask = 0;

    // horizontal movement
    [SerializeField] float walkAcceleration = 0;
    [SerializeField] float runAcceleration = 0;
    [SerializeField] float maxWalkSpeed = 0;
    [SerializeField] float maxRunSpeed = 0;

    // vertical movement
    [SerializeField] float jumpVelocity = 0;
    [SerializeField] float maxVerticalVelocity = 0;
    [SerializeField] float fallMultiplier = 0;
    [SerializeField] float lowJumpMultiplier = 0;

    [SerializeField] float drag = 0;


    CharacterController character;
    Camera cameraObject;

    Vector3 currentVelocity;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        cameraObject = Camera.main;

        currentHeading = transform.rotation.eulerAngles.y;
        currentPitch = cameraObject.transform.rotation.eulerAngles.x;
    }

    private void Update()
    {
        if (!GameManager.IsPaused)
        {
            GetInput();
            Looking();
            Jumping();

            Shooting();
            PlacingMines();
            ThrowingGrenades();
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.IsPaused)
        {
            Drag();
            Moving();
            Gravity();

            // move character
            character.Move(transform.TransformVector(currentVelocity) * Time.fixedDeltaTime);
        }
    }

    Vector2 mouse;
    Vector2 joystick;
    bool runInput;
    bool jumpInput;
    bool longJumpInput;

    bool shootInput;
    bool throwGrenadeInput;
    bool placeMineInput;

    void GetInput()
    {
        mouse.x = Input.GetAxis("Mouse X");
        mouse.y = Input.GetAxis("Mouse Y");

        joystick.x = Input.GetAxis("Horizontal");
        joystick.y = Input.GetAxis("Vertical");

        runInput = Input.GetButton("Run");
        jumpInput = Input.GetButtonDown("Jump");
        longJumpInput = Input.GetButton("Jump");

        shootInput = Input.GetMouseButtonDown(0);
        placeMineInput = Input.GetMouseButtonDown(1);
        throwGrenadeInput = Input.GetButtonDown("Throw");
    }

    float currentHeading = 0;
    float currentPitch = 0;

    void Looking()
    {
        // rotate player left/right
        float x = mouse.x;
        currentHeading += x;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentHeading);

        // rotate camera up/down
        float y = mouse.y;
        currentPitch -= y;
        currentPitch = Mathf.Clamp(currentPitch, -90, 90);

        cameraObject.transform.localEulerAngles = new Vector3(currentPitch, cameraObject.transform.localEulerAngles.y);
    }

    void Drag()
    {
        // apply drag
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        currentVelocity -= currentVelocity * drag * Time.fixedDeltaTime;

        // cancel out movements that are too small
        if (currentVelocity.magnitude <= 0.005f)
            currentVelocity = Vector3.zero;

        // cap horizontal velocity
        float horizontalSpeed = horizontalVelocity.magnitude;

        if (horizontalSpeed > CurrentMaxHorizontalVelocity()) // walking/running different cap
        {
            Vector3 newVel = horizontalVelocity.normalized * CurrentMaxHorizontalVelocity();
            currentVelocity = new Vector3(newVel.x, currentVelocity.y, newVel.z);
        }

        // cap vertical velocity
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, -maxVerticalVelocity, maxVerticalVelocity);
    }

    void Moving()
    {
        Vector3 movement = new Vector3(joystick.x, 0, joystick.y).normalized;
        Vector3 acceleration = movement * CurrentHorizontalAcceleration();
        currentVelocity += acceleration;
    }

    void Gravity()
    {
        Vector3 acceleration = Vector3.up * Physics.gravity.y;
        currentVelocity += acceleration * Time.fixedDeltaTime;

        if (currentVelocity.y <= 0)
            currentVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (currentVelocity.y > 0 && !longJumpInput)
            currentVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    void Jumping()
    {
        if (jumpInput && IsGrounded())
            currentVelocity.y = jumpVelocity;
    }

    void Shooting()
    {
        if (shootInput)
            firearm.Shoot();
    }

    void PlacingMines()
    {
        if (placeMineInput)
            mines.Use();
    }

    void ThrowingGrenades()
    {
        if (throwGrenadeInput)
            grenades.Use();
    }

    float CurrentHorizontalAcceleration()
    {
        float acceleration;

        if (runInput)
            acceleration = runAcceleration;
        else
            acceleration = walkAcceleration;

        return acceleration;
    }

    float CurrentMaxHorizontalVelocity()
    {
        float velocity;

        if (runInput)
            velocity = maxRunSpeed;
        else
            velocity = maxWalkSpeed;

        return velocity;
    }

    bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.TransformPoint(groundCheckArea.center), groundCheckArea.radius, groundLayerMask, QueryTriggerInteraction.Ignore))
            return true;
        else
            return false;
    }

    public void ResetVelocity()
    {
        currentVelocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (groundCheckArea != null)
        {
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(groundCheckArea.center), groundCheckArea.radius);
        }
    }
}