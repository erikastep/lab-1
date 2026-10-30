using UnityEngine;
using UnityEngine.AI;

// Code agent: pursue when far, evade when close
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

    // Seek: go to a position
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    // Flee: go away from a position
    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - transform.position;
        agent.SetDestination(transform.position - fleeVector);
    }

    // Pursue: seek where the player will be
    void Pursue()
    {
        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / (agent.speed + Mathf.Abs(targetDrive.currentSpeed));
        Seek(target.transform.position + target.transform.forward * targetDrive.currentSpeed * lookAhead);
    }

    // Evade: flee from where the player will be
    void Evade()
    {
        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / (agent.speed + Mathf.Abs(targetDrive.currentSpeed));
        Flee(target.transform.position + target.transform.forward * targetDrive.currentSpeed * lookAhead);
    }

    void Update()
    {
        float distance = Vector3.Distance(target.transform.position, transform.position);

        // Switch between pursue and evade (buffer stops flickering)
        if (distance < safetyDistance - buffer)
            isFleeing = true;
        else if (distance > safetyDistance + buffer)
            isFleeing = false;

        // Do the behaviour
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

    // Draw the safety distance circles
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safetyDistance);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, safetyDistance - buffer);
        Gizmos.DrawWireSphere(transform.position, safetyDistance + buffer);
    }
}
