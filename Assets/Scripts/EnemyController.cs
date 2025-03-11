using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AprendaUnity;

public class EnemyController : MonoBehaviour
{
    private Vector3 playerDestiny;
    private Vector3 playerPreDestiny;
    private Vector3 playerPos;
    Vector3 direction;
    private Quaternion playerRot;
    public Animator animator;
    public Rigidbody enemyRb;
    private GameController _GameController;
    private PlayerController _playerController;
    private Cannon _Cannon;
    private float cannonDamage;
    private float knockBackForce = 5000f;
    private bool isDead = false; // Flag para evitar múltiplas ativações da morte


    [Header("PARAMETROS")]
    public float hp = 0f;
    public int dano;
    public float speed;
    public AudioClip fxHit;



    // Start is called before the first frame update
    void Start()
    {
        _GameController = FindObjectOfType(typeof(GameController)) as GameController;
        _playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        _Cannon = FindObjectOfType(typeof(Cannon)) as Cannon;
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame

    void Update()
    {
        if (hp <= 0 && !isDead)
        {
            isDead = true; // Marca que já iniciou a morte
            animator.SetTrigger("dead"); // Ativa a animação de morte
        }
        else
        {
            MoveEnemy();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        switch (col.tag)
        {
            case "Projetil":
                hp -= _Cannon.damage;
                _GameController.PlayFX(fxHit);

                // Aplica uma força contrária ao movimento
                Vector3 knockbackDirection = -direction.normalized; // Direção oposta ao movimento
                enemyRb.AddForce(knockbackDirection * knockBackForce); // Ajuste a força conforme necessário
                break;
        }
    }


    void MoveEnemy()
    {
        //PUXA POSIÇÃO ATUAL DO JOGADOR
        playerPos = new Vector3(_playerController.playerTransform.position.x, _playerController.playerTransform.position.y, _playerController.playerTransform.position.z);
        playerDestiny = playerPos;

        //FAZ O INIMIGO OLHAR PARA O PERSONAGEM
        direction = _playerController.playerTransform.position - transform.position;
        playerRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, playerRot, speed * Time.deltaTime);

        animator.SetTrigger("jump");
        //transform.position = Vector3.MoveTowards(transform.position, playerDestiny, speed * Time.deltaTime);

        // Mover com física em vez de alterar transform diretamente
        enemyRb.MovePosition(Vector3.MoveTowards(transform.position, playerDestiny, speed * Time.deltaTime));
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
