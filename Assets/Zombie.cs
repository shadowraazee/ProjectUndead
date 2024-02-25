using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Include the AI namespace for NavMesh components

public class Zombie : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public float attackRange = 1f; // The range within which the zombie will attack the player
    public float detectionRange = 10f; // How far the zombie can detect the player from
    public int attackDamage = 20; // Damage dealt to the player on each attack
    public float attackCooldown = 1f; // Time between attacks
    private Animator animator;

    private Transform player; // To store the player's transform
    private Vector2 movement;
    private float originalSpeed;
    private bool isPlayerInRange = false; // To check if the player is within the detection range
    private float lastAttackTime; // To keep track of the time when the last attack occurred

    void Start()
    {
        originalSpeed = moveSpeed;
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            movement = (player.position - transform.position).normalized;
            isPlayerInRange = true;
        }
        else
        {
            movement = Vector2.zero; // Zombie stops moving if player is out of range
            isPlayerInRange = false;
        }
    }

    void FixedUpdate()
    {
        if (isPlayerInRange)
        {
            // Move zombie towards the player
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            FlipSpriteBasedOnMovement();

            // Check if it's time to attack again
            if (Vector2.Distance(rb.position, player.position) <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time; // Update the time of the last attack
            }
        }
    }

    private void FlipSpriteBasedOnMovement()
    {
        if (movement.x < 0)
        {
            // Flip sprite horizontally to face left
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movement.x > 0)
        {
            // Reset sprite to face right
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void AttackPlayer()
    {
        // Access the PlayerHealth component and call TakeDamage
        HealthController playerHealth = player.GetComponent<HealthController>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Zombie is attacking the player!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bush") || other.CompareTag("swamp"))
        {
            moveSpeed /= 2; // Slow down when entering a bush or swamp
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("bush") || other.CompareTag("swamp"))
        {
            moveSpeed = originalSpeed; // Restore speed when exiting a bush or swamp
        }
    }
}