using System;
using Scripts.Logica;
using UnityEngine;

public class CompilatoreElencoNuoviDipendenti : MonoBehaviour
{
    public GameObject prefabNuovoDipendente;
    public void OnEnable()
    {
        // Ripulisci la lista dei dipendenti
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        
        Transform contenitore = gameObject.transform;
        // Riempi la lista con dei nuovi dipendenti
        for (int i = 0; i < 4; i++)
        {   
            Dipendente dip = Dipendente.GeneraDipendente2();
            GameObject nuovoDipe = Instantiate(prefabNuovoDipendente, contenitore);
            nuovoDipe.GetComponent<CompilatoreEtichettaNuoviDipendenti>().Compila(dip);
        }
    }
}
