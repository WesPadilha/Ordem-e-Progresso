using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public InventoryObject playerInventory; // Referência ao inventário do jogador
    public TextMeshProUGUI moneyText; // Texto para exibir o dinheiro do jogador
    void Start()
    {
        UpdateMoneyDisplay(); // Atualiza o dinheiro ao iniciar
    }
    public void UpdateMoneyDisplay()
    {
        moneyText.text = "$" + playerInventory.Money;
    }
}
