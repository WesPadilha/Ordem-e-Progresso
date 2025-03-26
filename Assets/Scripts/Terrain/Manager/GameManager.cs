using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pathfinding;

    public class GameManager : MonoBehaviour
    {
        public List<UnitController> units = new List<UnitController>(); 

        public UnitController curUnit;
        public bool movingPlayer;
        bool hasPath;

        Node curNode;
        Node prevNode;

        List<PathInfo> redInfo;
        List<PathInfo> pathInfo;

        public Material blue;
        public Material red;

        public LineRenderer pathRed;
        public LineRenderer pathBlue;
        GridBase grid;

        public void Init()
        {
            grid = GridBase.singleton;

            GameObject go = new GameObject();
            go.name = "line vis blue";
            pathBlue = go.AddComponent<LineRenderer>();
            pathBlue.startWidth = 0.2f;
            pathBlue.endWidth = 0.2f;
            pathBlue.material = blue;

            GameObject go2 = new GameObject();
            go2.name = "line vis red";
            pathRed = go2.AddComponent<LineRenderer>();
            pathRed.startWidth = 0.2f;
            pathRed.endWidth = 0.2f;
            pathRed.material = red;

            for (int i = 0; i < units.Count; i++)
            {
                units[i].Init();

            }
        }

        void Update()
        {
            if (GridBase.singleton.isInit == false)
                return;

            bool overUIElement = EventSystem.current.IsPointerOverGameObject();

            FindNode();
         
            if(Input.GetMouseButton(1) && !overUIElement)
            {
                UnitController hasUnit = NodeHasUnit(curNode);

                if(curUnit != null)
                {
                    int lastX = curUnit.x1;
                    int lastZ = curUnit.z1;

                    if (curUnit.moving)
                        return;
                }

                if (hasUnit == null && curUnit != null)
                {
                    if (hasPath && pathInfo != null)
                    {
                        curUnit.AddPath(pathInfo);
                    }
                }
                else
                {
                    curUnit = hasUnit;
                }
            }

          

            if (curUnit == null)
                return;

            if (curUnit.moving)
                return;

            #region Pathfinder

            if (prevNode != curNode)
            {
                PathfindMaster.GetInstance().RequestPathfind(curUnit.node, curNode, PathfinderCallback); 
            }

            prevNode = curNode;

            if(hasPath && pathInfo != null)
            {
                if(pathInfo.Count > 0)
                {
                    pathBlue.positionCount = pathInfo.Count;

                    for (int i = 0; i < pathInfo.Count; i++)
                    {
                        pathBlue.SetPosition(i, pathInfo[i].targetPosition);
                    }
                }

                if (redInfo != null)
                {
                    if (redInfo.Count > 1)
                    {
                        pathRed.positionCount = redInfo.Count;

                        pathRed.gameObject.SetActive(true);

                        for (int i = 0; i < redInfo.Count; i++)
                        {
                            pathRed.SetPosition(i, redInfo[i].targetPosition);
                        }
                    }
                    else
                    {
                        pathRed.gameObject.SetActive(false);
                    }
                }
            }

            #endregion
        }

        void PathfinderCallback(List<Node> p)
        {
            int curAp = curUnit.actionPoints;
            int needAp = 0;

            List<PathInfo> tp = new List<PathInfo>();
            PathInfo p1 = new PathInfo();
            p1.ap = 0;
            p1.targetPosition = curUnit.transform.position;
            tp.Add(p1);

            List<PathInfo> red = new List<PathInfo>();

            int baseAction = 1;
            int diag = 1;

            for (int i = 0; i < p.Count; i++)
            {
                Node n = p[i];
                Vector3 wp = grid.GetWorldCoordinatesFromNode(n.x, n.y, n.z);
                Vector3 dir = Vector3.zero;

                if (i == 0)
                    dir = GetPathDir(curUnit.node, n);
                else
                    dir = GetPathDir(p[i - 1], p[i]);

                if (dir.x != 0 & dir.z != 0)
                    baseAction = diag;

                needAp += baseAction;

                PathInfo pi = new PathInfo();
                pi.ap = baseAction;
                pi.targetPosition = wp;

                if(needAp > curAp)
                {
                    if(red.Count == 0)
                    {
                        red.Add(tp[i]);
                    }

                    red.Add(pi);
                }
                else
                {
                    tp.Add(pi);
                }
            }

            UIManager.singleton.UpdateActionPointsIndicator(needAp);

            pathInfo = tp;
            redInfo = red;
            hasPath = true;
        }

        void FindNode()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Node nodeAtHitPoint = GridBase.singleton.GetNodeFromWorldPosition(hit.point);

                // Verifique se o Node é acessível (isWalkable)
                if (nodeAtHitPoint != null && nodeAtHitPoint.isWalkable)
                {
                    curNode = nodeAtHitPoint;
                }
            }
        }

        UnitController NodeHasUnit(Node n)
        {
            for (int i = 0; i < units.Count; i++)
            {
                Node un = units[i].node;

                if (un.x == n.x && un.y == n.y && un.z == n.z)
                    return units[i];
            }

            return null;
        }

        Vector3 GetPathDir(Node n1, Node n2)
        {
            Vector3 dir = Vector3.zero;
            dir.x = n2.x - n1.x;
            dir.y = n2.y - n1.y;
            dir.z = n2.z - n1.z;
            return dir;
        }

        public void EndTurn()
        {
            for (int i = 0; i < units.Count; i++)
            {
                units[i].EndTurn();
            }
        }

        public static GameManager singleton;
        void Awake()
        {
            singleton = this;
        } 

        void OnDestroy()
        {
            if (pathBlue != null) Destroy(pathBlue.gameObject);
            if (pathRed != null) Destroy(pathRed.gameObject);
        }
    }

    [System.Serializable]
    public class PathInfo
    {
        public int ap;
        public Vector3 targetPosition;
    }

