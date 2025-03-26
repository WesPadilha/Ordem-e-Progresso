using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public InventoryObject playerInventory;
    public TextMeshProUGUI moneyText;
    public GameObject talk;

    private bool isMouseOverNPC = false;

    void Start()
    {
        UpdateMoneyDisplay();
        if (talk != null)
        {
            talk.SetActive(false); // Começa desativado
        }
    }

    void Update()
    {
        if (talk != null)
        {
            talk.transform.position = Input.mousePosition; // Sempre segue o mouse
        }

        DetectNPC(); // Verifica se o mouse está sobre um NPC
    }

    public void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = "$" + playerInventory.Money.ToString();
        }
    }

    void DetectNPC()
    {
        // Lança um raio da posição do mouse para o mundo
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Verifica se o raio colidiu com algo que tenha a tag "NPC"
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("NPC"))
        {
            if (!isMouseOverNPC) // Só ativa se ainda não estiver ativado
            {
                isMouseOverNPC = true;
                if (talk != null)
                {
                    talk.SetActive(true);
                }
            }
        }
        else
        {
            if (isMouseOverNPC) // Só desativa se estiver ativado
            {
                isMouseOverNPC = false;
                if (talk != null)
                {
                    talk.SetActive(false);
                }
            }
        }
    }
}
