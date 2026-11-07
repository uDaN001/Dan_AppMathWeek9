using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;

public class ItemSpawner : MonoBehaviour
{
    public Item obstaclePrefab;
    public Transform parentCanvasOrWorld;
    public float spawnInterval = 0.8f;
    public float obstacleSpeed = 60f;
    public float startSpawnZ = 200f;

    private float timer = 0f;
    private PlayerController player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    private void SpawnObstacle()
    {
        if (player == null) return;

        // Pick a random lane (0, 1, or 2)
        int randomLane = Random.Range(0, player.laneXPositions.Length);
        float xPos = player.laneXPositions[randomLane];
        float yPos = player.groundY; // Spawn at ground height

        Item spawned = Instantiate(obstaclePrefab, parentCanvasOrWorld);
        spawned.itemPosition = new Vector3(xPos, yPos, startSpawnZ);
        spawned.laneIndex = randomLane;
        spawned.moveSpeed = obstacleSpeed;
    }

}