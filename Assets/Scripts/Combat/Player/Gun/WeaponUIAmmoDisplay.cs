using UnityEngine;
using TMPro;

public class WeaponUIAmmoDisplay : MonoBehaviour
{
    [Header("Texto da Munição")]
    public TextMeshProUGUI ammoText;

    [Header("Referência do Transform com Arma Equipada")]
    public Transform weaponSlot;

    [Header("Inventário do Jogador")]
    public InventoryObject playerInventory;

    private WeaponController currentWeapon;
    private WeaponLoader currentLoader;

    void Update()
    {
        UpdateAmmoDisplay();
    }

    void UpdateAmmoDisplay()
    {
        if (weaponSlot == null || weaponSlot.childCount == 0)
        {
            ammoText.text = "-- / --";
            return;
        }

        if (weaponSlot.GetChild(0).TryGetComponent(out currentWeapon))
        {
            currentLoader = currentWeapon.weaponLoader;

            if (currentLoader != null)
            {
                ammoText.text = $"{currentLoader.ammoCurrent} / {currentLoader.ammoCapacity}";
            }
            else
            {
                ammoText.text = "-- / --";
            }
        }
        else
        {
            ammoText.text = "-- / --";
        }
    }

    public void ReloadWeapon()
    {
        if (weaponSlot == null || weaponSlot.childCount == 0)
            return;

        if (!weaponSlot.GetChild(0).TryGetComponent(out currentWeapon))
            return;

        currentLoader = currentWeapon.weaponLoader;

        if (currentLoader == null || currentLoader.ammoCurrent >= currentLoader.ammoCapacity)
            return;

        int neededAmmo = currentLoader.ammoCapacity - currentLoader.ammoCurrent;

        // Procurar no inventário uma munição do tipo certo
        InventorySlot foundSlot = null;
        int amountToReload = 0;

        foreach (InventorySlot slot in playerInventory.GetSlots)
        {
            ItemObject itemObj = slot.ItemObject;

            if (itemObj != null && itemObj.type == ItemType.Ammo && itemObj.ammoType == currentWeapon.requiredAmmoType)
            {
                amountToReload = Mathf.Min(neededAmmo, slot.amount);
                foundSlot = slot;
                break;
            }
        }

        // Se não encontrou munição compatível, não gasta PA e retorna
        if (foundSlot == null || amountToReload <= 0)
        {
            Debug.Log("Sem munição compatível no inventário. Recarregamento cancelado.");
            return;
        }

        // Verifica se está em combate e se tem PA suficiente
        CombatStatusChecker combatChecker = FindObjectOfType<CombatStatusChecker>();
        if (combatChecker != null && combatChecker.IsInCombat())
        {
            MovimentCombat playerCombat = GameObject.FindGameObjectWithTag("Player")?.GetComponent<MovimentCombat>();
            if (playerCombat != null)
            {
                if (playerCombat.GetCurrentActionPoints() < 2)
                {
                    Debug.Log("Pontos de ação insuficientes para recarregar.");
                    return;
                }
                else
                {
                    playerCombat.SpendActionPoints(2);
                }
            }
        }

        // Recarrega e remove munição do inventário
        currentLoader.ammoCurrent += amountToReload;
        foundSlot.AddAmount(-amountToReload);
        playerInventory.NotifyItemRemoved(foundSlot.ItemObject, amountToReload);

        if (foundSlot.amount <= 0)
        {
            foundSlot.RemoveItem();
        }

        Debug.Log("Arma recarregada com " + amountToReload + " munições.");
    }
}
