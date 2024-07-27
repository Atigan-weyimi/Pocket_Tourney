﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    [SerializeField] private string _formationPrefsKey;
    [SerializeField] private string _flagPrefsKey;
    [SerializeField] private string _pucksTag;
    [SerializeField] private string _puckNamePrefix;
    [SerializeField] private int _direction;
    [SerializeField] private Texture2D[] _availableFlags; //array of all available teams
    
    public static GameObject[] team; //array of all player units
    public static int playerFormation; //player formation
    public static int playerFlag; //player team flag
    
    private List<PlayerController> controllers = new(10);
    private bool _canShoot;
    
    public void SetCanShoot(bool value)
    {
        _canShoot = value;
        foreach (var controller in controllers)
        {
            controller.CanShoot = value;
        }
    }

    //*****************************************************************************
    // Init. 
    //*****************************************************************************
    private void Awake()
    {
        playerFormation = PlayerPrefs.GetInt(_formationPrefsKey);
        playerFlag = PlayerPrefs.GetInt(_flagPrefsKey);
        team = GameObject.FindGameObjectsWithTag(_pucksTag);
        
        for (var i = 0; i < team.Length; i++)
        {
            var unit = team[i];
            unit.name = _puckNamePrefix + i;
            var controller = unit.GetComponent<PlayerController>();
            controller.unitIndex = i;
            controller.SetTexture(_availableFlags[playerFlag]);
            controllers.Add(controller);
        }
    }

    private void OnEnable()
    {
        foreach (var controller in controllers)
        {
            controller.Shot += OnPlayerShot;
            controller.DragStarted += ControllerOnDragStarted;
            controller.DragEnded += ControllerOnDragEnded;
            
            controller.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (var controller in controllers)
        {
            controller.Shot -= OnPlayerShot;
            controller.DragStarted -= ControllerOnDragStarted;
            controller.DragEnded -= ControllerOnDragEnded;
            
            controller.gameObject.SetActive(false);
        }
    }

    private void OnPlayerShot(PlayerController playerController)
    {
        SetCanShoot(false);
    }

    private void ControllerOnDragStarted(PlayerController playerController)
    {
        foreach (var controller in controllers)
        {
            if (controller == playerController)
            {
                continue;
            }

            controller.CanShoot = false;
        }
    }

    private void ControllerOnDragEnded(PlayerController playerController)
    {
        foreach (var controller in controllers)
        {
            controller.CanShoot = _canShoot;
        }
    }

    private void Start()
    {
        StartCoroutine(ChangeFormation(playerFormation, 1));
    }

    //*****************************************************************************
    // changeFormation function take all units, selected formation and side of the player (left half or right half)
    // and then position each unit on it's destination.
    // speed is used to fasten the translation of units to their destinations.
    //*****************************************************************************
    public IEnumerator ChangeFormation(int formationIndex, float speed)
    {
        //cache the initial position of all units
        var unitsSartingPosition = new List<Vector3>();
        var unitsTargetPosition = new List<Vector3>();
        for (int i = 0; i < team.Length; i++)
        {
            var unit = team[i];
            unitsSartingPosition.Add(unit.transform.position); //get the initial postion of this unit for later use.

            var formationPosition = FormationManager.getPositionInFormation(formationIndex, i);
            var targetPosition = new Vector3(formationPosition.x, formationPosition.y * _direction, FormationManager.fixedZ);
            unitsTargetPosition.Add(targetPosition);
            unit.GetComponent<MeshCollider>().enabled = false; //no collision for this unit till we are done with re positioning.
        }

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            for (var cnt = 0; cnt < team.Length; cnt++)
            {
                team[cnt].transform.position = new Vector3(
                    Mathf.SmoothStep(unitsSartingPosition[cnt].x, unitsTargetPosition[cnt].x, t),
                    Mathf.SmoothStep(unitsSartingPosition[cnt].y, unitsTargetPosition[cnt].y, t),
                    FormationManager.fixedZ);
            }
                
            yield return null;
        }

        if (t >= 1)
        {
            foreach (var unit in team)
            {
                unit.GetComponent<MeshCollider>().enabled = true; //collision is now enabled.
            }
        }
    }
}