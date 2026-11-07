using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefab & References")]
    public Item obstaclePrefab;
    public Transform parentCanvasOrWorld;

    [Header("Spawn Interval Settings")]
    [Tooltip("Minimum time between obstacle spawns")]
    public float minSpawnInterval = 0.5f;
    [Tooltip("Maximum time between obstacle spawns")]
    public float maxSpawnInterval = 1.5f;

    [Header("Speed & Progression Settings")]
    public float obstacleSpeed = 60f;
    public float startSpawnZ = 200f;
    [SerializeField] private float yPos = 5f;

    [Header("Difficulty Scaling Multipliers")]
    [Tooltip("Rate at which obstacle speed increases per second")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [Tooltip("Rate at which spawn intervals scale or tighten over time")]
    [SerializeField] private float intervalScalingMultiplier = 0.05f;

    private float timer = 0f;
    private float targetSpawnInterval = 1f;
    private PlayerController player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        GetNewRandomInterval();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        obstacleSpeed += speedMultiplier * deltaTime;

        maxSpawnInterval = Mathf.Max(minSpawnInterval + 0.2f, maxSpawnInterval - (intervalScalingMultiplier * deltaTime));

        timer += deltaTime;
        if (timer >= targetSpawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
            GetNewRandomInterval();
        }
    }

    private void GetNewRandomInterval()
    {
        targetSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void SpawnObstacle()
    {
        if (player == null) return;

        int randomLane = Random.Range(0, player.laneXPositions.Length);
        float xPos = player.laneXPositions[randomLane];

        Item spawned = Instantiate(obstaclePrefab, parentCanvasOrWorld);
        spawned.itemPosition = new Vector3(xPos, yPos, startSpawnZ);
        spawned.laneIndex = randomLane;
        spawned.moveSpeed = obstacleSpeed;
    }
}