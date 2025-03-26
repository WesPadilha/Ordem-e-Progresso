using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

        public string levelName;
        public bool saveLevel;
        public bool loadLevel;

        SaveLevelFile savedLevel;

        public bool debugNode = true;
        public Material debugMaterial;
        public Material unwalkableMaterial;
        GameObject debugNodeObj;

        void Start()
        {
            InitPhase();
        }

        public void InitPhase()
        {
            if (debugNode)
                debugNodeObj = WorldNode();

            bool hasSavedLevel = (loadLevel)? CheckForSavedLevel() : false;

            if(hasSavedLevel)
            {
                sizeX = savedLevel.sizeX;
                sizeY = savedLevel.sizeY;
                sizeZ = savedLevel.sizeZ;
                scaleXZ = savedLevel.scaleXZ;
                scaleY = savedLevel.scaleY;
            }

            Check();
            CreateGrid();

            GameManager.singleton.Init();

            if (hasSavedLevel == false)
                LevelManager.singleton.LoadObstacles(this);
            else
                LoadLevel();

            isInit = true;
        }

        public void InitForEditor(LevelManager lvlManager)
        {
            if (debugNode)
                debugNodeObj = WorldNode(true);

            Check();
            CreateGrid();
            lvlManager.LoadObstacles(this, true);
        }

        public bool LoadForEditor(LevelManager lvlManager)
        {
            SaveLevelFile s = Serialization.LoadLevel(levelName);
            if (s == null)
                return false;

            if (debugNode)
                debugNodeObj = WorldNode(true);

            sizeX = s.sizeX;
            sizeY = s.sizeY;
            sizeZ = s.sizeZ;
            scaleXZ = s.scaleXZ;
            scaleY = s.scaleY;

            Check();
            CreateGrid();
            LoadLevel(s);
            return true;
        }

        bool CheckForSavedLevel()
        {
            SaveLevelFile s = Serialization.LoadLevel(levelName);

            if (s == null)
                return false;

            savedLevel = s;
            return true;
        }

        void LoadLevel(SaveLevelFile sf = null)
        {
            SaveLevelFile targetSavedFile = sf;
            if (targetSavedFile == null)
                targetSavedFile = savedLevel;

            List<SaveableNode> s = targetSavedFile.savedNodes;

            for (int i = 0; i < s.Count; i++)
            {
                grid[s[i].x, s[i].y, s[i].z].ChangeNodeStatus(s[i].isWalkable, this);
            }

            Debug.Log("Load Level");
        }

        void Check()
        {
            if(sizeX == 0)
            {
                Debug.Log("Size x is 0, assigning min");
                sizeX = 16;
            }
            if (sizeY == 0)
            {
                Debug.Log("Size y is 0, assigning min");
                sizeY = 1;
            }
            if (sizeZ == 0)
            {
                Debug.Log("Size z is 0, assigning min");
                sizeZ = 1;
            }
            if(scaleXZ == 0)
            {
                Debug.Log("scale xz is 0, assignin 1");
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
            grid = new Node[sizeX, sizeY, sizeZ];

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
                        n.ChangeNodeStatus(true, this);

                        if(debugNode)
                        {
                            Vector3 targetPosition = GetWorldCoordinatesFromNode(x, y, z);
                            targetPosition.y += 0.1f;

                            GameObject go = Instantiate(debugNodeObj,
                                targetPosition,
                                Quaternion.identity
                                ) as GameObject;

                            go.transform.parent = ylvl.nodeParent.transform;
                            go.SetActive(true);
                            n.worldObject = go;
                        }

                        grid[x, y, z] = n;
                    }
                }
            }
        }

        void CreateCollision(int y)
        {
            YLevels lvl = yLevels[y];
            GameObject go = new GameObject();
            BoxCollider box = go.AddComponent<BoxCollider>();
            box.size = new Vector3(sizeX * scaleXZ + (scaleXZ * 2),
                0.2f, sizeZ * scaleXZ + (scaleXZ * 2));

            box.transform.position = new Vector3((sizeX * scaleXZ) * .5f - (scaleXZ * .5f),
                y * scaleY,
                (sizeZ * scaleXZ) * 0.5f - (scaleXZ * .5f));

            lvl.collisionsObj = go;
            lvl.collisionsObj.name = "lvl " + y + "collision";
        }

        public Node GetNodeFromWorldPosition(Vector3 wp, bool dontClamp = false)
        {
            int x = Mathf.RoundToInt(wp.x / scaleXZ);
            int y = Mathf.RoundToInt(wp.y / scaleY);
            int z = Mathf.RoundToInt(wp.z / scaleXZ);

            return GetNode(x, y, z);
        }

        public Node GetNode(int x, int y, int z, bool dontClamp = false)
        {
            if (dontClamp == false)
            {
                x = Mathf.Clamp(x, 0, sizeX - 1);
                y = Mathf.Clamp(y, 0, sizeY - 1);
                z = Mathf.Clamp(z, 0, sizeZ - 1);
            }
            else
            {
                if ( x < 0 || x > sizeX ||
                    y < 0 || y > sizeY  ||
                    z < 0 || z > sizeZ)
                    return null;
            }

            return grid[x, y, z];
        }

        public Vector3 GetWorldCoordinatesFromNode(int x,int y,int z)
        {
            Vector3 r = Vector3.zero;
            r.x = x * scaleXZ;
            r.y = y * scaleY;
            r.z = z * scaleXZ;
            return r;
        }

        GameObject WorldNode(bool inEdit = false)
        {
            GameObject go = new GameObject();
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

            if (inEdit == false)
                Destroy(quad.GetComponent<Collider>());
            else
                DestroyImmediate(quad.GetComponent<Collider>());

            quad.transform.parent = go.transform;
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localEulerAngles = new Vector3(90, 0, 0);
            quad.transform.localScale = Vector3.one * 0.95f;
            quad.GetComponentInChildren<MeshRenderer>().material = debugMaterial;
            go.SetActive(false);
            return go;
        }

        public void ClearLevel(bool inEdit = false)
        {
            for (int i = 0; i < yLevels.Count; i++)
            {
                if(inEdit)
                {
                    DestroyImmediate(yLevels[i].nodeParent);
                    DestroyImmediate(yLevels[i].collisionsObj);
                }
                else
                {
                    Destroy(yLevels[i].nodeParent);
                    Destroy(yLevels[i].collisionsObj);
                }
            }

            if (debugNodeObj)
            {
                if (inEdit)
                {
                    DestroyImmediate(debugNodeObj);
                }
                else
                {
                    Destroy(debugNodeObj);
                }
            }
            yLevels.Clear();
        }

        public static GridBase singleton;
        void Awake()
        {
            singleton = this;
        }

        void OnDestroy()
        {
            ClearLevel(true);
            if (debugNodeObj != null) DestroyImmediate(debugNodeObj);
        }
    }

    [System.Serializable]
    public class YLevels
    {
        public int y;
        public GameObject nodeParent;
        public GameObject collisionsObj;
    }

