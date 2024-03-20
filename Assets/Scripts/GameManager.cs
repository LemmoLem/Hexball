using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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
    public Player player;
    private HexTile lastTileClicked;
    public GameObject buttonToMove, buttonToRotate, slider, buttonToControl, buttonToPass;
    private List<GameObject> currentButtons = new List<GameObject>();
    private bool playerWantsToMove, playerWantsToRotate, playerWantsToPass = false;
    private int playerRotation = 0;
    public Football football;
    private List<HexTile> highLightedTiles = new List<HexTile>();

    // Start is called before the first frame update
    void Start()
    {
        CreatePitch();
        CreatePlayers(5);

        // SPAWN BALL - MAKE IT A FUNCTION
        GameObject centre = pitch[((pitchWidth + 1) / 2 - 1, pitchLength -1)].gameObject;
        football.gameObject.transform.parent = centre.transform;
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
        for (int i = 0; i < amountOfPlayers; i++)
        {
            Player newPlayer = Instantiate(player, pitch[(pitchWidth / 2, i*2)].transform);
        }
    }

    public void MovePlayerToTile()
    {

    }

    public HexTile GetLastTileClicked()
    {
        return lastTileClicked;
    }

    public void SetLastTileClicked(HexTile newTileClicked)
    {
        DestroyButtons();
        lastTileClicked = newTileClicked;
        if (newTileClicked != null)
        {
            DisplayTileOptions(newTileClicked);
        }
    }

    public void DisplayTileOptions(HexTile tileClicked)
    {
        // change this so checks whether there is a player on the tile and if there is then will create a button to move player
        //
        if (tileClicked.GetComponentInChildren<Player>() != null)
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
        }
    }
    public void DestroyButtons() 
    {
        for(int i = 0;i < currentButtons.Count; i++)
        {
            Destroy(currentButtons[i]);
        }
        currentButtons.Clear();

    }
    public Canvas GetCanvas()
    {
        return canvas;
    }

    public void ResolveClick(HexTile hex)
    {
        UnhighLightTiles();
        HighLightTiles(hex);
        // check whether player pressed button to move a player
        // need to create a button only if there is a player on it
        if(GetLastTileClicked().GetComponentInChildren<Player>() != null && playerWantsToMove == true)
        {
            Player child = GetLastTileClicked().GetComponentInChildren<Player>();
            child.gameObject.transform.parent = hex.transform;
            child.gameObject.transform.localPosition = new Vector3(0, 0, -1);
            SetLastTileClicked(null);
            playerWantsToMove = false;

            if (football.GetPlayer() == child)
            {
                // if player in control of ball is same as football then move it to tile with it
                football.gameObject.transform.parent = hex.transform;
                football.gameObject.transform.localPosition = new Vector3(0, 0, -5);
            }
        }
        else if (GetLastTileClicked().GetComponentInChildren<Player>() != null && playerWantsToPass == true)
        {
            football.gameObject.transform.parent = hex.transform;
            football.gameObject.transform.localPosition = new Vector3(0, 0, -5);
            playerWantsToPass = false;
            SetLastTileClicked(null);
        }
        else
        {
            SetLastTileClicked(hex);
        }
        playerWantsToRotate = false;
        slider.SetActive(false);
    }


    public void HighLightTiles(HexTile hex)
    {
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
                switch (playerRotation)
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
                switch (playerRotation)
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
            switch (playerRotation)
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

        int moveDistanceRange = 3;
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
        playerWantsToMove = !playerWantsToMove;
        playerWantsToPass = false;
    }
    public bool GetPlayerWantsToMove()
    {
        return playerWantsToMove;
    }

    public void SetPlayerWantsToRotate()
    {
        playerWantsToRotate = !playerWantsToRotate;
        // so create subbuttons to rotate
        // for now just try to rotate
        if (GetLastTileClicked().GetComponentInChildren<Player>() != null)
        {
            // so set the slider active
            // this isnt a part to get hung upon rn
            //slider.GetComponent<Slider>().value = GetLastTileClicked().transform.GetChild(2).gameObject.transform.rotation.z / 60;
            slider.SetActive(playerWantsToRotate);
        }
    }
    public void SetPlayerRotation(int rotation)
    {
        playerRotation = rotation;
        Debug.Log(playerRotation);
        if (GetLastTileClicked() != null)
        {
            if (GetLastTileClicked().GetComponentInChildren<Player>() != null)
            {
                Debug.Log("ROTATION");
                // GameObject child = GetLastTileClicked().transform.GetChild(2).gameObject;
                Player child = GetLastTileClicked().GetComponentInChildren<Player>();
                // Assuming 'child' is your GameObject's Transform
                // so should acc 
                // child.transform.Rotate(0, 0, 60*playerRotation);

                // vector 3 is essentially new vector (0,0,1)
                child.transform.rotation = Quaternion.Euler(Vector3.forward * 60 * playerRotation);

                // idk how well this will work
            }
        }
    }

    public void SetPlayerControlBall()
    {
        // this check allows a player to uncontrol the ball
        if (football.GetPlayer() == GetLastTileClicked().GetComponentInChildren<Player>())
        {
            football.SetPlayer(null);
        }
        else
        {
            football.SetPlayer(GetLastTileClicked().GetComponentInChildren<Player>());
        }
    }

    public void SetPassBall()
    {
        // so if this is set then when the player nexts clicks a tile the ball will move there
        // dont allow player to have pass ball and wants to move at the same time !
        playerWantsToPass = !playerWantsToPass;
        playerWantsToMove = false;
    }

    // aso when u press
}
