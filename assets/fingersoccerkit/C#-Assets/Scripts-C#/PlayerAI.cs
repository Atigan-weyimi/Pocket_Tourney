using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public static GameObject[] playerTeam; //array of all player-1 units
    public static int playerFormation; //player-1 formation
    public static int playerFlag; //player-1 team flag

    //for two player game
    public static GameObject[] player2Team; //array of all player-2 units
    public static int player2Formation; //player-2 formation
    public static int player2Flag; //player-2 team flag

    /// *************************************************************************///
    /// Main Player AI class.
    /// why do we need a player AI class?
    /// This class has a reference to all player-1 (and player-2) units, their formation and their position
    /// and will be used to setup new formations for these units at the start of the game or when a goal happens.
    /// *************************************************************************///
    public Texture2D[] availableFlags; //array of all available teams

    //flags
    internal bool canChangeFormation;

    //*****************************************************************************
    // Init. 
    //*****************************************************************************
    private void Awake()
    {
        canChangeFormation =
            true; //we just change the formation at the start of the game. No more change of formation afterwards!

        //fetch player_1's formation
        if (PlayerPrefs.HasKey("PlayerFormation"))
            playerFormation = PlayerPrefs.GetInt("PlayerFormation");
        else
            playerFormation = 0; //Default Formation

        //fetch player_1's flag
        if (PlayerPrefs.HasKey("PlayerFlag"))
            playerFlag = PlayerPrefs.GetInt("PlayerFlag");
        else
            playerFlag = 0; //Default team

        //cache all player_1 units
        playerTeam = GameObject.FindGameObjectsWithTag("Player");
        //debug
        var i = 1;
        foreach (var unit in playerTeam)
        {
            //Optional
            unit.name = "PlayerUnit-" + i;
            unit.GetComponent<PlayerController>().unitIndex = i;
            unit.GetComponent<Renderer>().material.mainTexture = availableFlags[playerFlag];
            i++;
        }

        //if this is a 2-player local game
        if (GlobalGameManager.gameMode == 1)
        {
            //fetch player_2's formation
            if (PlayerPrefs.HasKey("Player2Formation"))
                player2Formation = PlayerPrefs.GetInt("Player2Formation");
            else
                player2Formation = 0; //Default Formation

            //fetch player_2's flag
            if (PlayerPrefs.HasKey("Player2Flag"))
                player2Flag = PlayerPrefs.GetInt("Player2Flag");
            else
                player2Flag = 0; //Default team

            //cache all player_2 units
            player2Team = GameObject.FindGameObjectsWithTag("Player_2");
            var j = 1;
            foreach (var unit in player2Team)
            {
                //Optional
                unit.name = "Player2Unit-" + j;
                unit.GetComponent<PlayerController>().unitIndex = j;
                unit.GetComponent<Renderer>().material.mainTexture = availableFlags[player2Flag];
                j++;
            }
        }
    }


    private void Start()
    {
        StartCoroutine(changeFormation(playerTeam, playerFormation, 1, 1));
        //For two-player mode,
        if (GlobalGameManager.gameMode == 1)
            StartCoroutine(changeFormation(player2Team, player2Formation, 1, -1));

        canChangeFormation = false;
    }

    //*****************************************************************************
    // changeFormation function take all units, selected formation and side of the player (left half or right half)
    // and then position each unit on it's destination.
    // speed is used to fasten the translation of units to their destinations.
    //*****************************************************************************
    public IEnumerator changeFormation(GameObject[] _team, int _formationIndex, float _speed, int _dir)
    {
        //cache the initial position of all units
        var unitsSartingPosition = new List<Vector3>();
        foreach (var unit in _team)
        {
            unitsSartingPosition.Add(unit.transform.position); //get the initial postion of this unit for later use.
            unit.GetComponent<MeshCollider>().enabled =
                false; //no collision for this unit till we are done with re positioning.
        }

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * _speed;
            for (var cnt = 0; cnt < _team.Length; cnt++)
                _team[cnt].transform.position = new Vector3(Mathf.SmoothStep(unitsSartingPosition[cnt].x,
                        FormationManager.getPositionInFormation(_formationIndex, cnt).x * _dir,
                        t),
                    Mathf.SmoothStep(unitsSartingPosition[cnt].y,
                        FormationManager.getPositionInFormation(_formationIndex, cnt).y,
                        t),
                    FormationManager.fixedZ);
            /*
                _team[cnt].transform.position.x = Mathf.SmoothStep(	unitsSartingPosition[cnt].x, 
                                                                    FormationManager.getPositionInFormation(_formationIndex, cnt).x * _dir,
                                                                    t);
                _team[cnt].transform.position.y = Mathf.SmoothStep(	unitsSartingPosition[cnt].y, 
                                                                    FormationManager.getPositionInFormation(_formationIndex, cnt).y,
                                                                    t);															
                _team[cnt].transform.position.z = FormationManager.fixedZ; //always fixed on -0.5f
                */
            yield return 0;
        }

        if (t >= 1)
        {
            canChangeFormation = true;
            foreach (var unit in _team)
                unit.GetComponent<MeshCollider>().enabled = true; //collision is now enabled.
        }
    }
}