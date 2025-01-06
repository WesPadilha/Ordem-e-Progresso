using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [ExecuteInEditMode]
    public class LevelEditor : MonoBehaviour
    {
        [Header("States")]
        public string levelName;
        public bool levelHasInited;
        public int sizeX = 32;
        public int sizeY = 1;
        public int sizeZ = 32;
        public float scaleXZ =1;
        public float scaleY =2;

        [Header("Modes")]
        public bool editMode;

        [Header("Prompts")]
        public bool initLevel;
        [Space]
        public bool clearLevel;
        [Space]
        public bool saveLevel;
        [Space]
        public bool loadLevel;
        

        public HotKeys hotKeys;

        public GridBase targetGrid;
        public LevelManager lvlManager;

    

        public static LevelEditor singleton;

        void Update()
        {
            if (singleton == null)
                singleton = this;

            if(levelHasInited == false)
                editMode = false;

            if (targetGrid == null || lvlManager == null)
                editMode = false;

            if(initLevel)
            {
                initLevel = false;
                if (levelHasInited)
                {
                    ClearLevel();
                }

                InitializeLevel();
            }

            if(clearLevel)
            {
                clearLevel = false;
                if (levelHasInited)
                    ClearLevel();
            }

            if(saveLevel)
            {
                saveLevel = false;
                SaveLevel();
            
            }

            if(loadLevel)
            {
                loadLevel = false;
                LoadLevel();
            }
        }

        public void ChangeNodeStatusOnPosition(Vector3 targetPosition, bool status)
        {
            Node n = targetGrid.GetNodeFromWorldPosition(targetPosition,true);

            if (n == null)
                return;

            n.ChangeNodeStatus(status,targetGrid);
        }

        public void InitializeLevel()
        {
            if(targetGrid == null)
            {
                Debug.Log("To init the level you need to assign the target GridBase first!");
                return;
            }

            if (lvlManager == null)
            {
                Debug.Log("To init the level you need to assign the target lvlManager first!");
                return;
            }

            targetGrid.scaleXZ = scaleXZ;
            targetGrid.scaleY = scaleY;
            targetGrid.sizeX = sizeX;
            targetGrid.sizeY = sizeY;
            targetGrid.sizeZ = sizeZ;

            targetGrid.levelName = levelName;
            targetGrid.InitForEditor(lvlManager);
            levelHasInited = true;
            editMode = true;
        }

        public void LoadLevel()
        {
            ClearLevel();

            if (targetGrid == null)
            {
                Debug.Log("To init the level you need to assign the target GridBase first!");
                return;
            }

            if (lvlManager == null)
            {
                Debug.Log("To init the level you need to assign the target lvlManager first!");
                return;
            }

            targetGrid.levelName = levelName;
            bool canLoad = targetGrid.LoadForEditor(lvlManager);
            if(canLoad == false)
            {
                Debug.Log("Cant find level " + levelName);
                return;
            }

            levelHasInited = true;
            editMode = true;

        }

        public void ClearLevel()
        {
            if (targetGrid == null)
            {
                Debug.Log("To init the level you need to assign the target GridBase first!");
                return;
            }

            if (lvlManager == null)
            {
                Debug.Log("To init the level you need to assign the target lvlManager first!");
                return;
            }

            targetGrid.ClearLevel(true);
            levelHasInited = false;
            editMode = false;
        }

        public void SaveLevel()
        {
            if (targetGrid == null)
            {
                Debug.Log("To init the level you need to assign the target GridBase first!");
                return;
            }

            if (lvlManager == null)
            {
                Debug.Log("To init the level you need to assign the target lvlManager first!");
                return;
            }

            Serialization.SaveLevel(levelName, targetGrid);
        }
    }

    [System.Serializable]
    public class HotKeys
    {
        public KeyCode editMode = KeyCode.Alpha1;
        public KeyCode initLevel = KeyCode.Alpha2;
        public KeyCode saveLevel = KeyCode.Alpha5;
        public KeyCode clearLevel = KeyCode.Alpha0;
        public KeyCode canWalk = KeyCode.G;
        public KeyCode dontWalk = KeyCode.B;
    }
