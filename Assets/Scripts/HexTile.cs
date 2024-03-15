using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    // Start is called before the first frame update
    public GameManager manager;
    public GameObject tileToChangeColour;
    public Color baseColor, highlightedColor;
    private int[] coordinates = new int[2];
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDown()
    {
        // so now what i want a button to pop up if theres a player on the tile
        // then if u click the button u can click another tile and it will move there
        // (then adjust method to move only to tiles within a few distance) 
        if (manager.GetLastTileClicked() == null)
        {
            manager.SetLastTileClicked(this);
        }
        else
        {
            // if theres a player child for last tile then move its position and set it to be child of this

            // so check


            manager.ResolveClick(this);
        }
    }
    public void SetGameManager(GameManager manager)
    {
        //this.manager = manager;
    }

    public int[] GetCoord()
    {
        return coordinates;
    }

    public void SetCoords(int x, int y)
    {
        coordinates[0] = x;
        coordinates[1] = y;
    }

    public void HighLightTile()
    {
        tileToChangeColour.GetComponent<SpriteRenderer>().color = highlightedColor;
    }

    public void UnHighLightTile()
    {
        tileToChangeColour.GetComponent<SpriteRenderer>().color = baseColor;
    }
}
