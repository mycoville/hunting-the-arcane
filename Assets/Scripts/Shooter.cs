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

            if(distanceToPlayer > noticeDistance)
            {
                if(wanderTimer >= directionDuration)
                {
                    wanderDirection = RandomWanderingDirection();     
                }

                rb2d.velocity = wanderDirection * moveSpeed;
                EnemyHelper.RotateSpriteToDirection(wanderDirection, spriteTF);
            }
            else // if the player is close enough
            {
                currentState = State.Shooting;
            }
        }
        else if(currentState == State.Shooting)
        {
            shootingTimer += Time.fixedDeltaTime;

            if(distanceToPlayer > noticeDistance)
            {
                currentState = State.Wandering;
            }
            else
            {
                Vector2 aimDirection = StaticPlayer.playerTransform.position - this.transform.position;
                EnemyHelper.RotateSpriteToDirection(aimDirection, spriteTF);
                // Enemy shoots towards the player
                if(shootingTimer > shootDelay)
                { 
                    ShootTowardsPlayer();
                }
            }
        }
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
