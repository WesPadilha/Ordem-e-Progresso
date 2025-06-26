using UnityEngine;
using DialogueEditor;

public class MoneyChecker : MonoBehaviour
{
    [Header("Referências")]
    public InventoryObject playerInventory;  // Referência ao inventário do jogador
    public int requiredAmount;               // Quantidade de dinheiro necessária
    public string dialogueParameterName = "HasEnoughMoney"; // Nome da variável no Dialogue Editor

    // Método para verificar e deduzir o dinheiro
    public int CheckAndDeductMoney()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("Inventário do jogador não atribuído ao MoneyChecker.");
            ConversationManager.Instance.SetInt(dialogueParameterName, 0); // false
            return 0;
        }

        // Verifica se tem dinheiro suficiente
        if (playerInventory.Money >= requiredAmount)
        {
            // Desconta o dinheiro
            playerInventory.Money -= requiredAmount;
            Debug.Log($"[MoneyChecker] Dinheiro deduzido. Novo saldo: {playerInventory.Money}");
            
            // Atualiza a variável de diálogo se estiver em uma conversa
            if (ConversationManager.Instance != null)
            {
                ConversationManager.Instance.SetInt(dialogueParameterName, 1); // true
            }
            
            return 1;
        }
        else
        {
            Debug.Log($"[MoneyChecker] Dinheiro insuficiente. Necessário: {requiredAmount}, Disponível: {playerInventory.Money}");
            
            // Atualiza a variável de diálogo se estiver em uma conversa
            if (ConversationManager.Instance != null)
            {
                ConversationManager.Instance.SetInt(dialogueParameterName, 0); // false
            }
            
            return 0;
        }
    }

    // Versão alternativa que pode ser chamada pelo Dialogue Editor
    public void CheckMoneyForDialogue()
    {
        int result = CheckAndDeductMoney();
        // O resultado já é setado automaticamente na função principal
    }
}