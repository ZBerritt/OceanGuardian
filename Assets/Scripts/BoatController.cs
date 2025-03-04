using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TopDown
{
    public class BoatController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        private Vector2 facing = Vector2.up;
        private Vector2 movementDirection;
        private Vector2 currentInput;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = movementDirection * moveSpeed;
        }

        
        private Vector3 GetDirection(Vector3 input)
        {
            Vector3 finalDirection = Vector2.zero;
            if (input.y > 0.01f)
            {
                finalDirection = new Vector2(0, 1);
                rb.SetRotation(0);
            }
            else if (input.y < -0.01f)
            {
                finalDirection = new Vector2(0, -1);
                rb.SetRotation(180);
            }
            else if (input.x > 0.01f)
            {
                finalDirection = new Vector2(1, 0);
                rb.SetRotation(270);
            }
            else if (input.x < -0.01f)
            {
                finalDirection = new Vector2(-1, 0);
                rb.SetRotation(90);
            }
            else
                finalDirection = Vector2.zero;

            return finalDirection;
        }

        #region Input
        private void OnMove(InputValue value)
        {
            currentInput = value.Get<Vector2>().normalized;
            movementDirection = GetDirection(currentInput);
        }

        private void OnInteract(InputValue value)
        {
            SceneManager.LoadScene("TrashFacilityScene");
        }
        #endregion
    }
}