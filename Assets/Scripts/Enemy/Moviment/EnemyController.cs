using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    public float walk_speed = 2;
    public float rot_speed = 8;
    public int stopDistance = 1;
    public int actionPoints = 20; // Configurable in Inspector
    private int initialActionPoints; // To store initial value

    private List<PathInfo> path;
    private int pathIndex;
    private float moveT;
    private float rotateT;
    private float speedActual;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool initLerp;
    public bool moving;
    private Node currentNode;
    private bool isPlayerInRange = false;
    private bool isInitialized = false;
    private bool isActive = false;

    void Start()
    {
        initialActionPoints = actionPoints; // Store initial value
        StartCoroutine(InitializeWhenReady());
    }

    IEnumerator InitializeWhenReady()
    {
        while (GridBase.singleton == null || !GridBase.singleton.isInit)
        {
            yield return null;
        }
        yield return null;
        
        Init();
        isInitialized = true;
    }

    public void Init()
    {
        Vector3 worldPos = transform.position;
        currentNode = GridBase.singleton?.GetNodeFromWorldPosition(worldPos);
        
        if (currentNode != null)
        {
            transform.position = GridBase.singleton.GetWorldCoordinatesFromNode(currentNode.x, currentNode.y, currentNode.z);
            currentNode.ChangeNodeStatus(false, GridBase.singleton);
        }
    }

    void Update()
    {
        if (!isInitialized || Pause.GameIsPaused || !isActive) return; 

        CheckPlayerDistance();

        if (moving)
        {
            MovingLogic();
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (active)
        {
            isPlayerInRange = false;
            moving = false;
            path = null;
            actionPoints = initialActionPoints; // Reset action points when activated
        }
    }

    void CheckPlayerDistance()
    {
        if (currentNode == null || GridBase.singleton == null || actionPoints <= 0) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Node playerNode = GridBase.singleton.GetNodeFromWorldPosition(player.transform.position);
        if (playerNode == null) return;

        int distanceToPlayer = Mathf.Abs(currentNode.x - playerNode.x) + Mathf.Abs(currentNode.z - playerNode.z);

        if (!isPlayerInRange && distanceToPlayer <= stopDistance)
        {
            isPlayerInRange = true;
            StopMoving();
        }
        else if (isPlayerInRange && distanceToPlayer > stopDistance)
        {
            isPlayerInRange = false;
            StartChasing(playerNode);
        }
        else if (!isPlayerInRange && !moving && actionPoints > 0)
        {
            StartChasing(playerNode);
        }
    }

    void StartChasing(Node targetNode)
    {
        if (targetNode == null || !targetNode.isWalkable || actionPoints <= 0) return;

        PathfindMaster.GetInstance().RequestPathfind(currentNode, targetNode, PathfinderCallback);
    }

    void StopMoving()
    {
        moving = false;
        path = null;
    }

    void PathfinderCallback(List<Node> p)
    {
        if (p == null || p.Count == 0 || currentNode == null || actionPoints <= 0) return;

        Node lastNode = p[p.Count - 1];
        int finalDistance = Mathf.Abs(currentNode.x - lastNode.x) + Mathf.Abs(currentNode.z - lastNode.z);
        if (finalDistance <= stopDistance)
        {
            isPlayerInRange = true;
            return;
        }

        // Calculate total AP cost of the path
        int totalAPCost = p.Count;
        if (totalAPCost > actionPoints)
        {
            // Trim path to only what we can afford
            p = p.GetRange(0, actionPoints);
        }

        if (p.Count > stopDistance)
        {
            p = p.GetRange(0, p.Count - stopDistance);
        }

        List<PathInfo> tp = new List<PathInfo>();
        PathInfo p1 = new PathInfo();
        p1.ap = 0;
        p1.targetPosition = transform.position;
        tp.Add(p1);

        for (int i = 0; i < p.Count; i++)
        {
            Node n = p[i];
            Vector3 wp = GridBase.singleton.GetWorldCoordinatesFromNode(n.x, n.y, n.z);
            PathInfo pi = new PathInfo();
            pi.ap = 1; // Each step costs 1 AP
            pi.targetPosition = wp;
            tp.Add(pi);
        }

        path = tp;
        pathIndex = 1;
        moving = true;
        isPlayerInRange = false;
    }

    void MovingLogic()
    {
        if (!initLerp)
        {
            if (pathIndex >= path.Count || actionPoints <= 0)
            {
                moving = false;
                return;
            }

            if (currentNode != null)
            {
                currentNode.ChangeNodeStatus(true, GridBase.singleton);
            }

            moveT = 0;
            rotateT = 0;
            startPosition = transform.position;
            targetPosition = path[pathIndex].targetPosition;
            float distance = Vector3.Distance(startPosition, targetPosition);
            speedActual = distance > 0 ? walk_speed / distance : 0;
            initLerp = true;
        }

        moveT += Time.deltaTime * speedActual;

        if (moveT > 1)
        {
            moveT = 1;
            initLerp = false;
            
            // Deduct action points when reaching a node
            actionPoints -= path[pathIndex].ap;
            
            currentNode = GridBase.singleton.GetNodeFromWorldPosition(targetPosition);
            if (currentNode != null)
            {
                currentNode.ChangeNodeStatus(false, GridBase.singleton);
            }

            if (pathIndex < path.Count - 1)
                pathIndex++;
            else
                moving = false;
        }

        if (pathIndex < path.Count)
        {
            Vector3 newPos = Vector3.Lerp(startPosition, targetPosition, moveT);
            transform.position = newPos;

            rotateT += Time.deltaTime * rot_speed;
            Vector3 lookDirection = targetPosition - startPosition;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateT);
            }
        }
    }

    public void EndTurn()
    {
        actionPoints = initialActionPoints; // Reset action points at end of turn
    }

    void OnDestroy()
    {
        if (currentNode != null && GridBase.singleton != null)
        {
            currentNode.ChangeNodeStatus(true, GridBase.singleton);
        }
    }
}