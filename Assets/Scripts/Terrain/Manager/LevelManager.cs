using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class LevelManager : MonoBehaviour
    {
        public List<Obstacle> obstacles = new List<Obstacle>();

        public void LoadObstacles(GridBase grid, bool inEdit = false)
        {
            Obstacle[] allObstacles = GameObject.FindObjectsOfType<Obstacle>();
            for (int i = 0; i < allObstacles.Length; i++)
            {
                if (obstacles.Contains(allObstacles[i]) == false)
                    obstacles.Add(allObstacles[i]);
            }

            foreach (Obstacle o in obstacles)
            {
                 List<Vector3> l = o.GetLocatorPositions();

              /*   if (l != null)
                 {
                     if (l.Count > 0)
                     {
                         for (int i = 0; i < l.Count; i++)
                         {
                             Node n = grid.GetNodeFromWorldPosition(l[i]);
                             n.ChangeNodeStatus(false, grid);
                         }
                         return;
                     }
                 }*/

                BoxCollider bx = o.mainRender.gameObject.AddComponent<BoxCollider>();

                float halfX = bx.size.x * 0.5f;
                float halfY = bx.size.y * 0.5f;
                float halfZ = bx.size.z * 0.5f;

                Vector3 center = o.mainRender.bounds.center;
                Vector3 from = o.mainRender.bounds.min;
                from.y = 0;
                Vector3 to = o.mainRender.bounds.max;
                to.y = 0;

                int stepX = Mathf.CeilToInt(Mathf.Abs(from.x - to.x) / grid.scaleXZ);
                int stepZ = Mathf.CeilToInt(Mathf.Abs(from.z - to.z) / grid.scaleXZ);

                for (int x = 0; x < stepX; x++)
                {
                    for (int z = 0; z < stepZ; z++)
                    {

                        Vector3 tp = from;
                        tp.x += grid.scaleXZ * x;
                        tp.z += grid.scaleXZ * z;
                       //tp.y = o.mainRender.transform.position.y;

                        Vector3 p = o.mainRender.transform.InverseTransformPoint(tp) - bx.center;
                        tp.y = 0;


                        if (p.x < halfX && p.y < halfY && p.z < halfZ
                            && p.x > -halfX && p.y > -halfY && p.z > -halfZ)
                        {
                            Node n = grid.GetNodeFromWorldPosition(tp);
                            n.ChangeNodeStatus(false,grid);
                        }
                    }
                }

                if(inEdit)
                {
                    DestroyImmediate(bx);
                }
                else
                {
                    Destroy(bx);
                }
            }
        }

        public static LevelManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
