using AprendaUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Transform closestEnemy;

    [Header("PARAMETROS")]
    public string enemysTag;
    public float rotationSpeed;
    public float shootSpeed;
    public float shootRate = 1f;
    private float nextShootRate = 0f;
    public Transform shootPoint;
    public GameObject[] shootType;
    public float[] damageShootType;
    public float damage;
    public AudioClip fxShoot;
    private GameController _GameController;


    // Start is called before the first frame update
    void Start()
    {
        _GameController = FindObjectOfType(typeof(GameController)) as GameController;
    }
    // Update is called once per frame
    void Update()
    {
        closestEnemy = GetClosestTarget();

        if (closestEnemy != null)
        {
            // Calcula a direção para o alvo mais próximo
            Vector3 direction = closestEnemy.position - transform.position;

            // Cria a rotação desejada olhando para o alvo mais próximo
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Aplica suavemente a rotação
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Atira se estiver no tempo certo
            if (Time.time >= nextShootRate)
            {
                damage = ShootEnemy();
                nextShootRate = Time.time + 1f / shootRate; // Controla a cadência do tiro
            }
        }
    }

    Transform GetClosestTarget()
    {
        GameObject[] potentialEnemy = GameObject.FindGameObjectsWithTag(enemysTag);
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in potentialEnemy)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy.transform;
            }
        }

        return closest;
    }


    public float ShootEnemy()
    {
        if (closestEnemy == null) return 0f;

        // Direção do tiro para o inimigo
        Vector3 direction = (closestEnemy.position - shootPoint.position).normalized;

        // Criar e disparar a bala
        GameObject bullet = Instantiate(shootType[0], shootPoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = direction * shootSpeed;
        }

        _GameController.PlayFX(fxShoot);

        return damageShootType[0];
    }

    // Método auxiliar para disparar uma bala em uma determinada direção
    void DispararBala(Vector3 direction)
    {
        GameObject bullet = Instantiate(shootType[0], shootPoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = direction * shootSpeed;
        }
    }


}
