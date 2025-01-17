using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class UnitController : MonoBehaviour
    {
        List<PathInfo> path;
        int pathIndex;
        float moveT;
        float rotateT;
        float speedActual;
        Vector3 startPosition;
        Vector3 targetPosition;
        bool initLerp;
        public bool moving;

        public ScreenController screenController;
        public int actionPoints = 20; // Configurado no Inspector
        private int initialActionPoints; // Para armazenar o valor inicial de actionPoints
        public float walk_speed = 2;
        public float rot_speed = 8;

        public Node node
        {
            get
            {
                return GridBase.singleton.GetNodeFromWorldPosition(transform.position);
            }
        }

        Node prevNode;

        public int x1, z1;
        
        void Start()
        {
            // Armazena o valor inicial de actionPoints ao iniciar o objeto
            initialActionPoints = actionPoints;
        }

        public void Init()
        {
            Vector3 worldPos = GridBase.singleton.GetWorldCoordinatesFromNode(x1, 0, z1);
            transform.position = worldPos;
            node.ChangeNodeStatus(false, GridBase.singleton);
        }


        void Update()
        {
            if (screenController != null && screenController.IsAnyUIActive() || Pause.GameIsPaused)
                return;

            x1 = Mathf.FloorToInt(transform.position.x);
            z1 = Mathf.FloorToInt(transform.position.z);

            if (moving)
                MovingLogic();
        }

        void MovingLogic()
        {
            if (!initLerp)
            {
                if (pathIndex == path.Count)
                {
                    moving = false;
                    return;
                }

                node.ChangeNodeStatus(true, GridBase.singleton);
                moveT = 0;
                rotateT = 0;
                startPosition = this.transform.position;
                targetPosition = path[pathIndex].targetPosition;
                float distance = Vector3.Distance(startPosition, targetPosition);
                speedActual = walk_speed / distance;
                initLerp = true;
            }

            moveT += Time.deltaTime * speedActual;
            
            if (moveT > 1)
            {
                moveT = 1;
                initLerp = false;
                RemoveAP(path[pathIndex]);
                if (pathIndex < path.Count - 1)
                    pathIndex++;
                else    
                    moving = false;
            }

            Vector3 newPos = Vector3.Lerp(startPosition, targetPosition, moveT);
            transform.position = newPos;

            rotateT += Time.deltaTime * rot_speed;

            Vector3 lookDirection = targetPosition - startPosition;
            lookDirection.y = 0;
            if (lookDirection == Vector3.zero)
                lookDirection = transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateT);
        }

        void RemoveAP(PathInfo p)
        {
            actionPoints -= p.ap;
            node.ChangeNodeStatus(false, GridBase.singleton);
        }

        public void AddPath(List<PathInfo> p)
        {
            pathIndex = 1;
            path = p;
            moving = true;
        }

        public void EndTurn()
        {
            actionPoints = initialActionPoints; // Restaura o valor original de actionPoints
        }
    }

