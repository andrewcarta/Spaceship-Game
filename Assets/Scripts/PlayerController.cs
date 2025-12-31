using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


//! example
//? example
//x example
//!? example
//TODO example

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private PlayerInput playerInput;
    //parts of the player's unity components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D cd;
    [SerializeField] private SpriteRenderer sp;
    [SerializeField] private RaycastHit2D interactRay;
    [SerializeField] private LayerMask interactRayInclude;
    [SerializeField] private Camera personalCamera;
    [SerializeField] private LayerMask boardedMask;
    [SerializeField] private LayerMask AllRenderMask;

    //All player stats and specs we can give an int value to
    private enum stats : int { 
    moveSpeed = 10
    }
    private float dashCooldown = 10;
    private Vector2 move;
    private Collider2D interactRayCollider;
    private bool boardedShip;
    private GameObject currentShip;
    private GameObject mostRecentHit;
    private bool piloting;

    //Input Actions


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boardedShip = false;
        
        move = Vector2.zero;
        print("<color=green> Player Spawned at "+gameObject.transform.position.x+" "+ gameObject.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        cameraLayerRenderSet();
        movePlayer(Time.deltaTime);
        rotateSprite();
        targetRaycasts();
        checkRaycastContext();
        getCurrentShip();
    }


    private void movePlayer(float delta)
    {
        Vector2 shipVelocity = Vector2.zero;
        if (boardedShip)
        {
            GameObject parent = this.transform.parent.gameObject;
            ShipController shipScript = (ShipController)parent.GetComponent<MonoBehaviour>();
            shipVelocity = shipScript.getShipVelocity();
        }
        move = playerInput.actions["PlayerMove"].ReadValue<Vector2>();
        if (playerInput.actions["BoostMovement"].IsPressed() && dashCooldown <= 0 && move != Vector2.zero) {
            //TODO Add some particles for this
            move *= 5;
            dashCooldown = 10;
        }
        dashCooldown -= delta;

        rb.linearVelocity = (move * (int)(stats.moveSpeed)) + shipVelocity;
    }
    private void rotateSprite() { 
        Vector2 thisPos = transform.position;
        Vector2 targetPos = thisPos+move;
        float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90;
        if (move != Vector2.zero)
        {
            //x transform.rotation = Quaternion.Euler(0, 0, angle);
            float facingAndMove = Vector2.SignedAngle(transform.up, move);
            transform.Rotate(0f, 0f, Mathf.Clamp(facingAndMove, -12, 12));
        }
    }

    private Collider2D checkRaycastContext() {

        if (interactRay.collider != null) {
            mostRecentHit = interactRay.collider.gameObject;
            //Runs through these to check which interaction effect should happen i.e. which button to click indicator appears
            //An obj needs to be on the interactable layer to be seen by the raycast (also colliders on base layer are included to block the raycasts so no through wall interactions)
            if (interactRay.collider.CompareTag("Enter"))
            {
                //When ANY interaction that says enter exists there will be a parent with other nodes to use to enter

                //print("At enter point");
                
                
            }
            if (interactRay.collider.CompareTag("Exit")) {

                //print("At exit point");
                
            }
            if (interactRay.collider.CompareTag("Use")) {

                //print("At node use point");

            }

            interactRayCollider = interactRay.collider;
            return interactRay.collider;
        } 
        else 
        {
        interactRayCollider = null;
        return null;
        }

    }

    private void targetRaycasts() {
        if (move != Vector2.zero)
        {
            interactRay = Physics2D.Raycast(transform.position, move * 3, 3, interactRayInclude);
        }
        //this raycast is no longer entirely accurate but is probably close
        Debug.DrawRay(transform.position, move*3, Color.orangeRed,0.5f);
    }

    //ALL methods linked to the inputMap and are called when the input happens (public so the methods can be used in other classes?)
    private void OnActionA(InputAction.CallbackContext context)
    {
        print("<color=orange> Input A");
    }
    private void OnActionB(InputAction.CallbackContext context)
    {
        print("<color=orange> Input B");
    }
    private void OnActionC(InputAction.CallbackContext context)
    {
        print("<color=orange> Input C");
        if (interactRayCollider != null)
        {
            print(interactRayCollider.gameObject.name);
            if (interactRayCollider.gameObject.name.Contains("EnterArea")) { 
                
                //gets the parent of the interaction point by using transform component
                Transform parent = mostRecentHit.GetComponentInParent<Transform>().parent;
                //brings the player into the ship on the ship entry point's coords
                //Maybe for bigger ships rework the system a little to make an array of all and pick the closest one to put them in
                foreach (Transform child in parent)
                {
                    if (child.CompareTag("Point") && child.gameObject.name.Contains("EntryPoint")) { 
                        transform.position = child.position;
                        boardedShip = true;
                        this.transform.parent = child.transform.parent;
                    }
                    //teleports the player to the entry point on the ship(no special effects happen for this yet)
                }
                print("<color=yellow> Entered" + parent.name);
            }

            if (interactRayCollider.gameObject.name.Contains("ExitArea")) { 
                
                Transform parent = mostRecentHit.GetComponentInParent<Transform>().parent;
                foreach (Transform child in parent)
                {
                    if (child.CompareTag("Point") && child.gameObject.name.Contains("ExitPoint")) { 
                        transform.position = child.position; 
                        boardedShip = false;
                        this.transform.parent = null;
                    }
                }
                print("<color=yellow> Exited" + parent.name);
            }

            if (interactRayCollider.gameObject.name.Contains("ShipControls")) {
                //! This is diff from exit and enter if statements so it will swap from player controller to ship controller
                Transform parent = mostRecentHit.GetComponentInParent<Transform>().parent;
                //? vvv uses the code previous to find the shipController script and place it as an obj here
                ShipController shipScript = (ShipController)parent.GetComponent<MonoBehaviour>();
                parent.GetComponent<MonoBehaviour>().enabled = true;
                print("Debug: enabled spaceshipController script");
                shipScript.pilotShip(this.gameObject,playerInput);
                print("<color=yellow> Piloting" + parent.name);
                enabled = false;
                print("Debug: PlayerController deactivated");
            }


        }}

    private void OnBoostedMovement(InputAction.CallbackContext context) { }

    private void getCurrentShip() {
        if (boardedShip) { 
        Transform parent = mostRecentHit.GetComponentInParent<Transform>().parent;
        if (parent.CompareTag("Ship")) { currentShip = parent.gameObject; }
        }
    }
    
    private void cameraLayerRenderSet()
    {
        if (boardedShip) { personalCamera.cullingMask = boardedMask; }
        else { personalCamera.cullingMask = AllRenderMask; }
    }

    private void OnEnable()
    {
        print("Player controller enabled");
        //do the code: 'buttonName'.action.started += On+'buttonName'; to make a method instantiator for input method
        playerInput.actions["PlayerActionA"].started += OnActionA;
        playerInput.actions["PlayerActionB"].started += OnActionB;
        playerInput.actions["PlayerActionC"].started += OnActionC;
        playerInput.actions["BoostMovement"].started += OnBoostedMovement;

        
    }
    private void OnDisable()
    {
        //need this disable code or else if code is enabled twice the obj will run the method twice
        playerInput.actions["PlayerActionA"].started -= OnActionA;
        playerInput.actions["PlayerActionB"].started -= OnActionB;
        playerInput.actions["PlayerActionC"].started -= OnActionC;
        playerInput.actions["BoostMovement"].started -= OnBoostedMovement;
    }
}
