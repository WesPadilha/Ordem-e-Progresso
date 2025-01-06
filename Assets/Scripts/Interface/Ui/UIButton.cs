using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class UIButton : MonoBehaviour
    {
        public UIButtonType type;

        public void Press()
        {
            UIManager.singleton.PressButton(type); 
        }
    }
