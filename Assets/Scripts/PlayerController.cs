using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AprendaUnity
{
    public class PlayerController : MonoBehaviour
    {
        private GameController _GameController;
        private Animator animator;
        private Vector3 destino;
        private Vector3 preDestino;
        private bool isJumping;

        [Header("Configuração Player")]
        public int tamanhobloco;
        public float velocidadePulo;
        public LayerMask whatIsObstacle;
        public LayerMask whatIsGround;
        public Transform playerTransform;


        #region FUNÇÕES UNITY

        // Start is called before the first frame update
        void Start()
        {
            _GameController = FindObjectOfType(typeof(GameController)) as GameController;
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_GameController.currentState != GameState.GAMEPLAY)
            {
                return;
            }

            TouchInputController();
            InputController();
            MoverPersonagem();

        }

        private void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.tag)
            {
                case "Coletavel":
                    col.gameObject.SendMessage("Coletar", SendMessageOptions.DontRequireReceiver);
                    break;

                case "Perigo":
                    Morreu();
                    break;

                case "Inimigo":
                    Morreu();
                    break;
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if (_GameController.currentState != GameState.GAMEPLAY)
            {
                return;
            }

            switch (col.gameObject.tag)
            {
                case "Coletavel":
                    col.gameObject.SendMessage("Coletar", SendMessageOptions.DontRequireReceiver);
                    break;

                case "Perigo":
                    Morreu();
                    break;

                case "Inimigo":
                    Morreu();
                    break;
            }
        }

        #endregion

        #region FUNÇÕES

        void InputController()
        {
            if (isJumping == true)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                preDestino = transform.position + new Vector3(0, 0, tamanhobloco);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                PreJump();

            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                preDestino = transform.position - new Vector3(0, 0, tamanhobloco);
                transform.rotation = Quaternion.Euler(0, 180, 0);
                PreJump();

            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                preDestino = transform.position - new Vector3(tamanhobloco, 0, 0);
                transform.rotation = Quaternion.Euler(0, -90, 0);
                PreJump();

            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                preDestino = transform.position + new Vector3(tamanhobloco, 0, 0);
                transform.rotation = Quaternion.Euler(0, 90, 0);

                PreJump();
            }
        }

        void PreJump()
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, 5, 0), transform.forward, out hit, tamanhobloco, whatIsObstacle);
            if (hit.collider == null)
            {
                isJumping = true;
                animator.SetTrigger("jump");
            }
        }

        void Jump()
        {
            destino = preDestino;
            _GameController.PlayFX(_GameController.fxJump[Random.Range(0, _GameController.fxJump.Length)]);
        }

        void OnJumpComplete()
        {
            isJumping = false;

            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, 5, 0), Vector3.down, out hit, tamanhobloco, whatIsGround);

            if (hit.collider != null)
            {
                switch (hit.collider.gameObject.tag)
                {
                    case "Chegada":
                        _GameController.ChangeGameState(GameState.FASECOMPLETA);
                        break;

                    case "Tronco":

                        break;

                    case "Agua":

                        break;
                }
            }
        }

        void MoverPersonagem()
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadePulo * Time.deltaTime);
        }

        void Morreu()
        {
            _GameController.ChangeGameState(GameState.GAMEOVER);
            _GameController.PlayFX(_GameController.fxHit);
            animator.SetTrigger("die");
        }

        /*public void TouchComand(string tecla)
        {

            if (isJumping == true)
            {
                return;
            }

            switch (tecla)
            {
                case "W":
                    preDestino = transform.position + new Vector3(0, 0, tamanhobloco);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    PreJump();
                    break;

                case "A":
                    preDestino = transform.position - new Vector3(tamanhobloco, 0, 0);
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    PreJump();
                    break;

                case "S":
                    preDestino = transform.position - new Vector3(0, 0, tamanhobloco);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    PreJump();
                    break;

                case "D":
                    preDestino = transform.position + new Vector3(tamanhobloco, 0, 0);
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    PreJump();
                    break;
            }
        }*/

        void TouchInputController()
        {
            if (isJumping)
            {
                return;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                float deltaX = touch.deltaPosition.x;
                float deltaY = touch.deltaPosition.y;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // Apenas registra o toque inicial, sem movimento imediato
                        break;

                    case TouchPhase.Moved:
                        // Verifica se o movimento horizontal é maior que o vertical
                        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
                        {
                            if (Mathf.Abs(deltaX) > 20) // Limiar para evitar pequenos deslizes
                            {
                                if (deltaX > 0) // Arrastando para a direita
                                {
                                    preDestino = transform.position + new Vector3(tamanhobloco, 0, 0);
                                    transform.rotation = Quaternion.Euler(0, 90, 0);
                                }
                                else // Arrastando para a esquerda
                                {
                                    preDestino = transform.position - new Vector3(tamanhobloco, 0, 0);
                                    transform.rotation = Quaternion.Euler(0, -90, 0);
                                }
                                PreJump();
                                return; // Sai da função para evitar andar para frente depois do arrasto
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                        // Se o usuário apenas tocou sem mover, anda para frente
                        preDestino = transform.position + new Vector3(0, 0, tamanhobloco);
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                        PreJump();
                        break;
                }
            }
        }

        #endregion

    }
}