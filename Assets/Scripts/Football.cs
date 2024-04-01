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
    private List<Football> previousStates = new List<Football>();

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

    public Football PopLastState()
    {
        Football lastState = null;
        lastState = previousStates[previousStates.Count - 1];
        previousStates.Remove(lastState);
        return lastState;
    }
    public void AddPreviousState(Football fball)
    {
        Football fballClone = Instantiate(fball);
        previousStates.Add(fballClone   );
    }
}
