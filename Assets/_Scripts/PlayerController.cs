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
    [SerializeField] float jumpForce = 10f;
    
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
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main;
    }

    private void Update()
    {
        GroundCheck();
        Move();
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        isRunning = move.sqrMagnitude > 0;
        animator.SetBool(isRunningHash, isRunning);
        animator.SetBool(isGroundedHash, isGrounded);
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
        if(moveDir.sqrMagnitude > 0.1f){
            print($"moveDir: {moveDir}");
            print($"before Rotation : {transform.rotation}");
            //transform.rotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.LookRotation(moveDir));
            print($"after Rotation : {transform.rotation}");
        }

        //move player
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(!context.started) return;
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
    }

    public void ChangeGravity(InputAction.CallbackContext context)
    {
        
    }

    #region HelperMethods

    private void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * .2f;
        isGrounded = Physics.Raycast(origin, Vector3.down, .4f);
    }

    #endregion
    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    #endregion
}