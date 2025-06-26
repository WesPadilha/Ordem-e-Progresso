using UnityEngine;

public class QuestArea : MonoBehaviour
{
    [Header("Configuração da Área")]
    public string missionID;
    public string areaID; // ID único que identifica esta área (deve corresponder ao targetID no objetivo da missão)

    private void Start()
    {
        // Garantir que o collider está com IsTrigger ativo
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("Collider não encontrado no QuestArea!");
        }
        else if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning("Collider do QuestArea não estava marcado como Trigger. Agora está!");
        }

        // Garantir que tenha Rigidbody para o trigger funcionar
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            Debug.LogWarning("Rigidbody não encontrado no QuestArea, adicionado automaticamente como kinematic.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MissionManager missionManager = FindObjectOfType<MissionManager>();

            if (missionManager != null)
            {
                foreach (var mission in missionManager.acceptedMissions)
                {
                    mission.ReportAreaExplored(areaID);
                }

                Debug.Log($"Área {areaID} explorada - objetivos atualizados");
            }
            else
            {
                Debug.LogWarning("MissionManager não encontrado na cena");
            }

            GetComponent<Collider>().enabled = false;
        }
    }
}
