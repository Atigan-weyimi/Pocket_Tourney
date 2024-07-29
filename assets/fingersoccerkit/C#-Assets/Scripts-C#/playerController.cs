using System;
using UnityEngine;

public class PlayerController : Puck
{
    public event Action<PlayerController> DragStarted;
    public event Action<PlayerController> DragEnded;
    public event Action<PlayerController> Shot;

    /// *************************************************************************///
    /// Main Player Controller.
    /// This class manages the shooting process of human players.
    /// This also handles the rendering of debug lines in editor.
    /// *************************************************************************///

    // Public Variables //
    internal int unitIndex; //This unit's ID (given automatically by PlayerAI class)

    internal float shootTime;

    //Referenced GameObjects
    private GameObject helperBegin; //Start helper
    private GameObject helperEnd; //End Helper
    private TargetArrow arrow; //arrow plane which is used to show shotPower
    //private GameObject shootCircle; //shoot Circle plane which is used to show shotPower

    private GameObject gameController; //Reference to main game controller
    private float currentDistance; //real distance of our touch/mouse position from initial drag position
    private float safeDistance; //A safe distance value which is always between min and max to avoid supershoots

    private float pwr; //shoot power

    //this vector holds shooting direction
    private Vector3 shootDirectionVector;

    private readonly int
        timeAllowedToShoot =
            10000; //In Seconds (in this kit we give players unlimited time to perform their turn of shooting)

    //***************************************************************************//
    // Cast a ray forward and collect informations like if it hits anything...
    //***************************************************************************//
    private RaycastHit hitInfo;

    private Ray ray;

    private Camera _camera;

    //*****************************************************************************
    // Init
    //*****************************************************************************
    public override void Awake()
    {
        base.Awake();
        //Find and cache important gameObjects
        helperBegin = GameObject.FindGameObjectWithTag("mouseHelperBegin");
        helperEnd = GameObject.FindGameObjectWithTag("mouseHelperEnd");
        arrow = GameObject.FindGameObjectWithTag("helperArrow").GetComponent<TargetArrow>();
        gameController = GameObject.FindGameObjectWithTag("GameController");

        //Init Variables
        pwr = 0.1f;
        currentDistance = 0;
        shootDirectionVector = new Vector3(0, 0, 0);
        shootTime = timeAllowedToShoot;
        arrow.Hide();
        _camera = Camera.main;
    }
    
