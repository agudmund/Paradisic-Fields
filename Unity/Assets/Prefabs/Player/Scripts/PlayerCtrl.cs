using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Collections;
using Random = UnityEngine.Random;

///  Character Controller for Adulting.
///     -aeVar 2023
///  -Standard Camera and Motion Controls.
///  -Menu and Inventory access.
///  -Notification controller.
///  -Uses a combination of the Input System for gamepad control and default character controller for motion.
///  -Includes a series of mocap motions for 3rd person view.
///  -Some Hunger Control.
///  

public class PlayerCtrl : MonoBehaviour
{
    public NotificatCtrl msg;
    InputActions controls;
    GameCtrl ctrl;

    [Header("Temp Info Panel")]
    [SerializeField] TextMeshPro texttestLabel;
    int versecount = 0;
    public float versetimer = 0f;


    [SerializeField] TextMeshPro hungertimerLabel;
    [SerializeField] TextMeshPro hungerlevelLabel;
    [SerializeField] TextMeshPro hungerspeedfactorLabel;
    [SerializeField] GameObject infopanel;
    [SerializeField] bool infoOpen;
    [SerializeField] TextMeshPro timeElapsedLabel;
    [SerializeField] TextMeshPro sleepRequiredLabel;


    [Header("Camera")]
    [SerializeField] Transform EyeViewCamera;
    public Transform notebook;
    [SerializeField] Transform grabPoint;
    [SerializeField] float sensitivityX = 120f;
    [SerializeField] float sensitivityY = 7f;
    Vector2 lookInput;
    RaycastHit ray;
    float xClamp = 85f;
    float xRotation = 0f;
    float lookX, lookY;
    
