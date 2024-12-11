using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreScreen : MonoBehaviour
{
    public GameObject store;
    public GameObject inventoryPlayer;
    public GameObject playerEquip;
    public ScreenController screenController; // Referência ao controlador de telas

    private bool isStoreOpen = false; // Estado da loja

    public void OpenStore()
    {
        isStoreOpen = true;
        store.SetActive(true);
        inventoryPlayer.SetActive(true);
        playerEquip.SetActive(false);
        screenController.SetStoreState(true); // Notifica o controlador de telas
    }

    public void CloseStore()
    {
        isStoreOpen = false;
        playerEquip.SetActive(true);
        inventoryPlayer.SetActive(false);
        store.SetActive(false);
        screenController.SetStoreState(false); // Notifica o controlador de telas
    }
}