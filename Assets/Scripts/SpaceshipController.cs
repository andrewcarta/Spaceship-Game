using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
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

//This script will be attatched to the ship parent node
//? The pilot will stay a child of the ship but be stored as the pilot var in the private GameObj


public class ShipController : MonoBehaviour
{
    //parts of the player's unity components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D cdBase;
    [SerializeField] private Collider2D cdTop;
    [SerializeField] private SpriteRenderer spBase;
    [SerializeField] private SpriteRenderer spTop; 
    [SerializeField] private GameObject shipControls;
    [SerializeField] private int shipScale;
    [SerializeField] private int shipSpeed;
    [SerializeField] private int shipTurnSpeed;
    [SerializeField] private int shipArmor;
    [SerializeField] private int shipHealth;
    private PlayerInput pilotInput;

    //All ship stats and specs we can give an int value to
    private bool piloted;
    private bool boostersActive;
    private float boostBonus = 1;
    private Vector2 move;
    private Transform pilotSeat;
    private GameObject pilot;
    private List<Transform> mainEngineSystem;
    private Vector2 lastVelocity;
    private float stamina = 10;
    private bool boostOnCooldown;

    //TODO Make a method to assign values for ship size and possibly speed by default
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TODO It would be amazing to have smth to set all SerializeFields at game start code
        mainEngineSystem = new List<Transform>();
        move = Vector2.zero;
        foreach (Transform child in transform) {
            if (child.name.Contains("ShipControls")) {
                shipControls = child.gameObject;
            }
            if (child.name.Contains("PilotSeat")) {
                pilotSeat = child.transform;
            }
            //All ships will have a main engine system so I can use them to list off the engine points
            if (child.name.Contains("EngineSystem")){
                foreach (Transform enginePoint in child) { 
                mainEngineSystem.Add(enginePoint);
                    print("Added Engine "+enginePoint.name);
                }
                
            }
        }
        deactivateEngineParticles();
        print("<color=green> " + this.name + " spawned at " + gameObject.transform.position.x + " " + gameObject.transform.position.y);
        if (!piloted)
        {
            print("<color=magenta> No initial pilot, disabling SpaceshipController");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        
        if (piloted)
        {
            cameraLayerRenderSet();
            applyMovement();
            lockPilotPos();
        }
    }
    private void LateUpdate()
    { 

    }

    private void cameraLayerRenderSet() {
        PlayerController pilotScript = (PlayerController)pilot.GetComponent<MonoBehaviour>();
        LayerMask AllRenderMask = -1;
        pilotScript.getPersonalCamera().cullingMask = AllRenderMask;
    }


