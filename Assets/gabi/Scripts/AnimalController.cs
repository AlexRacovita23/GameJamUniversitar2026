using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalController : MonoBehaviour
{
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float walkRadius = 10f;
    [SerializeField] private float arrivalThreshold = 0.5f;

    private NavMeshAgent agent;
    private bool isWalking;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        StartCoroutine(MoveNPC());
    }

    private IEnumerator MoveNPC()
    {
        while (true)
        {
            isWalking = false;
            yield return new WaitForSeconds(waitTime);

            Vector2 randomDirection = Random.insideUnitCircle * walkRadius;
            Vector3 targetPosition = new Vector3(randomDirection.x, 0, randomDirection.y) + transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(targetPosition, out hit, walkRadius, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            isWalking = true;

            while (agent.remainingDistance > arrivalThreshold || agent.pathPending)
            {
                yield return null;
            }
        }
    }
}
