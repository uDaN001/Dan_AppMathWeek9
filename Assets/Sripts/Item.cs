using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector] public Vector3 itemPosition; 
    [HideInInspector] public int laneIndex;      
    [HideInInspector] public float moveSpeed = 50f;
    [HideInInspector] public bool isCollectedOrHit = false;

    private SpriteRenderer spriteRenderer;
    private PlayerController player;
    private CameraComponent cameraComponent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; 
        }
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        cameraComponent = FindFirstObjectByType<CameraComponent>();
    }

    private void Update()
    {
        itemPosition.z -= moveSpeed * Time.deltaTime;

        var perspective = CameraComponent.focalLenth / (CameraComponent.focalLenth + itemPosition.z);
        transform.localScale = Vector3.one * perspective;

        Vector2 vanishingPoint = cameraComponent != null ? cameraComponent.vanishingPoint : Vector2.zero;

        float screenX = vanishingPoint.x + (itemPosition.x - vanishingPoint.x) * perspective;
        float screenY = vanishingPoint.y + (itemPosition.y - vanishingPoint.y) * perspective;
        transform.position = new Vector2(screenX, screenY);

        if (!isCollectedOrHit && itemPosition.z <= 5f && itemPosition.z >= -2f)
        {
            CheckCollisionWithPlayer();
        }

        if (itemPosition.z < -20f)
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
            isCollectedOrHit = true;
            player.TakeDamage(5f); 
            Destroy(gameObject);
        }
        else if (itemPosition.z <= -1f && !isCollectedOrHit)
        {
            isCollectedOrHit = true;
        }
    }
}