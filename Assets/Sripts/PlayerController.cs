using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Lane Settings")]
    public float[] laneXPositions = { -3f, 0f, 3f };
    private int currentLane = 1;
    public float laneChangeSpeed = 10f; 
    private float targetX;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravity = -20f;
    private float verticalVelocity = 0f;
    private float currentY = 0f;
    private bool isGrounded = true;
    public float groundY = -2f; 

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public TextMeshProUGUI healthUIText; 
  

    private void Start()
    {
        currentHealth = maxHealth;
        targetX = laneXPositions[currentLane];
        currentY = groundY;
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleHealthRegen();
        UpdateUI();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentLane > 0)
            {
                currentLane--;
                targetX = laneXPositions[currentLane];
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentLane < laneXPositions.Length - 1)
            {
                currentLane++;
                targetX = laneXPositions[currentLane];
            }
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
        }
    }

    private void HandleMovement()
    {
        float currentX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * laneChangeSpeed);

        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            currentY += verticalVelocity * Time.deltaTime;

            if (currentY <= groundY)
            {
                currentY = groundY;
                verticalVelocity = 0f;
                isGrounded = true;
            }
        }

        transform.position = new Vector3(currentX, currentY, 0f);
    }

    private void HandleHealthRegen()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += 1f * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Game Over!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void UpdateUI()
    {
        if (healthUIText != null)
        {
            healthUIText.text = "HP: " + Mathf.Ceil(currentHealth).ToString() + " / " + maxHealth.ToString();
        }
    }
}