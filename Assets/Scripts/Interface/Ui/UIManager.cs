using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importar o namespace do TextMeshPro

    public class UIManager : MonoBehaviour
    {
        public GameObject endTurnButton;
        public GameObject mouseFollower;
        public TextMeshProUGUI actionPoints; // Alterar o tipo para TextMeshProUGUI

        public void PressButton(UIButtonType type)
        {
            switch(type)
            {
                case UIButtonType.endTurn:
                    GameManager.singleton.EndTurn();
                    break;
                default:
                    break;
            }
        }

        void Update()
        {
            mouseFollower.transform.position = Input.mousePosition;
        }

        public void UpdateActionPointsIndicator(int value)
        {
            actionPoints.text = value.ToString();
        }

        public static UIManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }

    public enum UIButtonType
    {
        endTurn
    }
