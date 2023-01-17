using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    public SpriteRenderer spriteBall;


    bool isdead = false;
    private void Start()
    {

    }

    private void OnEnable()
    { 
        double radian =  100 * 3.141592 / 180;
        Vector2 velocity = new Vector2(-350 * Mathf.Cos((float)radian), 350 * Mathf.Sin((float)radian));
        rb.velocity = velocity * 5.0f;
        isdead = false;
    }
}
