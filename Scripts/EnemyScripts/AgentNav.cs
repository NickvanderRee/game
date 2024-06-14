using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNav : MonoBehaviour
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;
    public Transform player; // Reference to the player
    public float hintInterval = 30f; // Interval in seconds to get a hint of player's position
    public float playerDetectionRadius = 10f; // Radius to detect the player
    public float minimalDistance = 15f;

    private NavMeshAgent agent;
    private bool chasing = false; // Flag to indicate if the enemy is currently chasing the player
    private Vector3 lastPlayerHint;
    private float hintTimer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        lastPlayerHint = transform.position;
    }

    private void Update()
    {
        if (IsPlayerDetected())
        {
            if (!chasing)
            {
                StartChasingPlayer();
            }
            else
            {
                agent.SetDestination(player.position); // Continue chasing the player
            }
        }
        else
        {
            StopChasingPlayer();
        }

        // Check if it's time to receive a hint
        hintTimer += Time.deltaTime;
        if (hintTimer >= hintInterval)
        {
            ReceiveHint();
            hintTimer = 0f;
        }

        Debug.DrawLine(transform.position, agent.destination, Color.blue);
    }

    private bool IsPlayerDetected()
    {
        return Vector3.Distance(transform.position, player.position) < playerDetectionRadius;
    }

    private void ReceiveHint()
    {
        lastPlayerHint = player.position;
    }

    private void StartChasingPlayer()
    {
        chasing = true;
        agent.speed = chaseSpeed; // Increase speed while chasing
        agent.SetDestination(player.position); // Start chasing the player
        Debug.Log("Chasing player!");
    }

    private void StopChasingPlayer()
    {
        if (chasing)
        {
            chasing = false;
            agent.speed = patrolSpeed; // Reset speed to patrol speed
            Debug.Log("Lost player, resuming patrol.");
        }
    }
}