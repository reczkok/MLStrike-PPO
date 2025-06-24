using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerShoot))]
public class PlayerControllerVR : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float shootCooldown = 0.5f;
    
    public InputActionProperty  triggerAction; 

    private Rigidbody rb;
    private PlayerShoot playerShoot;
    private float lastShotTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerShoot = GetComponent<PlayerShoot>();
    }

    private void Update()
    {
        var t = triggerAction.action.ReadValue<float>();
        Debug.Log($"Trigger value: {t}");
        if (t > 0.1f && Time.time - lastShotTime > shootCooldown)
        {
            playerShoot.Shoot(gameObject, 0);
            lastShotTime = Time.time;
        }
    }
}
