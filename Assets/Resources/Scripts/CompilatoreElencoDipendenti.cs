using System;
using Scripts.Logica;
using UnityEngine;
using UnityEngine.UI;

public class CompilatoreElencoDipendenti : MonoBehaviour
{

    public GameObject prefabDipendente;
    public GameObject prefabDipendenteNuovo;
    public void OnEnable()
    {
        // Ripulisci la lista dei dipendenti
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        
        Transform contenitore = gameObject.transform;
        // Riempi la lista dei dipendenti
        foreach (var reparto in Azienda.RepartiSbloccati())
        {
            foreach (var team in Azienda.reparti[reparto].teams)
            {
                foreach (var dipendente in team.dipendenti)
                {
                    GameObject nuovoDip = Instantiate(prefabDipendente, contenitore);
                    nuovoDip.GetComponent<CompilatoreEtichettaDipendenti>().Compila(dipendente);
                }
            }
        }

        foreach (var dipendente in Azienda.dipendentiNonAssegnati)
        {
            GameObject nuovoDip = Instantiate(prefabDipendente, contenitore);
            nuovoDip.GetComponent<CompilatoreEtichettaDipendenti>().Compila(dipendente);
        }
        
        // Aggiungi il pulsante per assumere un nuovo dipendente
        GameObject nuovoDipendente = Instantiate(prefabDipendenteNuovo, contenitore);
        Button bottone = nuovoDipendente.transform.Find("Bottone").GetComponent<Button>();
        Transform root = nuovoDipendente.transform.parent.parent.parent.parent.parent;
        bottone.onClick.AddListener(() =>
        {
            // Disattivo DipendentiPanel
            root.Find("DipendentiPanel").gameObject.SetActive(false);
            // Attivo NuoviDipendenti
            root.Find("NuoviDipendenti").gameObject.SetActive(true);
        });
    }
}
