using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb; // Referência para o Rigidbody da bala
    private bool isHit = false; // Flag para garantir que a animação de impacto só seja chamada uma vez

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Se a bala tiver colidido, paramos o movimento dela
        if (isHit)
        {
            rb.velocity = Vector3.zero; // Para o movimento
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!isHit && col.CompareTag("Inimigo"))
        {
            isHit = true;
            animator.SetTrigger("hit"); // Ativa a animação de explosão
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }



}
