using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] FirearmController firearm = null;
    [SerializeField] ExplosiveItemController grenades = null;
    [SerializeField] ExplosiveItemController mines = null;
    [SerializeField] LayerMask groundLayerMask = 0;

    // horizontal movement
    [SerializeField] float walkAcceleration = 0;
    [SerializeField] float runAcceleration = 0;
    [SerializeField] float maxWalkVelocity = 0;
    [SerializeField] float maxRunVelocity = 0;

    // vertical movement
    [SerializeField] float maxVerticalVelocity = 0;
    [SerializeField] float maxJumpHeight = 0;
    [SerializeField] float timeToApex = 0;

    [SerializeField] float drag = 0;


    CharacterController character;
    Camera cameraObject;
    SphereCollider groundCheck;

    Vector3 currentVelocity;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        cameraObject = Camera.main;
        groundCheck = GetComponent<SphereCollider>();

        currentHeading = transform.rotation.eulerAngles.y;
        currentPitch = cameraObject.transform.rotation.eulerAngles.x;

        gravity = 2 * maxJumpHeight / Mathf.Pow(timeToApex, 2);
        initialJumpVel = Mathf.Sqrt(2 * gravity * maxJumpHeight);
    }

    private void Update()
    {
        if (!GameManager.IsPaused)
        {
            GetInput();
            Looking();

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
            Jumping();
        }

        // move character
        character.Move(transform.TransformVector(currentVelocity) * Time.fixedDeltaTime);
    }

    Vector2 mouse;
    Vector2 joystick;
    bool runInput;
    bool jumpInput;
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

        if (jumpInput)
            initJump = true;

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
        float hVel = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
        float vVel = currentVelocity.y;

        currentVelocity -= currentVelocity * drag * Time.fixedDeltaTime;

        // cancel out movements that are too small
        if (currentVelocity.magnitude <= 0.05f)
            currentVelocity = Vector3.zero;

        // cap horizontal velocity
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        float v = horizontalVelocity.magnitude;

        if (v > CurrentMaxHorizontalVelocity()) // walking/running different cap
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

    float gravity = 0;

    void Gravity()
    {
        if (!IsGrounded())
        {
            Vector3 acceleration = Vector3.down * gravity;
            currentVelocity += acceleration * Time.fixedDeltaTime;
        }
    }

    bool initJump = false;
    bool isJumping = false;
    float initialJumpVel = 0;

    void Jumping()
    { 
        if (!IsGrounded() || isJumping)
            initJump = false;

        if (initJump)
        {
            currentVelocity.y = initialJumpVel;

            isJumping = true;
            initJump = false;
        }

        if (isJumping)
        {
            if (currentVelocity.y < 0)
            {
                isJumping = false;
                currentVelocity.y = 0;
            }
        }
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
            velocity = maxRunVelocity;
        else
            velocity = maxWalkVelocity;

        return velocity;
    }

    bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.TransformPoint(groundCheck.center), groundCheck.radius, groundLayerMask, QueryTriggerInteraction.Ignore))
            return true;
        else
            return false;
    }

    public void ResetVelocity()
    {
        currentVelocity = Vector3.zero;
    }
}