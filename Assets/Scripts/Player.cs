using System.Collections;
using System.Collections.Generic;
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
}
