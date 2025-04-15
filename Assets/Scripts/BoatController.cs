using System.Collections;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BoatController : MonoBehaviour
{
    private float currentRotation = 0f; // Rotation in degrees, 0 = North, 90 = East, etc.
    private Vector2 currentInput;
    private float currentSpeed = 0f;
    private bool freeze = false;
    private bool stunned = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cl;
    private BoatType boatType;

    [Header("Boat Handling")]
    [SerializeField] private float turnSpeed = 180f; // Degrees per second

    [Header("Stun Effects")]
    [SerializeField] private float stunDuration = 2f;

    private void Start()
    {
        GameManager.Instance.OnDayEnd += OnDayEnd;
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        boatType = GameManager.Instance.BoatDatabase.GetBoatType(GameManager.Instance.boatUpgradeLevel, GameManager.Instance.boatNetLevel);
        sr.sprite = boatType.northSprite;
        cl.size = sr.bounds.size;
        freeze = false;
        stunned = false;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnDayEnd -= OnDayEnd;
    }

    private void OnDayEnd()
    {
        freeze = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void FixedUpdate()
    {
        if (freeze || stunned) return; // Disable movement

        // Handle turning
        HandleSteering();

        // Handle acceleration/deceleration
        HandleAcceleration();

        // Apply movement
        ApplyMovement();

        // Update sprite based on rotation
        UpdateSprite();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Crash!");

        if (collision.gameObject.CompareTag("Obstacle") && !stunned)
        {
            StartCoroutine(StunBoat());
        }
    }

    private IEnumerator StunBoat()
    {
        stunned = true;
        rb.linearVelocity = -(rb.linearVelocity + (rb.linearVelocity.normalized / 4f)); // Bounce back
        currentSpeed = 0f;

        for (float spinTimer = 0f; spinTimer < stunDuration; spinTimer += Time.deltaTime)
        {
            currentRotation += 180f * Time.deltaTime;

            // Keep rotation between 0-360
            if (currentRotation < 0)
                currentRotation += 360f;
            else if (currentRotation >= 360f)
                currentRotation -= 360f;

            // Update transform rotation
            transform.rotation = Quaternion.Euler(0, 0, -currentRotation);

            // Update sprite based on rotation
            UpdateSprite();

            yield return null;
        }

        // Reset velocity after stun
        rb.linearVelocity = Vector2.zero;

        // End stun
        stunned = false;
    }

    private void HandleSteering()
    {
        // Handle left/right steering
        if (Mathf.Abs(currentInput.x) > 0.01f)
        {
            // Turn rate is affected by current speed (harder to turn at low speeds)
            float turnRate = turnSpeed * Time.fixedDeltaTime * Mathf.Max(0.2f, Mathf.Abs(currentSpeed) / boatType.MaxSpeed);

            // Always turn in the direction of input, regardless of forward/reverse
            currentRotation += currentInput.x * turnRate;

            // Keep rotation between 0-360
            if (currentRotation < 0)
                currentRotation += 360f;
            else if (currentRotation >= 360f)
                currentRotation -= 360f;
        }

        // Update transform rotation
        transform.rotation = Quaternion.Euler(0, 0, -currentRotation);
    }

    private void HandleAcceleration()
    {
        // Forward/backward movement
        float targetSpeed = 0f;

        if (Mathf.Abs(currentInput.y) > 0.01f)
        {
            if (currentInput.y > 0)
            {
                // Forward
                targetSpeed = boatType.MaxSpeed * currentInput.y;
            }
            else
            {
                // Reverse
                targetSpeed = boatType.MaxSpeed * 0.5f * currentInput.y; // Reverse is slower
            }
        }

        // Apply acceleration/deceleration
        if (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed))
        {
            // Accelerating
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, boatType.Acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerating
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, boatType.Deceleration * Time.fixedDeltaTime);
        }
    }

    private void ApplyMovement()
    {
        // Calculate direction vector based on current rotation
        // In Unity, 0 degrees points up (North), and degrees increase clockwise
        float angleRadians = currentRotation * Mathf.Deg2Rad;

        // The correct formula for direction vector in Unity's coordinate system:
        // x = sin(angle), y = cos(angle) for 0 degrees pointing up
        Vector2 directionVector = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));

        // Apply velocity - direction is already accounted for in the speed sign
        rb.linearVelocity = directionVector * currentSpeed;
    }

    private void UpdateSprite()
    {
        // Determine which sprite to use based on rotation
        // We'll use the closest cardinal direction
        float normalizedRotation = (currentRotation + 45) % 360;
        int directionIndex = Mathf.FloorToInt(normalizedRotation / 90);

        switch (directionIndex)
        {
            case 0: // North (315-45 degrees)
                sr.sprite = boatType.northSprite;
                break;
            case 1: // East (45-135 degrees)
                sr.sprite = boatType.eastSprite;
                break;
            case 2: // South (135-225 degrees)
                sr.sprite = boatType.southSprite;
                break;
            case 3: // West (225-315 degrees)
                sr.sprite = boatType.westSprite;
                break;
        }
    }

    #region Input
    private void OnMove(InputValue value)
    {
        currentInput = value.Get<Vector2>();
    }

    // private void OnInteract(InputValue value)
    // {
    //     SceneManager.LoadScene("TrashFacilityScene");
    // }
    #endregion
}