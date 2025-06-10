using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour
{
    public Texture2D customCursor; // Ícone personalizado
    private Texture2D defaultCursor; // Cursor padrão
    private bool isCustomCursorActive = false;

    void Start()
    {
        defaultCursor = null; // Usa o cursor padrão do sistema
    }

    void Update()
    {
        // Se clicar com o botão esquerdo do mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Verifica se clicou em um UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ResetCursor();
            }
        }
    }

    public void OnButtonClick()
    {
        // Muda para cursor customizado
        Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
        isCustomCursorActive = true;
    }

    public void ResetCursor()
    {
        if (isCustomCursorActive)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            isCustomCursorActive = false;
        }
    }
}
