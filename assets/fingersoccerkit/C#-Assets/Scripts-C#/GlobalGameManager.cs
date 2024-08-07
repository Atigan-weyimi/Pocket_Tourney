using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalGameManager : MonoBehaviour
{
	/// *************************************************************************///
	/// Main Game Controller.
	/// This class controls main aspects of the game like rounds, levels, scores and ...
	/// Please note that the game always happens between 2 player: (Player-1 vs Player-2) or (Player-1 vs AI)
	/// Player-2 and AI are the same in some aspects like when they got their turns, but they use different controllers.
	/// Player-2 uses a similar controller as Player-1, while AI uses an artificial intelligent routine to play the game.
	/// 
	/// Important! All units and ball object inside the game should be fixed at Z=-0.5f positon at all times. 
	/// You can do this with RigidBody's freeze position.
	/// *************************************************************************///
	public static string player1Name = "Player_1";

    public static string player2Name = "Player_2";
    public static string cpuName = "CPU";

    // Available Game Modes:
    /*
    Indexes:
    0 = 1 player against cpu (normal or Tournament)
    1 = 2 player against each other on the same platform/device
    */
    public static int gameMode;

    //Odd rounds are player (Player-1) turn and Even rounds are AI (Player-2)'s
    public static int round;

    //available time to think and shoot
    public static float baseShootTime = 15; //fixed shoot time for all players and AI

    //mamixmu distance that players can drag away from selected unit to shoot the ball (is in direct relation with shoot power)
    public static float maxDistance = 3.0f;

    //Turns in flags
    public static bool playersTurn;
    public static bool opponentsTurn;

    //After players did their shoots, the round changes after this amount of time.
    public static float timeStepToAdvanceRound = 3;

    //Special occasions
    public static bool goalHappened;
    public static bool shootHappened;
    public static bool gameIsFinished;
    public static int goalLimit = 5; //To finish the game quickly, without letting the GameTime end.

    ///Game timer vars
    public static float gameTimer; //in seconds

    public static float gameTime; //Main game timer (in seconds). Always fixed.

    //*****************************************************************************
    // This function gives turn to players in the game.
    //*****************************************************************************
    public static string whosTurn;

    //Game Status
    private static int playerGoals;
    private static int opponentGoals;

    public GameObject[] eng_text = new GameObject[10];
    public GameObject[] arab_text = new GameObject[10];
    public GameObject p1TimeBar;
    public GameObject p2TimeBar;
    public GameObject goalPlane;

    //AudioClips
    public AudioClip startWistle;
    public AudioClip finishWistle;
    public AudioClip[] goalSfx;
    public AudioClip[] goalHappenedSfx;
    public AudioClip[] crowdChants;

    //Public references
    public GameObject gameStatusPlane; //user to show win/lose result at the end of match
    public GameObject continueTournamentBtn; //special tournament button to advance in tournament incase of win
    public GameObject statusTextureObject;
    public GameObject statusTexture_arab; //plane we use to show the result texture in 3d world
    public Texture2D[] statusModes;
    public Texture2D[] statusModes_arab; //Available status textures

    //***************************************************************************//
    // Game status manager
    //***************************************************************************//
    public GameObject timeText; //UI 3d text object
    public GameObject playerGoalsText; //UI 3d text object
    public GameObject opponentGoalsText; //UI 3d text object
    public GameObject playerOneName; //UI 3d text object
    public GameObject playerTwoName; //UI 3d text object
    private float p1ShootTime; //additional time (based on the selected team) for p1
    private float p2ShootTime; //additional time (based on the selected team) for p2 or AI
    private float p1TimeBarInitScale;
    private float p1TimeBarCurrentScale;
    private float p2TimeBarInitScale;
    private float p2TimeBarCurrentScale;
    private string remainingTime;
    private int seconds;
    private int minutes;

    //gameObject references
    [SerializeField] private PlayerTeam player1Team;
    [SerializeField] private PlayerTeam player2Team;
    [SerializeField] private OpponentAI aiTeam;
    [SerializeField] private GameArea gameArea;
    private GameObject opponentAIController;
    private BallManager ball;
    private bool canPlayCrowdChants;
    private bool passHappened;


    //*****************************************************************************
    // We have all units inside the game scene by default, but at the start of the game,
    // we check which side in playing (should be active) and deactive the side that is
    // not playing by deactivating all it's units.
    //*****************************************************************************
    private GameObject[] cpuTeam; //array of all AI units in the game

    //*****************************************************************************
    // Init. 
    //*****************************************************************************
    private void Awake()
    {
        //init
        goalHappened = false;
        shootHappened = false;
        gameIsFinished = false;
        playerGoals = 0;
        opponentGoals = 0;
        gameTime = 0;
        round = 1;
        seconds = 0;
        minutes = 0;
        canPlayCrowdChants = true;

        //get additonal time for each player and AI
        p1ShootTime = baseShootTime + TeamsManager.getTeamSettings(PlayerPrefs.GetInt("PlayerFlag")).y;
        p2ShootTime = baseShootTime + TeamsManager.getTeamSettings(PlayerPrefs.GetInt("Player2Flag")).y;
        print("P1 shoot time: " + p1ShootTime + " // " + "P2 shoot time: " + p2ShootTime);

        //hide gameStatusPlane
        gameStatusPlane.SetActive(false);
        continueTournamentBtn.SetActive(false);

        //Translate gameTimer index to actual seconds
        switch (PlayerPrefs.GetInt("GameTime"))
        {
            case 0:
                gameTimer = 180;
                break;
            case 1:
                gameTimer = 300;
                break;
            case 2:
                gameTimer = 480;
                break;

            //You can add more cases and options here.
        }

        //fill player shoot timer to full
        p1TimeBarInitScale = p1TimeBar.transform.localScale.x;
        p1TimeBarCurrentScale = p1TimeBar.transform.localScale.x;
        p2TimeBarInitScale = p2TimeBar.transform.localScale.x;
        p2TimeBarCurrentScale = p2TimeBar.transform.localScale.x;
        p1TimeBar.transform.localScale = new Vector3(1, 1, 1);
        p2TimeBar.transform.localScale = new Vector3(1, 1, 1);

        //Get Game Mode
        if (PlayerPrefs.HasKey("GameMode"))
            gameMode = PlayerPrefs.GetInt("GameMode");
        else
            gameMode = 0; // Deafault Mode (Player-1 vs AI)

        opponentAIController = GameObject.FindGameObjectWithTag("opponentAI");
        ball = GameObject.FindGameObjectWithTag("ball").GetComponent<BallManager>();
    }

    private void manageGameModes()
    {
        switch (gameMode)
        {
            case 0:
                //find and deactive all player2 units. This is player-1 vs AI.
                player2Team.enabled = false;
                break;

            case 1:
                //find and deactive all AI Opponent units. This is Player-1 vs Player-2.
                cpuTeam = GameObject.FindGameObjectsWithTag("Opponent");
                foreach (var unit in cpuTeam)
                {
                    unit.SetActive(false);
                }
                //deactive opponent's AI
                opponentAIController.SetActive(false);
                break;
        }
    }

    private IEnumerator Start()
    {
        manageGameModes();
        StartTurn();
        yield return new WaitForSeconds(1.5f);
        playSfx(startWistle);
    }

    private void OnEnable()
    {
        player1Team.PassHapenned += PassHappenned;
    }

    private void OnDisable()
    {
        player1Team.PassHapenned -= PassHappenned;
    }

    private void PassHappenned()
    {
        passHappened = true;
    }

    //*****************************************************************************
    // FSM
    //*****************************************************************************
    private void Update()
    {
        language();
        //check game finish status every frame
        if (!gameIsFinished)
        {
            manageGameStatus();
            updateTimeBars();
        }

        //every now and then, play some crowd chants
        StartCoroutine(playCrowdChants());

        //If you ever needed debug inforamtions:
        //print("GameRound: " + round + " & turn is for: " + whosTurn + " and GoalHappened is: " + goalHappened);
    }

    //*****************************************************************************
    // language option
    //*****************************************************************************
    private void language()
    {
        if (pubR.language_option == 0)
        {
            for (var i = 0; i < eng_text.Length; i++) eng_text[i].SetActive(true);
            for (var i = 0; i < arab_text.Length; i++) arab_text[i].SetActive(false);
        }
        else
        {
            for (var i = 0; i < eng_text.Length; i++) eng_text[i].SetActive(false);
            for (var i = 0; i < arab_text.Length; i++) arab_text[i].SetActive(true);
            if (gameMode == 0)
                arab_text[2].SetActive(false);
            else if (gameMode == 1) arab_text[1].SetActive(false);
        }
    }

    private void StartTurn()
    {
        if (gameIsFinished || goalHappened)
            return;

        //reset shootHappened flag
        shootHappened = false;

        //fill time limit bars
        fillTimeBar(1);
        fillTimeBar(2);

        //if round number is odd, it's players turn, else it's AI or player-2 's turn
        if (!passHappened)
        {
            round++;
        }

        passHappened = false;
        
        var carry = round % 2;
        if (carry == 0)
        {
            playersTurn = true;
            opponentsTurn = false;
            
            player1Team.SetCanShoot(true);

            if (gameMode == 0)
            {
                aiTeam.SetCanShoot(false);
            }
            else
            {
                player2Team.SetCanShoot(false);
                player2Team.CancelTurn();
            }
            
            whosTurn = "player";
        }
        else
        {
            playersTurn = false;
            opponentsTurn = true;
            player1Team.SetCanShoot(false);
            player1Team.CancelTurn();
            
            if (gameMode == 0)
            {
                aiTeam.SetCanShoot(true);
                StartCoroutine(aiTeam.Shoot());
            }
            else
            {
                player2Team.SetCanShoot(true);
            }
            
            whosTurn = "opponent";
        }
    }


    //*****************************************************************************
    // Update timebars by changing their scale (x) over time.
    // If time ends, the turn will be transforred to opponent.
    //*****************************************************************************
    private void updateTimeBars()
    {
        if (gameIsFinished || goalHappened || shootHappened)
            return;

        //limiters and turn change
        //Also change turns incase of time running out!
        if (p1TimeBarCurrentScale <= 0)
        {
            p1TimeBarCurrentScale = 0;
            setNewRound(1);
            return;
        }

        if (p2TimeBarCurrentScale <= 0)
        {
            p2TimeBarCurrentScale = 0;
            setNewRound(2);
            return;
        }


        if (playersTurn)
        {
            p1TimeBarCurrentScale -= Time.deltaTime / p1ShootTime;
            p1TimeBar.transform.localScale = new Vector3(p1TimeBarCurrentScale, p1TimeBar.transform.localScale.y,
                p1TimeBar.transform.localScale.z);
            fillTimeBar(2);
        }
        else
        {
            p2TimeBarCurrentScale -= Time.deltaTime / p2ShootTime;
            p2TimeBar.transform.localScale = new Vector3(p2TimeBarCurrentScale, p2TimeBar.transform.localScale.y,
                p2TimeBar.transform.localScale.z);
            fillTimeBar(1);
        }
    }


    private void fillTimeBar(int _ID)
    {
        if (_ID == 1)
        {
            p1TimeBarCurrentScale = p1TimeBarInitScale;
            p1TimeBar.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            p2TimeBarCurrentScale = p2TimeBarInitScale;
            p2TimeBar.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void setNewRound(int _ID)
    {
        switch (_ID)
        {
            case 1:
                round = 2;
                break;
            case 2:
                round = 1;
                break;
        }

        StartTurn();
    }

    //*****************************************************************************
    // What happens after a shoot is performed?
    //*****************************************************************************
    public IEnumerator managePostShoot()
    {
        shootHappened = true;

        yield return new WaitForSeconds(1f);
        while (true)
        {
            var someMove = player1Team.HasMovingPuck || player2Team.HasMovingPuck || aiTeam.HasMovingPuck || ball.IsMoving;
            if (someMove)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                break;
            }
        }

        if (goalHappened)
        {
            yield break;
        }
        
        StartTurn(); //cycle again between players
    }

    //*****************************************************************************
    // If we had a goal in this round, this is the function that manages all aspects of it.
    //*****************************************************************************								
    public IEnumerator managePostGoal(string _goalBy)
    {
        //get who did the goal.

        //avoid counting a goal as two or more
        if (goalHappened)
            yield break;

        //soft pause the game for reformation and other things...
        goalHappened = true;
        shootHappened = false;
        
        player1Team.CancelTurn();
        player2Team.CancelTurn();
        
        ball.Velocity = Vector3.zero;
        ball.IsKinematic = true;
        ball.CollisionEnabled = false;

        //add to goal counters
        switch (_goalBy)
        {
            case "Player":
                playerGoals++;
                round = 2; //goal by player-1 and opponent should start the next round
                break;
            case "Opponent":
                opponentGoals++;
                round = 1; //goal by opponent and player-1 should start the next round
                break;
        }

        //wait a few seconds to show the effects , and physics cooldown
        playSfx(goalSfx[Random.Range(0, goalSfx.Length)]);
        GetComponent<AudioSource>().PlayOneShot(goalHappenedSfx[Random.Range(0, goalHappenedSfx.Length)], 1);
        //yield return new WaitForSeconds(1);
        //activate the goal event plane
        GameObject gp = null;
        gp = Instantiate(goalPlane, new Vector3(30, 0, -2), Quaternion.Euler(0, 180, 0));
        float t = 0;
        var speed = 2.0f;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            gp.transform.position = new Vector3(Mathf.SmoothStep(30, 0, t), 0, -2);
            yield return 0;
        }

        yield return new WaitForSeconds(0.75f);
        float t2 = 0;
        while (t2 < 1)
        {
            t2 += Time.deltaTime * speed;
            gp.transform.position = new Vector3(Mathf.SmoothStep(0, -30, t2), 0, -2);
            yield return 0;
        }

        Destroy(gp, 1.5f);

        //bring the ball back to it's initial position
        ball.GetComponent<TrailRenderer>().enabled = false;
        ball.Detach();
        
        ball.transform.position = new Vector3(0, 0, -0.5f);
        ball.CollisionEnabled = true;
        ball.IsKinematic = false;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ball.Velocity = Vector3.zero;
        
        yield return new WaitForSeconds(1);
        ball.GetComponent<TrailRenderer>().enabled = true;

        //*** reformation of units ***//
        //Reformation for player_1
        StartCoroutine(player1Team.ChangeFormation(PlayerPrefs.GetInt("PlayerFormation"), 0.6f));

        //if this is player-1 vs player-2 match:
        if (gameMode == 1)
        {
            StartCoroutine(player2Team.ChangeFormation(PlayerPrefs.GetInt("Player2Formation"), 0.6f));
        }
        else
        {
            //if this is player-1 vs AI match:
            //get a new random formation everytime
            StartCoroutine(opponentAIController.GetComponent<OpponentAI>()
                .ChangeFormation(Random.Range(0, FormationManager.formations), 0.6f));
        } 

        yield return new WaitForSeconds(3);

        //check if the game is finished or not
        if (playerGoals > goalLimit || opponentGoals > goalLimit)
        {
            gameIsFinished = true;
            manageGameFinishState();
            yield break;
        }

        //else, continue to the next round
        goalHappened = false;
        StartTurn();
        playSfx(startWistle);
    }

    private void manageGameStatus()
    {
        seconds = Mathf.CeilToInt(gameTimer - Time.timeSinceLevelLoad) % 60;
        minutes = Mathf.CeilToInt(gameTimer - Time.timeSinceLevelLoad) / 60;

        if (seconds == 0 && minutes == 0)
        {
            gameIsFinished = true;
            manageGameFinishState();
        }

        remainingTime = string.Format("{0:00} : {1:00}", minutes, seconds);
        timeText.GetComponent<TextMesh>().text = remainingTime;

        playerGoalsText.GetComponent<TextMesh>().text = playerGoals.ToString();
        opponentGoalsText.GetComponent<TextMesh>().text = opponentGoals.ToString();

        if (gameMode == 0)
        {
            playerOneName.GetComponent<TextMesh>().text = player1Name;
            playerTwoName.GetComponent<TextMesh>().text = cpuName;
        }
        else if (gameMode == 1)
        {
            playerOneName.GetComponent<TextMesh>().text = player1Name;
            playerTwoName.GetComponent<TextMesh>().text = player2Name;
        }
    }

    //*****************************************************************************
    // After the game is finished, this function handles the events.
    //*****************************************************************************
    private void manageGameFinishState()
    {
        //Play gameFinish wistle
        playSfx(finishWistle);
        print("GAME IS FINISHED.");

        //show gameStatusPlane
        gameStatusPlane.SetActive(true);

        //for single player game, we should give the player some bonuses in case of winning the match
        if (gameMode == 0)
        {
            if (playerGoals > goalLimit || playerGoals > opponentGoals)
            {
                print("Player 1 is the winner!!");

                //set the result texture
                statusTextureObject.GetComponent<Renderer>().material.mainTexture = statusModes[0];
                statusTexture_arab.GetComponent<Renderer>().material.mainTexture = statusModes_arab[0];

                var playerWins = PlayerPrefs.GetInt("PlayerWins");
                var playerMoney = PlayerPrefs.GetInt("PlayerMoney");

                PlayerPrefs.SetInt("PlayerWins", ++playerWins); //add to wins counter
                PlayerPrefs.SetInt("PlayerMoney", playerMoney + 100); //handful of coins as the prize!

                //if this is a tournament match, update it with win state and advance.
                if (PlayerPrefs.GetInt("IsTournament") == 1)
                {
                    PlayerPrefs.SetInt("TorunamentMatchResult", 1);
                    PlayerPrefs.SetInt("TorunamentLevel", PlayerPrefs.GetInt("TorunamentLevel", 0) + 1);
                    continueTournamentBtn.SetActive(true);
                }
            }
            else if (opponentGoals > goalLimit || opponentGoals > playerGoals)
            {
                print("CPU is the winner!!");
                statusTextureObject.GetComponent<Renderer>().material.mainTexture = statusModes[1];
                statusTexture_arab.GetComponent<Renderer>().material.mainTexture = statusModes_arab[1];

                //if this is a tournament match, update it with lose state.
                if (PlayerPrefs.GetInt("IsTournament") == 1)
                {
                    PlayerPrefs.SetInt("TorunamentMatchResult", 0);
                    PlayerPrefs.SetInt("TorunamentLevel", PlayerPrefs.GetInt("TorunamentLevel", 0) + 1);
                    continueTournamentBtn.SetActive(true);
                }
            }
            else if (opponentGoals == playerGoals)
            {
                print("(Single Player) We have a Draw!");
                statusTextureObject.GetComponent<Renderer>().material.mainTexture = statusModes[4];
                statusTexture_arab.GetComponent<Renderer>().material.mainTexture = statusModes_arab[4];

                //we count "draw" a lose in tournament mode.
                //if this is a tournament match, update it with lose state.
                if (PlayerPrefs.GetInt("IsTournament") == 1)
                {
                    PlayerPrefs.SetInt("TorunamentMatchResult", 0);
                    PlayerPrefs.SetInt("TorunamentLevel", PlayerPrefs.GetInt("TorunamentLevel", 0) + 1);
                    continueTournamentBtn.SetActive(true);
                }
            }
        }
        else if (gameMode == 1)
        {
            if (playerGoals > opponentGoals)
            {
                print("Player 1 is the winner!!");
                statusTextureObject.GetComponent<Renderer>().material.mainTexture = statusModes[2];
                statusTexture_arab.GetComponent<Renderer>().material.mainTexture = statusModes_arab[2];
            }
            else if (playerGoals == opponentGoals)
            {
                print("(Two-Player) We have a Draw!");
                statusTextureObject.GetComponent<Renderer>().material.mainTexture = statusModes[4];
                statusTexture_arab.GetComponent<Renderer>().material.mainTexture = statusModes_arab[4];
            }
            else if (playerGoals < opponentGoals)
            {
                print("Player 2 is the winner!!");
                statusTextureObject.GetComponent<Renderer>().material.mainTexture = statusModes[3];
                statusTexture_arab.GetComponent<Renderer>().material.mainTexture = statusModes_arab[3];
            }
        }

        admobdemo.mInstance.OnInterstitial();
    }

    //*****************************************************************************
    // Play a random crown sfx every now and then to spice up the game
    //*****************************************************************************
    private IEnumerator playCrowdChants()
    {
        if (canPlayCrowdChants)
        {
            canPlayCrowdChants = false;
            GetComponent<AudioSource>().PlayOneShot(crowdChants[Random.Range(0, crowdChants.Length)], 1);
            yield return new WaitForSeconds(Random.Range(15, 35));
            canPlayCrowdChants = true;
        }
    }

    //*****************************************************************************
    // Play sound clips
    //*****************************************************************************
    private void playSfx(AudioClip _clip)
    {
        GetComponent<AudioSource>().clip = _clip;
        if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
    }
}