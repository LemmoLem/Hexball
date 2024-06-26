using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditorInternal.VersionControl.ListControl;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    // take in hexagon prefab and pitch size 
    // use hexagon size and loop over pitch size to build pitch
    public Canvas canvas;
    public HexTile hexagon;
    public int pitchWidth, pitchLength, posOfPitch;
    private Dictionary<(int x, int y), HexTile> pitch;
    public GameObject pitchLine, circle, semiCircle, goalLine;
    public Camera camera;
    public Player playerBlue, playerRed;
    public GameObject buttonToMove, buttonToRotate, slider, buttonToControl, buttonToPass, buttonToCancel, buttonUndo, buttonConfirm, buttonWait;
    private List<GameObject> currentButtons = new List<GameObject>();
    private bool playerWantsToMove, playerWantsToRotate, playerWantsToPass = false;
    public Football football;
    private List<HexTile> highLightedTiles = new List<HexTile>();
    public Team player1, player2;
    private Team currentPlayer;
    private bool player1First = true;
    private List<GhostMarker> ghostMarkers = new List<GhostMarker>();
    public GhostMarker ghostMarker;
    private Player selectedPlayer;
    private HexTile currentTile;
    private int turnCount = 0;
    public int actionsPerTurn;


    // Start is called before the first frame update
    void Start()
    {
        CreatePitch();
        CreatePlayers(5);
        currentPlayer = player1;
        // SPAWN BALL - MAKE IT A FUNCTION
        GameObject centre = pitch[((pitchWidth + 1) / 2 - 1, pitchLength -1)].gameObject;
        football.gameObject.transform.parent = centre.transform;
        football.SetCoordinates(pitch[((pitchWidth + 1) / 2 - 1, pitchLength - 1)].GetCoord());
        football.gameObject.transform.localPosition = new Vector3(0, 0, -5);
        //football.gameObject.transform.localScale = new Vector3(0.82f, 0.82f, 0);

    }

    // Update is called once per frame
    void Update()
    {
    }

    // NOTE TO SELF CAMERA AND OTHERR STUFF AND EDITED IN HERE, NEED TO TIDY UP SO THIS PROGRAM SEPERATES STUFF IN TO ACTUAL FUNCTIONS
    void CreatePitch()
    {
        pitch = new Dictionary<(int x, int y), HexTile>();
        Vector3 hexagonBounds = hexagon.GetComponent<SpriteRenderer>().bounds.size;
        hexagonBounds.y = 2 * Mathf.Sqrt(Mathf.Pow(hexagonBounds.x / 2, 2) - Mathf.Pow(hexagonBounds.x / 4, 2));
        // maths for finding height from width - basically cus getting height from looking at bounds which inst great 
        // almost definetley a better way

        // so as layout alternates with one less or one more on outside this alternates, and the offset is - for when outside is less hexes
        
        // bit annoying but to avoid doing checks rows with extra hex also have extra null at the top of them
        for (int i = 0; i < pitchWidth; i++)
        {
            float yOffset = 0.5f * hexagonBounds.y;
            if (pitchWidth % 4 == 3)
            {
                // so this one is for pitch where outside is shorter (raised) (so will be 1,3,5,7 for heights)
                if (i % 2 == 0)
                {
                    for (int j = 0; j < pitchLength-1; j++)
                    {
                        HexTile hex = Instantiate(hexagon, new Vector3((float)(hexagonBounds.x * i * 0.75f), hexagonBounds.y * j, 0), Quaternion.identity, this.transform);
                        hex.SetGameManager(this);
                        pitch[(i,j*2+1)] = hex;
                        hex.SetCoords(i, j*2+1);
                    }
                }
                // so this one is for pitch where outside is shorter (raised) (so will be 0,2,4,6,8 for heights)
                else
                {
                    for (int j = 0; j < pitchLength; j++)
                    {
                        HexTile hex = Instantiate(hexagon, new Vector3((float)(hexagonBounds.x * i * 0.75f), -yOffset + hexagonBounds.y * j, 0), Quaternion.identity, this.transform);
                        hex.SetGameManager(this);
                        pitch[(i, j * 2)] = hex;
                        hex.SetCoords(i, j * 2);
                        

                        //pitch[i][j] = (GameObject)Instantiate(hexagon.gameObject, new Vector3((float)(hexagonBounds.x * i * 0.75f), -yOffset + hexagonBounds.y * j, 0), Quaternion.identity, this.transform);
                    }
                }
            }
            else
            {
                // so this one is for pitch where outside is longere (so will be 0,2,4,6,8 for heights)
                if (i % 2 == 0)
                {
                    for (int j = 0; j < pitchLength; j++)
                    {
                        HexTile hex = Instantiate(hexagon, new Vector3((float)(hexagonBounds.x * i * 0.75f), hexagonBounds.y * j, 0), Quaternion.identity, this.transform);
                        hex.SetGameManager(this);
                        pitch[(i, j * 2)] = hex;
                        hex.SetCoords(i, j * 2);
                        //pitch[i][j] = (GameObject)Instantiate(hexagon.gameObject, new Vector3((float)(hexagonBounds.x * i * 0.75f), hexagonBounds.y * j, 0), Quaternion.identity, this.transform);
                    }
                }
                // so this one is for pitch where outside is longer (so will be 1,3,5,7 for heights)
                else
                {
                    for (int j = 0; j < pitchLength - 1; j++)
                    {
                        HexTile hex = Instantiate(hexagon, new Vector3((float)(hexagonBounds.x * i * 0.75f), yOffset + hexagonBounds.y * j, 0), Quaternion.identity, this.transform);
                        hex.SetGameManager(this);
                        pitch[(i, j * 2 + 1)] = hex;
                        hex.SetCoords(i, j * 2 + 1);
                        //pitch[i][j] = (GameObject)Instantiate(hexagon.gameObject, new Vector3((float)(hexagonBounds.x * i * 0.75f), yOffset + hexagonBounds.y * j, 0), Quaternion.identity, this.transform);
                    }
                }
            }



        }

        // create pitch lines using transform.scale n stuff
        // so start with halfway line, have that go all the way across
        // then go to sides and add squares going vertically
        // then go to top n bottom n copy halfway line to those
        // then for keepers box its in between 13 - 17% of the whole pitch
        // and across its 18 metres so about 20-40%

        //find pos of middle hex

        // reusing this but saving it - can replace yscale in code below with hexagonBounds.y 
        float yScale = 2 * Mathf.Sqrt(Mathf.Pow(hexagonBounds.x / 2, 2) - Mathf.Pow(hexagonBounds.x / 4, 2));

        // make halfway line
        Vector3 centre = pitch[((pitchWidth-1)/2,(pitchLength-1))].transform.position;
        centre.z = centre.z - 0.4f;

        camera.transform.position = centre;
        camera.transform.position = camera.transform.position + new Vector3(0, 0, -20);
        camera.orthographicSize = pitchLength + 4;

        GameObject halfwayLine = Instantiate(pitchLine, centre, Quaternion.identity, this.transform);
        // the 0.25f is so it lines up properly (hard coded again but deal with it cus i hate scaling maths)
        halfwayLine.transform.localScale = new Vector3((pitchWidth * 3 * 0.75f) + 0.25f, 0.5f, 1);

        // make sidelines and goallines
        Vector3 topGoalPos = pitch[((pitchWidth + 1) / 2 - 1,(pitchLength - 1)*2)].transform.position;
        topGoalPos.z = topGoalPos.z - 0.4f;
        GameObject topGoalLine = Instantiate(halfwayLine, topGoalPos, Quaternion.identity, this.transform);

        Vector3 bottomGoalPos = pitch[((pitchWidth + 1) / 2 - 1, 0)].transform.position;
        bottomGoalPos.z = bottomGoalPos.z - 0.4f;
        GameObject bottomGoalLine = Instantiate(halfwayLine, bottomGoalPos, Quaternion.identity, this.transform);

        // work out whether width means it will be higher or lower
        int yTemp = 0;
        if (pitchWidth % 4 == 3)
        {
            yTemp = 1;
        }
        else
        {
            yTemp = 2;
        }

        Vector3 leftCentre = pitch[(0, yTemp)].transform.position;
        leftCentre.y = centre.y;
        leftCentre.z = leftCentre.z - 0.4f;
        leftCentre.x = leftCentre.x - 1.0f;
        GameObject leftLine = Instantiate(pitchLine, leftCentre, Quaternion.identity, this.transform);

        Vector3 rightCentre = pitch[(pitchWidth - 1, yTemp)].transform.position;
        rightCentre.y = centre.y;
        rightCentre.z = rightCentre.z - 0.4f;
        rightCentre.x = rightCentre.x + 1.0f;
        GameObject rightLine = Instantiate(pitchLine, rightCentre, Quaternion.identity, this.transform);

        if (pitchWidth % 4 == 3)
        {
            leftLine.transform.localScale = new Vector3(0.5f, (pitchLength - 1)* yScale, 1);
            rightLine.transform.localScale = new Vector3(0.5f, (pitchLength -1) * yScale, 1);
        }
        else
        {
            leftLine.transform.localScale = new Vector3(0.5f, (pitchLength - 1) * yScale, 1);
            rightLine.transform.localScale = new Vector3(0.5f, (pitchLength - 1) * yScale, 1);
        }

        // create penatly box, centre circle etc, (leave corners to much later as no impact to gameplay)
        // so these will be created relative to size of pitch. 
        // then for keepers box its in between 13 - 17% of the whole pitch
        // and across its 18 metres so about 20-40%

        // hexagon is 256 by 226 circle is 500 by 500, hexagon is scale 3
        GameObject centreCircle = Instantiate(circle, centre, Quaternion.identity, this.transform);
        centreCircle.transform.localScale = new Vector3((pitchWidth * 0.1f), (pitchWidth * 0.1f), -2);

        // these need to actually follow a scale ! so first decide goal size, line it up with tiles
        // then add one or two hexes for penalty box and double that amount for big box
        // then have line going up pitch for six yard box being same amount up as it was hexes across
        // same principle for twenty yard box. ( so like its that amount from where previous was)

        // so goal line should be a 15%-8% or so of pitch, so work out if one tile, three tiles etc is closest to the ratio
        int goalSize = Mathf.RoundToInt(pitchWidth * 0.15f);
        if (goalSize % 2 == 0)
        {
            goalSize++;
        }
        // need to move both goals up n down a bit
        topGoalPos = topGoalPos + new Vector3(0, 1.2f, 0);
        bottomGoalPos = bottomGoalPos + new Vector3(0, -1.2f, 0);
        GameObject topGoal = Instantiate(goalLine, topGoalPos, Quaternion.identity, this.transform);
        topGoal.transform.localScale = new Vector3((goalSize * 3 * 0.75f) + 0.25f, 2.5f, 1);
        GameObject bottomGoal = Instantiate(topGoal, bottomGoalPos, Quaternion.identity, this.transform);
    }

    void CreatePlayers(int amountOfPlayers) 
    {
        int rotation = 3;
        for (int i = 0; i < amountOfPlayers; i++)
        {
            Player newPlayer = Instantiate(playerBlue, pitch[(pitchWidth / 2, i*2)].transform);
            player1.AddToPlayers(newPlayer);
            newPlayer.gameObject.SetActive(true);
            newPlayer.SetTeam(player1);
            newPlayer.SetRotation(rotation);
            newPlayer.SetCoordinates(pitch[(pitchWidth / 2, i * 2)].GetCoord());
            newPlayer.SetId(i);
        }
        rotation = 0;
        for (int i = pitchLength-1; i > pitchLength - amountOfPlayers; i--)
        {
            Player newPlayer = Instantiate(playerRed, pitch[(pitchWidth / 2, i * 2)].transform);
            player2.AddToPlayers(newPlayer);
            newPlayer.gameObject.SetActive(true);
            newPlayer.SetTeam(player2);
            newPlayer.SetRotation(rotation);
            newPlayer.SetCoordinates(pitch[(pitchWidth / 2, i * 2)].GetCoord());
            newPlayer.SetId(i);
        }
    }

    public void DisplayTileOptions(HexTile tileClicked)
    {
        // change this so checks whether there is a player on the tile and if there is then will create a button to move player
        //
        if (tileClicked.GetComponentInChildren<Player>() != null)
        {
            if (tileClicked.GetComponentInChildren<Player>().GetTeam() == currentPlayer)
            {
                // move and rotate will always be options
                GameObject newButton = Instantiate(buttonToMove, canvas.transform);
                currentButtons.Add(newButton);
                newButton.SetActive(true);
                newButton = Instantiate(buttonToRotate, canvas.transform);
                currentButtons.Add(newButton);
                newButton.SetActive(true);

                if (tileClicked.GetComponentInChildren<Football>())
                {
                    newButton = Instantiate(buttonToControl, canvas.transform);
                    currentButtons.Add(newButton);
                    newButton.SetActive(true);
                    newButton = Instantiate(buttonToPass, canvas.transform);
                    currentButtons.Add(newButton);
                    newButton.SetActive(true);
                }
                if (tileClicked.GetComponentInChildren<Player>().CheckIfHasPreviousStates())
                {
                    newButton = Instantiate(buttonUndo, canvas.transform);
                    currentButtons.Add(newButton);
                    newButton.SetActive(true);
                }
                newButton = Instantiate(buttonWait, canvas.transform);
                currentButtons.Add(newButton);
                newButton.SetActive(true);
            }
        }
    }
    public void DestroyButtons() 
    {
        for(int i = 0;i < currentButtons.Count; i++)
        {
            Destroy(currentButtons[i]);
        }
        currentButtons.Clear();
        slider.SetActive(false);
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }

    public void ResolveClick(HexTile hex)
    {
        // rewrite this function - new implementation will work on basis 
        // if u click an action type then u will have to do it or back out of it
        // so will only accept tiles in that range
        // try for now turn of any type being an action.
        // this will add copy to player
        // and will allow undo button


        // if player wants to move - dont do anything to change unless clicks valid tile 
        if (playerWantsToMove)
        {
            if (highLightedTiles.Contains(hex))
            {
                // now create a previous state 
                selectedPlayer.AddPreviousState(selectedPlayer, currentTile);

                selectedPlayer.gameObject.transform.parent = hex.transform;
                selectedPlayer.gameObject.transform.localPosition = new Vector3(0, 0, -1);
                playerWantsToMove = false; 
                selectedPlayer.SetLastActionUsedBall(false);
                if (football.GetPlayer() == selectedPlayer)
                {
                    football.AddPrevious(currentTile.GetCoord(),selectedPlayer.GetLastPlayer());
                    football.SetCoordinates(hex.GetCoord());
                    football.gameObject.transform.parent = hex.transform;
                    football.gameObject.transform.localPosition = new Vector3(0, 0, -5);
                    selectedPlayer.SetLastActionUsedBall(true);
                }
                selectedPlayer.SetCoordinates(hex.GetCoord());
                selectedPlayer = null;
                currentTile = null;
                UnhighLightTiles();
                DestroyButtons();
            }
        }
        else if (playerWantsToPass)
        {
            if (highLightedTiles.Contains(hex))
            {
                football.gameObject.transform.parent = hex.transform;
                football.gameObject.transform.localPosition = new Vector3(0, 0, -5);
                playerWantsToPass = false;
                football.SetPlayer(null);
                selectedPlayer.SetLastActionUsedBall(true);
                selectedPlayer.AddPreviousState(selectedPlayer, currentTile);
                football.AddPrevious(currentTile.GetCoord(), selectedPlayer.GetLastPlayer());
                football.SetCoordinates(hex.GetCoord());
                selectedPlayer = null;
                currentTile = null;
                UnhighLightTiles();
                DestroyButtons();
            }
        }

        else if (playerWantsToRotate)
        {
            Debug.Log("must rotate");
        }
        // so if not in a current player and no options are set and need resolving allow to change current tile and selected player
        else
        {
            DestroyButtons();
            DisplayTileOptions(hex);
            UnhighLightTiles();
            currentTile = hex;
            selectedPlayer = hex.GetComponentInChildren<Player>();
        }
            // then when a button is pressed u should be locked into that choice
        // or u unpress the button
        // u can then click on other tiles
        
        /*
        // if player in last tile clicked and player wants to move and move tile is in range 
      
       // if there was a player and wants to pass and pass is in range
        playerWantsToRotate = false;
        slider.SetActive(false);
        UnhighLightTiles();
        */
    }

    public void LockInAction()
    {
        DestroyButtons();
        GameObject newButton = Instantiate(buttonToCancel, canvas.transform);
        currentButtons.Add(newButton);
        newButton.SetActive(true);
    }

    public void HighLightTiles(HexTile hex, int range, Player player)
    {

        UnhighLightTiles();
        // first gotta find where tile is in array
        // so this func should move from a specific tile (and direction)
        // then change found tiles to be diff colour
        // save it in an array
        Debug.Log("X:" + hex.GetCoord()[0] + "Y:" + hex.GetCoord()[1]);
        // so see if there is tiles to left above right and below
        // just try get a hexagon around first of all
        int[] coordinates = hex.GetCoord();


        // so based on tile clicked use trygetvalue -
        // tile -1x and +1 and -1 y
        // tile +1x and +1 and -1 y
        // tile +0 and +2 and -2 y

        // AddToHighlightedTiles(coordinates[0], coordinates[1] + 2);
        //AddToHighlightedTiles(coordinates[0], coordinates[1] - 2);
        //AddToHighlightedTiles(coordinates[0] - 1, coordinates[1] + 1);
        //AddToHighlightedTiles(coordinates[0] + 1, coordinates[1] + 1);
        //AddToHighlightedTiles(coordinates[0] - 1, coordinates[1] - 1);
        //AddToHighlightedTiles(coordinates[0] + 1, coordinates[1] - 1);

        // so apply certain move across until reach mid point 
        // based on rotation the incrementation should be different
        // and coordinate to start from should be different
        // surely programatic way to do it !!!
        int[] IncrementBasedOnRotation(bool firstHalf)
        {
            int[] increments = new int[] { 0, 0 };
            if (firstHalf)
            {
                switch (player.GetRotation())
                {
                    // looking down
                    case 0 or 6:
                        increments[0] = -1;
                        increments[1] = -1;
                        break;
                    // looking downright
                    case 1:
                        increments[0] = 0;
                        increments[1] = -2;
                        break;
                    // looking up right
                    case 2:
                        increments[0] = 1;
                        increments[1] = -1;
                        break;
                    // looking up
                    case 3:
                        increments[0] = 1;
                        increments[1] = 1;
                        break;
                    // looking up left
                    case 4:
                        increments[0] = 0;
                        increments[1] = 2;
                        break;
                    // looking down left
                    case 5:
                        increments[0] = -1;
                        increments[1] = 1;
                        break;
                }
            }
            else
            {
                switch (player.GetRotation())
                {
                    // looking down
                    case 0 or 6:
                        increments[0] = -1;
                        increments[1] = 1;
                        break;
                    // looking downright
                    case 1:
                        increments[0] = -1;
                        increments[1] = -1;
                        break;
                    // looking up right
                    case 2:
                        increments[0] = 0;
                        increments[1] = -2;
                        break;
                    // looking up
                    case 3:
                        increments[0] = 1;
                        increments[1] = -1;
                        break;
                    // looking up left
                    case 4:
                        increments[0] = 1;
                        increments[1] = 1;
                        break;
                    // looking down left
                    case 5:
                        increments[0] = 0;
                        increments[1] = 2;
                        break;
                }
            }
            return increments;
        }
        
        int[] GetStartingCoord(int x, int y, int offset)
        {
            int[] coord = new int[2];
            switch (player.GetRotation())
            {
                // looking down
                case 0 or 6:
                    x += offset;
                    y -= offset;
                    break;
                // looking downright
                case 1:
                    x += offset;
                    y += offset;
                    break;
                // looking up right
                case 2:
                    x = x;
                    y += 2 * offset;
                    break;
                // looking up
                case 3:
                    x -= offset;
                    y += offset;
                    break;
                // looking up left
                case 4:
                    x -= offset;
                    y -= offset;
                    break;
                // looking down left
                case 5:
                    x = x;
                    y -= 2 * offset;
                    break;
            }
            coord[0] = x;
            coord[1] = y;
            return coord;
        }

        int moveDistanceRange = range;
        // bad name for variable cant think of anything else tho lol
        // have to now set starting tile
        int[] increments = IncrementBasedOnRotation(true);
        int[] decrements = IncrementBasedOnRotation(false);
            

        
        for (int i = 1; i <= moveDistanceRange; i++)
        {
            int[] coord = GetStartingCoord(coordinates[0], coordinates[1], i);
            for (int j = 0; j < i * 2 + 1; j++)
            {
                // if its in first half (and for middle) then apply certain transformation
                if (j < i)
                {
                    AddToHighlightedTiles(coord[0], coord[1]);
                    coord[0] += increments[0];
                    coord[1] += increments[1];
                }
                else
                {
                    AddToHighlightedTiles(coord[0], coord[1]);
                    coord[0] += decrements[0];
                    coord[1] += decrements[1];
                }
            }
        }

        // then for every tile in highlighted tiles - highlight them
        for (int i = 0; i < highLightedTiles.Count; i++)
        {
            if (highLightedTiles[i] != null)
            {
                highLightedTiles[i].HighLightTile();
            }
        }
    }


    private void AddToHighlightedTiles(int x, int y)
    {
        if (pitch.TryGetValue((x,y), out HexTile hex2Add))
                {
                    highLightedTiles.Add(hex2Add);
                }
    }

    public void UnhighLightTiles()
    {
        for (int i = 0; i < highLightedTiles.Count; i++)
        {
            if (highLightedTiles[i] != null) { 

                highLightedTiles[i].UnHighLightTile();
            }   
        }
        highLightedTiles.Clear();
    }

    public void MoveThisPlayer()
    {
        playerWantsToMove = true;
        
        // this should tell gamemanager that player wants to move a player, 
        // so when clicked alert gamemanager -> gamemanager will save this somewhere 
        // and then when next tile is clicked the player will be moved to that tile and gamemanager update button clicked
    }

    public void SetPlayerWantsToMove()
    {
        playerWantsToMove = true;
        if (playerWantsToMove)
        {
            HighLightTiles(currentTile, 3, selectedPlayer);
        }
        LockInAction();
    }
    public bool GetPlayerWantsToMove()
    {
        return playerWantsToMove;
    }

    public void SetPlayerWantsToRotate()
    {
        playerWantsToRotate = true;
        // so create subbuttons to rotate
        // for now just try to rotate
        if (currentTile.GetComponentInChildren<Player>() != null)
        {
            // so set the slider active
            // this isnt a part to get hung upon rn
            //slider.GetComponent<Slider>().value = GetLastTileClicked().transform.GetChild(2).gameObject.transform.rotation.z / 60;
            LockInAction();
            slider.SetActive(playerWantsToRotate);
            slider.GetComponent<Slider>().value = currentTile.GetComponentInChildren<Player>().GetRotation();
            currentTile.GetComponentInChildren<Player>().SetLastRotation(currentTile.GetComponentInChildren<Player>().GetRotation());
            GameObject newButton = Instantiate(buttonConfirm, canvas.transform);
            currentButtons.Add(newButton);
            newButton.SetActive(true);
        }
    }
    public void SetPlayerRotation(int rotation)
    {
        int playerRotation = rotation;
        Debug.Log(playerRotation);
        if (currentTile != null)
        {
            if (currentTile.GetComponentInChildren<Player>() != null)
            {
                Debug.Log("ROTATION");
                // GameObject child = GetLastTileClicked().transform.GetChild(2).gameObject;
                Player child = currentTile.GetComponentInChildren<Player>();
                // Assuming 'child' is your GameObject's Transform
                // so should acc 
                // child.transform.Rotate(0, 0, 60*playerRotation);

                // vector 3 is essentially new vector (0,0,1)
                child.transform.rotation = Quaternion.Euler(Vector3.forward * 60 * playerRotation);
                child.SetRotation(rotation);
                UnhighLightTiles();
                playerWantsToMove = false;
                playerWantsToPass = false;
            }
        }
    }

    public void SetPlayerControlBall()
    {
        // this check allows a player to uncontrol the ball
        if (football.GetPlayer() == currentTile.GetComponentInChildren<Player>())
        {
            football.SetPlayer(null);
        }
        else
        {
            football.SetPlayer(currentTile.GetComponentInChildren<Player>());
        }
    }

    public void SetPassBall()
    {
        // so if this is set then when the player nexts clicks a tile the ball will move there
        // dont allow player to have pass ball and wants to move at the same time !
        playerWantsToPass = !playerWantsToPass;
        playerWantsToMove = false;
        if (playerWantsToPass)
        {
            if (currentTile != null)
            {
                HighLightTiles(currentTile, 6, selectedPlayer);
            }
        }
    }

    public void EndPlayersTurn()
    {
        Debug.Log("I HAVE JUST ENDED PLAYER " + currentPlayer + "'s turn");
        DestroyButtons();
        UnhighLightTiles();
        turnCount++;
        ClearIntentions();
        // so if its 2 then that means both players have made choices
        if (turnCount % 2 == 2)
        {
            ResolveTurnActions();
            ClearGhostMarkers();
        }
        // 
        else
        {
            StoreAndGoBackActions(currentPlayer);
        }
        if (currentPlayer == player1)
        {
            currentPlayer = player2;
        }
        else
        {
            currentPlayer = player1;
        }
        
    }

    public void WaitAPhase()
    {
        // so for player add a previous state where its the same as this basically
        selectedPlayer.AddPreviousState(selectedPlayer, currentTile);
        DestroyButtons();
        UnhighLightTiles();
        selectedPlayer.SetLastActionUsedBall(false);
    }

    public void ClearIntentions()
    {
        playerWantsToMove = false;
        playerWantsToRotate = false;
        playerWantsToPass = false;
    }

    private void StoreAndGoBackActions(Team t)
    {
        // so for every player set active false for the last state in them
        // and then set true the first state in all of them
        // and then make ghost marker in where second state is - ghost marker will have more info later
        List<Player> playersList = t.GetPlayers();
        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].gameObject.SetActive(false);
            playersList[i].GetFirstPlayer().gameObject.SetActive(true);
            int[] ghostMarkerCoord = playersList[i].GetSecondPlayer().GetCoordinates();

            // now create GhostMarkers for where the second state of player is
            GhostMarker gM = Instantiate(ghostMarker, pitch[(ghostMarkerCoord[0], ghostMarkerCoord[1])].transform);
            gM.gameObject.SetActive(true);
            ghostMarkers.Append(gM);
        }
        // move football back to state
        // ALERT THIS IS PROBS A BAD WAY TO DO IT
        football.AddPrevious(football.GetCoordinates(), null);
        football.SetCoordinates(football.GetFirstFootballPos());
    }

    private void ClearGhostMarkers()
    {
        for (int i = 0; i < ghostMarkers.Count; i++)
        {
            Destroy(ghostMarkers[i].gameObject);
        }
        ghostMarkers.Clear();
    }

    public void GoBackPhase()
    {
        // so if control of ball then should bring ball back and ball should go back to having player
        // gotta adjust team for new player like
       
        if (selectedPlayer.GetLastPlayer() != null)
        {



            // if it was a ball action then need special part
            if (selectedPlayer.CheckLastActionWasBall())
            {
                if (selectedPlayer.GetLastPlayer() != football.GetLastPlayer())
                {
                    GoBackPass(selectedPlayer.GetLastPlayer());
                }
                Player lastState = selectedPlayer.PopLastState();
                Debug.Log("GOING BACK AS FOOTBALL LAST PLAYER IS THIS PLAYERS LAST STATE");
                currentPlayer.UpdatePlayer(selectedPlayer,lastState);
                Destroy(selectedPlayer.gameObject);
                //now player is last state, add it to team, set it active, place it correctly
                selectedPlayer = lastState;
                selectedPlayer.gameObject.SetActive(true);
                int[] lastFBState = football.PopLastState();
                Player lastFBPlayer = football.PopLastPlayer();

                football.gameObject.transform.parent = pitch[(lastFBState[0], lastFBState[1])].transform;
                football.gameObject.transform.localPosition = new Vector3(0, 0, -5);
                football.SetPlayer(lastFBPlayer);
                football.SetCoordinates(lastFBState);
                
               
            }
            else
            {
                Player lastState = selectedPlayer.PopLastState();
                Debug.Log(lastState.GetCoordinates()[0]);
                //remove player from team, destroy it
                currentPlayer.UpdatePlayer(selectedPlayer, lastState);
                Destroy(selectedPlayer.gameObject);
                //now player is last state, add it to team, set it active, place it correctly
                selectedPlayer = lastState;
                selectedPlayer.gameObject.SetActive(true);

            }
        }
        DestroyButtons();
        UnhighLightTiles();
    }

    private void GoBackPass(Player desiredGoBack)
    {
        // so for players in ball last state -> they need to go back to where they were before ballaction
        // and ball needs to go back actions 
        // these are done in order from most recent 

        while (football.GetLastPlayer() != desiredGoBack)
        {
            // so uh oh. as players can move after pass, need way to get player n go back to them

            // use id for player as index in currentplayer and then go back to last states
            Player lastState = football.PopLastPlayer();
            // so with this player state need to pop it from that player until get to it
            Player playerOfNeededState = currentPlayer.GetPlayers()[lastState.GetId()];
            Player playerOfNeededStateLastState = playerOfNeededState.PopLastState();
            while (playerOfNeededStateLastState != lastState) 
            {
                // undo player actions, update team, destroy current player marker, set old one active
                currentPlayer.UpdatePlayer(playerOfNeededState, playerOfNeededStateLastState);
                Destroy(playerOfNeededState.gameObject);
                playerOfNeededStateLastState.gameObject.SetActive(true);

                // so this basically moves back one more last state 
                playerOfNeededState = currentPlayer.GetPlayers()[lastState.GetId()];
                playerOfNeededStateLastState = playerOfNeededState.PopLastState();

            }

            // now will be at go back phase for ball action so use   playerOfNeededStateLastState to do ball go back

            currentPlayer.UpdatePlayer(playerOfNeededState, playerOfNeededStateLastState);
            Destroy(playerOfNeededState.gameObject);
            //now player is last state, add it to team, set it active, place it correctly
            playerOfNeededStateLastState.gameObject.SetActive(true);


            int[] lastFBState = football.PopLastState();

            football.gameObject.transform.parent = pitch[(lastFBState[0], lastFBState[1])].transform;
            football.gameObject.transform.localPosition = new Vector3(0, 0, -5);
            football.SetPlayer(lastState);
            football.SetCoordinates(lastFBState);
        }

        


    }

    public void ConfirmRotateAction()
    {
        // needs to change rotation and add as previous state + plus cancel for rotation needs to go back to what was

        // Assuming 'child' is your GameObject's Transform
        // so should acc 
        // child.transform.Rotate(0, 0, 60*playerRotation);

        // vector 3 is essentially new vector (0,0,1)
        UnhighLightTiles();
        DestroyButtons();
        playerWantsToRotate = false;

        Player p = currentTile.GetComponentInChildren<Player>();
        int rotation = p.GetRotation();
        p.SetRotation(p.GetLastRotation());
        currentTile.GetComponentInChildren<Player>().transform.rotation = Quaternion.Euler(Vector3.forward * 60 * p.GetLastRotation());
        selectedPlayer.AddPreviousState(selectedPlayer, currentTile);
        p.SetRotation(rotation);
        currentTile.GetComponentInChildren<Player>().transform.rotation = Quaternion.Euler(Vector3.forward * 60 * rotation);
        selectedPlayer.SetLastActionUsedBall(false);

    }
    public void CancelAction()
    {
        playerWantsToMove = false;
        playerWantsToPass = false;
        playerWantsToRotate = false;
        UnhighLightTiles();
        DestroyButtons();
        DisplayTileOptions(currentTile);
        Player p = currentTile.GetComponentInChildren<Player>();
        p.SetRotation(p.GetLastRotation());
        currentTile.GetComponentInChildren<Player>().transform.rotation = Quaternion.Euler(Vector3.forward * 60 * p.GetLastRotation());
    }

    private void ResolveTurnActions()
    {
        // so after a player has made all their choices, undo all of them (go back to first states for all players in team
        // , move ball back to where it was - this should be done fine by go back anyway)
        // or dont undo everything just display the first and second players in players previous actions !!!
        // then when both players done their turns go thru only seeing first in each prev state
        // resolve any interactions/overlap yeye

        // will go thru priority players first

        // go thru 

        // for player in whoever moves first go one by one doing previous state at a time
        // move ball forward state as well
        // then have to loop thru teams as thats were current state is
        Team[] teamList = new Team[2];
        if (currentPlayer == player2)
        {
            teamList[0] = (player1);
            teamList[1] = (player2);
        }
        else
        {
            teamList[0] = (player2);
            teamList[1] = (player1);
        }
        for (int i = 0; i < actionsPerTurn; i++)
        {
            for (int j = 0; j < teamList.Length; j++)
            {
                // now go thru players n show stuff happening
                StartCoroutine(WaitCoroutine());
                
            }
            // do ball phases too
        }
        // then do current phase (all players in both teams yknow)
    }
    IEnumerator WaitCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
