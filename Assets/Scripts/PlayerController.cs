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
    [SerializeField] private LayerMask shipCeilingMask;

    //All player stats and specs we can give an int value to
    private enum stats : int { 
    moveSpeed = 10
    }
    private float stamina = 5;
    private Vector2 move;
    private Collider2D interactRayCollider;
    private bool boardedShip;
    private GameObject currentShip;
    private GameObject mostRecentHit;
    private bool piloting;
    private bool dashOnCooldown;
    private ShipController shipScript;
    private Vector2 lastPos;

    //Input Actions


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastPos = transform.position;
        boardedShip = false;
        piloting = false;
        
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
        getCurrentShip();
        movePlayer(Time.deltaTime);
        rotateSprite();
        targetRaycasts();
        checkRaycastContext();
        lastPos = transform.position;
    }
    private void LateUpdate() { }


    private void movePlayer(float delta)
    {
        Vector2 shipVelocity = Vector2.zero;
        getCurrentShip();
        if (boardedShip)
        {
            shipScript = (ShipController)currentShip.GetComponent<MonoBehaviour>();
            shipVelocity = shipScript.getShipVelocity();
        }
        move = playerInput.actions["PlayerMove"].ReadValue<Vector2>();
        //? The player can dash for 5 seconds and takes 5 secs to recharge but if only partially empty takes longer to charge
        if (playerInput.actions["BoostMovement"].IsPressed() && stamina >= 0 && move != Vector2.zero && !dashOnCooldown)
        {
            //TODO Add some particles for this
            stamina -= Time.deltaTime;
            move *= 2;
        }
        else { if (stamina < 10) { stamina += Time.deltaTime / 1.25f; } }
        if (stamina <= 0) { dashOnCooldown = true;}
        if (dashOnCooldown == true) { if (stamina <= 0) { stamina = 0; } stamina += Time.deltaTime/1f; }
        if (stamina >= 10) { stamina = 5; dashOnCooldown = false;}

        rb.linearVelocity = (move * (int)(stats.moveSpeed)) + shipVelocity;
        
    }
    private void rotateSprite() { 
        Vector2 thisPos = transform.position;
        Vector2 targetPos = thisPos+move;
        float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90;
        if (move != Vector2.zero)
        {
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

                //! TO rework for full physics give the ShipController class this object with a method to add them to an array of boarded people
                //! Also save the shipScript as a class var in player class for getting rb physics
                /*
                gets the parent of the interaction point by using transform component
                brings the player into the ship on the ship entry point's coords
                Maybe for bigger ships rework the system a little to make an array of all and pick the closest one to put them in
                */
                GameObject hitShip = mostRecentHit.transform.parent.gameObject;
                print("Entering " +hitShip.name);
                shipScript = (ShipController)hitShip.GetComponent<MonoBehaviour>();
                foreach(Transform child in hitShip.transform) {
                    if (child.CompareTag("Point") && child.gameObject.name.Contains("EntryPoint")) { 
                    this.transform.position = child.transform.position;
                    boardedShip = true;
                    }
                
                }
                currentShip = hitShip;
                shipScript.addPassenger(this.transform);
                print("<color=yellow> Entered" + currentShip.name);
            }

            if (interactRayCollider.gameObject.name.Contains("ExitArea")) { 
                shipScript = (ShipController)currentShip.GetComponent<MonoBehaviour>();
                shipScript.removePassenger(this.transform);
                foreach (Transform child in currentShip.transform)
                {
                    if (child.CompareTag("Point") && child.gameObject.name.Contains("ExitPoint")) { 
                        //TODO Make this code check a circle(r<=1) around the point for best value
                        RaycastHit2D exitChecker = Physics2D.Raycast(child.transform.position, child.transform.position, 2, interactRayInclude);
                        if(exitChecker.collider != null){print("Player shouldn't exit"); }
                        //else{}
                        //! Currently the code is giving false positives and that needs to be fixed(due to space being there[make a new mask elxuding supership stuff mebe?])
                            transform.position = child.position; 
                            boardedShip = false;
                            this.transform.parent = null;
                            print("<color=yellow> Exited" + parent.name);
                        
                        
                    }
                }
                
            }

            if (interactRayCollider.gameObject.name.Contains("ShipControls")) {
                //! This is diff from exit and enter if statements so it will swap from player controller to ship controller
                //? Collects the ship's script to use for the player to pilot the ship
                shipScript = (ShipController)currentShip.GetComponent<MonoBehaviour>();
                //TODO Change the code so the ship only overrides the player controls when they are piloting and it adds/subtrascts them from the list when it is piloted/unpiloted
                shipScript.enabled = true;
                print("Debug: enabled spaceshipController script");
                shipScript.pilotShip(this.gameObject,playerInput);
                print("<color=yellow> Piloting" + currentShip.name);
                piloting = true;
                enabled = false;
                print("Debug: PlayerController deactivated");
            }


        }}

    private void OnBoostedMovement(InputAction.CallbackContext context) { }

    public GameObject getCurrentShip() {
        if (Mathf.Abs(lastPos.x-transform.position.x) > 0.01 && Mathf.Abs(lastPos.y - transform.position.y) > 0.01) {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.position, 1, shipCeilingMask);
            if (ray.collider != null)
            {
                currentShip = ray.collider.transform.parent.gameObject;
                shipScript = (ShipController)currentShip.GetComponent<MonoBehaviour>();
                //!? debug print(currentShip.name);
                return currentShip;
            }
            else { if (!piloting) { return currentShip; }
                else { shipScript = null; currentShip = null; return null; } }
        }
        else { return currentShip; }
        //! LayerMask(256)
        /*
        if (boardedShip) { 
        Transform parent = mostRecentHit.GetComponentInParent<Transform>().parent;
        if (parent.CompareTag("Ship")) { currentShip = parent.gameObject; return currentShip; }
        }
        return null;
        */
    }
    public bool getPiloting() { return piloting;}
    public Camera getPersonalCamera() { return personalCamera; }
    
    private void cameraLayerRenderSet()
    {
        if (boardedShip) 
        {
            if (piloting) 
            {
                personalCamera.cullingMask = AllRenderMask;
            }
            else {
                personalCamera.cullingMask = boardedMask; 
            }
        }
        else { personalCamera.cullingMask = AllRenderMask; }
    }

    private void OnEnable()
    {
        print("Player controller enabled");
        //do the code: 'buttonName'.action.started += On+'buttonName'; to make a method instantiator for input method
        piloting = false;
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
