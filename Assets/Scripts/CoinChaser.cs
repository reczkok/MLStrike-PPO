using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

[RequireComponent(typeof(Rigidbody))]
public class CoinChaser : Agent
{
    [SerializeField] private GameObject target;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GameObject navMeshSamplerObject;
    [SerializeField] private GameObject eyes;
    //[SerializeField] private GameObject projectile;
    //[SerializeField] private Transform firePoint;

    private Rigidbody rb;
    private float _moveInput = 0f;
    private float _turnInput = 0f;
    private float _jumpInput = 0f;
    private bool _isGrounded = true;
    private float _shootInput = 0f;
    public float shotCooldown = 0.5f;

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
        var randomPosition = navMeshSamplerObject.GetComponent<NavMeshSampler>().GetRandomNavMeshPosition() + new Vector3(0, 1, 0);
        
        if (target != null)
        {
            target.transform.position = randomPosition;
        }
        else
        {
            Debug.LogWarning("Target is not set. Please assign a target GameObject.");
        }
        
        var agentPosition = navMeshSamplerObject.GetComponent<NavMeshSampler>().GetRandomNavMeshPosition() + new Vector3(0, 1, 0);
        
        transform.position = agentPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        _moveInput = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        _turnInput = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        _jumpInput = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);
        
        var eyesAngle = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);
        var eyesAngleMapped = Mathf.Lerp(-20f, 60f, (eyesAngle + 1f) / 2f);

        _shootInput = Mathf.Clamp(actions.ContinuousActions[4], 0f, 1f);

        if (eyes != null)
        {
            var eyesTransform = eyes.transform;
            eyesTransform.localRotation = Quaternion.Euler(eyesAngleMapped, 0f, 0f);
        }
        else
        {
            Debug.LogWarning("Eyes GameObject not found. Cannot set rotation.");
        }

        if (target != null && Vector3.Distance(transform.position, target.transform.position) < 1.0f)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        else
        {
            AddReward(-0.01f);
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

        //if (_shootInput > 0.5f && Time.time - lastShotTime > shotCooldown)
        //{
        //    PlayerShoot playerShoot = GetComponent<PlayerShoot>();
        //    if (playerShoot != null)
        //    {
        //        playerShoot.ShootBullet();
        //        Bullet bullet = 
        //    }
        //    else
        //    {
        //        Debug.LogWarning("PlayerShoot component not found. Cannot shoot.");
        //    }
        //    lastShotTime = Time.time;
        //}
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
        continuousActionsOut[3] = Keyboard.current.upArrowKey.isPressed ? 1f : 0f;
        continuousActionsOut[4] = Keyboard.current.rKey.isPressed ? 1f : 0f;
    }

    //public void ShootBullet()
    //{
    //    Debug.Log("Shoot() called");
    //    if (projectile == null)
    //    {
    //        Debug.LogError("Projectile prefab is not assigned!");
    //        return;
    //    }
    //    // Utwórz pocisk w pozycji i rotacji firePoint
    //    GameObject projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation);
    //    Debug.Log("Projectile instantiated at " + firePoint.position);
    //    //Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
    //    //if (rb != null)
    //    //{
    //    //    rb.linearVelocity = transform.right * 10f; // lub projectileScript.speed
    //    //}

    //    // (Opcjonalnie) przeka¿ referencjê do agenta do pocisku
    //    Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
    //    if (projectileScript != null)
    //    {
    //        projectileScript.SetShooter(this);
    //    }
    //}
}
