using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float velocidade = 50f;
    private float tempoDestruir = 3f;

    public int dano { get; set; }

    [SerializeField]
    public Vector3 posicaoAlvo { get; set; }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerLife player = other.GetComponent<PlayerLife>();

        if (player != null)
        {
            player.TakeDamage(dano);
        }

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        Destroy(gameObject, tempoDestruir);
    }
}