using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AprendaUnity
{
    public class MapaProcedural : MonoBehaviour
    {
        private GameController _GameController;
        private EnemyController _EnemyController;
        private Cannon _Cannon;
        Transform playerTransform;

        [Header("Blocos e Decorações")]
        public GameObject[] bloco;
        public int[] ocupaBloco;
        public bool[] podeDecorar;
        public bool[] podeElevar;
        public GameObject blocoFimDeFase;
        public Material materialFimFase;
        public GameObject[] decoracao;
        public GameObject[] coletaveis;
        public Transform decoracoesFase;
        public int percDecoracao;
        public int percDecoracoesLimite;
        public int percColetaveis;

        [Header("Regras para gerar")]
        public int qtdBloco;
        public int qtdBlocoLimite;
        public int linhasFase;
        public int linhasStart;
        public Transform blocosFase;
        private int tamanhoBloco;
        private int linhaAtual;
        private GameObject blocoTemp;

        [Header("Config Carros")]
        public int posEsquerda;
        public int posDireita;
        public float distanciaCarros;
        [SerializeField] private float qtdCarros;
        public GameObject carroPrefab;
        public SpawnScript[] s;

        [Header("Spawn Enemy")]
        public GameObject spawnA;
        public GameObject spawnB;
        public GameObject spawnC;
        public GameObject spawnD;
        public GameObject enemyPrefab;
        public float spawnEnemyInterval;
        private bool isSpawning = false;

        [Header("Inicializar var")]
        private float enemyHP;


        #region FUNÇÕES UNITY

        // Start is called before the first frame update
        void Start()
        {
            _GameController = FindObjectOfType(typeof(GameController)) as GameController;
            _EnemyController = FindObjectOfType(typeof(EnemyController)) as EnemyController;
            _Cannon = FindObjectOfType(typeof(Cannon)) as Cannon;

            tamanhoBloco = _GameController.tamanho;
            playerTransform = _GameController.playerTransform;

            RegraProgressaoFase(PlayerPrefs.GetInt("idFaseAtual"));
            qtdCarros = Mathf.RoundToInt((((qtdBloco + qtdBlocoLimite) * 2 + 1) * tamanhoBloco) / distanciaCarros);

            posDireita = (qtdBloco + qtdBlocoLimite) * tamanhoBloco;
            posEsquerda = (qtdBloco + qtdBlocoLimite) * tamanhoBloco * -1;

            enemyHP = _EnemyController.hp;


            GerarMapa();
            StartCoroutine("SpawnEnemy");

        }

        #endregion

        #region MINHAS FUNÇÕES

        void GerarMapa()
        {
            for (int i = 0; i < linhasStart; i++)
            {
                GerarInicioFase(i, 0);
            }

            linhaAtual = 1;
            for (int i = linhaAtual; i <= linhasFase; i++)
            {
                int idBlocoLinha = Random.Range(0, bloco.Length);
                GerarLinha(linhaAtual, idBlocoLinha);
                linhaAtual += ocupaBloco[idBlocoLinha];
            }

            for (int i = 0; i <= linhasStart; i++)
            {
                int idBlocoLinha = 999; // ID GAMBIARRA PARA BLOCO CHEGADA
                GerarLinha(linhaAtual, idBlocoLinha);
                linhaAtual += 1;
            }

            //INICIALIZA LIMITADORES NA POSIÇÃO CORRETA
            _GameController.limiteEsque.position -= new Vector3((qtdBloco + 1) * tamanhoBloco, 0, 0);
            _GameController.limiteDirei.position += new Vector3((qtdBloco + 1) * tamanhoBloco, 0, 0);
            _GameController.limiteTras.position -= new Vector3(0, 0, (linhasStart - 8) * tamanhoBloco);
        }

        void GerarInicioFase(int i, int idBloco)
        {
            blocoTemp = Instantiate(bloco[idBloco], transform.position - new Vector3(0, 0, i * tamanhoBloco), transform.localRotation, blocosFase);

            if (podeDecorar[idBloco] == true && i > 0)
            {
                SetDecoracao(blocoTemp.transform, false);
            }


            // GERAR BLOCOS PARA ESQUERDA
            int blocLinh = 0;
            for (int b = 1; b <= qtdBloco; b++)
            {
                blocoTemp = Instantiate(bloco[idBloco], transform.position - new Vector3(b * tamanhoBloco, 0, i * tamanhoBloco), transform.localRotation, blocosFase);
                blocLinh = b;

                if (podeDecorar[idBloco] == true)
                {
                    SetDecoracao(blocoTemp.transform, false);
                }
            }

            // GERAR BLOCO LIMITE PARA ESQUERDA
            for (int b = 1; b <= qtdBlocoLimite; b++)
            {
                blocLinh++;

                blocoTemp = Instantiate(bloco[idBloco], transform.position - new Vector3(blocLinh * tamanhoBloco, Random.Range(-5, 5), i * tamanhoBloco), transform.localRotation, blocosFase);
                blocoTemp.GetComponentInChildren<Renderer>().material = materialFimFase;

                if (podeDecorar[idBloco] == true)
                {
                    SetDecoracao(blocoTemp.transform, true);
                }
            }

            // GERAR BLOCOS PARA DIREITA
            for (int b = 1; b <= qtdBloco; b++)
            {
                blocoTemp = Instantiate(bloco[idBloco], transform.position + new Vector3(b * tamanhoBloco, 0, i * tamanhoBloco * -1), transform.localRotation, blocosFase);
                blocLinh = b;

                if (podeDecorar[idBloco] == true)
                {
                    SetDecoracao(blocoTemp.transform, false);
                }
            }

            // GERAR BLOCO LIMITE PARA DIREITA
            for (int b = 1; b <= qtdBlocoLimite; b++)
            {
                blocLinh++;
                blocoTemp = Instantiate(bloco[idBloco], transform.position + new Vector3(blocLinh * tamanhoBloco, Random.Range(-5, 5), i * tamanhoBloco * -1), transform.localRotation, blocosFase);
                blocoTemp.GetComponentInChildren<Renderer>().material = materialFimFase;

                if (podeDecorar[idBloco] == true)
                {
                    SetDecoracao(blocoTemp.transform, true);
                }
            }
        }

        void GerarLinha(int i, int idBloco)
        {
            float posY = 0;
            GameObject blocoInstanciar = null;

            if (idBloco == 999)
            {
                blocoInstanciar = blocoFimDeFase;
            }
            else
            {
                blocoInstanciar = bloco[idBloco];
            }

            // BLOCO CENTRAL

            blocoTemp = Instantiate(blocoInstanciar, transform.position + new Vector3(0, 0, i * tamanhoBloco), transform.localRotation, blocosFase);
            if (idBloco != 999)
            {
                if (podeDecorar[idBloco] == true && i > 1)
                {
                    SetDecoracao(blocoTemp.transform, false);
                }
                else if (podeDecorar[idBloco] == false)
                {
                    SetColetavel(blocoTemp.transform);
                }
            }

            // GERAR BLOCOS PARA ESQUERDA
            int blocLinh = 0;
            for (int b = 1; b <= qtdBloco; b++)
            {
                blocoTemp = Instantiate(blocoInstanciar, transform.position - new Vector3(b * tamanhoBloco, 0, i * tamanhoBloco * -1), transform.localRotation, blocosFase);
                blocLinh = b;

                if (idBloco != 999)
                {
                    if (podeDecorar[idBloco] == true)
                    {
                        SetDecoracao(blocoTemp.transform, false);
                    }
                    else if (podeDecorar[idBloco] == false)
                    {
                        SetColetavel(blocoTemp.transform);
                    }
                }
            }


            // GERAR BLOCO LIMITE PARA ESQUERDA
            for (int b = 1; b <= qtdBlocoLimite; b++)
            {
                blocLinh++;

                if (idBloco != 999 && podeElevar[idBloco] == true)
                {
                    posY = Random.Range(-5, 5);
                }

                blocoTemp = Instantiate(blocoInstanciar, transform.position - new Vector3(blocLinh * tamanhoBloco, posY, i * tamanhoBloco * -1), transform.localRotation, blocosFase);
                blocoTemp.GetComponentInChildren<Renderer>().material = materialFimFase;

                if (idBloco != 999 && podeDecorar[idBloco] == true)
                {
                    SetDecoracao(blocoTemp.transform, true);
                }
            }



            // GERAR BLOCOS PARA DIREITA
            for (int b = 1; b <= qtdBloco; b++)
            {
                blocoTemp = Instantiate(blocoInstanciar, transform.position + new Vector3(b * tamanhoBloco, 0, i * tamanhoBloco), transform.localRotation, blocosFase);
                blocLinh = b;

                if (idBloco != 999)
                {
                    if (podeDecorar[idBloco] == true)
                    {
                        SetDecoracao(blocoTemp.transform, false);
                    }
                    else if (podeDecorar[idBloco] == false)
                    {
                        SetColetavel(blocoTemp.transform);
                    }
                }
            }

            // GERAR BLOCO LIMITE PARA DIREITA
            for (int b = 1; b <= qtdBlocoLimite; b++)
            {
                blocLinh++;

                if (idBloco != 999 && podeElevar[idBloco] == true)
                {
                    posY = Random.Range(-5, 5);
                }

                blocoTemp = Instantiate(blocoInstanciar, transform.position + new Vector3(blocLinh * tamanhoBloco, posY, i * tamanhoBloco), transform.localRotation, blocosFase);
                blocoTemp.GetComponentInChildren<Renderer>().material = materialFimFase;

                if (idBloco != 999 && podeDecorar[idBloco] == true)
                {
                    SetDecoracao(blocoTemp.transform, true);
                }
            }

            if (idBloco != 999 && s[idBloco].isSpawn == true)
            {
                SetVeiculo(blocoTemp, idBloco);
            }



        }

        void SetVeiculo(GameObject blocoRef, int idBloco)
        {
            bool isLeft = false;

            if (s[idBloco].isReverse == true)
            {
                if (Random.Range(0, 100) < 50)
                {
                    isLeft = false;
                }
                else
                {
                    isLeft = true;
                }
            }

            if (s[idBloco].isTrain == true)
            {
                qtdCarros = 1;
            }
            else
            {
                qtdCarros = Mathf.RoundToInt((((qtdBloco + qtdBlocoLimite) * 2 + 1) * tamanhoBloco) / distanciaCarros);
            }


            Vector3 posIni = Vector3.zero;

            int speedLinha = Random.Range(s[idBloco].minSpeed, s[idBloco].maxSpeed);

            switch (isLeft)
            {
                case true: //MOVER DA DIREITA PARA ESQUERDA

                    posIni = new Vector3(posEsquerda + distanciaCarros, blocoRef.transform.position.y, blocoRef.transform.position.z);

                    if (s[idBloco].isTrain == true)
                    {
                        posIni += new Vector3(0, 0, tamanhoBloco);

                    }

                    for (int i = 0; i < qtdCarros; i++)
                    {
                        int idCarro = Random.Range(0, s[idBloco].prefabs.Length);
                        carroPrefab = s[idBloco].prefabs[idCarro];

                        GameObject temp = Instantiate(carroPrefab, posIni, transform.localRotation);
                        temp.GetComponent<Veiculo>().velocidade = speedLinha;

                        posIni += new Vector3(distanciaCarros, 0, 0);
                    }

                    break;

                case false: //MOVER DA ESQUERDA PARA DIREITA

                    posIni = new Vector3(posDireita - distanciaCarros, blocoRef.transform.position.y, blocoRef.transform.position.z);

                    if (s[idBloco].isTrain == true)
                    {
                        posIni += new Vector3(0, 0, tamanhoBloco);

                    }

                    for (int i = 0; i < qtdCarros; i++)
                    {
                        int idCarro = Random.Range(0, s[idBloco].prefabs.Length);
                        carroPrefab = s[idBloco].prefabs[idCarro];

                        GameObject temp = Instantiate(carroPrefab, posIni, transform.localRotation);
                        temp.GetComponent<Veiculo>().velocidade = speedLinha;
                        temp.transform.rotation = Quaternion.Euler(0, 180, 0);

                        posIni -= new Vector3(distanciaCarros, 0, 0);
                    }


                    //INSTANCIA A SEGUNDA FAIXA DE VEICULOS
                    if (s[idBloco].isDouble == true)
                    {
                        posIni = new Vector3(posEsquerda + distanciaCarros, blocoRef.transform.position.y, blocoRef.transform.position.z + tamanhoBloco);
                        speedLinha = Random.Range(s[idBloco].minSpeed, s[idBloco].maxSpeed);

                        for (int i = 0; i < qtdCarros; i++)
                        {
                            int idCarro = Random.Range(0, s[idBloco].prefabs.Length);
                            carroPrefab = s[idBloco].prefabs[idCarro];

                            GameObject temp = Instantiate(carroPrefab, posIni, transform.localRotation);
                            temp.GetComponent<Veiculo>().velocidade = speedLinha;


                            posIni += new Vector3(distanciaCarros, 0, 0);
                        }

                    }

                    break;

            }




        }

        void SetDecoracao(Transform t, bool isLimite)
        {
            if (isLimite == false)
            {
                if (Rand() <= percDecoracao)
                {
                    GameObject temp = Instantiate(decoracao[Random.Range(0, decoracao.Length)]);
                    temp.transform.position = t.position;
                    temp.transform.parent = decoracoesFase;
                }
                else // SE NAO TIVER DECORACAO, TENTA INSEIR COLETAVEL
                {
                    if (Rand() <= percColetaveis)
                    {
                        SetColetavel(t);
                    }
                }
            }
            else // SE FOR UM BLOCO DE LIMITE DA FASE
            {
                if (Rand() <= percDecoracoesLimite)
                {
                    GameObject temp = Instantiate(decoracao[Random.Range(0, decoracao.Length)]);
                    temp.transform.position = t.position;
                    temp.transform.parent = decoracoesFase;
                }
            }

        }

        void SetColetavel(Transform t)
        {
            if (Rand() <= percColetaveis)
            {
                GameObject temp = Instantiate(coletaveis[Random.Range(0, coletaveis.Length)]);
                temp.transform.position = t.position;
                temp.transform.parent = decoracoesFase;
            }
        }

        int Rand()
        {
            int r = Random.Range(0, 100);
            return r;
        }

        void RegraProgressaoFase(int idFase)
        {

            if (idFase > 5)
            {
                qtdBloco = 8;
                linhasFase = 25;
                spawnEnemyInterval = 1f;

                enemyHP = 200f;
            }
            else if (idFase > 3)
            {
                qtdBloco = 5;
                linhasFase = 10;
                spawnEnemyInterval = 3f;

                enemyHP = 100f;
            }
            else if (idFase <= 3)
            {
                qtdBloco = 3;
                linhasFase = 5;
                spawnEnemyInterval = 5f;

                enemyHP = 75f;
            }
        }

        public IEnumerator SpawnEnemy()
        {
            while (true)
            {
                if (!isSpawning)
                {
                    // Calcula posições para cada spawn
                    spawnA.transform.position = new Vector3(playerTransform.position.x + ((qtdBloco + 5) * tamanhoBloco), spawnA.transform.position.y, playerTransform.position.z + Random.Range(10, 100)); // Direita
                    spawnB.transform.position = new Vector3(playerTransform.position.x - ((qtdBloco + 5) * tamanhoBloco), spawnB.transform.position.y, playerTransform.position.z - Random.Range(10, 100)); // Esquerda
                    spawnC.transform.position = new Vector3(playerTransform.position.x + Random.Range(10, 100), spawnC.transform.position.y, playerTransform.position.z + ((qtdBloco + 5) * tamanhoBloco)); // Frente
                    spawnD.transform.position = new Vector3(playerTransform.position.x - Random.Range(10, 100), spawnD.transform.position.y, playerTransform.position.z - ((qtdBloco + 5) * tamanhoBloco)); // Trás

                    // Spawna os inimigos nas posições calculadas
                    Instantiate(enemyPrefab, spawnA.transform.position, Quaternion.identity);
                    Instantiate(enemyPrefab, spawnB.transform.position, Quaternion.identity);
                    Instantiate(enemyPrefab, spawnC.transform.position, Quaternion.identity);
                    Instantiate(enemyPrefab, spawnD.transform.position, Quaternion.identity);

                    // Aguarda o intervalo definido antes de permitir o próximo spawn
                    yield return new WaitForSeconds(spawnEnemyInterval);
                }

            }

            #endregion
        }


    }
}

