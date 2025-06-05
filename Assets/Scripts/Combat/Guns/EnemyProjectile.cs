using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float velocidade = 50f;
    private float tempoDestruir = 5f;

    public int dano { get; set; }

    [SerializeField]
    public Vector3 posicaoAlvo { get; set; }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se colidir com um inimigo, ignora
        if (other.CompareTag("Enemy"))
        {
            return;
        }

        // Se colidir com o player, aplica o dano
        PlayerLife player = other.GetComponent<PlayerLife>();
        if (player != null)
        {
            player.TakeDamage(dano);
            Destroy(gameObject);
            return;
        }

        // Se colidir com qualquer outra coisa, destrói
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        Destroy(gameObject, tempoDestruir);
    }
}
