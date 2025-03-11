using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public float distancia;
    public GameObject preFab;
    public Transform posInicial;
    public int blocos;

    // Start is called before the first frame update
    void Start()
    {
        float carros = (blocos * 20) / distancia;
        print(posInicial.position.x + (blocos * 20));

        Vector3 posIni = new Vector3(posInicial.position.x + distancia, posInicial.position.y, posInicial.position.z);

        for (int i = 0; i < carros; i++)
        {
            Instantiate(preFab, posIni, transform.localRotation);
            posIni += new Vector3(distancia, 0, 0);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
