using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public UnitController unitController;  // Referência para o controle da unidade do jogador
    public List<EnemyController> enemiesInTurn = new List<EnemyController>();  // Lista de inimigos manualmente configurada no Inspector
}
