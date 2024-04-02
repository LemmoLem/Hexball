using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    // so contains stats for the player like speed, passing etc
    // boolean maybe for in control of ball or isGoalkeeper? - probs seperate class for keeper
    //
    //
    // Start is called before the first frame update
    private Team team;
    private int rotation;
    private ArrayList actions;
    private List<Player> previousStates = new List<Player>();
    private int[] coordinates = new int[2];
    private bool lastActionUsedBall = false;
    private int id;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetTeam(Team t)
    {
        team = t;
    }
    public Team GetTeam() 
    { 
        return team; 
    }
    public int GetRotation()
    {
        return rotation;
    }
    public void SetRotation(int r)
    {
        rotation = r;
    }
    public int[] GetCoordinates()
    {
        Debug.Log(coordinates[0] + " " + coordinates[1]);
        return coordinates;
    }
    public void SetCoordinates(int[] coords)
    {
        coordinates[0] = coords[0];
        coordinates[1] = coords[1];
        Debug.Log("inside player"+coordinates[0] + " " + coordinates[1]);

    }


    public Player PopLastState()
    {
        Player lastState = null;
        if (previousStates.Count > 0)
        {
            lastState = previousStates[previousStates.Count - 1];
            previousStates.Remove(lastState);
        }
        return lastState;
    }
    public void AddPreviousState(Player player, HexTile hex)
    {
        Player playerClone = Instantiate(player, hex.transform);
        previousStates.Add(playerClone);
        playerClone.gameObject.SetActive(false);
        playerClone.gameObject.transform.localPosition = new Vector3(0, 0, -1);
        Debug.Log(playerClone.GetCoordinates()[0]);
        playerClone.SetCoordinates(coordinates);
        playerClone.SetTeam(team);
        playerClone.SetRotation(rotation);
        playerClone.SetId(id);
        // these ones probs need to be done with proper methods
        playerClone.actions = actions;
        playerClone.previousStates = previousStates;
        playerClone.SetLastActionUsedBall(lastActionUsedBall);
    }

    // should be called after add previous state
    public Player GetLastPlayer()
    {
        return previousStates[previousStates.Count - 1];
    }

    public bool CheckLastActionWasBall()
    {
        return lastActionUsedBall;
    }
    public void SetLastActionUsedBall(bool lastAction)
    {
        lastActionUsedBall = lastAction;
    }

    public void SetId(int num)
    {
        id = num;
    }
    public int GetId()
    {
        return id;
    }
}
