using UnityEngine;
using UnityEngine.UI;

public class LifeEnemy : MonoBehaviour
{
    public Slider slider; // Slider para exibir a vida
    public int maxLife = 10; // Vida máxima do inimigo
    private int currentLife; // Vida atual do inimigo
    public int expAmount = 25; // Quantidade de XP que o inimigo dá ao morrer
    
    private EnemyChase enemyChase;
    private EnemyGroupController groupController;
    private PlayerControllerSwitcher playerControllerSwitcher;
    private TurnManager turnManager;

    void Start()
    {
        currentLife = maxLife;
        slider.maxValue = maxLife;
        slider.value = currentLife;
        
        // Obtém as referências no Start
        enemyChase = GetComponent<EnemyChase>();
        groupController = GetComponentInParent<EnemyGroupController>();
        playerControllerSwitcher = FindObjectOfType<PlayerControllerSwitcher>();
        turnManager = FindObjectOfType<TurnManager>();

        // Garante que os componentes estão ativos
        if (enemyChase != null && !enemyChase.enabled)
        {
            enemyChase.enabled = true;
        }
    }

    public void TakeDamage(int damage, bool isCritical)
    {
        // Ativa o combate antes de aplicar o dano
        ActivateCombatState();
        
        currentLife -= damage;
        slider.value = currentLife;

        // Cria o popup de dano
        DamagePopupPro.Create(transform.position, damage, isCritical);

        if (currentLife <= 0)
        {
            Die();
        }
    }

    private void ActivateCombatState()
    {
        // Ativa o inimigo se não estiver ativo
        if (enemyChase != null && !enemyChase.enabled)
        {
            enemyChase.enabled = true;
            enemyChase.ResetActionPoints();
        }

        // Inicia a perseguição do grupo
        if (groupController != null && !groupController.IsGroupChasing())
        {
            groupController.StartGroupChase();
        }

        // Muda o player para modo combate
        if (playerControllerSwitcher != null && 
            playerControllerSwitcher.mainController.enabled)
        {
            playerControllerSwitcher.SwitchControllers();
        }

        // Registra no TurnManager
        if (turnManager != null && groupController != null)
        {
            turnManager.RegisterEnemyGroup(groupController);
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo derrotado! XP concedido: " + expAmount);

        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(expAmount);
        }

        // Atualiza progresso da missão, se o inimigo tiver QuestKillEnemy
        QuestKillEnemy questKill = GetComponent<QuestKillEnemy>();
        if (questKill != null)
        {
            MissionManager missionManager = FindObjectOfType<MissionManager>();
            if (missionManager != null)
            {
                foreach (var mission in missionManager.acceptedMissions)
                {
                    if (!mission.isActive || mission.isCompleted) continue;

                    if (mission.missionID == questKill.missionID)
                    {
                        mission.ReportEnemyKilled(questKill.enemyID);
                        Debug.Log($"Progresso de missão atualizado para {questKill.enemyID} na missão {mission.missionID}");
                        break;
                    }
                }
            }
        }

        // Notifica o groupController antes de destruir
        if (groupController != null && enemyChase != null)
        {
            groupController.RemoveEnemy(enemyChase);
        }

        Destroy(gameObject);
    }
}