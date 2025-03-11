using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;
using UnityEngine.XR;

namespace AprendaUnity
{
    public enum tipoItem
    {
        MOEDA
    }

    public enum GameState
    {
        GAMEPLAY,
        GAMEOVER,
        FASECOMPLETA,
        TITULO
    }

    public class GameController : MonoBehaviour
    {
        private MapaProcedural _MapaProcedural;
        private Cannon _Cannon;
        private Upgrades _Upgrades;

        public Mesh[] personagens;

        private Camera cam;
        public int moedasColetadas;
        private int tempoAtual;
        private bool isFollowCam = true;
        public GameState currentState;
        public Transform playerTransform;

        [Header("Configuração fase")]
        public int tempoMax;
        public int bonusTempoSeg;
        public int tamanho;
        public int addTempoTomate;

        [Header("Camera")]
        public float velocidadeCamera;
        public int margem;

        [Header("Limitadores Fase")]
        public Transform limiteEsque;
        public Transform limiteDirei;
        public Transform limiteTras;


        [Header("HUD")]
        public GameObject hudGameplay;
        public GameObject hudGameOver;
        public GameObject hudFaseCompleta;
        public GameObject hudTouch;
        public GameObject hudUpgrade;
        public TMP_Text moedasTMP;
        public TMP_Text tempoTMP;
        public TMP_Text faseAtualTMP;
        public TMP_Text bonusTempoTMP;
        public TMP_Text moedasColetadasTMP;
        public TMP_Text tempoAtualTMP;
        public TMP_Text faseCompletaTMP;
        public TMP_Text moedasTotalTMP;

        [Space(20)]
        public GameObject btnJogar;
        public GameObject btnVoltar;

        [Header("FX")]
        public AudioSource fx;
        public AudioClip fxColeta;
        public AudioClip fxHit;
        public AudioClip[] fxJump;
        public AudioClip fxAgua;




        #region FUNÇÕES UNITY
        void Start()
        {
            // --GAMBI PRA RESETAR PLAYERPREFS--
            //PlayerPrefs.DeleteAll();
            //PlayerPrefs.SetInt("moedasColetadas", 99999);

            // ---------------------------------

            _MapaProcedural = FindObjectOfType(typeof(MapaProcedural)) as MapaProcedural;
            _Cannon = FindObjectOfType(typeof(Cannon)) as Cannon;
            _Upgrades = FindObjectOfType(typeof(Upgrades)) as Upgrades;

            cam = Camera.main;

            _Upgrades.LoadUpgrades();


            Application.targetFrameRate = 60;
            hudGameOver.SetActive(false);
            hudFaseCompleta.SetActive(false);
            hudUpgrade.SetActive(false);
            hudGameplay.SetActive(true);
            hudTouch.SetActive(false);

            moedasTotalTMP.text = PlayerPrefs.GetInt("moedasColetadas").ToString("N0");
            faseAtualTMP.text = "Fase atual <color=#FFFF00>" + PlayerPrefs.GetInt("idFaseAtual").ToString() + "</color>";

            SetSkinPlayer();

        }

        private void LateUpdate()
        {
            if (currentState != GameState.GAMEPLAY)
            {
                return;
            }

            CameraController();


            limiteEsque.position = new Vector3(limiteEsque.position.x, limiteEsque.position.y, playerTransform.position.z);
            limiteDirei.position = new Vector3(limiteDirei.position.x, limiteDirei.position.y, playerTransform.position.z);
            limiteTras.position = new Vector3(playerTransform.position.x, limiteTras.position.y, limiteTras.position.z);
        }

        #endregion

        #region MINHAS FUNÇÕES

        public void ColetarItem(tipoItem item, int valor)
        {
            switch (item)
            {
                case tipoItem.MOEDA:
                    moedasColetadas += valor;
                    UpdateHud();
                    break;
            }

            PlayFX(fxColeta);

        }

        public void UpdateHud()
        {
            moedasTMP.text = moedasColetadas.ToString("N0");
            tempoTMP.text = tempoAtual.ToString();
        }

        void CameraController()
        {
            if (isFollowCam == false)
            {
                return;
            }
            float posX = Mathf.Clamp(playerTransform.position.x, limiteEsque.position.x + margem, limiteDirei.position.x - margem);
            Vector3 destinoCamera = new Vector3(0, playerTransform.position.y, playerTransform.position.z);
            cam.transform.position = Vector3.Lerp(cam.transform.position, playerTransform.position, velocidadeCamera * Time.deltaTime);

        }

