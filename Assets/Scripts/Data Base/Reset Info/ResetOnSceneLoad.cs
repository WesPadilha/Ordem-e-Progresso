using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetOnSceneLoad : MonoBehaviour
{
    public List<CreateMissions> allMissionsToReset;
    public CharacterData characterDataToReset;
    public InventoryObject playerInventory;
    public List<InventoryObject> inventoriesToClear;
    public List<WeaponLoader> weaponLoadersToReset; // NOVO: Lista de munições para resetar

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            // Resetar objetivos de missão
            foreach (var mission in allMissionsToReset)
            {
                if (mission != null)
                    mission.ResetObjectives();
            }

            // Resetar dados do personagem
            if (characterDataToReset != null)
                characterDataToReset.ResetData();

            // Limpar todos os inventários
            foreach (var inventory in inventoriesToClear)
            {
                if (inventory != null)
                    inventory.Clear();
            }

            if (playerInventory != null)
                playerInventory.Money = 0;

            // Resetar loaders de munição
            foreach (var loader in weaponLoadersToReset)
            {
                if (loader != null)
                    loader.ammoCurrent = loader.ammoCapacity;
            }
        }
    }
}
