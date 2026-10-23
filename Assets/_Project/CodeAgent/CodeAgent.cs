using UnityEngine;
using UnityEngine.AI;

// Part 1: the agent pursues the player when it is far away
// and evades the player when it is closer than safetyDistance.
public class CodeAgent : MonoBehaviour
{
    public GameObject target;
    public float safetyDistance = 5f;
    public float buffer = 1f;

    NavMeshAgent agent;
    Drive targetDrive;
    bool isFleeing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetDrive = target.GetComponent<Drive>();
    }

    // Go to a position
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    // Go away from a position
    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - transform.position;
        agent.SetDestination(transform.position - fleeVector);
    }

    // Seek the position where the player is going to be
    void Pursue()
    {
        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        Seek(target.transform.position + target.transform.forward * targetDrive.currentSpeed * lookAhead);
    }

    // Flee from the position where the player is going to be
    void Evade()
    {
        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        Flee(target.transform.position + target.transform.forward * targetDrive.currentSpeed * lookAhead);
    }

    void Update()
    {
        float distance = Vector3.Distance(target.transform.position, transform.position);

        // buffer around safetyDistance so the agent does not flicker at the border
        if (distance < safetyDistance - buffer)
            isFleeing = true;
        else if (distance > safetyDistance + buffer)
            isFleeing = false;

        if (isFleeing)
        {
            Evade();
            Debug.DrawLine(transform.position, target.transform.position, Color.red);
        }
        else
        {
            Pursue();
            Debug.DrawLine(transform.position, target.transform.position, Color.green);
        }
    }

    // Draw the safety distance and the buffer in the Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safetyDistance);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, safetyDistance - buffer);
        Gizmos.DrawWireSphere(transform.position, safetyDistance + buffer);
    }
}
