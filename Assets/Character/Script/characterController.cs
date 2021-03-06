﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController : MonoBehaviour {
	static Animator anim;

    public Camera sceneCamera;

    Vector3 inputVec;

    float x;
    float z;

    public float vida = 100;
	public float speed = 20.0F;
	public float rotationSpeed = 10.0F;
    private bool isDead;
    public Transform respawn;
    private bool hasFlash;
    //hud variables
    public Bonus elbono;
    //Jump Variables

    public float jumpForce = 2.0F;
	public Vector3 jumpVector;
	public bool isGrounded;
	Rigidbody rb;

    public Vida vida1;

    private string power;

    public SlowMotionEffect slowTime;

    public int flashCounter;

    //jump
    public bool doublejump;
	
	// Use this for initialization
	void Start () 
	{
        flashCounter = 0;
        power = "";
        hasFlash = true;
        isDead = false;
		anim= GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		jumpVector = new Vector3(0.0F,2.0F,0.0F);
    }
	
	void OnCollisionExit(){
		isGrounded=false;
	}
	void OnCollisionStay(){
		isGrounded = true;
		anim.SetBool("isJumping",false);
		anim.SetBool("isFalling",false);
	}
	
	// Update is called once per frame
	void Update () {
        CameraRelativeMovement();
        //update bono hud
        if (Input.GetMouseButtonDown(0) && elbono.activo == true)
        {

            elbono.activo = false;
            power = elbono.img;
            Debug.Log(power);
        }
        //

        float translation = z * speed;
		float straffe = x * speed;
		translation*=Time.deltaTime;
		straffe*=Time.deltaTime;
		
		transform.Translate(straffe,0,translation);
        if (power == "lobo" && Input.GetButtonDown("Fire3") && hasFlash) {
            transform.position += new Vector3(0, 0, 10);
            slowTime.activo = false;
            flashCounter += 1;
            if (flashCounter == 3) {
                hasFlash = false;
                flashCounter = 0;
            }
            doublejump = false;
            //hasFlash = false;
        }
        if (power == "rel") {
            slowTime.activo = true;
            power = "";
            doublejump = false;
            hasFlash = true;
            flashCounter = 0;
        }
        if (power == "cal") {
            //vida1.TakeDamage(100f);
            StartCoroutine(_Death());
            slowTime.activo = false;
            doublejump = false;
            power = "";
            hasFlash = true;
            flashCounter = 0;
        }

        if (power == "alas")
        {
    
            slowTime.activo = false;
            doublejump = true;
            power = "";
            hasFlash = true;
            flashCounter = 0;
        }

        if (Input.GetButtonDown("Jump") && (isGrounded || doublejump)){
			rb.AddForce(jumpVector * jumpForce, ForceMode.Impulse);
			anim.SetBool("isJumping",true);
			isGrounded=false;
		}
	
		
		if( !(isGrounded) && !(anim.GetBool("isJumping"))){
			anim.SetBool("isFalling",true);
		}
		if(translation!=0 || straffe!=0){
			anim.SetBool("isRunning",true);
			anim.SetBool("isIdle",false);
		}
		else{
			anim.SetBool("isRunning",false);
			anim.SetBool("isIdle",true);
		}
        RotateTowardsMovementDir();
		
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Killzone" && !isDead)
        {
            StartCoroutine(_Death());
        }
        else if (other.gameObject.tag == "damageObject") {
            vida1.TakeDamage(20);
            this.vida -= 20;
            if (this.vida <= 0) {
                StartCoroutine(_Death());
            }
        }
    
    }

    //coger el bono
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("poder"))
        {
            hasFlash = true;
            doublejump = true;
            slowTime.activo = false;
            other.gameObject.SetActive(false);
            elbono.activo = true;
        }
    }

    void CameraRelativeMovement()
    {

        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");

        //converts control input vectors into camera facing vectors
        Transform cameraTransform = sceneCamera.transform;
        //Forward vector relative to the camera along the x-z plane   
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        //Right vector relative to the camera always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        //directional inputs
        x = inputHorizontal;
        z = inputVertical;
        inputVec = x * right + z * forward;
    }

    void RotateTowardsMovementDir()
    {
        if (inputVec != Vector3.zero )
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
        }
    }

    public IEnumerator _Death(){
        this.power = "";
        slowTime.activo = false;
        doublejump = false;
		anim.SetBool("isDead",true);
		yield return new WaitForSeconds(3.9F);
		anim.SetBool("isDead",false);
        this.transform.position = respawn.position;
        this.vida = 100;
        vida1.restartHeatlth();
		
    }
}
