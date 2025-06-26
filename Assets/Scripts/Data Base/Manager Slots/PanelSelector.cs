using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using UnityEngine.AI;

public class PanelSelector : MonoBehaviour
{
    [Header("Slots")]
    public List<SelectablePanel> savePanels;
    public List<SelectablePanel> loadPanels;
    public GameObject deleteConfirmationPanel; 
    private SelectablePanel currentlySelected;

    [Header("Player e Camera")]
    public Transform cameraTransform;
    public Transform playerTransform;

    [Header("Inventário e Dados do Personagem")]
    public List<InventoryObject> inventoryObjects;
    public CharacterData characterData;
    public List<WeaponLoader> weaponLoaderObjects; // Atribua no Inspetor

    [Header("Missões")]
    public MissionManager missionManager;
    private int slotToDelete = -1;
    public List<ActiveArea> activeAreas = new List<ActiveArea>();

    [Header("Coletáveis")]
    public List<CollectableStorage> collectableStorages;
    public List<Storage> storages;

    [Header("XP")]
    public List<XPButtonHandler> xpButtons;

    void Start()
    {
        for (int i = 0; i < savePanels.Count; i++)
        {
            savePanels[i].slotIndex = i;
            savePanels[i].isSaveSlot = true;
            savePanels[i].Initialize(this);
            string savedName = LoadSlotName(i);
            savePanels[i].SetSlotName(savedName);
        }

        for (int i = 0; i < loadPanels.Count; i++)
        {
            loadPanels[i].slotIndex = i;
            loadPanels[i].isSaveSlot = false;
            loadPanels[i].Initialize(this);
            string savedName = LoadSlotName(i);
            loadPanels[i].SetSlotName(savedName);
        }

        // Verifica se há dados a carregar
        if (LoadSlotHandler.Instance != null && LoadSlotHandler.Instance.hasSlotToLoad)
        {
            int index = LoadSlotHandler.Instance.slotIndexToLoad;
            LoadSlotHandler.Instance.hasSlotToLoad = false;
            LoadFromSlot(index);
        }
    }

    public void SelectPanel(SelectablePanel panel)
    {
        if (currentlySelected != null)
            currentlySelected.Deselect();

        currentlySelected = panel;
        currentlySelected.Select();
    }

    public void OpenDeleteConfirmation()
    {
        if (currentlySelected == null) return;
        
        slotToDelete = currentlySelected.slotIndex;
        deleteConfirmationPanel.SetActive(true);
    }

    public void ConfirmDelete()
    {
        if (slotToDelete < 0) return;

        string basePath = Application.persistentDataPath;

        // Arquivos a deletar
        string[] files = {
            $"{basePath}/Slot{slotToDelete}_Name.dat",
            $"{basePath}/_Slot{slotToDelete}_Location.dat",
            $"{basePath}/_Slot{slotToDelete}_Character.dat",
            $"{basePath}/_Slot{slotToDelete}_Camera.dat",
            $"{basePath}/_Slot{slotToDelete}_Inventories.dat",
            $"{basePath}/_Slot{slotToDelete}_WeaponLoaders.dat",
            $"{basePath}/_Slot{slotToDelete}_Missions.dat",
            $"{basePath}/_Slot{slotToDelete}_ActiveAreas.dat",
            $"{basePath}/_Slot{slotToDelete}_Scene.dat",
            $"{basePath}/_Slot{slotToDelete}_StorageCollected.dat",
            $"{basePath}/_Slot{slotToDelete}_XPState.dat",
            $"{basePath}/_Slot{slotToDelete}_StorageXPState.dat"
        };


        foreach (var file in files)
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        Debug.Log($"Slot {slotToDelete} deletado.");

        // Atualiza a UI com "[Vazio]"
        savePanels[slotToDelete].SetSlotName("[Vazio]");
        loadPanels[slotToDelete].SetSlotName("[Vazio]");

        slotToDelete = -1;
        deleteConfirmationPanel.SetActive(false);
    }