    [Header("Motion")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float gravity = -30f;
    [SerializeField] float jumpHeight = 3.5f;
    [SerializeField] float speed;
    [SerializeField] float basespeed = 11;
    [SerializeField] bool isGrounded;
    CharacterController controller;
    Vector2 horizontalInput;
    Vector3 verticalVelocity = Vector3.zero;
    bool isJumping;
    
    [Header("Interactions")]
    [SerializeField] LayerMask pickupMask;
    [SerializeField] float maxSelectDistance = 10f;
    [SerializeField] Transform grabSpot;
    public Grabbable grabz;

    [Header("Emotional Control")]
    [SerializeField] GameObject myBag;
    [SerializeField] bool bagOpen;
    [Range(0.0f, 9.0f)]
    public float shyness = 5f;
    [Range(0.0f, 9.0f)]
    public float magnetism = 5f;
    [Range(0.0f, 9.0f)]
    public float aggression = 5f;
    [Range(0.0f, 9.0f)]
    public float empathy = 5f;
    [Range(0.0f, 9.0f)]
    public float luck = 5f;
    [Range(0.0f, 9.0f)]
    public float uncertainty = 5f;

    [Header("General Wellbeing")]
    [Range(0.0f, 10f)]
    public float hunger = 10f;
    float hungerfactor = 1f;
    int hungerlevel = 0;
    float hungertimer = 0;
    [Range(0.0f, 9.0f)]
    public float strength = 5f;
    [Range(0.0f, 9.0f)]
    public float soul = 5f;
    [Range(0.0f, 9.0f)]
    public float intellect = 5f;

    public float sleepfactor = 1f;
    public int sleeplevel = 0;

    void Awake(){
        // this is the base notification mechanics not per page....
        msg = GameObject.FindGameObjectWithTag("Notificat").GetComponent<NotificatCtrl>();
        ctrl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameCtrl>();
        InitControls();
        speed = basespeed;
    }

    void OnEnable(){
        controls.Enable();
    }

    void OnDisable(){
        controls.Disable();
    }

    void FixedUpdate(){
        speed = basespeed * hungerfactor * sleepfactor;
        OnJump();
        OnMove(horizontalInput);
        OnLook(lookInput);
        // HungerCtrl();
        SleepCtrl();
        TextTest();
    }

    void SleepCtrl(){
        string sleepyMessage = "";
        sleepfactor = sleepfactor - (sleeplevel/10);
        if(sleepfactor<0.4f) sleepfactor = 0.4f;
        if(sleeplevel<3){
            sleepyMessage = "Feeling kinda ok really.";
        }
        else{
            sleepyMessage = "Need sleep soon.";
        }

        if(infoOpen){
            timeElapsedLabel.text = ctrl.hours.ToString() + ":" + ctrl.minutes.ToString() + " in hell so far.";
            sleepRequiredLabel.text = sleepyMessage;
        }
    }

    public IEnumerator Sleep()
    {
        int hoursToSleep = 5;
        for(int i=0;i<hoursToSleep;i++){
            ctrl.minutes = ctrl.minutes+60;
            yield return new WaitForSeconds(.01f);
        }
        hungerlevel = hungerlevel + 1;
        sleeplevel = 0;
        sleepfactor = 1;
    }

    void HungerCtrl()
    {
        
        if(hungerlevel<0)hungerlevel=0;
        if(hungerlevel>10)hungerlevel=10;
        if(hungerfactor>1)hungerfactor=1;

        hungertimer = hungertimer + Time.deltaTime;
        if(hungertimer>60){
            msg.Notificat("Alpaca Cooking Show!", hungerMessages[Random.Range(0,hungerMessages.Length)],notebook);
            hungertimer = 0;
            hungerlevel = hungerlevel +1;
            hungerfactor = (float)Math.Round( hungerfactor * 0.9f, 1) ;
        }
        int hungerdisplaytimer = (int)hungertimer;
        if(infoOpen){
            hungertimerLabel.text = "Hunger Timer: " + hungerdisplaytimer.ToString();
            hungerlevelLabel.text = "Hunger Level: " + hungerlevel.ToString();
            hungerspeedfactorLabel.text = "Hunger Speed Factor: " + hungerfactor.ToString();
        }   
    }
    void TextTest()
    {
        if(msg==null)
        {
            msg = GameObject.FindGameObjectWithTag("NotificatCtrl").GetComponent<NotificatCtrl>();
        }
        if(versecount<0)versecount=0;
        if(versecount>wordSelects.Length)versecount=wordSelects.Length;

        versetimer = versetimer + Time.deltaTime;
        if(versetimer>20)
        {
            versetimer = 0;
            msg.Notificat("This thing.", wordSelects[versecount],notebook);
            
            versecount = versecount +1;
            
        }
    }

     string[] wordSelects = new string[] {
        "I, Nephi, having been born of goodly parents, therefore I was taught somewhat in all the learning of my father; and having seen many afflictions in the course of my days, nevertheless, having been highly favored of the Lord in all my days; yea, having had a great knowledge of the goodness and the mysteries of God, therefore I make a record of my proceedings in my days.",
        "Yea, I make a record in the language of my father, which consists of the learning of the Jews and the language of the Egyptians.",
        "And I know that the record which I make is true; and I make it with mine own hand; and I make it according to my knowledge.",
        "For it came to pass in the commencement of the first year of the reign of Zedekiah, king of Judah, (my father, Lehi, having dwelt at Jerusalem in all his days); and in that same year there came many prophets, prophesying unto the people that they must repent, or the great city Jerusalem must be destroyed.",
        "Wherefore it came to pass that my father, Lehi, as he went forth prayed unto the Lord, yea, even with all his heart, in behalf of his people.",
        "And it came to pass as he prayed unto the Lord, there came a pillar of fire and dwelt upon a rock before him; and he saw and heard much; and because of the things which he saw and heard he did quake and tremble exceedingly.",
        "And it came to pass that he returned to his own house at Jerusalem; and he cast himself upon his bed, being overcome with the Spirit and the things which he had seen.",
        "And being thus overcome with the Spirit, he was carried away in a vision, even that he saw the heavens open, and he thought he saw God sitting upon his throne, surrounded with numberless concourses of angels in the attitude of singing and praising their God.",
        "And it came to pass that he saw One descending out of the midst of heaven, and he beheld that his luster was above that of the sun at noon-day.",
        "And he also saw twelve others following him, and their brightness did exceed that of the stars in the firmament."
    };

    string[] hungerMessages = new string[] {
        "We are kinda hungry.",
        "We could really really go for some chicken, what do you think?",
        "mmmhmmmmmm, some food does sound like a good idea right now.",
        "This faint rumbling sound, that is a stomach sound. does it remind us of something?",
        "The stomach is growling at us for some reason.",
        "Remember the good old days two days ago when we ate alllllll the chicken?",
        "If I would ask me I would totally suggest we just pause and go get some chicken right now.",
        "Perhaps we should be looking for takeaway food delivery phone numbers instead of food?",
        "what is there to say really, we should eat some food!!",
        "I seriously think we need some kind of a 5 step action plan which results in us actually eating something."
    };

    void InitControls(){
        controls = new InputActions();
        controller = GetComponent<CharacterController>();
        controls.Player.Shoot.performed += OnShoot;
        controls.Player.Grab.performed += OnGrab;
        controls.Player.Inventory.performed += Inventory;
        controls.Player.Introspection.performed += Introspection;
        controls.Player.Jump.performed += _ => isJumping = true;
        controls.Player.Movement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        controls.Player.lookX.performed += ctx => lookInput.x = ctx.ReadValue<float>();
        controls.Player.lookY.performed += ctx => lookInput.y = ctx.ReadValue<float>();
    }

    void OnMove(Vector2 _horizontalInput){
        horizontalInput = _horizontalInput;
        Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * speed;
        controller.Move(horizontalVelocity * Time.deltaTime);
    }

    void OnLook(Vector2 lookInput){
        lookX = lookInput.x * sensitivityX;
        lookY = lookInput.y * sensitivityY;
        transform.Rotate(Vector3.up, lookX * Time.deltaTime);
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        EyeViewCamera.eulerAngles = targetRotation;
    }

    void OnJump(){
        isGrounded = Physics.CheckSphere(transform.position,0.1f,groundMask);
        if(isGrounded) verticalVelocity.y = 0f;
        verticalVelocity.y += gravity * Time.deltaTime;
        if(isJumping && isGrounded){
                verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
                isJumping = false;
            }
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    void OnGrab(InputAction.CallbackContext context){
        if (grabz==null){
            if(Rayhit()){

                if(ray.transform.name.Contains("Puff") ){
                    PuffCtrl puff = ray.transform.GetComponent<PuffCtrl>();
                    if(puff.detached){
                        grabz = puff.GetComponent<Grabbable>();
                        grabz.Grab(grabPoint.transform);
                    }}}
            }
        else{
            if(bagOpen){
                // Replace and add to actual inventory
                Destroy(grabz.gameObject);
                grabz = null;
            }
            else{
                grabz.Drop();
                grabz = null;
        }}
    }

    void Inventory(InputAction.CallbackContext context)
    {
        if (msg.page.active)
        {
            msg.page.Drop();
        }
        else
        {
            bagOpen = !bagOpen;
            if (bagOpen){
                myBag.SetActive(true);
            }
            else{
                myBag.SetActive(false);
            }
        }
    }
    void Introspection(InputAction.CallbackContext context)
    {
        infoOpen = !infoOpen;
        if (infoOpen){
            // Should get the notebook from the bag actually.
            infopanel.SetActive(true);
        }
        else{
            infopanel.SetActive(false);
        }
    }

    void OnShoot(InputAction.CallbackContext context){
        if(grabz==null){
            if(Rayhit()){
                // if(ray.transform.name.Contains("heart") ){
                //         HeartCtrl heart = ray.transform.GetComponent<HeartCtrl>();
                //         heart.deezNuts();
                //         }
                //     // Do things to the puff when it gets hit.
                //     if(ray.transform.name.Contains("Puff") ){
                //         PuffCtrl puff = ray.transform.GetComponent<PuffCtrl>();
                //         puff.deezNuts();
                //         }
                //     if(ray.transform.name.Contains("Bed") ){
                //         BedCtrl puff = ray.transform.GetComponent<BedCtrl>();
                //         puff.deezNuts();
                //         }
            }
        }
        else{
            hungerlevel = hungerlevel - 1;
            hungerfactor = (float)Math.Round( hungerfactor * 1.1f, 1) ;
            Destroy(grabz.gameObject);
            grabz = null;
        }
    
    }

    bool Rayhit(){
        if(Physics.Raycast( EyeViewCamera.position,EyeViewCamera.forward, 
                            out ray, 
                            maxSelectDistance,pickupMask)){
            return true;}
        else{
            return false;
        }
        }
    
}
