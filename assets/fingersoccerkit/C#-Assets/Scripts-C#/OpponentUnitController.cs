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