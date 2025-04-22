using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    private Vector2 movementDirection;
    private Vector2 facing = Vector2.down;
    private Vector2 currentInput;

    [Header("Animations")]
    private Animator animator;
    private string lastDirection = "Down";

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 0.16f;

    [SerializeField] private Canvas canvas;

    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.position = GameManager.Instance.playerPosition;
    }

    private void OnDestroy()
    {
        GameManager.Instance.playerPosition = rb.position;
    }
    private void Update()
    {
        HandleAnimations();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = movementDirection * moveSpeed;
    }

    private void HandleAnimations()
    {
        if (animator == null) return;

        string animationPrefix = movementDirection == Vector2.zero ? "Idle" : "Walking";
        animator.Play("Player_" + animationPrefix + lastDirection);
    }
    private Vector3 GetDirection(Vector3 input)
    {
        Vector3 finalDirection;

        if (input.y > 0.01f)
        {
            lastDirection = "Up";
            finalDirection = new Vector2(0, 1);
        }
        else if (input.y < -0.01f)
        {
            lastDirection = "Down";
            finalDirection = new Vector2(0, -1);
        }
        else if (input.x > 0.01f)
        {
            lastDirection = "Right";
            finalDirection = new Vector2(1, 0);
        }
        else if (input.x < -0.01f)
        {
            lastDirection = "Left";
            finalDirection = new Vector2(-1, 0);
        }
        else
        {
            finalDirection = Vector2.zero;
        }

        return finalDirection;
    }

    // Player Input Callbacks
    private void OnMove(InputValue value)
    {
        currentInput = value.Get<Vector2>().normalized;
        movementDirection = GetDirection(currentInput);
        if (movementDirection.x > 0 || movementDirection.y > 0)
        {
            facing = movementDirection;
        }
    }

    private void OnInteract(InputValue value)
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, facing, interactDistance, LayerMask.GetMask("Default"));

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name); // Debugging output
            switch(hit.collider.gameObject.name)
            {
                case "BoatDockArea":
                    if (GameManager.Instance.timeOfDay == TimeOfDay.Morning)
                    {
                        SceneManager.LoadScene("OceanScene");
                    }
                    break;
                case "MiniGameArea":
                    if (GameManager.Instance.timeOfDay == TimeOfDay.Evening)
                    {
                        SceneManager.LoadScene("Minigame");
                    }
                    break;
                case "WorkshopMenuArea":
                    ToggleWorkshopMenu();
                    break;
            }
        }
    }
    private void ToggleWorkshopMenu()
    {
        canvas.enabled = true;
    }
}