using AprendaUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    private Cannon _Cannon;
    private GameController _GameController;

    public Image[] upgradeSlot;
    public int[] upgradeId;
    public TMP_Text[] upgradeTMPSlot;
    public string[] upgradeNameId;
    public Sprite[] upgradeImgLevel1;
    public Sprite[] upgradeImgLevel2;
    private bool[] thisSlotIsLevel1;
    private bool[] thisSlotIsLevel2;
    private bool[] haveUp1 = new bool[3];
    private bool[] haveUp2 = new bool[3];
    private int[] idUpLevelOnSlot;

    void Start()
    {
        _Cannon = FindObjectOfType<Cannon>();
        _GameController = FindObjectOfType<GameController>();

        // Verifique se os arrays são nulos antes de inicializar
        if (haveUp1 == null || haveUp1.Length == 0)
        {
            haveUp1 = new bool[upgradeImgLevel1.Length];
        }

        if (haveUp2 == null || haveUp2.Length == 0)
        {
            haveUp2 = new bool[upgradeImgLevel2.Length];
        }

        // Inicialize os outros arrays
        thisSlotIsLevel1 = new bool[3];
        thisSlotIsLevel2 = new bool[3];
        idUpLevelOnSlot = new int[upgradeSlot.Length];
    }

    public void ChangeUpgradeImage()
    {
        List<int> tempUpgrade = new List<int>(); //serve somente para comparar se ta repetindo algum upgrade

        for (int i = 0; i < upgradeSlot.Length; i++)
        {
            int rand = 0;
            do
            {
                rand = Random.Range(0, upgradeId.Length);
            } while (tempUpgrade.Contains(rand));

            if (haveUp1[rand] == false)
            {
                thisSlotIsLevel1[i] = true;
                print(thisSlotIsLevel1[i]);
                idUpLevelOnSlot[i] = rand;
                upgradeSlot[i].sprite = upgradeImgLevel1[rand];

                upgradeTMPSlot[i].text = upgradeNameId[rand];
            }
            else if (haveUp2[rand] == false && haveUp1[rand] == true)
            {
                thisSlotIsLevel2[i] = true;
                print(thisSlotIsLevel2[i]);
                idUpLevelOnSlot[i] = rand;
                upgradeSlot[i].sprite = upgradeImgLevel2[rand];

                upgradeTMPSlot[i].text = upgradeNameId[rand];
            }
            tempUpgrade.Add(rand);
        }
    }

    public void UpgradeChoice(int choice)
    {
        if (thisSlotIsLevel1[choice])
        {
            print("CHAMA 1");

            GerenciaUpgrade(idUpLevelOnSlot[choice], 0);
        }
        else if (thisSlotIsLevel2[choice])
        {
            print("CHAMA 2");
            GerenciaUpgrade(idUpLevelOnSlot[choice], 1);
        }

        SaveUpgrades();



        _GameController.hudUpgrade.SetActive(false);
        _GameController.FaseCompleta();
    }

    void GerenciaUpgrade(int idUpgrade, int levelUpgrade)
    {
        switch (idUpgrade)
        {
            case 0:
                UpgradeShootDamage(levelUpgrade);
                break;
            case 1:
                UpgradeShootRate(levelUpgrade);
                break;
            case 2:
                UpgradeShootRate(levelUpgrade);
                break;
            case 3:
                UpgradeShootRate(levelUpgrade);
                break;
            case 4:
                UpgradeShootRate(levelUpgrade);
                break;
        }
    }

    void UpgradeShootDamage(int nivelUpgrade)
    {
        if (nivelUpgrade == 0)
        {
            haveUp1[0] = true;
            _Cannon.damageShootType[0] = 75f;
        }
        else
        {
            haveUp2[0] = true;
            _Cannon.damage = 100f;
        }
    }

    void UpgradeShootRate(int nivelUpgrade)
    {
        if (nivelUpgrade == 0)
        {
            haveUp1[1] = true;
            _Cannon.shootRate = 2f;
        }
        else
        {
            haveUp2[1] = true;
            _Cannon.shootRate = 5f;
        }
    }

    // Salvar upgrades usando PlayerPrefs
    void SaveUpgrades()
    {
        print("CHAMOU SAVEUP");
        for (int i = 0; i < haveUp1.Length; i++)
        {
            PlayerPrefs.SetInt("Upgrade_" + i + "_Level1", haveUp1[i] ? 1 : 0);
            PlayerPrefs.SetInt("Upgrade_" + i + "_Level2", haveUp2[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // Carregar upgrades usando PlayerPrefs
    public void LoadUpgrades()
    {
        if (haveUp1 == null || haveUp2 == null)
        {
            Debug.LogError("Erro: haveUp1 ou haveUp2 estão nulos!");
            return;
        }

        print("CHAMOU LOADUP");
        for (int i = 0; i < haveUp1.Length; i++)
        {
            haveUp1[i] = PlayerPrefs.GetInt("Upgrade_" + i + "_Level1", 0) == 1;
            haveUp2[i] = PlayerPrefs.GetInt("Upgrade_" + i + "_Level2", 0) == 1;
        }

        // Aplicar os upgrades carregados
        for (int i = 0; i < haveUp1.Length; i++)
        {
            if (haveUp1[i])
            {
                GerenciaUpgrade(i, 0);
            }
            if (haveUp2[i])
            {
                GerenciaUpgrade(i, 1);
            }
        }
    }
}


