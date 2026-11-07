using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector] public Vector3 itemPosition; // z starts far (e.g., 200) and decreases to 0
    [HideInInspector] public int laneIndex;       // 0: Left, 1: Center, 2: Right
    [HideInInspector] public float moveSpeed = 50f;
    [HideInInspector] public bool isCollectedOrHit = false;

    private SpriteRenderer spriteRenderer;
    private PlayerController player;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; // Distinct obstacle color
        }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        // Move towards the player by decreasing Z
        itemPosition.z -= moveSpeed * Time.deltaTime;

        // Perspective projection calculation
        var perspective = CameraComponent.focalLenth / (CameraComponent.focalLenth + itemPosition.z);
        transform.localScale = Vector3.one * perspective;
        transform.position = new Vector2(itemPosition.x, itemPosition.y) * perspective;

        // Check collision when obstacle reaches the player's Z range (Z close to 0)
        if (!isCollectedOrHit && itemPosition.z <= 5f && itemPosition.z >= -2f)
        {
            CheckCollisionWithPlayer();
        }

        // Destroy obstacle if it passes the camera
        if (itemPosition.z < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void CheckCollisionWithPlayer()
    {
        if (player == null) return;

        // Check if player is in the same lane
        float playerX = player.transform.position.x;
        float laneX = player.laneXPositions[laneIndex];

        bool inSameLane = Mathf.Abs(playerX - laneX) < 1f;

        // Check if player is grounded (if jumping over low obstacles, adjust condition as needed)
        bool isPlayerJumping = player.transform.position.y > player.groundY + 0.5f;

        if (inSameLane && !isPlayerJumping)
        {
            isCollectedOrHit = true;
            player.TakeDamage(5f); // Remove 5 HP on hit
            Destroy(gameObject);
        }
        else if (itemPosition.z <= -1f && !isCollectedOrHit)
        {
            // Successfully evaded once it passes Z = 0
            isCollectedOrHit = true;
        }
    }

}