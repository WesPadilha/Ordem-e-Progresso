using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreScreen : MonoBehaviour
{
    public GameObject store;
    public GameObject inventoryPlayer;
    public GameObject playerEquip;
    [SerializeField] private GameObject[] MainButton;
    public ScreenController screenController; // Referência ao controlador de telas
    public GameObject hud;

    public void OpenStore()
    {
        store.SetActive(true);
        inventoryPlayer.SetActive(true);
        playerEquip.SetActive(false);
        screenController.SetStoreState(true); // Notifica o controlador de telas
        DesableMainButton();
        hud.SetActive(false);
    }

    public void CloseStore()
    {
        playerEquip.SetActive(true);
        inventoryPlayer.SetActive(false);
        store.SetActive(false);
        screenController.SetStoreState(false); // Notifica o controlador de telas
        StartMainButton();
        hud.SetActive(true);
    }

    private void DesableMainButton()
    {
        foreach (GameObject button in MainButton)
        {
            button.SetActive(false);
        }
    }

    private void StartMainButton()
    {
        foreach (GameObject button in MainButton)
        {
            button.SetActive(true);
        }
    }
}
