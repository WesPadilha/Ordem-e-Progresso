using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreationPerk : MonoBehaviour
{
    public Button perk1;
    public Button perk2;
    public Button perk3;
    public Button perk4;
    public Button perk5;

    public TMP_Text point;

    private bool perk1Chosen = false;
    private bool perk2Chosen = false;
    private bool perk3Chosen = false;
    private bool perk4Chosen = false;
    private bool perk5Chosen = false;

    private int points = 1;

    void Start()
    {
        // Adiciona os listeners aos botões
        perk1.onClick.AddListener(() => TogglePerk(1));
        perk2.onClick.AddListener(() => TogglePerk(2));
        perk3.onClick.AddListener(() => TogglePerk(3));
        perk4.onClick.AddListener(() => TogglePerk(4));
        perk5.onClick.AddListener(() => TogglePerk(5));
        
        // Atualiza o texto de pontos
        UpdatePointsText();
    }

    void TogglePerk(int perkNumber)
    {
        if (points == 0 && !IsPerkChosen(perkNumber))
            return; // Não faz nada se não houver pontos disponíveis e o perk não for escolhido

        // Verifica se o perk já foi escolhido e desmarque-o
        switch (perkNumber)
        {
            case 1:
                if (perk1Chosen)
                {
                    perk1Chosen = false;
                    points = 1; // Recupera o ponto
                }
                else if (points > 0)
                {
                    perk1Chosen = true;
                    points = 0; // Gasta o ponto
                }
                break;

            case 2:
                if (perk2Chosen)
                {
                    perk2Chosen = false;
                    points = 1; // Recupera o ponto
                }
                else if (points > 0)
                {
                    perk2Chosen = true;
                    points = 0; // Gasta o ponto
                }
                break;

            case 3:
                if (perk3Chosen)
                {
                    perk3Chosen = false;
                    points = 1; // Recupera o ponto
                }
                else if (points > 0)
                {
                    perk3Chosen = true;
                    points = 0; // Gasta o ponto
                }
                break;

            case 4:
                if (perk4Chosen)
                {
                    perk4Chosen = false;
                    points = 1; // Recupera o ponto
                }
                else if (points > 0)
                {
                    perk4Chosen = true;
                    points = 0; // Gasta o ponto
                }
                break;

            case 5:
                if (perk5Chosen)
                {
                    perk5Chosen = false;
                    points = 1; // Recupera o ponto
                }
                else if (points > 0)
                {
                    perk5Chosen = true;
                    points = 0; // Gasta o ponto
                }
                break;
        }

        // Atualiza o texto de pontos
        UpdatePointsText();
    }

    void UpdatePointsText()
    {
        point.text = "Pontos: " + points.ToString();
    }

    // Método que verifica se algum perk foi escolhido
    public bool IsAnyPerkChosen()
    {
        return perk1Chosen || perk2Chosen || perk3Chosen || perk4Chosen || perk5Chosen;
    }

    // Método auxiliar para verificar se o perk específico foi escolhido
    private bool IsPerkChosen(int perkNumber)
    {
        switch (perkNumber)
        {
            case 1: return perk1Chosen;
            case 2: return perk2Chosen;
            case 3: return perk3Chosen;
            case 4: return perk4Chosen;
            case 5: return perk5Chosen;
            default: return false;
        }
    }
}
