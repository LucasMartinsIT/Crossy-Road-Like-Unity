using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AprendaUnity
{
    public class Titulo : MonoBehaviour
    {
        public Mesh[] personagens;
        public int[] precoPersonagem;

        // Removido a declaração de personagemLiberado global e ajuste para verificar o PlayerPrefs diretamente
        public GameObject cadeado;
        public TMP_Text precoTMP;

        public Sprite[] imgBtn;
        public TMP_Text btnText;
        public Image btnImg;

        public MeshFilter personagem;
        public MeshFilter personagemEsquerda;
        public MeshFilter personagemDireita;

        private int idPersonagemAtual = 0;

        public TMP_Text totalMoedaTMP;


        // Start is called before the first frame update
        void Start()
        {

            // Garantir que o personagem 0 seja desbloqueado ao iniciar
            if (PlayerPrefs.GetInt("personagemLiberado_0", 0) == 0)
            {
                PlayerPrefs.SetInt("personagemLiberado_0", 1);
            }

            if (PlayerPrefs.GetInt("idFaseAtual") == 0)
            {
                PlayerPrefs.SetInt("idFaseAtual", 1);
            }

            idPersonagemAtual = PlayerPrefs.GetInt("idPersonagemAtual");

            personagem.mesh = personagens[idPersonagemAtual];
            int idTemp = idPersonagemAtual - 1;

            if (idTemp < 0)
            {
                idTemp = personagens.Length - 1;
            }
            personagemEsquerda.mesh = personagens[idTemp];

            idTemp = idPersonagemAtual + 1;
            if (idTemp >= personagens.Length)
            {
                idTemp = 0;
            }

            // Verifica se o personagem atual foi liberado ou não
            if (PlayerPrefs.GetInt("personagemLiberado_" + idPersonagemAtual, 0) == 1)
            {
                cadeado.SetActive(false);
            }
            else
            {
                precoTMP.text = precoPersonagem[idPersonagemAtual].ToString();
            }

            totalMoedaTMP.text = PlayerPrefs.GetInt("moedasColetadas").ToString("N0");
        }


        #region MINHAS FUNÇÕES 

        public void SelecionarPersonagem(int i)
        {
            idPersonagemAtual += i;

            if (idPersonagemAtual < 0)
            {
                idPersonagemAtual = personagens.Length - 1;
            }
            else if (idPersonagemAtual >= personagens.Length)
            {
                idPersonagemAtual = 0;
            }

            if (PlayerPrefs.GetInt("personagemLiberado_" + idPersonagemAtual, 0) == 1)
            {
                cadeado.SetActive(false);
                btnImg.sprite = imgBtn[1];
                btnText.text = "Jogar";
            }
            else
            {
                precoTMP.text = precoPersonagem[idPersonagemAtual].ToString();
                cadeado.SetActive(true);
                btnImg.sprite = imgBtn[0];
                btnText.text = "Liberar";
            }

            personagem.mesh = personagens[idPersonagemAtual];

            int idEsquerda = idPersonagemAtual - 1;

            if (idEsquerda < 0)
            {
                idEsquerda = personagens.Length - 1;
            }

            personagemEsquerda.mesh = personagens[idEsquerda];

            int idDireita = idPersonagemAtual + 1;

            if (idDireita > personagens.Length - 1)
            {
                idDireita = 0;
            }

            personagemDireita.mesh = personagens[idDireita];
        }

        public void btnAcao()
        {
            if (PlayerPrefs.GetInt("personagemLiberado_" + idPersonagemAtual, 0) == 1)
            {
                PlayerPrefs.SetInt("idPersonagemAtual", idPersonagemAtual);
                SceneManager.LoadScene("GamePlay");
            }
            else
            {
                ComprarPersonagem();
            }
        }

        void ComprarPersonagem()
        {
            int totalMoeda = PlayerPrefs.GetInt("moedasColetadas");

            if (totalMoeda >= precoPersonagem[idPersonagemAtual])
            {
                totalMoeda -= precoPersonagem[idPersonagemAtual];
                PlayerPrefs.SetInt("moedasColetadas", totalMoeda);

                totalMoedaTMP.text = totalMoeda.ToString("N0");

                // Salvando o personagem como liberado no PlayerPrefs
                PlayerPrefs.SetInt("personagemLiberado_" + idPersonagemAtual, 1);
                PlayerPrefs.Save();

                cadeado.SetActive(false);
                btnImg.sprite = imgBtn[1];
                btnText.text = "Jogar";
            }
            else
            {
                // Adicionar feedback para o usuário (som de erro, mensagem etc)
            }
        }

        #endregion
    }
}
