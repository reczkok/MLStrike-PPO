using System;
using Unity.MLAgents;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }
    public  GameObject player;
    public  Transform playerSpawnPoint;
    public  GameObject agent;
    public  Transform agentSpawnPoint;
    
    public void ResetMatch()
    {
        agent.GetComponent<Agent>().EndEpisode();
        agent.GetComponent<Agent>().OnEpisodeBegin();
        
        player.transform.position = playerSpawnPoint.position;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
