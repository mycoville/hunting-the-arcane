/*
In this script:
- Simple AI: Wandering, moving straight towards player at a certain distance
- 2 versions of the Follower: Normal, Aggressive
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Follower : MonoBehaviour
{
    // AI States
    private enum State
    {
        Wandering,
        Chasing,
        Stunned,
    }

    public enum Version
    {
        Normal,
        Aggressive,
    }

    public float noticeDistance = 8f;
    
    // Wandering state variables
    private Vector2 wanderStartPos;
    private Vector2 wanderDirection;
    private float directionDuration;
    private float wanderTimer = 0f;

    // Getting shot and stunned
    private float stunTimer = 0f;
    public float stunAmount = 0.15f;

    // Movement
    private Rigidbody2D followerRB;
    private State currentState;
    private Transform followerSpriteTF;
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 5f;
    private float originalMoveSpeed;

    // Enemy version and HP relations
    public Version enemyVersion;
    private Enemy enemyScript;
    

    void Start()
    {
        followerRB = this.GetComponent<Rigidbody2D>();
        followerSpriteTF = this.transform.GetChild(0).transform; // Referencing the helper object's Transform used for rotating the object containing the actual Sprite
        
        currentState = State.Wandering;
        originalMoveSpeed = moveSpeed;
        enemyScript = this.GetComponent<Enemy>();

        wanderStartPos = this.transform.position;
        currentState = State.Wandering;
        wanderDirection = RandomWanderingDirection();
    }

    void FixedUpdate()
    {
        // Constantly checking for distance to the player since this is relevant to most state changes
        float distanceToPlayer = Vector2.Distance(this.transform.position, StaticPlayer.playerTransform.position);

        if(currentState == State.Wandering)
        {
            wanderTimer += Time.fixedDeltaTime;

            if(distanceToPlayer <= noticeDistance)
            {
                currentState = State.Chasing;
            }

            if(wanderTimer >= directionDuration)
            {
                wanderDirection = RandomWanderingDirection();
            }

            // Moving to the wandering direction at half the maximum movement speed
            followerRB.velocity = wanderDirection * moveSpeed/2;
            RotateSpriteToDirection(wanderDirection);
        }
        else if(currentState == State.Chasing)
        {
            if(StaticPlayer.alive)
            {
                Vector2 chaseDirection = StaticPlayer.playerTransform.position - this.transform.position;

                followerRB.velocity = chaseDirection.normalized * moveSpeed;
                RotateSpriteToDirection(chaseDirection);

                if(distanceToPlayer > noticeDistance)
                {
                    currentState = State.Wandering;
                }
            }
            else
            {
                currentState = State.Wandering;
            }

        }
        else if(currentState == State.Stunned)
        {
            if(stunTimer > stunAmount)
            {
                // Returning from the Stunned State according to distance to player
                if(distanceToPlayer <= noticeDistance)
                {
                    currentState = State.Chasing;
                }
                else
                {
                    currentState = State.Wandering;
                }
                
            }
            stunTimer += Time.fixedDeltaTime;
        }

        // The aggressive version moves quicker based (mostly) on its damage received
        if(enemyVersion == Version.Aggressive)
        {
            if(enemyScript.currHP < enemyScript.maxHP)
            {
                moveSpeed = originalMoveSpeed + 0.6f + (1 - enemyScript.currHP / enemyScript.maxHP) * originalMoveSpeed;
            }
        }

    }

    // Rotating the Sprite's parent object according to the given vector
    private void RotateSpriteToDirection(Vector2 moveDirection)
    {
        if(moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            followerSpriteTF.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // Getting stunned when shot, slowing the movement down briefly and pausing the wandering/chasing for the duration
    public void Stun(float duration)
    {
        stunTimer = 0f;
        stunAmount = duration;
        followerRB.velocity *= 0.5f;
        currentState = State.Stunned;
    }

    // Getting a random wandering direction vector, slightly randomizing the time spend moving to that direction
    private Vector2 RandomWanderingDirection()
    {
        wanderTimer = 0f;
        directionDuration = Random.Range(2f, 3f);
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    // Turning the a Follower-type enemy into a "Rager" version with more hp and larger size, and with a different sprite
    public void MakeAggressive()
    {
        enemyVersion = Version.Aggressive;
        this.transform.localScale = new Vector2(1.3f, 1.3f);
        enemyScript = this.GetComponent<Enemy>();
        enemyScript.maxHP *= 1.75f;
        enemyScript.currHP = enemyScript.maxHP;
        spriteRenderer.sprite = Resources.Load<Sprite>("hta-enemy-rager");
    }
}