    private void applyMovement()
    {
        //TODO Make the engines work slower at the start and faster later on
        move = pilotInput.actions["PlayerMove"].ReadValue<Vector2>();
        // Rotates the ship left and right(balanced by ship size to slow turning)
        //? Used when a keyboard is used to pilot
        if (pilot.GetComponent<PlayerInput>().currentControlScheme.Equals("Keyboard"))
        {
            transform.Rotate(0f,0f, Mathf.Clamp(-1 * move.x,-(int)(shipTurnSpeed),(int)(shipTurnSpeed)));
            //applies a velocity on the ship pointing in the direction it is facing
            if (move.y != 0)
            {
                boostMovement();
                //TODO Make 2 timers, one to count down while active and one to count down when it is done based upon the value of the timers(stamina and cooldown)

                if (move.y > 0) { rb.linearVelocity = transform.up * (int)(shipSpeed * boostBonus); }
                activateEngineParticles();
            }
            lastVelocity = rb.linearVelocity;
            deactivateEngineParticles();
        }
        //? Used when a controller is used to pilot
        if (pilot.GetComponent<PlayerInput>().currentControlScheme.Equals("Controller"))
        {
            //!? NEED TO UPDATE WITH THE SAME THING AS THE KEYBOARD STUFF (MAYBE MAKE INTO A METHOD)
            boostMovement();
            Angle moveAndFacingAngle = Vector2.SignedAngle(transform.up, move);
            Angle unitCircleAngle = Vector2.SignedAngle(new Vector2(0, 1), move);
            float facingAndMove = Vector2.SignedAngle(transform.up, move);
            if (!(move == Vector2.zero)) { 

            if (unitCircleAngle.value > 45 && unitCircleAngle.value <= 135)
            {
                    if ((moveAndFacingAngle.value < 45 && moveAndFacingAngle.value >= 0) || (moveAndFacingAngle.value > -45 && moveAndFacingAngle.value <= 0))
                    {
                        rb.linearVelocity = transform.up * (int)(shipSpeed * boostBonus);
                        activateEngineParticles();
                    }
                    transform.Rotate(0f, 0f,Mathf.Clamp(facingAndMove,-shipTurnSpeed,shipTurnSpeed));
                    
            }
            //if ship is facing down
            if ((unitCircleAngle.value > 135 && unitCircleAngle.value <= 180) || (unitCircleAngle.value <= -135 && unitCircleAngle.value > -180))
            {
                    if ((moveAndFacingAngle.value < 45 && moveAndFacingAngle.value >= 0) || (moveAndFacingAngle.value > -45 && moveAndFacingAngle.value <= 0))
                    {
                        rb.linearVelocity = transform.up * (int)(shipSpeed * boostBonus);
                        activateEngineParticles();
                    }
                    transform.Rotate(0f, 0f, Mathf.Clamp(facingAndMove, -shipTurnSpeed, shipTurnSpeed));
                    
            }
            //if ship is facing left
            if (unitCircleAngle.value > -135 && unitCircleAngle.value <= -45)
            {
                    if ((moveAndFacingAngle.value < 45 && moveAndFacingAngle.value >= 0) || (moveAndFacingAngle.value > -45 && moveAndFacingAngle.value <= 0))
                    {
                        rb.linearVelocity = transform.up * (int)(shipSpeed * boostBonus);
                        activateEngineParticles();
                    }
                    transform.Rotate(0f, 0f, Mathf.Clamp(facingAndMove, -shipTurnSpeed, shipTurnSpeed));
                    
            }
            //if ship if facing right
            if ((unitCircleAngle.value > -45 && unitCircleAngle.value <= 0) || (unitCircleAngle.value >= 0 && unitCircleAngle.value <= 45))
            {
                    if ((moveAndFacingAngle.value < 45 && moveAndFacingAngle.value >= 0) || (moveAndFacingAngle.value > -45 && moveAndFacingAngle.value <= 0))
                    {
                        rb.linearVelocity = transform.up * (int)(shipSpeed * boostBonus);
                        activateEngineParticles();
                    }
                    transform.Rotate(0f, 0f, Mathf.Clamp(facingAndMove, -shipTurnSpeed, shipTurnSpeed));
                    
            }
            }
            if (rb.linearVelocity == Vector2.zero) { deactivateEngineParticles(); }
        }
    }
    private void boostMovement() {
        if (pilotInput.actions["BoostMovement"].IsPressed() && stamina >= 0 && move != Vector2.zero && !boostOnCooldown)
        {
            //TODO Add some extra engine particles for this
            stamina -= Time.deltaTime;
            boostBonus = 2;
        }
        else { if (stamina < 10) { stamina += Time.deltaTime / 1.25f;boostBonus = 1; } }
        if (stamina <= 0) { boostOnCooldown = true; boostBonus = 1; }
        if (boostOnCooldown == true) { if (stamina <= 0) { stamina = 0; } stamina += Time.deltaTime / 1f; }
        if (stamina >= 10) { stamina = 10; boostOnCooldown = false; }
    }
    //TODO Finish this and make it work properly so player isn't moved weirdly in the ship
    public Vector2 getShipVelocity() {
        return rb.linearVelocity;
    }

    //? This method will run when two things collide mainly to deal damage as continuous collision will stop squishes
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Vector2 hitAt = collision.rigidbody.position;
        Rigidbody2D hitRB = collision.rigidbody;
        GameObject collisionObj = collision.transform.gameObject;

