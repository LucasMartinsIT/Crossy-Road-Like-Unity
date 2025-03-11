using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AprendaUnity;


public class Veiculo : MonoBehaviour
{
    public int velocidade;
    public int tamanhoVeiculo;
    private MapaProcedural _mapa;


    // Start is called before the first frame update
    void Start()
    {
        _mapa = FindObjectOfType(typeof(MapaProcedural)) as MapaProcedural;

        velocidade *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocidade * Time.deltaTime, 0, 0);

        switch (transform.rotation.eulerAngles.y)
        {
            case 0: // INDO PARA ESQUERDA
                if (transform.position.x <= _mapa.posEsquerda - tamanhoVeiculo)
                {
                    transform.position = new Vector3(_mapa.posDireita, transform.position.y, transform.position.z);
                }
                    break;
                

            case 180: // INDO PARA DIREITA
                if (transform.position.x >= _mapa.posDireita + tamanhoVeiculo)
                {
                    transform.position = new Vector3(_mapa.posEsquerda, transform.position.y, transform.position.z);
                }

                break;
        }
    }
}
