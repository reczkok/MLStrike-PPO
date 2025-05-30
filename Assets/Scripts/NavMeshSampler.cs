using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshSampler : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Vector2 sampleArea = new(50f, 50f);
    [SerializeField] private float sampleHeight = 0.1f;

    private void Awake()
    {
        if (navMeshSurface == null)
            navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public Vector3 GetRandomNavMeshPosition()
    {
        return GetRandomNavMeshPosition(sampleArea);
    }

    public Vector3 GetRandomNavMeshPosition(Vector2 area)
    {
        if (navMeshSurface?.navMeshData == null)
            return Vector3.zero;

        var randomPos = new Vector3(
            Random.Range(-area.x, area.x),
            sampleHeight,
            Random.Range(-area.y, area.y)
        ) + transform.position;

        return NavMesh.SamplePosition(randomPos, out var hit, 100f, NavMesh.AllAreas)
            ? hit.position
            : Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        var center = transform.position;
        var size = new Vector3(sampleArea.x * 2, 0.1f, sampleArea.y * 2);
        Gizmos.DrawWireCube(center, size);
        Gizmos.DrawLine(center + Vector3.up * sampleHeight, center + Vector3.up * (sampleHeight + 1f));
    }
}