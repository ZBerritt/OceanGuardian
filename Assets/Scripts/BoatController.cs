using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BoatController : MonoBehaviour
{
    private string facing = "North";
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;
    private Vector2 currentInput;
    private float currentSpeed = 0f;

    private GameManager gameManager;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D cl;
    private BoatType boatType;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        boatType = gameManager.BoatDatabase.GetBoatType(gameManager.boatUpgradeLevel, gameManager.boatNetLevel);
    }

    private void FixedUpdate()
    {
        // Update sprite
        switch(facing)
        {
            case "North":
                sr.sprite = boatType.northSprite;
                break;
            case "South":
                sr.sprite = boatType.southSprite;
                break;
            case "East":
                sr.sprite = boatType.eastSprite;
                break;
            case "West":
                sr.sprite = boatType.westSprite;
                break;
        }

        // Update collisions
        cl.size = sr.bounds.size;

        // Update movement data
        if (movementDirection != Vector2.zero)
        {
            lastMovementDirection = movementDirection;
            currentSpeed = Mathf.Min(currentSpeed + boatType.Acceleration * Time.fixedDeltaTime, boatType.MaxSpeed);
        }
        else
        {
            currentSpeed = Mathf.Max(currentSpeed - boatType.Deceleration * Time.fixedDeltaTime, 0f);
        }

        Vector2 directionToUse = movementDirection != Vector2.zero ? movementDirection : lastMovementDirection;
        rb.linearVelocity = directionToUse * currentSpeed;
    }


    private Vector3 GetDirection(Vector3 input)
    {
        Vector3 finalDirection = Vector2.zero;
        if (input.y > 0.01f)
        {
            finalDirection = new Vector2(0, 1);
            facing = "North";
        }
        else if (input.y < -0.01f)
        {
            finalDirection = new Vector2(0, -1);
            facing = "South";
        }
        else if (input.x > 0.01f)
        {
            finalDirection = new Vector2(1, 0);
            facing = "East";
        }
        else if (input.x < -0.01f)
        {
            finalDirection = new Vector2(-1, 0);
            facing = "West";
        }
        else
            finalDirection = Vector2.zero;

        return finalDirection;
    }

    #region Input
    private void OnMove(InputValue value)
    {
        currentInput = value.Get<Vector2>();
        movementDirection = GetDirection(currentInput);
    }

    private void OnInteract(InputValue value)
    {
        SceneManager.LoadScene("TrashFacilityScene");
    }
    #endregion
}