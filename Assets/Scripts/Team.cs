using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Team : MonoBehaviour
{
    // so this is effectively irl player
    // everything pertaining to irl player
    //
    //
    private List<Player> players = new List<Player>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddToPlayers(Player player)
    {
        players.Add(player);
    }
    public List<Player> GetPlayers()
    {
        return players;
    }
    public void RemovePlayer(Player player)
    {
        players.Remove(player);
    }
    public void UpdatePlayer(Player playerToReplace, Player updatedPlayer)
    {
        int index = players.IndexOf(playerToReplace);
        if (index != -1)
        {
            players[index] = updatedPlayer;
        }
    }
}
