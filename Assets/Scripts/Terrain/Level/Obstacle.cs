using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Obstacle : MonoBehaviour
    {
        public MeshRenderer mainRender;

        public Transform locatorParent;
        
        public List<Vector3> GetLocatorPositions()
        {
            if (locatorParent == null)
                return null;

            List<Vector3> r = new List<Vector3>();

            Transform[] childs = locatorParent.GetComponentsInChildren<Transform>();

            foreach (Transform g in childs)
            {
                if (g == locatorParent.gameObject)
                    continue;

                r.Add(g.transform.position);
            }

            return r;
        }
    }

