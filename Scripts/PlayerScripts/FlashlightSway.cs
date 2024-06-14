using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightSway : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private float timer = 0.0f;
    public float bobbingSpeed = 0.05f;
    public float bobbingAmount = 0.04f;
    public float midpoint = 0.22f;
    public float flashlightSwayAmount = 0.02f;
    public float flashlightSwaySpeed = 1f;
    public float movingFlashlightSwayAmount = 0.04f; // Increased sway amount when moving
    public float movingFlashlightSwaySpeed = 1.5f;  // Increased sway speed when moving
    public float swayTransitionSpeed = 2f; // Speed of transition between sway and original position

    private bool isMoving = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // Check if the player is moving
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        isMoving = Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0;

        // Set the target position based on movement
        targetPosition = isMoving ? originalPosition : (originalPosition + new Vector3(0f, flashlightSwayAmount, 0f));
    }

    void FixedUpdate()
    {
        float waveslice = 0.0f;

        // Calculate head bobbing only when moving
        if (isMoving)
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        else
        {
            // Reset timer when not moving to prevent idle sway
            timer = 0.0f;
        }

        // Calculate flashlight sway
        float horizontalSway = Mathf.Sin(Time.time * flashlightSwaySpeed) * flashlightSwayAmount;
        float verticalSway = Mathf.Cos(Time.time * flashlightSwaySpeed) * flashlightSwayAmount;

        // Apply combined movement
        Vector3 cSharpConversion = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * swayTransitionSpeed);
        cSharpConversion.x += horizontalSway;

        // Apply head bobbing when moving
        if (isMoving)
        {
            float translateChange = waveslice * bobbingAmount;
            cSharpConversion.y = midpoint + translateChange;
        }

        cSharpConversion.z += verticalSway;

        transform.localPosition = cSharpConversion;
    }
}