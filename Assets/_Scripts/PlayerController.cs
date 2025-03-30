using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(PlayerInput))] 
public class PlayerController : MonoBehaviour
{
    [Header("Movement Configuration")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float rotationSpeed = 100f;
    
    Rigidbody rb;
    Animator animator;
    Camera cam;

    Vector2 move;
    
    //Animation parameters
    bool isGrounded;
    bool isRunning;
    readonly int isGroundedHash = Animator.StringToHash("isGrounded");
    readonly int isRunningHash = Animator.StringToHash("isRunning");
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    private void Update()
    {
        GroundCheck();
        Move();
        UpdateAnimations();
    }

    private void Move()
    {
        if(!isGrounded) return;
        
        Vector3 forward = cam.transform.forward, right = cam.transform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();
        
        Vector3 moveDir = forward * move.y + right * move.x;
        Vector3 targetVelocity = moveDir * moveSpeed;

        //Rotate player
        if(moveDir.sqrMagnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(moveDir);

        //move player
        rb.velocity = targetVelocity;
    }

    private void UpdateAnimations()
    {
        isRunning = move.sqrMagnitude > 0;
        animator.SetBool(isRunningHash, isRunning);
        animator.SetBool(isGroundedHash, isGrounded);
    }

    #region HelperMethods

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, .3f);
    }

    #endregion
    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    #endregion
}