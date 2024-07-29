using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class BallManager : MonoBehaviour
{
    public AudioClip _ballHitPost; //Sfx for hitting the poles

    //*****************************************************************************
    // Main Ball Manager.
    // This class controls ball collision with Goal triggers and gatePoles, 
    // and also stops the ball when the spped is too low.
    //*****************************************************************************

    private GameObject _gameController; //Reference to main game controller
    private Rigidbody _rigidbody;
    private SpringJoint _springJoint;
    private Collider _collider;

    //*****************************************************************************
    // Check ball's speed at all times.
    //*****************************************************************************
    private float _ballSpeed;

    public Vector3 Velocity
    {
        get => _rigidbody.velocity;
        set => _rigidbody.velocity = value;
    }

    public bool IsMoving => _rigidbody.velocity.sqrMagnitude > 0.01f;

    public bool IsKinematic
    {
        get => _rigidbody.isKinematic;
        set => _rigidbody.isKinematic = value;
    }
    
    public bool CollisionEnabled
    {
        get => _collider.enabled;
        set => _collider.enabled = value;
    }

    public void Attach(Rigidbody rigidbody)
    {
        _springJoint.connectedBody = rigidbody;
        _springJoint.maxDistance = 0f;
    }

    public void Detach()
    {
        _springJoint.connectedBody = null;
        _springJoint.maxDistance = 1000f;
    }

    private void Awake()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController");
        _rigidbody = GetComponent<Rigidbody>();
        _springJoint = GetComponent<SpringJoint>();
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        ManageBallFriction();
    }

    private void LateUpdate()
    {
        //we restrict rotation and position once again to make sure that ball won't has an unwanted effect.
        transform.position = new Vector3(transform.position.x,  transform.position.y, -0.5f);

        //if you want a fixed ball with no rotation, uncomment the following line:
        //transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    private void ManageBallFriction()
    {
        _ballSpeed = _rigidbody.velocity.magnitude;
        //print("Ball Speed: " + rigidbody.velocity.magnitude);
        if (_ballSpeed < 0.5f)
        {
            _rigidbody.drag = 5;
        }
        else
        {
            //let it slide
            _rigidbody.drag = 0.9f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "gatePost":
                PlaySfx(_ballHitPost);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "opponentGoalTrigger":
                StartCoroutine(_gameController.GetComponent<GlobalGameManager>().managePostGoal("Player"));
                break;

            case "playerGoalTrigger":
                StartCoroutine(_gameController.GetComponent<GlobalGameManager>().managePostGoal("Opponent"));
                break;
        }
    }
    
    //*****************************************************************************
    // Play sound clips
    //*****************************************************************************
    private void PlaySfx(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
    }
}