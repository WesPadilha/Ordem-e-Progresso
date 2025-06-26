using UnityEngine;

public class QuestItemHandler : MonoBehaviour
{
    // Referência ao inventário do jogador (pode ser atribuída no Inspector ou encontrada dinamicamente)
    public InventoryObject playerInventory;

    private void Start()
    {
        // Se não foi atribuído no Inspector, tenta encontrar dinamicamente
        if (playerInventory == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var collect = player.GetComponent<Collect>();
                if (collect != null)
                {
                    playerInventory = collect.inventory;
                }
            }
        }
    }

    public void RemoveQuestItem(ItemObject questItem)
    {
        if (questItem == null || questItem.type != ItemType.Quest)
        {
            Debug.LogWarning("Item inválido ou não é um item de missão");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogError("Inventário do jogador não atribuído");
            return;
        }

        bool itemRemovido = false;

        // Verifica todos os slots e remove todas as instâncias do item
        for (int i = 0; i < playerInventory.GetSlots.Length; i++)
        {
            InventorySlot slot = playerInventory.GetSlots[i];
            if (slot.ItemObject == questItem)
            {
                int quantidadeRemovida = slot.amount;
                slot.RemoveItem();
                playerInventory.NotifyItemRemoved(questItem, quantidadeRemovida);
                itemRemovido = true;
            }
        }

        if (itemRemovido)
        {
            Debug.Log($"Todos os itens de missão '{questItem.name}' foram removidos do inventário.");
        }
        else
        {
            Debug.LogWarning($"Item de missão '{questItem.name}' não encontrado no inventário.");
        }
    }
}