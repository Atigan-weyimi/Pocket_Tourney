using UnityEngine;

public class playerColliderManager : MonoBehaviour
{
	/// *************************************************************************///
	/// Optional controller for collision of player units vs other items in the scene like ball or opponent units
	/// *************************************************************************///
	public AudioClip unitsBallHit; //units hits the ball sfx

    public AudioClip unitsGeneralHit; //units general hit sfx (Not used)

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Opponent":
                //PlaySfx(unitsGeneralHit);
                if (BallManager.instance._shootingPuck == other.gameObject.GetComponent<Puck>())
                {
                    Debug.Log($"Shooting Puck Hitted me");
                    //Check if the hit was intentional or not

                    if (BallManager.instance._shooterHitTheBall)
                    {
                        //Shooting puck first hit the ball then collided with us... Totally unintentional :)

                    }
                    else
                    {
                        //Hitted me intentionally how dare you >:(
                        Debug.Log($"Intentionally hitted me");
                        var _mypuck = transform.GetComponent<Puck>();
                        _mypuck.OnReaction(_mypuck._angryEmojie);
                    }

                }
                break;
            case "Player":
                //PlaySfx(unitsGeneralHit);

                if(gameObject.tag != "Player")
                {
                    if (BallManager.instance._shootingPuck == other.gameObject.GetComponent<Puck>())
                    {
                        Debug.Log($"Shooting Puck Hitted me");
                        //Check if the hit was intentional or not

                        if (BallManager.instance._shooterHitTheBall)
                        {
                            //Shooting puck first hit the ball then collided with us... Totally unintentional :)

                        }
                        else
                        {
                            //Hitted me intentionally how dare you >:(
                            Debug.Log($"Intentionally hitted me");
                            var _mypuck = transform.GetComponent<Puck>();
                            _mypuck.OnReaction(_mypuck._angryEmojie);
                            
                        }

                    }
                }
                
                break;
            case "Player_2":
                //PlaySfx(unitsGeneralHit);
                if(gameObject.tag != "Player_2")
                {
                    if (BallManager.instance._shootingPuck == other.gameObject.GetComponent<Puck>())
                    {
                        Debug.Log($"Shooting Puck Hitted me");
                        //Check if the hit was intentional or not

                        if (BallManager.instance._shooterHitTheBall)
                        {
                            //Shooting puck first hit the ball then collided with us... Totally unintentional :)

                        }
                        else
                        {
                            //Hitted me intentionally how dare you >:(
                            Debug.Log($"Intentionally hitted me");
                            var _mypuck = transform.GetComponent<Puck>();
                            _mypuck.OnReaction(_mypuck._angryEmojie);
                        }

                    }
                }
                
                break;
            case "ball":

                if(BallManager.instance._shootingPuck == transform.gameObject.GetComponent<Puck>())
                {
                    Debug.Log($"Shooting Puck Hit the ball");
                    BallManager.instance._shooterHitTheBall = true;
                    
                }
                PlaySfx(unitsBallHit);
                break;
        }
    }

    //*****************************************************************************
    // Play sound clips
    //*****************************************************************************
    private void PlaySfx(AudioClip _clip)
    {
        GetComponent<AudioSource>().clip = _clip;
        if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
    }
}