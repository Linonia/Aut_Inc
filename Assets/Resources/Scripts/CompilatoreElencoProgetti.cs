using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CompilatoreElencoProgetti : MonoBehaviour
{
    public GameObject prefabProgetto;
    public GameObject prefabProgettoNuovo;
    public Azienda azienda;

    public void OnEnable()
    {
        // Ripulisci la lista dei progetti
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        Transform contenitore = gameObject.transform;
        // Riempi la lista dei progetti
        var progetti = azienda.progettiInCorso.OrderBy(p => p.durataRimanente)
            .ToList();
        foreach (var progetto in progetti)
        {
            GameObject nuovoProgetto = Instantiate(prefabProgetto, contenitore);
            nuovoProgetto.GetComponent<CompilatoreEtichettaProgetti>().Compila(progetto);
        }

        // Aggiungi il pulsante per creare un nuovo progetto
        GameObject nuovoProgettoBtn = Instantiate(prefabProgettoNuovo, contenitore);
        Button bottone = nuovoProgettoBtn.transform.Find("Bottone").GetComponent<Button>();
        Transform root = nuovoProgettoBtn.transform.parent.parent.parent.parent.parent;
        bottone.onClick.AddListener(() =>
        {
            // Disattivo ProgettiPanel
            root.Find("ProgettiPanel").gameObject.SetActive(false);
            // Attivo NuoviProgetti
            root.Find("NuoviProgetti").gameObject.SetActive(true);
        });
    }
}
