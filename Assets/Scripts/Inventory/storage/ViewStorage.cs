using System.Collections;
using UnityEngine;
using TMPro;

public class ViewStorage : MonoBehaviour
{
    public GameObject messageUI;
    public TextMeshProUGUI messageText;

    public void ShowInsufficientMessage(int required)
    {
        StartCoroutine(ShowMessage(required));
    }

    IEnumerator ShowMessage(int required)
    {
        messageUI.SetActive(true);
        messageText.text = "Você precisa de " + required + " pontos de arrombamento.";

        yield return new WaitForSeconds(3f);

        messageUI.SetActive(false);
    }
}
