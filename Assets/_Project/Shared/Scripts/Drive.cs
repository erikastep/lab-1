using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves the player with the arrow keys: the player walks in the pressed direction and faces it
public class Drive : MonoBehaviour
{
    public float speed = 10.0f;
    public float currentSpeed = 0;   // speed per second, read by the agents to predict where the player goes

    void Update()
    {
        // Arrow keys give a direction on the ground (x = left/right, z = up/down)
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1);   // diagonals are not faster
        currentSpeed = input.magnitude * speed;

        if (input.magnitude > 0.01f)
        {
            // Face the direction we are moving
            transform.rotation = Quaternion.LookRotation(input);

            // Move forward, but only if no wall or obstacle is in the way
            float step = currentSpeed * Time.deltaTime;
            if (!Physics.SphereCast(transform.position, 0.4f, transform.forward, out _, step + 0.1f))
                transform.Translate(0, 0, step);
        }

        // Show the forward direction in the Scene view
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }
}
