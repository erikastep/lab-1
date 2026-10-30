using System.Collections.Generic;
using UnityEngine;

// Spawns the flock and holds all the settings
public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;

    public GameObject boidPrefab;
    public Material leaderMaterial;
    public Vector3 limits = new Vector3(18, 0, 18);   // spawn area

    public GameObject[] allBoids;
    public List<GameObject> leaders = new List<GameObject>();

    [Header("Flock Size (change in Play mode to respawn)")]
    [Range(1, 100)] public int numBoids = 30;
    [Range(0, 5)] public int numLeaders = 2;

    [Header("Movement")]
    [Range(0.5f, 10f)] public float minSpeed = 2;
    [Range(0.5f, 10f)] public float maxSpeed = 4;
    [Range(1f, 10f)] public float neighbourDistance = 3;
    [Range(0.5f, 5f)] public float rotationSpeed = 4;

    [Header("Rule Weights")]
    [Range(0f, 5f)] public float cohesionWeight = 0.8f;
    [Range(0f, 5f)] public float alignWeight = 1;
    [Range(0f, 5f)] public float separationWeight = 3f;
    [Range(0.5f, 3f)] public float separationDistance = 2f;
    [Range(0f, 5f)] public float leaderWeight = 2;
    [Range(0.5f, 5f)] public float followDistance = 2f;
    [Range(0f, 10f)] public float avoidWeight = 5;
    [Range(1f, 5f)] public float avoidDistance = 2;

    int spawnedBoids, spawnedLeaders;

    void OnEnable()
    {
        FM = this;
    }

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        // Respawn when the sliders change
        if (numBoids != spawnedBoids || numLeaders != spawnedLeaders)
            Spawn();
    }

    // Create the flock (first boids = leaders)
    void Spawn()
    {
        // Remove the old flock
        if (allBoids != null)
            foreach (GameObject b in allBoids) Destroy(b);
        leaders.Clear();

        // Create the new flock
        allBoids = new GameObject[numBoids];
        for (int i = 0; i < numBoids; i++)
        {
            allBoids[i] = Instantiate(boidPrefab, RandomFreePoint(), Quaternion.Euler(0, Random.Range(0, 360), 0), transform);

            // Leader: bigger and different colour
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

    // Random point that is not inside an obstacle
    public Vector3 RandomFreePoint()
    {
        Vector3 pos;
        do pos = transform.position + new Vector3(Random.Range(-limits.x, limits.x), 0, Random.Range(-limits.z, limits.z));
        while (Physics.CheckSphere(pos, 0.8f));
        return pos;
    }
}
