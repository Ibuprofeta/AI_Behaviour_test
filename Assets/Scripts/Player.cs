using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 movementVector;
    private Animator animator;
    private float speed;
    private Rigidbody body;
    private Health health;

    private void Start(){
        health = GetComponent<Health>();
        body = GetComponent<Rigidbody>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        speed = 1.5f;
    }

    private void CalculateMovement(){
        movementVector = new Vector3 (Input.GetAxis("Horizontal"), 
        body.velocity.y, Input.GetAxis("Vertical"));

        body.velocity = movementVector * speed;
    }

    private void Update(){
        CalculateMovement();
        if (movementVector != Vector3.zero){
            transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.LookRotation(movementVector), 0.25f);
        }
        animator.SetBool("Walking", movementVector != Vector3.zero);
        if(Input.GetKeyDown(KeyCode.K)){
            Hurt(2,2);
        }
    }

    public void Hurt(int amount, int delay = 0){
        StartCoroutine(health.TakeDamageDelayed(amount, delay));
        print("a");
    }
}
