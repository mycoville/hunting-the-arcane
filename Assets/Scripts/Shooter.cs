using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    // AI States
    private enum State
    {
        Wandering,
        Shooting,
    }

    // Shooter enemy version
    public enum Version
    {
        Wizard,
    }

    public float noticeDistance = 9f;

    // Wandering state variables
    private Vector2 wanderStartPos;
    private Vector2 wanderDirection;
    private float directionDuration;
    private float wanderTimer = 0f;

    // Movement
    private Rigidbody2D rb2d;
    private State currentState;
    private Transform spriteTF;
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 3.5f;

    // Shooting
    private float shootingTimer = 0;
    private float shootDelay = 1f;
    public Transform muzzleTransform;
    public LayerMask aimIgnoreLayers;

    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        spriteTF = this.transform.GetChild(0).transform; // Referencing the helper object's Transform used for rotating the object containing the actual Sprite
        
        currentState = State.Wandering;

        wanderStartPos = this.transform.position;
        wanderDirection = RandomWanderingDirection();
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(this.transform.position, StaticPlayer.playerTransform.position);

        if(currentState == State.Wandering)
        {
            wanderTimer += Time.fixedDeltaTime;

            if(wanderTimer >= directionDuration)
            {
                wanderDirection = RandomWanderingDirection();     
            }

            EnemyHelper.RotateSpriteToDirection(wanderDirection, spriteTF);
            rb2d.velocity = wanderDirection * moveSpeed;
            
            if(distanceToPlayer <= noticeDistance)
            {
                if(CheckLineOfSight())
                {
                    currentState = State.Shooting;
                }
            }

        }
        else if(currentState == State.Shooting)
        {
            shootingTimer += Time.fixedDeltaTime;

            // The Shooter wanders in case he is far away or has no direct line of sight
            if(distanceToPlayer > noticeDistance || !CheckLineOfSight())
            {
                currentState = State.Wandering;
            }
            else
            {
                Vector2 aimDirection = StaticPlayer.playerTransform.position - this.transform.position;
                EnemyHelper.RotateSpriteToDirection(aimDirection, spriteTF);

                if(shootingTimer > shootDelay)
                {
                    ShootTowardsPlayer();
                }
            }
        }
    }

    private bool CheckLineOfSight()
    {
        bool lineOfSight = false;
        Vector2 aimDirection = StaticPlayer.playerTransform.position - this.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, aimDirection.normalized, 10f, ~aimIgnoreLayers);
        if(hit.collider != null)
        {
            if(hit.collider.CompareTag("Player"))
            {
                lineOfSight = true;
            }
        }
        return lineOfSight;
    }

    private void ShootTowardsPlayer()
    {
        //Debug.Log("Shooting towards player");
        ObjectPooler.opInstance.SpawnEnemyProjectile(muzzleTransform.position, spriteTF.rotation);
        shootingTimer = 0f;
    }

    private Vector2 RandomWanderingDirection()
    {
        wanderTimer = 0f;
        directionDuration = Random.Range(2f, 3f);
        return EnemyHelper.GetRandomDirection();
    }

}
