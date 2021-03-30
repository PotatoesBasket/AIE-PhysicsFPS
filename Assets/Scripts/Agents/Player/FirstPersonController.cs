using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    CharacterController character;
    Camera cameraObject;

    [SerializeField] FirearmController firearm = null;
    [SerializeField] ExplosiveItemController grenades = null;
    [SerializeField] ExplosiveItemController mines = null;

    // x/z move
    [SerializeField] float walkAcceleration = 0;
    [SerializeField] float runAcceleration = 0;
    [SerializeField] float maxWalkVelocity = 0;
    [SerializeField] float maxRunVelocity = 0;

    // y move
    [SerializeField] float gravity = 0;
    [SerializeField] float jumpAcceleration = 0;
    [SerializeField] float maxVerticalVelocity = 0;


    [SerializeField] float mass = 0;
    [SerializeField] float drag = 0;
    Vector3 currentVelocity;


    [SerializeField] float groundRayDist = 0;
    [SerializeField] LayerMask groundLayerMask = 0;

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
            Moving();

            Gravity();
            Jumping();

            Shooting();
            PlacingMines();
            ThrowingGrenades();
        }
    }

    private void FixedUpdate()
    {
        // apply drag
        currentVelocity -= currentVelocity * drag * Time.fixedDeltaTime;

        if (currentVelocity.magnitude <= 0.1f)
            currentVelocity = Vector3.zero;
        
        // cap horizontal velocity
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        float v = horizontalVelocity.magnitude;

        if (v > CurrentMaxHorizontalVelocity())
        {
            Vector3 newVel = horizontalVelocity.normalized * CurrentMaxHorizontalVelocity();
            currentVelocity = new Vector3(newVel.x, currentVelocity.y, newVel.z);
        }

        // cap vertical velocity
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, -maxVerticalVelocity, maxVerticalVelocity);

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

    void Moving()
    {
        Vector3 movement = new Vector3(joystick.x, 0, joystick.y).normalized;
        Vector3 acceleration = movement * CurrentHorizontalAcceleration() / mass;
        currentVelocity += acceleration;
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            Vector3 acceleration = Vector3.down * gravity / mass;
            currentVelocity += acceleration;
        }
    }

    void Jumping()
    { 
        if (IsGrounded() && jumpInput)
        {
            currentVelocity.y = 0; // reset y vel first because game jumps aren't realistic lol

            Vector3 acceleration = Vector3.up * jumpAcceleration / mass;
            currentVelocity += acceleration;
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
        if (Physics.Raycast(transform.position, -transform.up, groundRayDist, groundLayerMask))
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
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -transform.up * groundRayDist);
    }
}