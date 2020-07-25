using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float shotSpeed = 5f;

    void Awake()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rb2d.velocity = this.transform.right * shotSpeed;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        this.gameObject.SetActive(false);
    }
}
