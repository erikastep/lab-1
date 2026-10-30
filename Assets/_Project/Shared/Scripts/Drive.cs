using UnityEngine;

// Player movement with the arrow keys
public class Drive : MonoBehaviour
{
    public float speed = 10.0f;
    public float currentSpeed = 0;   // used by the code agent

    void Update()
    {
        // Arrow keys -> direction
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1);
        currentSpeed = input.magnitude * speed;

        if (currentSpeed > 0)
        {
            // Face the direction
            transform.rotation = Quaternion.LookRotation(input);

            // Move (only if nothing is in front)
            float step = currentSpeed * Time.deltaTime;
            if (!Physics.SphereCast(transform.position - transform.forward * 0.3f, 0.4f, transform.forward, out _, step + 0.4f))
                transform.Translate(0, 0, step);
        }

        PushOutOfColliders();
    }

    // Push out if something is inside the player
    void PushOutOfColliders()
    {
        Collider me = GetComponent<Collider>();
        foreach (Collider other in Physics.OverlapSphere(transform.position, 1f))
        {
            if (other != me && Physics.ComputePenetration(me, transform.position, transform.rotation,
                                                          other, other.transform.position, other.transform.rotation,
                                                          out Vector3 direction, out float distance))
            {
                direction.y = 0;
                transform.position += direction * distance;
            }
        }
    }
}
