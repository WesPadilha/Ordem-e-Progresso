using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.TB
{
    public class Node 
    {
        public int x;
        public int y;
        public int z;

        public float hCost;
        public float gCost;

        public float fCost
        {
            get
            {
                return gCost + hCost; 
            }
        }

        public Node parentNode;
        public bool isWalkable = true;

        public GameObject worldObject;

        public NodeType nodeType;
        public enum NodeType
        {
            ground,
            air
        }
    }
}
