using System;
using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CompilatoreElencoNuoviDipendenti : MonoBehaviour
{
    public GameObject prefabNuovoDipendente;
    public Azienda azienda;
    public GameObject bottoneRicarica;
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

        var testo = bottoneRicarica.GetComponentInChildren<TMP_Text>();
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
    }

    public void Ricarica()
    {
        if (azienda.capitale > azienda.costoAssunzioneDipendente)
        {
            azienda.ShowWarningMessage("avvisoRicaricaDipendenti", () =>
            {
                if(azienda.ricercheDipendentiGratuite > 0)
                    azienda.capitale -= azienda.costoAssunzioneDipendente;
                azienda.ricercheDipendentiGratuite = azienda.ricercheDipendentiGratuite > 0 ? azienda.ricercheDipendentiGratuite - 1 : 0;
                azienda.aggiornaCapitale();
                OnEnable();
            }, () => {}, "conferma", "annulla", "avvisoRicaricaDipendenti");
        }
        else
        {
            azienda.ShowErrorMessage("erroreNoCapitaleAssunzione", () => {}, "chiudi", "");
        }
    }
}
