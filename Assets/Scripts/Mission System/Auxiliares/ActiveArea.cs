using UnityEngine;

public class ActiveArea : MonoBehaviour
{
    [Header("Objeto visual da área")]
    public GameObject objetoParaAtivar;

    [Header("Objetos a serem coletados")]
    public GameObject[] objetosColetaveis;

    public bool isActive = false;

    void Start()
    {
        if (objetoParaAtivar != null)
        {
            objetoParaAtivar.SetActive(isActive);
        }
    }

    public void AtivarGameObject()
    {
        if (objetoParaAtivar != null)
        {
            isActive = true;
            objetoParaAtivar.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Objeto para ativar não está atribuído.");
        }
    }

    public void DesativarGameObject()
    {
        if (objetoParaAtivar != null)
        {
            isActive = false;
            objetoParaAtivar.SetActive(false);
        }

        // Se quiser desativar o próprio objeto ActiveArea também:
        // gameObject.SetActive(false);
    }

    public void ToggleGameObject()
    {
        if (objetoParaAtivar != null)
        {
            isActive = !objetoParaAtivar.activeSelf;
            objetoParaAtivar.SetActive(isActive);
        }
    }

    void Update()
    {
        VerificarObjetosColetados();
    }

    private void VerificarObjetosColetados()
    {
        foreach (GameObject obj in objetosColetaveis)
        {
            if (obj != null && obj.activeSelf)
            {
                return; // Ainda tem objeto ativo
            }
        }

        // Todos os objetos foram coletados
        Debug.Log("Todos os objetos coletados. Desativando a área.");
        DesativarGameObject(); // Usa o método correto
    }
}
