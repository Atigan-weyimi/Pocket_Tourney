using UnityEngine;

public class OpponentUnitController : Puck
{
    public AudioClip unitsBallHit; //units hits the ball sfx

    /// *************************************************************************///
    /// Unit controller class for AI units
    /// *************************************************************************///
    internal int unitIndex; //every AI unit has an index. this is for the AI controller to know which unit must be selected.
    //Indexes are given to units by the AIController itself.

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "ball":
                PlaySfx(unitsBallHit);
                if (BallManager.instance._shootingPuck == this)
                {
                    Debug.Log($"Opponent Shooting Puck Hit the ball");
                    BallManager.instance._shooterHitTheBall = true;

                }
                break;
            case "Player":
                PlaySfx(unitsBallHit);
                if (BallManager.instance._shootingPuck == other.gameObject.GetComponent<Puck>())
                {
                    Debug.Log($"Shooting Puck Hitted me");
                    //Check if the hit was intentional or not

                    if(BallManager.instance._shooterHitTheBall )
                    {
                        //Shooting puck first hit the ball then collided with us... Totally unintentional :)

                    }
                    else
                    {
                        //Hitted me intentionally how dare you >:(
                        OnReaction(_angryEmojie);
                    }

                }
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