using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace SA.TB
{
    public class GameManager : MonoBehaviour
    {
        public Transform curUnit;
        public bool movingPlayer;
        bool hasPath;

        public int actionPoints = 20;
        
        Node unitNode;
        Node curNode;
        Node prevNode;

        List<PathInfo> redInfo;
        List<PathInfo> pathInfo;

        public Material blue;
        public Material red;

        LineRenderer pathRed;
        LineRenderer pathBlue;
        GridBase grid;

        public void Init()
        {
            grid = GridBase.singleton;

            Vector3 worldPos = grid.GetWorldCoordinatesFromNode(5, 0, 6);
            curUnit.transform.position = worldPos;
            
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
        }

        void Update()
        {
            if(GridBase.singleton.isInit == false)
                return;

            FindNode();

            if(unitNode == null && curUnit != null)
            {
                unitNode = grid.GetNodeFromWorldPosition(curUnit.transform.position);
            }

            if(unitNode == null)
                return;

            if(prevNode != curNode)
            {
                PathfindMaster.GetInstance().RequestPathfind(unitNode, curNode, PathfinderCallbakc);
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

                if(redInfo != null)
                {
                    if(redInfo.Count > 0)
                    {
                        pathRed.positionCount = redInfo.Count;

                        for (int i = 0; i < redInfo.Count; i++)
                        {
                            pathRed.SetPosition(i, redInfo[i].targetPosition);
                        }
                    }
                }
            }
        }

        void PathfinderCallbakc(List<Node> p)
        {
            int curAp = actionPoints;
            int needAp = 0;

            List<PathInfo> tp = new List<PathInfo>();
            PathInfo p1 = new PathInfo();
            p1.ap = 0;
            p1.targetPosition = curUnit.transform.position;
            tp.Add(p1);

            List<PathInfo> red = new List<PathInfo>();

            int baseAction = 2;
            int diag = 3;

            for (int i = 0; i < p.Count; i++)
            {
                Node n = p[i];
                Vector3 wp = grid.GetWorldCoordinatesFromNode(n.x, n.y, n.z);
                Vector3 dir = Vector3.zero;

                if(i == 0)
                    dir = GetPathDir(curNode, n);
                else
                    dir = GetPathDir(p[i - 1], p[i]);

                if(dir.x != 0 && dir.z != 0)
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

            pathInfo = tp;
            redInfo = red;
            hasPath = true;
        }

        void FindNode()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                curNode = GridBase.singleton.GetNodeFromWorldPosition(hit.point);
            }
        }

        Vector3 GetPathDir(Node n1, Node n2)
        {
            Vector3 dir = Vector3.zero;
            dir.x = n2.x - n1.x;
            dir.y = n2.y - n1.y;
            dir.z = n2.z - n2.z;
            return dir;
        }

        public static GameManager singleton;
        void Awake()
        {
            singleton = this;
        } 
    }

    [System.Serializable]
    public class PathInfo
    {
        public int ap;
        public Vector3 targetPosition;
    }
}

