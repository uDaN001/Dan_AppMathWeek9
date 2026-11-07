using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector] public Vector3 itemPosition;
    [HideInInspector] public int laneIndex;
    [HideInInspector] public float moveSpeed = 50f;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] public float spawnZ = 200f; // Track total travel distance

    private SpriteRenderer spriteRenderer;
    private PlayerController player;
    private CameraComponent cameraComponent;
    private float initialY;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        cameraComponent = FindFirstObjectByType<CameraComponent>();
        initialY = itemPosition.y; // Capture the high spawn Y position
    }

    private void Update()
    {
        // Move closer to the player along the Z axis
        itemPosition.z -= moveSpeed * Time.deltaTime;

        // Smoothly glide the obstacle's world Y position from its high spawn point down to the player's ground Y
        if (player != null)
        {
            float progress = Mathf.Clamp01(1f - (itemPosition.z / spawnZ));
            itemPosition.y = Mathf.Lerp(initialY, player.groundY, progress);
        }

        // Once it passes the player (Z < 0), let it rush down past the screen
        if (itemPosition.z < 0f)
        {
            itemPosition.y -= (moveSpeed * 0.3f) * Time.deltaTime;
        }

        // Perspective projection calculation
        var perspective = CameraComponent.focalLenth / (CameraComponent.focalLenth + itemPosition.z);
        transform.localScale = Vector3.one * perspective;

        Vector2 vanishingPoint = cameraComponent != null ? cameraComponent.vanishingPoint : Vector2.zero;

        float screenX = vanishingPoint.x + (itemPosition.x - vanishingPoint.x) * perspective;
        float screenY = vanishingPoint.y + (itemPosition.y - vanishingPoint.y) * perspective;
        transform.position = new Vector2(screenX, screenY);

        // Precise hit window right at Z = 0 (player's exact depth plane)
        if (!isHit && itemPosition.z <= 0.2f && itemPosition.z >= -0.2f)
        {
            CheckCollisionWithPlayer();
        }

        // Destroy when far off screen
        if (itemPosition.z < -60f || transform.position.y < -15f)
        {
            Destroy(gameObject);
        }
    }

    private void CheckCollisionWithPlayer()
    {
        if (player == null) return;

        float playerX = player.transform.position.x;
        float laneX = player.laneXPositions[laneIndex];

        bool inSameLane = Mathf.Abs(playerX - laneX) < 1f;
        bool isPlayerJumping = player.transform.position.y > player.groundY + 0.5f;

        if (inSameLane && !isPlayerJumping)
        {
            isHit = true;
            player.TakeDamage(5f);
            Destroy(gameObject);
        }
        else if (itemPosition.z <= -0.1f && !isHit)
        {
            // Successfully evaded!
            isHit = true;
        }
    }
}