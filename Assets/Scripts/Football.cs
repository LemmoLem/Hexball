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
}
