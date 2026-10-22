using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// One boid. Leaders wander; followers use cohesion, align, separation and follow the closest leader.
public class Flock : MonoBehaviour
{
    public bool isLeader = false;
    float speed;
    Vector3 wanderGoal;

    void Start()
    {
        // Leaders are a bit slower so the followers can keep up
        speed = isLeader ? FlockManager.FM.minSpeed : Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
        wanderGoal = FlockManager.FM.RandomFreePoint();
    }

    void Update()
    {
        // Combine the rules into one direction and stay on the floor
        Vector3 direction = isLeader ? Wander() : FlockingRules();
        direction += AvoidObstacles() * FlockManager.FM.avoidWeight;
        direction.y = 0;

        // Turn smoothly towards the direction
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                  FlockManager.FM.rotationSpeed * Time.deltaTime);

        // Only move forward if nothing is right in front, so boids never pass through obstacles
        if (!Physics.SphereCast(transform.position, 0.3f, transform.forward, out _, speed * Time.deltaTime + 0.2f))
            transform.Translate(0, 0, speed * Time.deltaTime);
    }

    // Leaders go to a random point and pick a new one when they arrive
    Vector3 Wander()
    {
        if (Vector3.Distance(transform.position, wanderGoal) < 2)
            wanderGoal = FlockManager.FM.RandomFreePoint();
        return (wanderGoal - transform.position).normalized;
    }

    // Cohesion + align + separation with the neighbours, plus following the closest leader
    Vector3 FlockingRules()
    {
        FlockManager fm = FlockManager.FM;
        Vector3 cohesion = Vector3.zero, align = Vector3.zero, separation = Vector3.zero;
        int num = 0;

        foreach (GameObject go in fm.allBoids)
        {
            if (go == this.gameObject) continue;
            float distance = Vector3.Distance(go.transform.position, transform.position);
            if (distance <= fm.neighbourDistance)
            {
                cohesion += go.transform.position;
                align += go.transform.forward;
                separation += (transform.position - go.transform.position) / Mathf.Max(distance * distance, 0.01f);
                num++;
            }
        }

        Vector3 direction = Vector3.zero;
        if (num > 0)
        {
            cohesion = (cohesion / num - transform.position).normalized;
            align = (align / num).normalized;
            direction = cohesion * fm.cohesionWeight + align * fm.alignWeight + separation * fm.separationWeight;
        }

        // Leader following: closest leader (Linq, like in the slides)
        GameObject leader = fm.leaders.OrderBy(l => Vector3.Distance(l.transform.position, transform.position)).FirstOrDefault();
        if (leader != null)
            direction += (leader.transform.position - transform.position).normalized * fm.leaderWeight;

        return direction;
    }

    // Look ahead with a sphere cast; if a wall or obstacle is in front, turn away from it
    Vector3 AvoidObstacles()
    {
        if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out RaycastHit hit, FlockManager.FM.avoidDistance))
            return Vector3.Reflect(transform.forward, hit.normal);
        return Vector3.zero;
    }
}
