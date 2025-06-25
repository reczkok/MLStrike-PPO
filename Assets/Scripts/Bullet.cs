using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float lifeTime = 3;
    private GameObject shooterAgent;
    public void SetShooter(GameObject agent)
    {
        shooterAgent = agent;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;

        if (!(lifeTime < 0)) return;
        if (shooterAgent)
        {
            shooterAgent.GetComponent<Agent>()?.AddReward(-0.1f);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent") && other.gameObject != shooterAgent.gameObject)
        {
            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.ResetMatch();
                Destroy(gameObject);
                return;
            }
            
            var hitAgent = other.GetComponent<CoinChaser>();
            if (shooterAgent != null && hitAgent != null)
            {
                shooterAgent.GetComponent<Agent>()?.AddReward(100.0f);
                hitAgent.AddReward(-50.0f);
                hitAgent.EndEpisode();
                shooterAgent.GetComponent<Agent>()?.EndEpisode();
            }
            
        }
        else
        {
            if (shooterAgent != null)
            {
                shooterAgent.GetComponent<Agent>()?.AddReward(-0.1f);
            }
        }
        Destroy(gameObject);
    }
}