    public void CancelDelete()
    {
        slotToDelete = -1;
        deleteConfirmationPanel.SetActive(false);
    }

    public void SetSelectedSlot(SelectablePanel panel)
    {
        currentlySelected = panel;
    }

    public void ConfirmSave()
    {
        if (currentlySelected == null || !currentlySelected.isSaveSlot)
            return;

        string newName = currentlySelected.inputField.text;
        currentlySelected.SetSlotName(newName);

        int index = currentlySelected.slotIndex;

        SaveSlotName(index, newName);

        if (index >= 0 && index < loadPanels.Count)
        {
            loadPanels[index].SetSlotName(newName);
        }

        currentlySelected.Deselect();
        currentlySelected = null;

        // Salvar cena
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SceneSaveData sceneData = new SceneSaveData(currentScene);
        SaveSystem.SaveData(sceneData, $"_Slot{index}_Scene.dat");

        // Salvar posição do jogador
        Vector3 playerPos = playerTransform.position;
        PlayerLocationData playerData = new PlayerLocationData(playerPos);
        SaveSystem.SaveData(playerData, $"_Slot{index}_Location.dat");

        // Salvar status do personagem
        CharacterStatusData statusData = new CharacterStatusData(characterData);
        SaveSystem.SaveData(statusData, $"_Slot{index}_Character.dat");

        // Salvar posição da câmera e rotação
        Vector3 camPos = cameraTransform.position;
        Quaternion camRot = cameraTransform.rotation;
        CameraLocationData cameraData = new CameraLocationData(camPos, camRot);
        SaveSystem.SaveData(cameraData, $"_Slot{index}_Camera.dat");

        // Salvar os dois inventários
        InventoryDataWrapper inventoryData = new InventoryDataWrapper(inventoryObjects[0], inventoryObjects[1]);
        SaveSystem.SaveData(inventoryData, $"_Slot{index}_Inventories.dat");

        // Salvar dados dos ScriptableObjects (WeaponLoader)
        WeaponLoaderListData weaponLoaderData = new WeaponLoaderListData(weaponLoaderObjects);
        SaveSystem.SaveData(weaponLoaderData, $"_Slot{index}_WeaponLoaders.dat");

        // Salvar missões
        MissionManagerData missionData = missionManager.GetSaveData();
        SaveSystem.SaveData(missionData, $"_Slot{index}_Missions.dat");

        // Salvar estados das áreas ativas
        ActiveAreasData activeAreasData = new ActiveAreasData(activeAreas);
        SaveSystem.SaveData(activeAreasData, $"_Slot{index}_ActiveAreas.dat");

        // Salvar coleta de storages
        List<string> collectedIDs = new List<string>();
        foreach (var storage in collectableStorages)
        {
            if (storage.alreadyCollected)
                collectedIDs.Add(storage.uniqueID);
        }
        StorageCollectedData collectedData = new StorageCollectedData(collectedIDs);
        SaveSystem.SaveData(collectedData, $"_Slot{index}_StorageCollected.dat");

        // Salvar estados de XP concedido
        List<bool> xpStates = new List<bool>();
        foreach (var xpButton in xpButtons)
        {
            xpStates.Add(xpButton.IsXPGranted());
        }
        XPStateData xpStateData = new XPStateData(xpStates);
        SaveSystem.SaveData(xpStateData, $"_Slot{index}_XPState.dat");

        // Salvar estado do XP concedido em storages
        StorageXPStateListData storageXPStateData = new StorageXPStateListData(storages);
        SaveSystem.SaveData(storageXPStateData, $"_Slot{index}_StorageXPState.dat");

        Debug.Log($"Dados salvos no slot {index} com nome '{newName}' na posição {playerPos}");
    }

