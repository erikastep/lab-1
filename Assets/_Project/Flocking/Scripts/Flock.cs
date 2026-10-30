using System.Linq;
using UnityEngine;

// One boid (leader or follower)
public class Flock : MonoBehaviour
{
    public bool isLeader = false;
    float speed;
    Vector3 wanderGoal;

    void Start()
    {
        // Leaders are slower so followers can keep up
        speed = isLeader ? FlockManager.FM.minSpeed : Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
        wanderGoal = FlockManager.FM.RandomFreePoint();
    }

    void Update()
    {
        FlockManager fm = FlockManager.FM;

        // Direction: flocking rules (or wander for leaders) + obstacle avoidance
        Vector3 direction = isLeader ? Wander() : FlockingRules();
        direction += AvoidObstacles() * fm.avoidWeight;
        direction.y = 0;

        // Check if something is right in front
        bool blocked = Physics.SphereCast(transform.position - transform.forward * 0.3f, 0.3f, transform.forward, out _, 0.6f);

        // Turn
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                  fm.rotationSpeed * (blocked ? 4 : 1) * Time.deltaTime);

        // Move forward (only if not blocked)
        if (!blocked)
            transform.Translate(0, 0, speed * Time.deltaTime);
        else if (isLeader)
            wanderGoal = fm.RandomFreePoint();   // stuck leader picks a new goal

        // Don't overlap other boids
        if (!isLeader)
            KeepPersonalSpace();
    }

    // Leaders: walk to random points
    Vector3 Wander()
    {
        if (Vector3.Distance(transform.position, wanderGoal) < 2)
            wanderGoal = FlockManager.FM.RandomFreePoint();
        return (wanderGoal - transform.position).normalized;
    }

    // Followers: cohesion + align + separation + follow the leader
    Vector3 FlockingRules()
    {
        FlockManager fm = FlockManager.FM;
        Vector3 cohesion = Vector3.zero, align = Vector3.zero, separation = Vector3.zero;
        int num = 0;

        foreach (GameObject other in fm.allBoids)
        {
            if (other == gameObject) continue;
            float distance = Vector3.Distance(transform.position, other.transform.position);

            // Neighbours
            if (distance <= fm.neighbourDistance)
            {
                cohesion += other.transform.position;
                align += other.transform.forward;
                num++;
            }

            // Separation (closer = stronger)
            if (distance > 0 && distance < fm.separationDistance)
                separation += (transform.position - other.transform.position).normalized * (1 - distance / fm.separationDistance);
        }

        Vector3 direction = separation * fm.separationWeight;

        // Cohesion and align
        if (num > 0)
        {
            cohesion = (cohesion / num - transform.position).normalized;
            align = (align / num).normalized;
            direction += cohesion * fm.cohesionWeight + align * fm.alignWeight;
        }

        // Follow a point behind the closest leader
        GameObject leader = fm.leaders.OrderBy(l => Vector3.Distance(l.transform.position, transform.position)).FirstOrDefault();
        if (leader != null)
        {
            Vector3 followPoint = leader.transform.position - leader.transform.forward * fm.followDistance;
            direction += (followPoint - transform.position).normalized * fm.leaderWeight;
        }

        return direction;
    }

    // Obstacle avoidance
    Vector3 AvoidObstacles()
    {
        Vector3 avoid = Vector3.zero;

        // Obstacle in front: slide along it
        if (Physics.SphereCast(transform.position - transform.forward * 0.5f, 0.4f, transform.forward, out RaycastHit hit, FlockManager.FM.avoidDistance + 0.5f))
        {
            Vector3 along = Vector3.Cross(Vector3.up, hit.normal);
            if (Vector3.Dot(along, transform.forward) < 0) along = -along;
            avoid += along + hit.normal;
        }

        // Obstacle very close on any side: push away
        foreach (Collider c in Physics.OverlapSphere(transform.position, 1f))
        {
            Vector3 away = transform.position - c.ClosestPoint(transform.position);
            away.y = 0;
            if (away.magnitude > 0)
                avoid += away.normalized * (1 - away.magnitude);
        }

        return avoid;
    }

    // Step away from boids that are too close
    void KeepPersonalSpace()
    {
        FlockManager fm = FlockManager.FM;
        float personalSpace = fm.separationDistance * 0.6f;
        Vector3 push = Vector3.zero;

        foreach (GameObject other in fm.allBoids)
        {
            Vector3 away = transform.position - other.transform.position;
            if (other != gameObject && away.magnitude > 0 && away.magnitude < personalSpace)
                push += away.normalized * (personalSpace - away.magnitude) * 0.5f;
        }
        push.y = 0;

        // Only if no wall is in the way
        if (push != Vector3.zero && !Physics.SphereCast(transform.position, 0.3f, push, out _, push.magnitude + 0.2f))
            transform.position += push;
    }
}
