using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyLocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    PathManager pathManager;
    public Path path;
    public Vector3 spawnPosition;

    public Waypoint currentWaypoint;
    public int currentWaypointIDX = 0;
    public bool isTakingAlternatePath = false;
    public bool isGargantuan = false;

    public bool isFireTotemEnemy = false;

    Enemy enemy;
    Rigidbody rb;

    //Backtracking
    public Stack<Waypoint> backtrackStack;
    int maxStack = 5;
    public bool reversePathFinding;
    public bool pushPathFinding;
    public Vector3 lastPosition;
    bool fixing;

    bool destinationReached()
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.

        backtrackStack = new Stack<Waypoint>();

        pushPathFinding = false;

        lastPosition = Vector3.zero;
        spawnPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    // Update is called once per frame
    void Update()
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.

        if (!pushPathFinding)
            rb.constraints = RigidbodyConstraints.FreezeAll;
        else
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        //Redacted code. I did not write the code here, so it was removed to not steal from author.

        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (pushPathFinding)
        {
            Vector3 projVec = transform.position;

            //lock enemy position to path
            while (currentWaypoint.parent != null)
            {
                Vector3 pathVec = currentWaypoint.position - currentWaypoint.parent.position;
                Vector3 posVec = transform.position - currentWaypoint.parent.position;
                projVec = ((Vector3.Dot(posVec, pathVec) / pathVec.sqrMagnitude) * pathVec) + currentWaypoint.parent.position;

                if (Vector3.Dot(posVec, pathVec) / pathVec.sqrMagnitude > 0)
                    break;
                else
                    UpdateWaypoint(currentWaypoint.parent);
            }

            if(currentWaypoint.parent == null)
            {
                Vector3 spawnPos = spawnPosition; //GameObject.Find("Enemy Spawner").transform.position;
                Vector3 pathVec = currentWaypoint.position - spawnPos;
                Vector3 posVec = transform.position - spawnPos;
                projVec = ((Vector3.Dot(posVec, pathVec) / pathVec.sqrMagnitude) * pathVec) + spawnPos;
            }

            transform.position = new(projVec.x, transform.position.y, projVec.z);
        }
    }

    Waypoint GetWaypointByPosition(Vector3 pos)
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    // Set curIDX to the index of the point closest to the end of the alternate path.
    void GetClosestPointToAlternatePathEnd(Vector3 currentPos)
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    void RegisterBacktrackNode(Waypoint wp)
    {
        backtrackStack.Push(wp);

        //get rid of excess nodes
        if (backtrackStack.Count >= maxStack)
        {
            Stack<Waypoint> tempStack = new Stack<Waypoint>();

            for (int i = 0; i < maxStack; i++)
            {
                tempStack.Push(backtrackStack.Pop());
            }

            backtrackStack.Clear();

            for (int i = 0; i < tempStack.Count; i++)
            {
                backtrackStack.Push(tempStack.Pop());
            }
        }
    }

    void UpdateWaypoint(Waypoint wp)
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.

        // Note: I had to write this because the original author had set up the waypoints
        //  with parent/child variables that we could use if we needed to go backwards
        //  along the path, however they never set them so I had to do it here.

        foreach (Waypoint child in wp.children)
        {
            child.parent = wp;
        }

        currentWaypoint = wp;
        agent.SetDestination(currentWaypoint.position);
    }

    public void ActivateReversePathfinding(float speed)
    {
        reversePathFinding = true;
        if (currentWaypoint != null && currentWaypoint.parent != null)
            UpdateWaypoint(currentWaypoint.parent);
        else
            UpdateWaypoint(GetWaypointByPosition(path.StartPosition));
        agent.speed = speed;
    }

    public void DeactivateReversePathfinding()
    {
        reversePathFinding = false;
        ReachedCurrentWaypoint();
        agent.speed = enemy.speed;
    }

    public void ActivatePushPathfinding()
    {
        pushPathFinding = true;
    }

    public void DeactivatePushPathfinding()
    {
        pushPathFinding = false;
    }

    private void ReachedCurrentWaypoint()
    {
        if (currentWaypoint.type == PathNodeType.End)
        {
            //currentWaypoint = null;
            if (reversePathFinding)
                UpdateWaypoint(currentWaypoint.parent);
            else
                UpdateWaypoint(null);
        }
        else if (currentWaypoint.children.Count > 1)
        {
            int chance = Random.Range(0, currentWaypoint.children.Count);
            if (reversePathFinding)
                UpdateWaypoint(currentWaypoint.parent);
            else
                UpdateWaypoint(currentWaypoint.children[chance]);
        }
        else if (currentWaypoint.children.Count == 1)
        {
            if (reversePathFinding)
                UpdateWaypoint(currentWaypoint.parent);
            else
                UpdateWaypoint(currentWaypoint.children[0]);
        }
        else
        {
            //RegisterBacktrackNode(currentWaypoint);
            if (currentWaypoint.type == PathNodeType.Alternate_Traversal)
            {
                GetClosestPointToAlternatePathEnd(currentWaypoint.position);
            }
            else
            {
                // If the waypoint is a normal waypoint yet has no children,
                // it's probably a waypoint whose type was incorrectly set
                // so treat it like an end waypoint

                if (reversePathFinding)
                    UpdateWaypoint(currentWaypoint.parent);
                else
                    UpdateWaypoint(null);
            }
        }

        if (currentWaypoint != null && currentWaypoint.type == PathNodeType.Alternate_Vanish)
        {
            enemy.ToggleVanish(true);
        }
        else
        {
            enemy.ToggleVanish(false);
        }
    }

    private IEnumerator RBFix()
    {
        fixing = true;
        yield return new WaitForFixedUpdate();
        transform.position += (currentWaypoint.position - transform.position).normalized * 0.1f;
        fixing = false;
    }
}
