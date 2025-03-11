using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AprendaUnity;

public class Coletaveis : MonoBehaviour
{
    private GameController _GameController;
    public int valor;

    public tipoItem item;

    // Start is called before the first frame update
    void Start()
    {
        _GameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    #region

    private void Coletar()
    {
        _GameController.ColetarItem(item, valor);
        Destroy(this.gameObject);
    }

    #endregion
}
