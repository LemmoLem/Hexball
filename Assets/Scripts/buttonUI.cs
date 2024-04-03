using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonUI : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        // so get 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MovePlayer()
    {
        // this should tell gamemanager that player wants to move a player, 
        // so when clicked alert gamemanager -> gamemanager will save this somewhere 
        // and then when next tile is clicked the player will be moved to that tile and gamemanager update button clicked
        gameManager.SetPlayerWantsToMove();
    }
    public void RotatePlayer()
    {
        gameManager.SetPlayerWantsToRotate();
        // so will have to create sub buttons or some method
        // will need to be able to rotate players 60, 120 or 180 degrees in either direction (60 degrees out of 360)
        // using transform.rotation.z
    }

    public void ControlBall() 
    {
        gameManager.SetPlayerControlBall();
        // change move player to check if player moving is also player who is in control  of the ball
        // and then change where the ball is in that case
    }

    public void PassBall()
    {
        // so make sure to change player controll ball of this
        // this will require player clicking button and then clicking tile 
        // and the then the ball will move to the tile clicked
        gameManager.SetPassBall();

    }

    public void EndTurn()
    {
        gameManager.EndPlayersTurn();
    }

    public void GoBackPhase()
    {
        gameManager.GoBackPhase();
    }

    public void CancelAction()
    {
        gameManager.CancelAction();
    }
    public void ConfirmRotateAction()
    {
        gameManager.ConfirmRotateAction();
    }
    public void WaitAPhase()
    {
        gameManager.WaitAPhase();
    }
}
