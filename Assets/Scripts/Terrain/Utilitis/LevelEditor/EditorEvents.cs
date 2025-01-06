using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

    [CustomEditor(typeof(LevelEditor))]
    public class EditorEvents : Editor
    {
        LevelEditor lvl;

        void OnSceneGUI()
        {
            if (lvl == null)
                lvl = LevelEditor.singleton;

            if (lvl == null)
            {
                Debug.Log("f");
                return;
            }

            Event e = Event.current;

            HandleKeys(e);
        }

        void HandleKeys(Event e)
        {
            Vector3 mousePos = Vector3.up * - 500;

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                mousePos = hit.point;
            }

            if (e.keyCode == lvl.hotKeys.editMode)
            {
                lvl.editMode = !lvl.editMode;
            }
            if(e.keyCode == lvl.hotKeys.initLevel)
            {
                lvl.InitializeLevel();
            }
            if (e.keyCode == lvl.hotKeys.saveLevel)
            {
                lvl.SaveLevel();
            }
            if(e.keyCode == lvl.hotKeys.clearLevel)
            {
                lvl.ClearLevel();
            }
            if(e.keyCode == lvl.hotKeys.canWalk)
            {
                lvl.ChangeNodeStatusOnPosition(mousePos, true);
            }
            if(e.keyCode == lvl.hotKeys.dontWalk)
            {
                lvl.ChangeNodeStatusOnPosition(mousePos, false);
            }
        }

    }