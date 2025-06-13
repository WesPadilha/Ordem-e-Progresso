using UnityEngine;
using UnityEngine.AI;

public class InventoryWeightManager : MonoBehaviour
{
    public InventoryObject playerInventory;
    public InventoryObject playerEquip;
    public CharacterData characterData;
    public NavMeshAgent playerNavMeshAgent;
    
    private float originalSpeed;
    private float originalAngularSpeed;
    private float originalAcceleration;

    private void Awake()
    {
        if (playerNavMeshAgent != null)
        {
            // Guarda os valores originais do NavMeshAgent
            originalSpeed = playerNavMeshAgent.speed;
            originalAngularSpeed = playerNavMeshAgent.angularSpeed;
            originalAcceleration = playerNavMeshAgent.acceleration;
        }
    }

    private void OnEnable()
    {
        playerInventory.OnItemAdded += OnItemChanged;
        playerInventory.OnItemRemoved += OnItemChanged;
        playerEquip.OnItemAdded += OnItemChanged;
        playerEquip.OnItemRemoved += OnItemChanged;
        CalculateTotalWeight(); // Calcular peso inicial
    }

    private void OnDisable()
    {
        playerInventory.OnItemAdded -= OnItemChanged;
        playerInventory.OnItemRemoved -= OnItemChanged;
        playerEquip.OnItemAdded -= OnItemChanged;
        playerEquip.OnItemRemoved -= OnItemChanged;
    }

    private void OnItemChanged(ItemObject item, int amount)
    {
        CalculateTotalWeight();
    }

    public void CalculateTotalWeight()
    {
        float totalWeight = 0f;
        
        foreach (InventorySlot slot in playerInventory.GetSlots)
        {
            if (slot.ItemObject != null)
            {
                totalWeight += slot.ItemObject.weight * slot.amount;
            }
        }
        
        foreach (InventorySlot slot in playerEquip.GetSlots)
        {
            if (slot.ItemObject != null)
            {
                totalWeight += slot.ItemObject.weight * slot.amount;
            }
        }
        
        characterData.currentWeight = totalWeight;
        characterData.NotifyChanges();
        
        // Atualiza a velocidade do NavMeshAgent baseado no peso
        UpdateMovementSpeed();
    }

    private void UpdateMovementSpeed()
    {
        if (playerNavMeshAgent == null || characterData == null) return;

        if (characterData.currentWeight >= characterData.maxWeight)
        {
            // Peso máximo ou acima - reduz velocidade
            playerNavMeshAgent.speed = 1f;
            playerNavMeshAgent.angularSpeed = 120f; // Reduz a velocidade de rotação também
            playerNavMeshAgent.acceleration = 2f; // Reduz a aceleração
        }
        else
        {
            // Abaixo do peso máximo - volta aos valores normais
            playerNavMeshAgent.speed = originalSpeed;
            playerNavMeshAgent.angularSpeed = originalAngularSpeed;
            playerNavMeshAgent.acceleration = originalAcceleration;
        }
    }
}