using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns the flock and holds all the settings. Attach to an empty object in the arena.
public class FlockManager : MonoBehaviour
{
    // Static instance so every boid can read the settings
    public static FlockManager FM;

    public GameObject boidPrefab;
    public Material leaderMaterial;
    public Vector3 limits = new Vector3(18, 0, 18);   // area where boids spawn and leaders wander

    public GameObject[] allBoids;
    public List<GameObject> leaders = new List<GameObject>();

    [Header("Flock Size (change in Play mode to respawn)")]
    [Range(1, 100)] public int numBoids = 30;
    [Range(0, 5)] public int numLeaders = 2;

    [Header("Movement")]
    [Range(0.5f, 10f)] public float minSpeed = 2;
    [Range(0.5f, 10f)] public float maxSpeed = 4;
    [Range(1f, 10f)] public float neighbourDistance = 3;
    [Range(0.5f, 5f)] public float rotationSpeed = 2;

    [Header("Rule Weights")]
    [Range(0f, 5f)] public float cohesionWeight = 1;
    [Range(0f, 5f)] public float alignWeight = 1;
    [Range(0f, 5f)] public float separationWeight = 1.5f;
    [Range(0f, 5f)] public float leaderWeight = 2;
    [Range(0f, 10f)] public float avoidWeight = 5;
    [Range(1f, 5f)] public float avoidDistance = 2;

    int spawnedBoids, spawnedLeaders;

    void Start()
    {
        FM = this;
        Spawn();
    }

    void Update()
    {
        // Respawn the flock when the counts are changed in the Inspector
        if (numBoids != spawnedBoids || numLeaders != spawnedLeaders)
            Spawn();
    }

    // Destroys the old flock and creates a new one; the first boids become leaders
    void Spawn()
    {
        if (allBoids != null)
            foreach (GameObject b in allBoids) Destroy(b);
        leaders.Clear();

        allBoids = new GameObject[numBoids];
        for (int i = 0; i < numBoids; i++)
        {
            allBoids[i] = Instantiate(boidPrefab, RandomFreePoint(), Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            if (i < numLeaders)
            {
                allBoids[i].GetComponent<Flock>().isLeader = true;
                allBoids[i].GetComponentInChildren<Renderer>().material = leaderMaterial;
                allBoids[i].transform.localScale *= 1.5f;
                leaders.Add(allBoids[i]);
            }
        }
        spawnedBoids = numBoids;
        spawnedLeaders = numLeaders;
    }

    // Random point inside the limits that is not inside a wall or obstacle
    public Vector3 RandomFreePoint()
    {
        Vector3 pos;
        do pos = transform.position + new Vector3(Random.Range(-limits.x, limits.x), 0, Random.Range(-limits.z, limits.z));
        while (Physics.CheckSphere(pos, 0.8f));
        return pos;
    }
}
