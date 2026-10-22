using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves the player with the arrow keys (same as the professor's Drive.cs)
public class Drive : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float currentSpeed = 0;   // read by the agents to predict where the player goes

    void Update()
    {
        // Get the horizontal and vertical axis (arrow keys), values from -1 to 1
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        // Move per second instead of per frame
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move forward/back on the z-axis and rotate around the y-axis
        transform.Translate(0, 0, translation);
        currentSpeed = translation;
        transform.Rotate(0, rotation, 0);

        // Show the forward direction in the Scene view
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }
}
