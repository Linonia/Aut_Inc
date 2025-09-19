using System;
using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CompilatoreElencoDipendenti : MonoBehaviour
{

    public GameObject prefabDipendente;
    public GameObject prefabDipendenteNuovo;
    public Azienda azienda;
    
    public void OnEnable()
    {
        // Ripulisci la lista dei dipendenti
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        
        Transform contenitore = gameObject.transform;
        // Riempi la lista dei dipendenti
        foreach (var reparto in azienda.RepartiSbloccati())
        {
            foreach (var team in azienda.reparti[reparto].teams)
            {
                foreach (var dipendente in team.dipendenti)
                {
                    GameObject nuovoDip = Instantiate(prefabDipendente, contenitore);
                    nuovoDip.GetComponent<CompilatoreEtichettaDipendenti>().Compila(dipendente);
                }
            }
        }

        foreach (var dipendente in azienda.dipendentiLiberi)
        {
            GameObject nuovoDip = Instantiate(prefabDipendente, contenitore);
            nuovoDip.GetComponent<CompilatoreEtichettaDipendenti>().Compila(dipendente);
        }
        
        // Aggiungi il pulsante per assumere un nuovo dipendente
        GameObject nuovoDipendente = Instantiate(prefabDipendenteNuovo, contenitore);
        Button bottone = nuovoDipendente.transform.Find("Bottone").GetComponent<Button>();
        TMP_Text testo = nuovoDipendente.transform.Find("NuovoDipText").GetComponent<TMP_Text>();
        if (azienda.ricercheDipendentiGratuite > 0)
        {
            testo.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "assumiNuovoDipendente")
                         + "\n<color=red>0$</color>";
        }
        else
        {
            testo.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "assumiNuovoDipendente")
                         + "\n<color=red>-" + string.Format("{0:N2}", azienda.costoAssunzioneDipendente) + "$</color>";
        }
        Transform root = nuovoDipendente.transform.parent.parent.parent.parent.parent;
        bottone.onClick.AddListener(() =>
        {
            if (azienda.capitale > azienda.costoAssunzioneDipendente)
            {
                // Disattivo DipendentiPanel
                root.Find("DipendentiPanel").gameObject.SetActive(false);
                // Attivo NuoviDipendenti
                root.Find("NuoviDipendenti").gameObject.SetActive(true);
                // paga il costo di assunzione
                if(azienda.ricercheDipendentiGratuite > 0)
                    azienda.capitale -= azienda.costoAssunzioneDipendente;
                azienda.ricercheDipendentiGratuite = azienda.ricercheDipendentiGratuite > 0 ? azienda.ricercheDipendentiGratuite - 1 : 0;
                azienda.aggiornaCapitale();
            }
            else
            {
                azienda.ShowErrorMessage("erroreNoCapitaleAssunzione", () => {}, "chiudi", "");
            }
            
        });
    }
}