        public void ChangeGameState(GameState newState)
        {
            currentState = newState;

            switch (currentState)
            {
                case GameState.FASECOMPLETA:
                    hudUpgrade.SetActive(true);
                    _Upgrades.ChangeUpgradeImage();
                    break;

                case GameState.GAMEOVER:
                    GameOver();
                    break;
            }
        }

        public void PlayFX(AudioClip clip)
        {
            fx.PlayOneShot(clip);
        }

        public void carregarCena(string nomeCena)
        {
            SceneManager.LoadScene(nomeCena);
        }

        void SetSkinPlayer()
        {
            playerTransform.GetComponentInChildren<MeshFilter>().mesh = personagens[PlayerPrefs.GetInt("idPersonagemAtual")];
        }

        public void FaseCompleta()
        {
            int idFaseAtual = PlayerPrefs.GetInt("idFaseAtual");
            isFollowCam = false;
            _MapaProcedural.StopCoroutine("SpawnEnemy");
            DestroyAllByTag("Inimigo");

            faseCompletaTMP.text = "Fase <color=#FFFF00>" + idFaseAtual + "</color> completa";


            StartCoroutine("AtualizaMoeda");

            hudTouch.SetActive(false);
            hudGameplay.SetActive(false);
            hudFaseCompleta.SetActive(true);

            PlayerPrefs.SetInt("idFaseAtual", idFaseAtual += 1);
        }

        void GameOver()
        {
            isFollowCam = false;

            StopCoroutine("tempoFase");
            _MapaProcedural.StopCoroutine("SpawnEnemy");
            DestroyAllByTag("Inimigo");

            hudGameOver.SetActive(true);
            hudTouch.SetActive(false);

            PlayerPrefs.SetInt("idFaseAtual", 0);

            // Remover os upgrades salvos
            for (int i = 0; i < 3; i++)  // Supondo que existam 3 upgrades possíveis
            {
                PlayerPrefs.DeleteKey("Upgrade_" + i + "_Level1");
                PlayerPrefs.DeleteKey("Upgrade_" + i + "_Level2");
            }
        }

        //DESTROI TODOS OS INIMIGOS QUANDO A FASE TERMINA
        public void DestroyAllByTag(string tag)
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objectsToDestroy)
            {
                Destroy(obj);
            }
        }


        IEnumerator AtualizaMoeda()
        {
            moedasColetadasTMP.text = moedasColetadas.ToString();

            int moedasTotal = PlayerPrefs.GetInt("moedasColetadas");

            for (int i = moedasColetadas; i >= 0; i--)
            {
                moedasColetadasTMP.text = i.ToString("N0");

                if (i > 0)
                {
                    moedasTotal += 1;
                }

                moedasTotalTMP.text = moedasTotal.ToString("N0");
                yield return new WaitForSeconds(0.05f);
            }

            PlayerPrefs.SetInt("moedasColetadas", moedasTotal);

            btnJogar.SetActive(true);
            btnVoltar.SetActive(true);
        }


        #endregion


        #region FUNÇÕES RETIRADAS
        /*
        IEnumerator tempoFase()
        {
            tempoTMP.text = tempoAtual.ToString();
            yield return new WaitForSeconds(1);
            tempoAtual -= 1;

            StartCoroutine("tempoFase");
        }

        IEnumerator GerarBonusTempo()
        {
            yield return new WaitForSeconds(1);

            int bonusPeloTempo = 0;
            int temp = 0;

            for (int i = tempoAtual; i >= 0; i--)
            {
                yield return new WaitForSeconds(0.05f);
                tempoAtualTMP.text = i.ToString();
                temp += 1;

                if (temp == 10)
                {
                    temp = 0;
                    bonusPeloTempo += 1;
                    bonusTempoTMP.text = "Bonus time:  <color=#00FFFF>" + bonusPeloTempo.ToString() + "</color>";
                }

            }

            moedasColetadas *= bonusPeloTempo;
            moedasColetadasTMP.text = moedasColetadas.ToString();

            int moedasTotal = PlayerPrefs.GetInt("moedasColetadas");


            for (int i = moedasColetadas; i >= 0; i--)
            {
                moedasColetadasTMP.text = i.ToString("N0");

                if (i > 0)
                {
                    moedasTotal += 1;
                }

                moedasTotalTMP.text = moedasTotal.ToString("N0");
                yield return new WaitForSeconds(0.05f);
            }

            PlayerPrefs.SetInt("moedasColetadas", moedasTotal);

            btnJogar.SetActive(true);
            btnVoltar.SetActive(true);
        }

        */
        #endregion

    }
}


