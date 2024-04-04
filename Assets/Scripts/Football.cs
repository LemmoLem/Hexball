using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Football : MonoBehaviour
{
    // so this is the football class
    // it will be passed into gamemanager
    // it will have variables like current player in possession
    // what tile on 
    // i will need a button for passing - rn it will just be click a tile and ball moves there
    private HexTile hextile;
    private Player player;
    public GameManager gameManager;
    private List<int[]> previousStates = new List<int[]>();
    private List<Player> previousPlayers = new List<Player>();
    private int[] coordinates = new int[2];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(Player name)
    {
        this.player = name;
    }

    public Player GetPlayer()
    {
        return this.player;
    }

    public int[] PopLastState()
    {
        int[] lastState = null;
        if (previousStates.Count > 0) { 
            lastState = previousStates[previousStates.Count - 1];
            previousStates.Remove(lastState);
        }
        return lastState;
    }
    public void AddPrevious(int[] coordinate, Player p)
    {
        previousStates.Add(coordinate);
        previousPlayers.Add(p);
    }

    public Player PopLastPlayer()
    {
        Player lastState = null;
        if (previousPlayers.Count > 0)
        {
            lastState = previousPlayers[previousPlayers.Count - 1];
            previousPlayers.Remove(lastState);
        }
        return lastState;
    }
    public Player GetLastPlayer()
    {
        return previousPlayers[previousPlayers.Count - 1];
    }

    public int[] GetFirstFootballPos()
    {
        return previousStates[0];
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
    }
}
