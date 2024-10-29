using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class BallManager : MonoBehaviour
{
    public static BallManager instance;
    public AudioClip _ballHitPost; //Sfx for hitting the poles

    //*****************************************************************************
    // Main Ball Manager.
    // This class controls ball collision with Goal triggers and gatePoles, 
    // and also stops the ball when the spped is too low.
    //*****************************************************************************

    private GameObject _gameController; //Reference to main game controller
    public Rigidbody _rigidbody;
    private SpringJoint _springJoint;
    private Collider _collider;
    public Puck _shootingPuck;
    public bool _shooterHitTheBall = false;
    //*****************************************************************************
    // Check ball's speed at all times.
    //*****************************************************************************
    [SerializeField]public float _ballSpeed;
    [SerializeField]private float _ballRotSpeed;
    [SerializeField]private float _torqueMultiplier;
    [SerializeField]private GameObject _ballSphere;

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
        instance = this;
        _gameController = GameObject.FindGameObjectWithTag("GameController");
        _rigidbody = GetComponent<Rigidbody>();
        _springJoint = GetComponent<SpringJoint>();
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        ManageBallFriction();



        


        //if(_ballSpeed > 0f)
        //{
        //    Vector3 _rotVect = _rigidbody.velocity;

        //    transform.rotation = Quaternion.Euler(transform.rotation.x + _rotVect.x*_ballRotSpeed, 
        //        transform.rotation.y + _rotVect.y * _ballRotSpeed,
        //        transform.rotation.z + _rotVect.z * _ballRotSpeed);
        //}
        //if(Input.GetKeyDown(KeyCode.M))
        //{
        //    _rigidbody.AddTorque(_rotVect, ForceMode.VelocityChange);
        //}
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
        if( _ballSpeed > 0.01f )
        {
            Vector3 _velVector = _rigidbody.velocity * _ballRotSpeed;
            Vector3 _newrot = new Vector3(_velVector.x + _ballSphere.transform.rotation.x, _velVector.y + _ballSphere.transform.rotation.y,
                _velVector.z + _ballSphere.transform.rotation.z);

            //_ballSphere.transform.rotation = Quaternion.Euler(_newrot);
            
            Debug.Log($"new rotation is {_newrot}");
        }

        if (_ballSpeed < 2f && _ballSpeed > 0.01f)
        {

            //_rigidbody.drag = 5;
            if(_rigidbody.drag < 5f)
            {
                _rigidbody.drag += Time.deltaTime;
                _rigidbody.angularDrag += Time.deltaTime;

            }
            
        }
        else
        {
            //let it slide
            _rigidbody.drag = 0.75f;
            _rigidbody.angularDrag = 0.75f;


        }


        //Ball Rotation while moving
        //Vector3 movementDirection = _rigidbody.velocity;
        //Vector3 _currentRot = new Vector3( transform.rotation.x, transform.rotation.y, transform.rotation.z);
        //_rigidbody.transform.rotation = Quaternion.Euler(_currentRot+ movementDirection.normalized*10);

        //// Check if there is some movement to avoid unnecessary rotation
        //if (movementDirection != Vector3.zero)
        //{
        //    movementDirection.y = 0;
        //    movementDirection.z = 0;
        //    movementDirection.x = _rigidbody.transform.rotation.x + 5f * _rigidbody.velocity.magnitude;
        //    // Calculate the target rotation to face the movement direction
        //    Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

        //    // Apply the rotation to the Rigidbody
        //    _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
        //}
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "gatePost":
                PlaySfx(_ballHitPost);
                break;

            case "Player":
                other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody _rbp);

                if(_rbp != null && _rbp.velocity.magnitude > 1f)
                {
                    Vector3 collisionPoint = other.GetContact(0).point;
                    Vector3 ballPosition = transform.position;
                    Vector3 ballToPuckVector = collisionPoint - ballPosition;
                    Vector3 perpendicular = Vector3.Cross(_rbp.velocity, ballToPuckVector).normalized;
                    Vector3 median = (_rbp.velocity + ballToPuckVector);

                    //_rigidbody.AddRelativeTorque(median * _torqueMultiplier, ForceMode.Impulse);
                    Debug.Log($"Torque Added {median}");
                }
                break;
            case "Opponent":
                other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody _rbOp);

                if (_rbOp != null && _rbOp.velocity.magnitude > 1f)
                {
                    Vector3 collisionPoint = other.GetContact(0).point;
                    Vector3 ballPosition = transform.position;
                    Vector3 ballToPuckVector = collisionPoint - ballPosition;
                    Vector3 perpendicular = Vector3.Cross(_rbOp.velocity, ballToPuckVector).normalized;
                    Vector3 median = (_rbOp.velocity + ballToPuckVector);


                    //_rigidbody.AddRelativeTorque(median * _torqueMultiplier, ForceMode.Impulse);
                    Debug.Log($"Torque Added {median}");
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "opponentGoalTrigger":
                StartCoroutine(_gameController.GetComponent<GlobalGameManager>().managePostGoal("Player"));
                if(_shootingPuck.gameObject.tag == "Player")
                {
                    Debug.Log($"Shooting Puck Tag is {_shootingPuck.gameObject.tag}");
                    _shootingPuck?.OnGoal();
                }
                
                break;

            case "playerGoalTrigger":
                StartCoroutine(_gameController.GetComponent<GlobalGameManager>().managePostGoal("Opponent"));
                if (_shootingPuck.gameObject.tag == "Opponent")
                {
                    Debug.Log($"Shooting Puck Tag is {_shootingPuck.gameObject.tag}");
                    _shootingPuck?.OnGoal();
                }
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