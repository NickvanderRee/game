using UnityEngine;

public class FlashlightRaycaster : MonoBehaviour
{
    public GameObject flashlight; 
    public float maxDistance = 10f; 
    public float flashlightRotationSpeed = 0.5f; 
    public LayerMask layerMask; 
    public float aimRadius = 0.2f; 
    public float flashlightSwaySpeed = 1.0f; 
    public float flashlightSwayAmount = 1.0f;
    public float hitPointSwaySpeed = 1.0f; 

    private Vector3 originalPosition; // Original position of the flashlight
    private Vector3 originalRotation; // Original rotation of the flashlight
    private bool isMoving = false; // Flag to indicate if the player is moving

    private void Start()
    {
        originalPosition = flashlight.transform.localPosition; // Store the original position
        originalRotation = flashlight.transform.localRotation.eulerAngles; // Store the original rotation
    }

    private void Update()
    {
        RotateFlashlightToHitPoint();
        ApplyFlashlightSway();
    }

    private void FixedUpdate()
    {
        // Check if the player is moving
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        isMoving = Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0;
    }


void RotateFlashlightToHitPoint()
{
    // Get the main camera transform
    Transform cameraTransform = Camera.main.transform;

    // Create a ray from the center of the camera
    Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

    // Perform a raycast into the scene with the layer mask
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
    {
        // Rotate the flashlight towards the hit point
        Vector3 directionToHitPoint = hit.point - flashlight.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToHitPoint);
        flashlight.transform.rotation = Quaternion.Lerp(flashlight.transform.rotation, targetRotation, flashlightRotationSpeed * Time.deltaTime);
    }
}

private void ApplyFlashlightSway()
{
    // Calculate flashlight positional sway
    float horizontalSway = Mathf.Sin(Time.time * flashlightSwaySpeed) * flashlightSwayAmount;
    float verticalSway = Mathf.Cos(Time.time * flashlightSwaySpeed) * flashlightSwayAmount;

    // Apply combined positional movement
    Vector3 newPosition = originalPosition;
    newPosition.x += horizontalSway;
    newPosition.z += verticalSway;

    flashlight.transform.localPosition = newPosition;
}
}