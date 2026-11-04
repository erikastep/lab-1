using System.Linq;
using UnityEngine;

// One boid (leader or follower)
public class Flock : MonoBehaviour
{
    public bool isLeader = false;
    float speed;
    float currentSpeed;
    Vector3 wanderGoal;

    void Start()
    {
        // Leaders are slower so followers can keep up
        speed = isLeader ? FlockManager.FM.minSpeed : Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
        currentSpeed = speed;
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
            transform.Translate(0, 0, currentSpeed * Time.deltaTime);
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
            wanderGoal = NewGoalAhead();
        return (wanderGoal - transform.position).normalized;
    }

    // Pick a new goal in front of the leader, so it doesn't turn around through its own flock
    Vector3 NewGoalAhead()
    {
        Vector3 goal = FlockManager.FM.RandomFreePoint();
        for (int i = 0; i < 10 && Vector3.Dot(transform.forward, goal - transform.position) < 0; i++)
            goal = FlockManager.FM.RandomFreePoint();
        return goal;
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

            // Leader right in front of us: steer around it (to the right), closer = stronger
            Vector3 toOther = other.transform.position - transform.position;
            float range = fm.separationDistance * 1.5f;
            if (fm.leaders.Contains(other) && distance < range && Vector3.Dot(transform.forward, toOther.normalized) > 0.7f)
                separation += Vector3.Cross(Vector3.up, toOther).normalized * (1 - distance / range);
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
            Vector3 toFollowPoint = leader.transform.position - leader.transform.forward * fm.followDistance - transform.position;
            float d = toFollowPoint.magnitude;
            direction += toFollowPoint.normalized * fm.leaderWeight * Mathf.Clamp01(d / fm.followDistance);   // weaker pull when close

            // Close to the leader: slow down to the leader's speed, so we don't overshoot and turn around
            currentSpeed = d < fm.followDistance * 2 ? fm.minSpeed : speed;
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
            bool otherIsLeader = fm.leaders.Contains(other);
            float space = otherIsLeader ? personalSpace * 1.5f : personalSpace;   // leaders are bigger
            float share = otherIsLeader ? 1f : 0.5f;                            // leaders don't move aside, so we move the whole way
            if (other != gameObject && away.magnitude > 0 && away.magnitude < space)
                push += away.normalized * (space - away.magnitude) * share;
        }
        push.y = 0;

        // Only if no wall is in the way
        if (push != Vector3.zero && !Physics.SphereCast(transform.position, 0.3f, push, out _, push.magnitude + 0.2f))
            transform.position += push;
    }
}