        //if a ship hits another ship
        if (collisionObj.CompareTag("Ship"))
        {
            
        }
        else 
        //? if the ship hits anything else
        //TODO can specialize an if statement to damage player later if I want
        {
            if (collisionObj.transform.parent != null && collisionObj.transform.parent.Equals(this.transform))
            {
                
            }
            else {
                
            }
        }
    }
    //? This method will make an even bigger force explosion in the direction an obj is facing
    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }


    private void targetRaycasts()
    {

    }

    private void ShipActionA(InputAction.CallbackContext context)
    {
        print("<color=orange> Ship Input A");
    }
    private void ShipActionB(InputAction.CallbackContext context)
    {
        print("<color=orange> Ship Input B");
    }
    private void ShipActionC(InputAction.CallbackContext context)
    {
        print("<color=orange> Ship Input C");
        //! This press will unpilot the ship and disable this script while enabling the playerController ship
        if (piloted)
        {
            //enable player script
            pilot.GetComponent<PlayerController>().enabled = true;
            //disable this script
            unpilotShip();
        }
    }

    
    private void OnBoostedMovement(InputAction.CallbackContext context) {
        boostersActive = pilotInput.actions["BoostMovement"].triggered;
    }
    //? use mainEngineSystem list(an array basically) to find the engines and toggle on and off the particle systems
    //! Currently having issues with specificEngine.gameObj.getComponent<ParticleSystem>() not returning the particleSystem
    private void deactivateEngineParticles() {
        
    }
    private void activateEngineParticles()
    {
        
    }
    private void lockPilotPos() {
        if (pilotSeat != null && pilot != null)
        {
            pilot.transform.position = pilotSeat.position;
            pilot.transform.rotation = this.transform.rotation;
            pilot.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
    public void damageShip(int damage, Vector2 damagePos) {
        if (shipArmor < 0) { shipArmor = 0; }
        shipHealth -= (damage-shipArmor);
        shipArmor --;
    }
    public int getArmor() {
        return shipArmor;
    }
    public int getScale() { 
    return shipScale;
    }

    //? This method will be called to let the ship know who the pilot is when the ship starts to piloted
    public void pilotShip(GameObject plt, PlayerInput input) {
        if (piloted) { swapPilot(plt,input); }
        enabled = true;
        pilot = plt;
        piloted = true;
        pilotInput = input;
        print("Ship being piloted by " + plt.name);
        pilotInput.actions["PlayerActionA"].started += ShipActionA;
        pilotInput.actions["PlayerActionB"].started += ShipActionB;
        pilotInput.actions["PlayerActionC"].started += ShipActionC;
        pilotInput.actions["BoostMovement"].started += OnBoostedMovement;
    }
    private void unpilotShip() {
        print("Ship unpiloted by " + pilot.name);
        pilot = null;
        piloted = false;
        rb.linearVelocity = rb.linearVelocity / 10;
        pilotInput.actions["PlayerActionA"].started -= ShipActionA;
        pilotInput.actions["PlayerActionB"].started -= ShipActionB;
        pilotInput.actions["PlayerActionC"].started -= ShipActionC;
        pilotInput.actions["BoostMovement"].started -= OnBoostedMovement;
        enabled = false;
        deactivateEngineParticles();
    }
    private void swapPilot(GameObject newPlt, PlayerInput newInput) {
        print("Ship pilot swapped from " + pilot.name+ " to " + newPlt);
        pilot.gameObject.GetComponent<MonoBehaviour>().enabled = true;
        pilot = newPlt;
        piloted = true;
        rb.linearVelocity = rb.linearVelocity / 10;
        pilotInput.actions["PlayerActionA"].started -= ShipActionA;
        pilotInput.actions["PlayerActionB"].started -= ShipActionB;
        pilotInput.actions["PlayerActionC"].started -= ShipActionC;
        pilotInput.actions["BoostMovement"].started -= OnBoostedMovement;
        pilotInput = newInput;
        pilotInput.actions["PlayerActionA"].started += ShipActionA;
        pilotInput.actions["PlayerActionB"].started += ShipActionB;
        pilotInput.actions["PlayerActionC"].started += ShipActionC;
        pilotInput.actions["BoostMovement"].started += OnBoostedMovement;
        deactivateEngineParticles();
    }

    private void OnEnable()
    {

        print("<color=magenta> SpaceshipController enabled");
    }
    private void OnDisable()
    {
        deactivateEngineParticles();
        print("<color=magenta> Spaceship controller disabled");
    }
}
