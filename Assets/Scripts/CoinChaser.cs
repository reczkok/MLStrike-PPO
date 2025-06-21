using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

[RequireComponent(typeof(Rigidbody))]
public class CoinChaser : Agent
{

    private GameObject opponent;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GameObject navMeshSamplerObject;
    [SerializeField] private GameObject eyes;
    private Rigidbody rb;
    private float _moveInput = 0f;
    private float _turnInput = 0f;
    private float _jumpInput = 0f;
    private bool _isGrounded = true;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (eyes != null) return;
        eyes = transform.Find("Eyes")?.gameObject;
        if (eyes == null)
        {
            Debug.LogWarning("Eyes GameObject not found. Please assign it in the inspector.");
        }
    }



    public override void OnEpisodeBegin()
    {
        var agentPosition = navMeshSamplerObject.GetComponent<NavMeshSampler>().GetRandomNavMeshPosition() + new Vector3(0, 1, 0);

        transform.position = agentPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        FindOpponent();
    }

    private void FindOpponent()
    {
        GameObject[] allAgents = GameObject.FindGameObjectsWithTag("Agent");
        opponent = null;
        foreach (GameObject agent in allAgents)
        {
            if (agent != this.gameObject)
            {
                Agent agentComponent = agent.GetComponent<Agent>();
                if (agentComponent != null &&
                    agentComponent.GetComponent<BehaviorParameters>().TeamId != this.GetComponent<BehaviorParameters>().TeamId)
                {
                    opponent = agent;
                    break;
                }
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        _moveInput = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        _turnInput = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        _jumpInput = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);

        var eyesAngle = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);
        var eyesAngleMapped = Mathf.Lerp(-20f, 60f, (eyesAngle + 1f) / 2f);

        if (eyes != null)
        {
            var eyesTransform = eyes.transform;
            eyesTransform.localRotation = Quaternion.Euler(eyesAngleMapped, 0f, 0f);
        }
        else
        {
            Debug.LogWarning("Eyes GameObject not found. Cannot set rotation.");
        }

        if (opponent != null && Vector3.Distance(transform.position, opponent.transform.position) < 1.5f)
        {
            AddReward(1.0f);
            opponent.GetComponent<Agent>().EndEpisode();
            EndEpisode();
        }
        else
        {
            AddReward(-0.001f);
        }
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (_jumpInput > 0.5f && _isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        var delta = transform.forward * (_moveInput * speed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + delta);

        var rot = Quaternion.Euler(0f, _turnInput * rotationSpeed * 180f * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * rot);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(eyes != null ? (eyes.transform.localRotation.eulerAngles.x + 20f) / 80f : 0f);
        sensor.AddObservation(_isGrounded ? 1f : 0f);

        if (opponent != null)
        {
            Vector3 relativePosition = transform.InverseTransformPoint(opponent.transform.position);
            sensor.AddObservation(relativePosition);
            
            float distance = Vector3.Distance(transform.position, opponent.transform.position);
            sensor.AddObservation(distance / 50f);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Keyboard.current == null)
        {
            continuousActionsOut[0] = 0f;
            continuousActionsOut[1] = 0f;
            continuousActionsOut[2] = 0f;
            continuousActionsOut[3] = 0f;
            return;
        }

        continuousActionsOut[0] = Keyboard.current.wKey.isPressed ? 1f :
                                  Keyboard.current.sKey.isPressed ? -1f : 0f;
        continuousActionsOut[1] = Keyboard.current.aKey.isPressed ? -1f :
                                  Keyboard.current.dKey.isPressed ? 1f : 0f;
        continuousActionsOut[2] = Keyboard.current.spaceKey.isPressed ? 1f : 0f;
        continuousActionsOut[3] = Keyboard.current.upArrowKey.isPressed ? 1f : 0f;
    }
    
        private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent"))
        {
            Agent otherAgent = other.GetComponent<Agent>();
            if (otherAgent != null &&
                otherAgent.GetComponent<BehaviorParameters>().TeamId != this.GetComponent<BehaviorParameters>().TeamId)
            {
                // na razie dalem nagrode dla 2 agentow bo ciezko stwierdzic ktory wygrywa jak w siebie wbiegaja xd
                AddReward(1.0f);
                otherAgent.AddReward(1.0f);
                
                otherAgent.EndEpisode();
                EndEpisode();
            }
        }
    }
}
