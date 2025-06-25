using System;
using System.Linq;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerShoot))]
public class OogaBooga : Agent
{
    private struct Sample { public Vector3 pos; public float time; }
    private readonly List<Sample> _history = new List<Sample>();
    
    [SerializeField] private float sampleWindow       = 4f;
    [SerializeField] private float spreadThreshold    = 8f;
    [SerializeField] private float stationaryPenalty  = -5f;
    private float _stationaryTimer;

    [SerializeField] private GameObject opponent;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GameObject navMeshSamplerObject;
    [SerializeField] private GameObject eyes;
    [SerializeField] private RayPerceptionSensorComponent3D eyeRaySensor;

    private Rigidbody rb;
    private float _moveInput = 0f;
    private float _strafeInput = 0f;
    private float _turnInput = 0f;
    private float _jumpInput = 0f;
    private bool _isGrounded = true;
    private float _shootInput = 0f;
    private float lastShotTime = 0f;
    public float shootCooldown = 0.5f;
    private PlayerShoot playerShoot;
    private float _lookDirection;
    
    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerShoot = GetComponent<PlayerShoot>();
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (eyes != null)
        { 
            eyeRaySensor = eyes.GetComponent<RayPerceptionSensorComponent3D>();
        }
        else
        {
            eyes = transform.Find("Eyes")?.gameObject;
            if (eyes == null)
            {
                Debug.LogWarning("Eyes GameObject not found. Please assign it in the inspector.");
            } else
            {
                eyeRaySensor = eyes.GetComponent<RayPerceptionSensorComponent3D>();
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        _history.Clear();
        var agentPosition = navMeshSamplerObject.GetComponent<NavMeshSampler>().GetRandomNavMeshPosition() + new Vector3(0, 1, 0);
        
        if (MatchManager.Instance != null)
        {
            agentPosition = MatchManager.Instance.agentSpawnPoint.position;
        }

        transform.position = agentPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        FindOpponent();
    }
    
    private void FindOpponent()
    {
        if (opponent) return;
        var allAgents = GameObject.FindGameObjectsWithTag("Agent");
        opponent = null;
        foreach (var agent in allAgents)
        {
            if (agent == gameObject) continue;
            var agentComponent = agent.GetComponent<Agent>();
            if (agentComponent == null ||
                agentComponent.GetComponent<BehaviorParameters>().TeamId ==
                GetComponent<BehaviorParameters>().TeamId) continue;
            opponent = agent;
            break;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        _moveInput = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        _strafeInput = Mathf.Clamp(actions.ContinuousActions[5], -1f, 1f);
        _turnInput = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        _jumpInput = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);

        var eyesAngle = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);
        var eyesAngleMapped = Mathf.Lerp(-20f, 60f, (eyesAngle + 1f) / 2f);

        _lookDirection = eyesAngleMapped;
        _shootInput = Mathf.Clamp(actions.ContinuousActions[4], -1f, 1f);

        if (eyes != null)
        {
            var eyesTransform = eyes.transform;
            eyesTransform.localRotation = Quaternion.Euler(eyesAngleMapped, 0f, 0f);
        }
        else
        {
            Debug.LogWarning("Eyes GameObject not found. Cannot set rotation.");
        }
        
        if (eyeRaySensor != null && _shootInput > 0.5f)
        {
            var whatIsSeen = eyeRaySensor.GetRayPerceptionInput();
            var output = RayPerceptionSensor.Perceive(whatIsSeen, true);
            var hasNoticedEnemy = false;
            foreach (var ray in output.RayOutputs)
            {
                if (ray.HitTagIndex == 1) 
                {
                    hasNoticedEnemy = true;
                }
            }
            if (hasNoticedEnemy)
            {
                AddReward(1f);
            }
        }
        
        if (Mathf.Abs(_moveInput) < 0.1f && Mathf.Abs(_strafeInput) < 0.1f)
        {
            AddReward(-0.05f);
        }
        
        var now = Time.time;
        _history.Add(new Sample{ pos = transform.position, time = now });
        _history.RemoveAll(s => now - s.time > sampleWindow);

        var maxDist = 0f;
        for(var i = 0; i < _history.Count; i++)
            for(var j = i + 1; j < _history.Count; j++)
                maxDist = Mathf.Max(maxDist, Vector3.Distance(_history[i].pos, _history[j].pos));

        if (maxDist < spreadThreshold)
        {
            _stationaryTimer += Time.deltaTime;
            if (_stationaryTimer > sampleWindow)
            {
                AddReward(stationaryPenalty);
            }
        }
        else
        {
            _stationaryTimer = 0f;
        }
        
        AddReward(-0.01f);
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
        
        var strafeDelta = transform.right * (_strafeInput * (speed / 2f) * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + strafeDelta);
        
        var rot = Quaternion.Euler(0f, _turnInput * rotationSpeed * 180f * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * rot);

        if (!(_shootInput > 0.5f) || !(Time.time - lastShotTime > shootCooldown)) return;

        playerShoot.Shoot(this.gameObject, _lookDirection);
        lastShotTime = Time.time;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(eyes != null ? (eyes.transform.localRotation.eulerAngles.x + 20f) / 80f : 0f);
        sensor.AddObservation(_isGrounded ? 1f : 0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Keyboard.current.wKey.isPressed ? 1f : 
                                  Keyboard.current.sKey.isPressed ? -1f : 0f;
        continuousActionsOut[1] = Keyboard.current.aKey.isPressed ? -1f  : 
                                  Keyboard.current.dKey.isPressed ? 1f : 0f;
        continuousActionsOut[2] = Keyboard.current.spaceKey.isPressed ? 1f : 0f;
        continuousActionsOut[3] = Keyboard.current.upArrowKey.isPressed ? 1f : -1f;
        continuousActionsOut[4] = Keyboard.current.rKey.isPressed ? 1f : 0f;
    }
}