    public static Vector3 RayPlaneIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 plainPosition, Vector3 plainNormal)
    {
        rayDirection.Normalize();
        plainNormal.Normalize();
        var denominator = Vector3.Dot(rayDirection, -plainNormal);
        var distance = Vector3.Dot(plainPosition - rayOrigin, -plainNormal) / denominator;
        return rayOrigin + rayDirection * distance;
    }

    private bool prevInDrag;
    private bool InDrag
    {
        set
        {
            if (prevInDrag == value)
            {
                return;
            }

            prevInDrag = value;
            if (value)
            {
                DragStarted?.Invoke(this);
            }
            else
            {
                DragEnded?.Invoke(this);
            }
        }

        get => prevInDrag;
    }

    //***************************************************************************//
    // Works fine with mouse and touch
    // This is the main functiuon used to manage drag on units, calculating the power and debug vectors, and set the final parameters to shoot.
    //***************************************************************************//
    private void OnMouseDrag()
    {
        if (!CanShoot)
        {
            return;
        }

        var mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
        var cursorWorldPosition = RayPlaneIntersection(mouseRay.origin, mouseRay.direction, Vector3.zero, Vector3.back);
        
        var worldPosition = transform.position;
        var delta = worldPosition - cursorWorldPosition;
        delta.z = 0;

        var dragDistance = delta.magnitude;
        
        currentDistance = dragDistance;
        safeDistance = Mathf.Clamp(currentDistance, 0f, GlobalGameManager.maxDistance);
        InDrag = currentDistance > 0.75f;

        pwr = safeDistance * 12; //this is very important. change with extreme caution.

        //show the power arrow above the unit and scale is accordingly.

        var normalizedDelta = delta.normalized * Mathf.Clamp(dragDistance / GlobalGameManager.maxDistance, 0f, 1f);
        arrow.ShowDrag(worldPosition, normalizedDelta);

        //position of helperEnd
        //HelperEnd is the exact opposite (mirrored) version of our helperBegin object 
        //and help us to calculate debug vectors and lines for a perfect shoot.
        //Please refer to the basic geometry references of your choice to understand the math.
        var dxy = helperBegin.transform.position - transform.position;
        var diff = dxy.magnitude;
        helperEnd.transform.position = transform.position + dxy / diff * currentDistance * -1;

        helperEnd.transform.position = new Vector3(helperEnd.transform.position.x,
            helperEnd.transform.position.y,
            -0.5f);

        //debug line from initial position to our current touch position
        Debug.DrawLine(transform.position, helperBegin.transform.position, Color.red);
        //debug line from initial position to maximum power position (mirrored)
        Debug.DrawLine(transform.position, arrow.transform.position, Color.blue);
        //debug line from initial position to the exact opposite position (mirrored) of our current touch position
        Debug.DrawLine(transform.position, 2 * transform.position - helperBegin.transform.position, Color.yellow);
        //cast ray forward and collect informations
        CastRay();

        //Not used! You can extend this function to have more precise control over physics of the game
        //sweepTest();

        //final vector used to shoot the unit.
        shootDirectionVector = delta.normalized;
        //print(shootDirectionVector);
    }

    //***************************************************************************//
    // Cast the rigidbody's shape forward to see if it is about to hit anything.
    //***************************************************************************//
    private void SweepTest()
    {
        RaycastHit hit;
        if (GetComponent<Rigidbody>()
            .SweepTest((helperEnd.transform.position - transform.position).normalized, out hit, 15))
            print("if hit ??? : " + hit.distance + " - " + hit.transform.gameObject.name);
    }

    private void CastRay()
    {
        //cast the ray from units position with a normalized direction out of it which is mirrored to our current drag vector.
        ray = new Ray(transform.position, (helperEnd.transform.position - transform.position).normalized);

        if (Physics.Raycast(ray, out hitInfo, currentDistance))
        {
            var objectHit = hitInfo.transform.gameObject;

            //debug line whenever the ray hits something.
            Debug.DrawLine(ray.origin, hitInfo.point, Color.cyan);

            //draw reflected vector like a billiard game. this is the out vector which reflects from targets geometry.
            var reflectedVector = Vector3.Reflect(hitInfo.point - ray.origin, hitInfo.normal);
            Debug.DrawRay(hitInfo.point, reflectedVector, Color.gray, 0.2f);

            //draw inverted reflected vector (useful for fine-tuning the final shoot)
            Debug.DrawRay(hitInfo.transform.position, reflectedVector * -1, Color.white, 0.2f);

            //draw the inverted normal which is more likely to be similar to real world response.
            Debug.DrawRay(hitInfo.transform.position, hitInfo.normal * -3, Color.red, 0.2f);

            //Debug
            print("Ray hits: " + objectHit.name + " At " + Time.time + " And Reflection is: " + reflectedVector);
        }
    }

    //***************************************************************************//
    // Actual shoot fucntion
    //***************************************************************************//
    private void OnMouseUp()
    {
        if (!InDrag)
        {
            arrow.Hide();
            return;
        }
        
        InDrag = false;
        
        //But if player wants to shoot anyway:
        //prevent double shooting in a round
        if (!CanShoot)
        {
            return;
        }

        //keep track of elapsed time after letting the ball go, 
        //so we can findout if ball has stopped and the round should be changed
        //this is the time which user released the button and shooted the ball
        shootTime = Time.time;

        //hide helper arrow object
        arrow.Hide();

        //do the physics calculations and shoot the ball 
        var outPower = shootDirectionVector * pwr;

        //add team power bonus
        //print ("OLD outPower: " + outPower.magnitude);
        outPower *= 1 + TeamsManager.getTeamSettings(PlayerPrefs.GetInt("PlayerFlag")).x / 50;
        //print ("NEW outPower: " + outPower.magnitude);

        //always make the player to move only in x-y plane and not on the z direction
        print("shoot power: " + outPower.magnitude);
        GetComponent<Rigidbody>().AddForce(outPower, ForceMode.Impulse);
        Shot?.Invoke(this);

        //change the turn
        StartCoroutine(gameController.GetComponent<GlobalGameManager>().managePostShoot());
    }
}