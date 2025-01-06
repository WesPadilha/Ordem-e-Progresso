using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

    public static class Serialization
    {
        
        public static void SaveLevel(string saveName, GridBase grid)
        {
            if (string.IsNullOrEmpty(saveName))
                saveName = "level1";

            SaveLevelFile saveFile = new SaveLevelFile();

            saveFile.sizeX = grid.sizeX;
            saveFile.sizeY = grid.sizeY;
            saveFile.sizeZ = grid.sizeZ;
            saveFile.scaleXZ = grid.scaleXZ;
            saveFile.scaleY = grid.scaleY;
            saveFile.savedNodes = NodeToSaveable(grid);
            
            string saveLocation = SaveLocation();
            saveLocation += saveName;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveLocation, FileMode.Create,
                FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveFile);
            stream.Close();

            Debug.Log(saveName + "saved!");
        }

        public static SaveLevelFile LoadLevel(string loadName)
        {
            SaveLevelFile saveFile = null;

            string targetName = SaveLocation();
            targetName += loadName;

            if(!File.Exists(targetName))
            {
                Debug.Log("Cant find level " + loadName);
            }
            else
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(targetName, FileMode.Open);
                SaveLevelFile save = (SaveLevelFile)formatter.Deserialize(stream);
                saveFile = save;
                stream.Close();
            }

            return saveFile;
        }

        public static List<SaveableNode> NodeToSaveable(GridBase grid)
        {
            List<SaveableNode> retVal = new List<SaveableNode>();

            for (int x = 0; x < grid.sizeX; x++)
            {
                for (int y = 0; y < grid.sizeY; y++)
                {
                    for (int z = 0; z < grid.sizeZ; z++)
                    {
                        SaveableNode sn = new SaveableNode();
                        Node n = grid.grid[x, y, z];
                        sn.x = n.x;
                        sn.y = n.y;
                        sn.z = n.z;
                        sn.isWalkable = n.isWalkable;
                        retVal.Add(sn);
                    }
                }
            }

            return retVal;
        }

        static string SaveLocation()
        {
            string saveLocation = Application.streamingAssetsPath + "/Levels/";

            if(!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
            }

            return saveLocation;
        }
    }

    [Serializable]
    public class SaveLevelFile
    {
        public int sizeX, sizeY, sizeZ;
        public float scaleXZ, scaleY;
        public List<SaveableNode> savedNodes;

    }

    [Serializable]
    public class SaveableNode
    {
        public int x, y, z;
        public bool isWalkable;
    }

