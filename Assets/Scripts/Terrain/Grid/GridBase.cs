using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.TB
{
    public class GridBase : MonoBehaviour
    {
        public bool isInit;
        public int sizeX = 32;
        public int sizeY = 3;
        public int sizeZ = 32;
        public float scaleXZ = 1;
        public float scaleY = 2.3f;

        public Node[,,] grid; 
        public List<YLevels> yLevels = new List<YLevels>();
        
        public bool debugNode =  true;
        public Material debugMaterial;
        GameObject debugNodeObj;

        void Start()
        {
            InitPhase();
        }

        public void InitPhase()
        {
            if(debugNode)
                debugNodeObj = WorldNode();

            Check();
            CreateGrid();

            GameManager.singleton.Init();
            isInit = true;
        }

        void Check()
        {
            if(sizeX == 0)
            {
                Debug.Log("Size x is 0, assigning min");
                sizeX = 16;
            }
            if(sizeY == 0)
            {
                Debug.Log("Size y is 0, assigning min");
                sizeY = 1;
            }
            if(sizeZ == 0)
            {
                Debug.Log("Size z is 0, assigning min");
                sizeZ = 16;
            }
            if(scaleXZ == 0)
            {
                Debug.Log("scale xz is 0, assigning 1");
                scaleXZ = 1;
            }
            if(scaleY == 0)
            {
                Debug.Log("scale y is 0, assigning 2");
                scaleY = 2;
            }
        }

        void CreateGrid()
        {
            grid = new Node [sizeX,sizeY,sizeZ];

            for (int y = 0; y < sizeY; y++)
            {
                YLevels ylvl = new YLevels();
                ylvl.nodeParent = new GameObject();
                ylvl.nodeParent.name = "level " + y.ToString();
                ylvl.y = y;
                yLevels.Add(ylvl);

                CreateCollision(y);

                for (int x = 0; x < sizeX; x++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        Node n = new Node();
                        n.x = x;
                        n.y = y;
                        n.z = z;
                        n.isWalkable = true;

                        if(debugNode)
                        {
                            Vector3 tergetPosition = GetWorldCoordinatesFromNode(x, y, z);
                            GameObject go = Instantiate(debugNodeObj, tergetPosition, Quaternion.identity) as GameObject;
                            go.transform.parent = ylvl.nodeParent.transform;
                            go.SetActive(true);
                        }

                        grid[x,y,z] = n;
                    }
                }
            }
        }

        void CreateCollision(int y)
        {
            YLevels lvl = yLevels[y];
            GameObject go = new GameObject();
            BoxCollider box = go.AddComponent<BoxCollider>();
            box.size = new Vector3(sizeX * scaleXZ + (scaleXZ * 2), 0.2f, sizeZ * scaleXZ + (scaleXZ * 2));

            box.transform.position =  new Vector3((sizeX * scaleXZ) * .5f - (scaleXZ* .5f), y * scaleY, (sizeZ * scaleXZ) * .5f - (scaleXZ * .5f));
            
            lvl.collisionsObj = go;
            lvl.collisionsObj.name = "lvl " + y + "collision";
        }

        public Node GetNodeFromWorldPosition(Vector3 wp)
        {
            int x = Mathf.RoundToInt(wp.x / scaleXZ);
            int y = Mathf.RoundToInt(wp.y / scaleY);
            int z = Mathf.RoundToInt(wp.z / scaleXZ);

            return GetNode(x, y, z);
        }

        public Node GetNode(int x, int y, int z)
        {
            x = Mathf.Clamp(x, 0, sizeX - 1);
            y = Mathf.Clamp(y, 0, sizeY - 1);
            z = Mathf.Clamp(z, 0, sizeZ - 1);

            return grid[x, y, z];
        }

        public Vector3 GetWorldCoordinatesFromNode(int x, int y, int z)
        {
            Vector3 r = Vector3.zero;
            r.x = x * scaleXZ;
            r.y = y * scaleY;
            r.z = z * scaleXZ;

            return r;
        }

        GameObject WorldNode()
        {
            GameObject go = new GameObject();
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(go.GetComponent<Collider>());
            quad.transform.parent = go.transform;
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localEulerAngles = new Vector3(90, 0, 0);
            quad.transform.localScale = Vector3.one * 0.9f;
            quad.GetComponentInChildren<MeshRenderer>().material = debugMaterial;
            go.SetActive(false);
            return go;
        }

        public static GridBase singleton;
        void Awake()
        {
            singleton = this;
        }
    }

    [System.Serializable]
    public class YLevels
    {
        public int y;
        public GameObject nodeParent;
        public GameObject collisionsObj;
    }
}

