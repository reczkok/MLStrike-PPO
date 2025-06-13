using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float lifeTime = 3;
    private CoinChaser shooterAgent;
    public void SetShooter(CoinChaser agent)
    {
        shooterAgent = agent;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            Debug.Log("Trafiono cel!");

            if (shooterAgent != null)
            {
                shooterAgent.AddReward(2.0f);
                shooterAgent.EndEpisode();
            }
            
        }
        else
        {
            if (shooterAgent != null)
            {
                shooterAgent.AddReward(-0.01f); // Kara za nietrafienie
            }
        }
        Destroy(gameObject);
    }
}