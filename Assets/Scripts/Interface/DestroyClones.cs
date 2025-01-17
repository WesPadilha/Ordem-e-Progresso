using UnityEngine;

public class DestroyClones : MonoBehaviour
{
    // Isso pode ser chamado quando você deseja destruir todos os clones na cena.
    void Start()
    {
        DestroyAllClones();
    }

    // Função que destrói todos os clones
    public void DestroyAllClones()
    {
        // Encontre todos os objetos na cena com o nome que começa com "(Clone)"
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Verifique se o nome do objeto contém "(Clone)"
            if (obj.name.Contains("(Clone)"))
            {
                Destroy(obj);
            }
        }
    }

    // Se você quiser destruir clones ao fechar a cena ou em algum outro evento, você pode usar o OnDestroy() ou outros eventos do Unity.
    void OnDestroy()
    {
        DestroyAllClones();
    }
}
