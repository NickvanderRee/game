using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public float jumpForce = 7f;
    public float gravityMultiplier = 2f;
    public Transform cameraTransform;
    public float interactionDistance = 3f;
    public float interactionHoldTime = 2f;
    public LayerMask interactableLayerMask;
    public GameObject interactionPrefab; // Reference to the interaction UI prefab

    private Rigidbody rb;
    private bool isGrounded;
    private float verticalRotation = 0f;
    private float interactionTimer = 3f;
    private bool isInteracting = false;

    private GameObject interactionTextObject; // Reference to the instantiated interaction UI
    private TextMeshProUGUI interactionTextComponent;
    private TextMeshProUGUI interactionText; // Reference to the TextMeshProUGUI component
    private Image background; // Reference to the background image

    private GameObject currentInteractable;

    public bool HasKey { get; private set; } = false;
    private bool atExit = false;
    private bool hasWon = false; // Flag to track if the game has been won

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        interactionTextObject = Instantiate(interactionPrefab, Vector3.zero, Quaternion.identity);
        interactionTextComponent = interactionTextObject.GetComponentInChildren<TextMeshProUGUI>(); // Assign interactionTextComponent
        interactionText = interactionTextObject.GetComponentInChildren<TextMeshProUGUI>();
        background = interactionTextObject.GetComponentInChildren<Image>();
        if (interactionText == null)
        {
            Debug.LogError("Interaction Text is not found in the prefab.");
        }
        if (background == null)
        {
            Debug.LogError("Background Image is not found in the prefab.");
        }
        interactionText.text = "";
        background.color = new Color(0f, 0f, 0f, 0.75f); // Transparent black
        interactionText.color = new Color(1f, 0.92f, 0.016f); // Golden color
    }

    void Update()
    {
        LookAround();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        CheckForInteractable();
        HandleInteraction();
    }

    void FixedUpdate()
    {
        CheckGround();
        MovePlayer();
        ApplyExtraGravity();
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localEulerAngles = new Vector3(verticalRotation, cameraTransform.localEulerAngles.y, cameraTransform.localEulerAngles.z);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jumped");
    }

    void ApplyExtraGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void CheckGround()
    {
        RaycastHit hit;
        float distance = 1.1f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance, LayerMask.GetMask("Floor")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void UpdateInteractionUI()
    {
        if (isInteracting)
        {
            // Calculate the progress ratio of the interaction timer
            float progressRatio = interactionTimer / interactionHoldTime;

            // Update the text and box color based on the progress ratio
            interactionText.text = progressRatio.ToString("P0"); // Display the progress percentage
            if (HasKey) // Player has the key
            {
                interactionText.color = Color.green; // Set text color to green
                background.color = Color.green; // Set box color to green
            }
            else // Player does not have the key
            {
                interactionText.color = Color.red; // Set text color to red
                background.color = Color.red; // Set box color to red
            }

            // Calculate the maximum line width
            float maxLineWidth = 200f;

            // Update the line size around the box based on the progress ratio
            RectTransform rectTransform = interactionTextObject.GetComponent<RectTransform>();
            float lineWidth = Mathf.Min(maxLineWidth * progressRatio, maxLineWidth); // Ensure line width doesn't exceed the maximum
            rectTransform.sizeDelta = new Vector2(lineWidth, 60f); // Adjust the line size dynamically
        }
        else
        {
            // Hide the UI when not interacting
            interactionText.text = "";
            interactionTextComponent.gameObject.SetActive(false);
        }
    }

    void HandleInteraction()
    {
        if (isInteracting && currentInteractable != null && Input.GetKey(KeyCode.E))
        {
            interactionTimer += Time.deltaTime;
            if (interactionTimer >= interactionHoldTime)
            {
                if (currentInteractable.CompareTag("Key"))
                {
                    Debug.Log("Key picked up!");
                    HasKey = true;
                    Destroy(currentInteractable);
                }
                else if (currentInteractable.CompareTag("Exit"))
                {
                    if (HasKey)
                    {
                        Debug.Log("Exiting the maze...");
                        hasWon = true; // Player has won
                        DisplayWinUI(); // Show "You win!" message
                        Time.timeScale = 0f; // Freeze the game
                    }
                    else
                    {
                        Debug.Log("You need to find the key first!");
                    }
                }
                interactionTimer = 0f;
                isInteracting = false;
            }
            else
            {
                // Update the interaction UI based on the interaction timer progress
                UpdateInteractionUI();
            }
        }
        else
        {
            interactionTimer = 0f;
        }
    }

    void CheckForInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance, interactableLayerMask))
        {
            if (hit.collider.CompareTag("Key"))
            {
                interactionText.text = "Hold 'E' to pick up the key";
                currentInteractable = hit.collider.gameObject;
                isInteracting = true;
                interactionTextComponent.gameObject.SetActive(true); // Show the UI
                background.gameObject.SetActive(true); // Show the panel
                return;
            }
            else if (hit.collider.CompareTag("Exit") && HasKey)
            {
                interactionText.text = "Hold 'E' to use the exit";
                currentInteractable = hit.collider.gameObject;
                isInteracting = true;
                interactionTextComponent.gameObject.SetActive(true); // Show the UI
                background.gameObject.SetActive(true); // Show the panel
                return;
            }
        }

        // If not looking at an interactable object or not having the key, hide the UI and panel
        interactionText.text = "";
        currentInteractable = null;
        isInteracting = false;
        interactionTextComponent.gameObject.SetActive(false); // Hide the UI
        background.gameObject.SetActive(false); // Hide the panel
    }

    void DisplayWinUI()
    {
        if (!hasWon) // Check if the game hasn't been won yet
        {
            interactionText.text = "You win!";
            interactionTextComponent.gameObject.SetActive(true);
            hasWon = true; // Set the hasWon flag to true to prevent further calls
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit"))
        {
            atExit = true;
            Debug.Log("Approached the exit.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Exit"))
        {
            atExit = false;
        }
    }
}