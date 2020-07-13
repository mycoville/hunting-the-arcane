/*
In this script:
- Player movement
- Player sprite rotation
- Player target checking
- Player aiming and shooting
- Player health and its health-related UI management
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // Movement and sprite rotation
    public Transform playerSpriteTF;
    private Rigidbody2D plrRB;
    private Transform playerObjTF;
    private bool plrMoving = false;
    public GameObject playerLegsObj; // Player leg sprites with their constant animation
    public FloatingJoystick joystick; // From "Joystick Pack" by Fenerax Studios

    // Shooting and aiming rotation
    private Transform targetTF = null;
    private float previousDistance = 0f;
    private float shootTimer = 1f;
    public GameObject shotPrefab;
    public Transform muzzleTF;
    private bool targetUpdated = false;
    public GameObject indicatorTarget;
    private Transform indicatorTF;
    public LayerMask aimIgnoreLayer;
    public Animator muzzleAnim;
    public List<AudioClip> shotSounds;
    public AudioSource muzzleAudioSrc;

    // Player Health and Damage
    private float damageTimer = 0f;
    public float damageDelay = 0.5f; // giving the player moments of invulnerability after being damaged
    public bool beingDamaged = false;

    public Image healthSpriteOverlay;
    public GameObject gameOverTextObj;
    private SpriteRenderer playerSprite;
    
    // Player state and sprite
    public List<Sprite> playerSprites;
    public SpriteRenderer playerSpriteRend;
    

    void Start()
    {
        plrRB = this.GetComponent<Rigidbody2D>();
        playerObjTF = this.transform;
               
        playerLegsObj.SetActive(false);
        StaticPlayer.currHP = StaticPlayer.maxHP; // Making sure our HP values match in case we've edited them in the inspector

        healthSpriteOverlay.fillAmount = 1f;

        // Checking & setting up static player values
        StaticPlayer.alive = true;
        StaticPlayer.playerTransform = playerObjTF;
        
        RefreshSprite();

        // Aim indicator setup
        indicatorTarget.SetActive(false);
        indicatorTF = indicatorTarget.GetComponent<Transform>();

        // Setting muzzle sound based on current weapon
        RefreshMuzzleSound();

    }

    void FixedUpdate()
    {
        // If we are alive, we can move etc.
        if(StaticPlayer.currHP > 0)
        {
            float horizontalMove = Input.GetAxisRaw("Horizontal");
            float verticalMove = Input.GetAxisRaw("Vertical");

            // Using the touch-based joystick if we build for Android
            #if UNITY_ANDROID
                horizontalMove = joystick.Horizontal;
                verticalMove = joystick.Vertical;
            #endif

            Vector2 moveDirection = new Vector2(horizontalMove, verticalMove).normalized;

            plrRB.velocity = moveDirection * StaticPlayer.moveSpeed;

            // We check for the closest enemy/target every time we stop
            if(horizontalMove == 0 && verticalMove == 0)
            {
                plrMoving = false;
                if(!targetUpdated)
                {
                    CheckForClosest();
                    targetUpdated = true;
                }
            }
            else
            {
                plrMoving = true;
                targetUpdated = false;
            }

            // We face the direction of the movement velocity if we are moving, otherwise we aim and shoot
            if(plrMoving)
            {
                Vector2 lookDir = new Vector2(horizontalMove, verticalMove);
                RotateSpriteToDirection(lookDir);

                indicatorTarget.SetActive(false);
                targetTF = null;

                playerLegsObj.SetActive(true);
            }
            else
            {
                if(targetTF) // We have a target -> Shooting towards it, updating the indicator
                {
                    ShootAtClosest();

                    if(targetUpdated)
                    {
                        indicatorTF.position = targetTF.position;
                        indicatorTarget.SetActive(true);
                    }
                }
                else // We don't have a target -> Checking constantly for new targets
                {
                    CheckForClosest();
                }

                playerLegsObj.SetActive(false);
            }

            // Checking for just taps on screen: allowing the player to switch the aiming target without actually moving the joystick
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Ended)
                {
                    CheckForClosest();
                }
            }

            if(beingDamaged)
            {
                // There is a delay so that the player's health is not drained in an instant
                if(damageTimer > damageDelay)
                {
                    StaticPlayer.DamagePlayer(50f); // Current damage is static amount
                    damageTimer = 0f;
                    playerSpriteRend.color = Color.red; // Changing the sprite color for a short while to better indicate being damaged
                    HealthUpdate();
                    StaticPlayer.statusChanged = true;
                }
            }
            else
            {
                playerSpriteRend.color = Color.white;
            }

            // Making the target indicator disappear if there is no target Transform
            if(!targetTF)
            {
                indicatorTarget.SetActive(false);
            }

            shootTimer += Time.fixedDeltaTime;
            damageTimer += Time.fixedDeltaTime;

        }
        else // Player is dead
        {
            if(StaticPlayer.statusChanged)
            {
                playerLegsObj.SetActive(false);
                joystick.gameObject.SetActive(false);
                gameOverTextObj.SetActive(true);
                indicatorTarget.SetActive(false);
                StaticPlayer.alive = false;
                playerSpriteRend.sprite = playerSprites[0];
                playerSpriteRend.color = Color.white;
                plrRB.velocity = Vector2.zero;
                this.GetComponent<CircleCollider2D>().enabled = false;
                StaticPlayer.statusChanged = false;
            }

        }

    }

    // Adjusting the Sprite rotation to match the given vector direction
    private void RotateSpriteToDirection(Vector2 spriteDirection)
    {
        if(spriteDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(spriteDirection.y, spriteDirection.x) * Mathf.Rad2Deg;
            playerSpriteTF.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // Shooting at our target in case we have one
    private void ShootAtClosest()
    {
        AimAtTarget();
        if(targetTF != null && shootTimer > StaticPlayer.shotDelayInSec)
        {

            // Using the object pooler instance to spawn bullets
            ObjectPooler.StaticObjectPooler.SpawnBullet(muzzleTF.position, muzzleTF.rotation);

            shootTimer = 0;

            muzzleAnim.SetTrigger("ShootMuzzle");

            muzzleAudioSrc.Play();
           
        }
    }

    // Called every time the player's health is updated
    public void HealthUpdate()
    {
        healthSpriteOverlay.fillAmount = StaticPlayer.currHP/StaticPlayer.maxHP;
    }

    // Aiming the player sprite (and its child, the muzzle) towards the target in case we have one
    private void AimAtTarget()
    {
        if(targetTF != null && !plrMoving)
        {
            Vector2 lookDir = targetTF.position - playerObjTF.position;
            if(lookDir != Vector2.zero)
            {
                float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
                playerSpriteTF.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
        else if (targetTF == null)
        {
            CheckForClosest();
        }
    }

    // Checking through the enemy transform list while comparing their position to that of the player
    private void CheckForClosest()
    {
        List<Transform> enemyTransforms = StaticManager.enemyTransforms;
        List<Transform> acceptedEnemyTFs = new List<Transform>();

        if(enemyTransforms.Count != 0) // There are still enemies
        {
            // Raycasting towards all the enemies, accepting them for distance checking only if the ray hits them before any obstacles
            for(int i = 0; i < enemyTransforms.Count; i++)
            {
                Vector2 rayDirection = enemyTransforms[i].position - this.transform.position;
                rayDirection = rayDirection.normalized;
                Vector2 shotOrigin = new Vector2(this.transform.position.x, this.transform.position.y) + rayDirection*0.5f; // Slight offset to avoid touching the player's colliders
                RaycastHit2D hit = Physics2D.Raycast(shotOrigin, rayDirection, 8f, ~aimIgnoreLayer);

                // If we hit something
                if(hit.collider != null)
                {
                    // If the object we've hit has the Enemy script component, it is a suitable target
                    if(hit.collider.GetComponent<Enemy>())
                    {
                        acceptedEnemyTFs.Add(hit.collider.transform);
                    }
                }

            }
            
            // Checking the distance only towards enemies properly hit by raycasts
            for(int i = 0; i < acceptedEnemyTFs.Count; i++)
            {
                float currentDistance = Vector2.Distance(playerObjTF.position, acceptedEnemyTFs[i].position);
                // At the first index we set up our values for comparison
                if(i == 0)
                {
                    previousDistance = currentDistance;
                    targetTF = acceptedEnemyTFs[0];
                }
                else
                {
                    if(currentDistance < previousDistance)
                    {
                        previousDistance = currentDistance;
                        targetTF = acceptedEnemyTFs[i];
                    }
                }
            }
        }
        else // No enemies left
        {
            targetTF = null;
        }

    }

    // Updating the player sprite in case a new weapon type is picked up
    public void RefreshSprite()
    {
        switch(StaticPlayer.weaponType)
        {
            case 0:
                playerSpriteRend.sprite = playerSprites[1];
            break;
            case 1:
                playerSpriteRend.sprite = playerSprites[2];
            break;
            case 2:
                playerSpriteRend.sprite = playerSprites[2];
            break;
            case 3:
                playerSpriteRend.sprite = playerSprites[3];
            break;
            default:
                playerSpriteRend.sprite = playerSprites[1];
            break;
        }

        RefreshMuzzleSound();
    }

    // Updating the sound based on weapon type
    private void RefreshMuzzleSound()
    {
        switch(StaticPlayer.weaponType)
        {
            case 0:
                muzzleAudioSrc.clip = shotSounds[0];
            break;
            case 1:
                muzzleAudioSrc.clip = shotSounds[1];
            break;
            case 3:
                muzzleAudioSrc.clip = shotSounds[2];
            break;
            default:
                muzzleAudioSrc.clip = shotSounds[0];
            break;
        }
    }

}
