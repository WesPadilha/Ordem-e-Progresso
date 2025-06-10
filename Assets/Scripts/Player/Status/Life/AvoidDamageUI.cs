using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AvoidDamageUI : MonoBehaviour
{
    public GameObject panel;            
    public TMP_Text messageText;        
    public float displayTime = 2f;      

    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowAvoidMessage(string message)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            messageText.text = message;
        }

        yield return new WaitForSeconds(displayTime);

        if (panel != null)
            panel.SetActive(false);
    }
}