    private void SaveSlotName(int index, string name)
    {
        string path = Application.persistentDataPath + "/Slot" + index + "_Name.dat";
        File.WriteAllText(path, name);
    }

    public void ConfirmLoad()
    {
        if (currentlySelected == null || currentlySelected.isSaveSlot)
            return;

        // Verifica se o slot não está vazio
        if (currentlySelected.GetSlotName() == "[Vazio]")
        {
            Debug.LogWarning("Não é possível carregar um slot vazio.");
            return;
        }

        int index = currentlySelected.slotIndex;

        // Salva o índice no handler persistente
        if (LoadSlotHandler.Instance == null)
        {
            GameObject obj = new GameObject("LoadSlotHandler");
            obj.AddComponent<LoadSlotHandler>();
        }

        // Carrega nome da cena
        SceneSaveData sceneData = SaveSystem.LoadData<SceneSaveData>($"_Slot{index}_Scene.dat");

        if (sceneData != null)
        {
            LoadSlotHandler.Instance.slotIndexToLoad = index;
            LoadSlotHandler.Instance.hasSlotToLoad = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneData.sceneName);
        }
    }

    private IEnumerator LoadSceneAndData(string sceneName, int index)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        // Espera a cena carregar completamente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Aguarda mais um frame para garantir que tudo está iniciado
        yield return null;

        // Agora que a cena está carregada, carrega os dados salvos
        LoadFromSlot(index);
    }

    public void LoadFromSlot(int index)
    {
        // Carregar posição do jogador
        PlayerLocationData playerData = SaveSystem.LoadData<PlayerLocationData>($"_Slot{index}_Location.dat");
        if (playerData != null)
        {
            StartCoroutine(DelayedPlayerPositionLoad(playerData));
        }

        // Carregar status do personagem
        CharacterStatusData statusData = SaveSystem.LoadData<CharacterStatusData>($"_Slot{index}_Character.dat");
        if (statusData != null)
        {
            statusData.LoadTo(characterData);
            Debug.Log("Status do personagem carregado com sucesso.");
        }

        // Carregar posição e rotação da câmera
        CameraLocationData cameraData = SaveSystem.LoadData<CameraLocationData>($"_Slot{index}_Camera.dat");
        if (cameraData != null)
        {
            cameraTransform.position = cameraData.GetPosition();
            cameraTransform.rotation = cameraData.GetRotation();
            Debug.Log($"Câmera carregada na posição {cameraData.GetPosition()} com rotação {cameraData.GetRotation()}");
        }

        // Carregar os dois inventários e o dinheiro
        InventoryDataWrapper inventoryData = SaveSystem.LoadData<InventoryDataWrapper>($"_Slot{index}_Inventories.dat");
        if (inventoryData != null)
        {
            // Carrega os slots do primeiro inventário
            for (int i = 0; i < inventoryObjects[0].GetSlots.Length; i++)
            {
                inventoryObjects[0].GetSlots[i].UpdateSlot(inventoryData.inventory1.Slots[i].item, inventoryData.inventory1.Slots[i].amount);
            }
            // Carrega o dinheiro do primeiro inventário
            inventoryObjects[0].Money = inventoryData.money1;

            // Carrega os slots do segundo inventário
            for (int i = 0; i < inventoryObjects[1].GetSlots.Length; i++)
            {
                inventoryObjects[1].GetSlots[i].UpdateSlot(inventoryData.inventory2.Slots[i].item, inventoryData.inventory2.Slots[i].amount);
            }
            // Carrega o dinheiro do segundo inventário
            inventoryObjects[1].Money = inventoryData.money2;

            Debug.Log("Inventários e dinheiro carregados com sucesso.");
        }

        // Carregar dados dos WeaponLoaders
        WeaponLoaderListData weaponLoaderData = SaveSystem.LoadData<WeaponLoaderListData>($"_Slot{index}_WeaponLoaders.dat");
        if (weaponLoaderData != null && weaponLoaderData.weaponLoaders.Count == weaponLoaderObjects.Count)
        {
            for (int i = 0; i < weaponLoaderObjects.Count; i++)
            {
                weaponLoaderObjects[i].ammoCapacity = weaponLoaderData.weaponLoaders[i].ammoCapacity;
                weaponLoaderObjects[i].ammoCurrent = weaponLoaderData.weaponLoaders[i].ammoCurrent;

                if (weaponLoaderObjects[i] is WeaponLoaderWithType withType)
                {
                    Enum.TryParse(weaponLoaderData.weaponLoaders[i].ammoType, out AmmoType parsedType);
                    withType.ammoType = parsedType;
                }
            }

            Debug.Log("Dados de WeaponLoader carregados com sucesso.");
        }

        // Carregar missões
        MissionManagerData missionData = SaveSystem.LoadData<MissionManagerData>($"_Slot{index}_Missions.dat");
        if (missionData != null)
        {
            missionManager.LoadFromData(missionData);
            Debug.Log("Dados de missões carregados com sucesso.");
        }

        // Carregar estados das áreas ativas
        ActiveAreasData activeAreasData = SaveSystem.LoadData<ActiveAreasData>($"_Slot{index}_ActiveAreas.dat");
        if (activeAreasData != null && activeAreasData.areaStates.Count == activeAreas.Count)
        {
            for (int i = 0; i < activeAreas.Count; i++)
            {
                if (activeAreas[i].objetoParaAtivar != null)
                {
                    activeAreas[i].objetoParaAtivar.SetActive(activeAreasData.areaStates[i]);
                }
            }
            Debug.Log("Estados das áreas ativas carregados com sucesso.");
        }

        // Carregar dados de coleta dos storages
        StorageCollectedData collectedData = SaveSystem.LoadData<StorageCollectedData>($"_Slot{index}_StorageCollected.dat");
        if (collectedData != null)
        {
            foreach (var storage in collectableStorages)
            {
                if (collectedData.collectedStorageIDs.Contains(storage.uniqueID))
                {
                    storage.alreadyCollected = true;
                    // Aqui você pode esconder/desativar o conteúdo do storage se quiser
                }
            }
            Debug.Log("Dados de coleta dos storages carregados com sucesso.");
        }

        // Carregar estados de XP concedido
        XPStateData xpStateData = SaveSystem.LoadData<XPStateData>($"_Slot{index}_XPState.dat");
        if (xpStateData != null && xpStateData.xpGrantedStates.Count == xpButtons.Count)
        {
            for (int i = 0; i < xpButtons.Count; i++)
            {
                xpButtons[i].SetXPGranted(xpStateData.xpGrantedStates[i]);
            }
            Debug.Log("Estados de XP carregados com sucesso.");
        }
        
        StorageXPStateListData storageXPStateData = SaveSystem.LoadData<StorageXPStateListData>($"_Slot{index}_StorageXPState.dat");
        if(storageXPStateData != null)
        {
            foreach(var xpState in storageXPStateData.storageXPStates)
            {
                var storage = storages.Find(s => s.uniqueID == xpState.uniqueID);
                if(storage != null)
                {
                    storage.SetHasGivenXP(xpState.hasGivenXP);
                }
            }
            Debug.Log("Estados de XP dos storages carregados com sucesso.");
        }
    }

    private IEnumerator DelayedPlayerPositionLoad(PlayerLocationData playerData)
    {
        yield return null; // Espera 1 frame

        NavMeshAgent agent = playerTransform.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        playerTransform.position = playerData.GetPosition();

        if (agent != null) agent.enabled = true;

        Debug.Log($"Posição do jogador carregada: {playerData.GetPosition()}");
    }

    private string LoadSlotName(int index)
    {
        string path = Application.persistentDataPath + "/Slot" + index + "_Name.dat";

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            return "[Vazio]"; // Nome padrão caso não exista um arquivo
        }
    }
}
