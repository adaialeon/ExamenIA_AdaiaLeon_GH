using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour
{
    public enum State
    {
        Patrolling,
        Chasing,
	  Attacking,
        //Traveling, 
        //Waiting,
        
    }

    public State currentState;

    UnityEngine.AI.NavMeshAgent agent;

    public Transform[] destinationPoints;
    public Transform player;
    [SerializeField] float visionRange;
    [SerializeField] float patrolRange = 10f; 
    [SerializeField] Transform patrolZone;
    [SerializeField] float visionAngle;
    [SerializeField] [Range(0, 360)]
    public LayerMask obstaclesMask;
    public Transform[] points;
    private int destPoint = 0;
    public float countdown = 5.0f;


    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }

    void Start()
    {
        currentState = State.Patrolling;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.autoBraking = false;
        GotoNextPoint();
    }


    void Update()
    {
        switch(currentState)
        {
            case State.Patrolling:
                Patrol();
            break;

            case State.Chasing:
                Chase();
            break;

            /*case State.Traveling:
                    Travel();
            break;*/

            /*case State.Waiting:
                    Wait();
            break;*/

            case State.Attacking:
                    Attack();
            break; 

            default:
                Chase();
            break;
        }

    }

    void GotoNextPoint() 
    {
        if (points.Length == 0)
        return;
        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }

    void Patrol() 
    {
        if (agent.remainingDistance < 0.5f)
        GotoNextPoint();

        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }

        //currentState = State.Traveling;
    }

    void Patrol2() 
    {
        Vector3 destPoint;
        if(Destination(patrolZone.position, patrolRange, out destPoint))
        {
            agent.destination = destPoint; 
            Debug.DrawRay(destPoint, Vector3.up * 5, Color.blue, 5f);
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }

        //currentState = State.Traveling; 
    }

    bool Destination(Vector3 center, float range, out Vector3 point)
    {
        Vector3 Destination = center * destPoint * range;
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(Destination, out hit, 4, UnityEngine.AI.NavMesh.AllAreas))
        {
            point = hit.position;
            return true; 
        }

        point = Vector3.zero;
        return false; 
    }
    

    /*void Travel()
    {
        if(agent.remainingDistance <= 0.2)
        {
            currentState = State.Waiting;
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }
    }*/

    void Chase()
    {
        agent.destination = player.position;

        if(!FindTarget())
        {
            currentState = State.Patrolling;
        }
    }

    /*void Wait()
    {
        countdown -= Time.deltaTime;

        if (!FindTarget())
        {
            if(countdown <= 0)
            {
                currentState = State.Patrolling;
                countdown = 5;
            }  
        }
    }*/

    void Attack()
    {
        if(FindTarget())
        {
            currentState = State.Attacking;
        }
    }

    bool FindTarget()
    {
        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            Vector3 directionToTarget = (player.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstaclesMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos() 
    {
        foreach(Transform point in destinationPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolZone.position, patrolRange);
        
    }
}











