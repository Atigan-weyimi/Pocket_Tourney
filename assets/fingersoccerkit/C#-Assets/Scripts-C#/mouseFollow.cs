using UnityEngine;

public class mouseFollow : MonoBehaviour
{
	/// *************************************************************************///
	/// Mouse Follower class.
	/// mouseHelperBegin is a helper gameObject which always follows mouse position
	/// and provides useful informations for various controllers.
	/// This script also works fine with touch.
	/// *************************************************************************///
	private readonly float zOffset = -0.5f; //fixed position on Z axis.

    private Vector3 tmpPosition;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x,
            transform.position.y,
            zOffset);
        //transform.position.z = zOffset; //apply fixed offset
    }

    private void Update()
    {
        //get mouse position in game scene.
        tmpPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y,
            10));
        //follow the mouse
        transform.position = new Vector3(tmpPosition.x,
            tmpPosition.y,
            zOffset);
    }
}