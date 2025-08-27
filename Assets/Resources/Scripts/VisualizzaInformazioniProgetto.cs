using System;
using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class VisualizzaInformazioniProgetto : MonoBehaviour
{
    Progetto progetto;
    
    public TMP_Text nomeProgetto;
    public TMP_Text difficolta;
    public TMP_Text repartiCoinvolti;
    public TMP_Text durata;
    public TMP_Text lavoro;
    public GameObject durataRimanente;
    public GameObject lavoroMancante;
    public TMP_Text anticipo;
    public TMP_Text settimanale;
    public TMP_Text finale;
    public TMP_Text detrazione;
    public TMP_Text rescissione;
    public TMP_Text percentuale;
    public TMP_Text ritardi;
    public Button terminaProgettoButton;
    
    public Azienda azienda;
    public GameObject elencoProgetti;
    
    public void OnEnable()
    {
        Clear();
    }
    
    public void Compila(Progetto progetto)
    {
        this.progetto = progetto;
        
        nomeProgetto.text = progetto.nome;
        // nomeProgetto.text = LocalizationSettings.StringDatabase.GetLocalizedString("Projects", progetto.nome);
        
        difficolta.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficolta") + progetto.difficolta switch
        {
            "bassa" => "<color=green>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficoltabassa") + "</color>",
            "media" => "<color=yellow>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficoltamedia") + "</color>",
            "alta" => "<color=red>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficoltaalta") + "</color>",
            _ => throw new System.ArgumentOutOfRangeException()
        };
        
        repartiCoinvolti.text = "";
        foreach (var reparto in progetto.repartiCoinvolti)
        {
            repartiCoinvolti.text += " - " + LocalizationSettings.StringDatabase.GetLocalizedString("Departments", reparto.ToString()) + "\n";
        }

        durata.text = $"<color=#D17A22>{progetto.durataRimanente}/{progetto.durata}</color>";
        durataRimanente.SetActive(true);
        durataRimanente.GetComponent<GestioneProgressBar>().ShowValue( Math.Max( (float) progetto.durataRimanente * 100 / progetto.durata, 0) );
        
        lavoro.text = $"<color=#2F6F4E>{progetto.lavoroMancante.ToString("F0")}/{progetto.lavoroRichiesto.ToString("F0")}</color>";
        lavoroMancante.SetActive(true);
        lavoroMancante.GetComponent<GestioneProgressBar>().ShowValue( Math.Max( (float) (progetto.lavoroRichiesto - progetto.lavoroMancante) * 100 / progetto.lavoroRichiesto, 0) );
        
        anticipo.text = "<color=green>+" + progetto.anticipo.ToString("F2") + "$</color>";
        settimanale.text = "<color=green>+" + progetto.settimanale.ToString("F2") + "$</color>";
        finale.text = "<color=green>+" + progetto.finale.ToString("F2") + "$</color>";
        detrazione.text = progetto.forcedEnd ? "<color=red>" + progetto.finaleDetrazione.ToString("F2") + "$</color>" : "<color=green>0.00$</color>";
        rescissione.text = "<color=red>" + progetto.detrazioneRescissione.ToString("F2") + "$</color>";
        percentuale.text = "<color=red>" + progetto.percentualeDetrazione.ToString("F0") + "%</color>";
        

        if (progetto.forcedEnd)
            ritardi.text = "<color=red>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "ritardono") + "</color>";
        else
            ritardi.text = "<color=green>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "ritardosi") + "</color>";
        
        terminaProgettoButton.gameObject.SetActive(true);
        terminaProgettoButton.GetComponent<Button>().onClick.RemoveAllListeners();
        terminaProgettoButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            azienda.OnTerminaProgetto(progetto, Clear, elencoProgetti.GetComponent<CompilatoreElencoProgetti>().OnEnable);
        });
    }

    public void Clear()
    {
        progetto = null;
        nomeProgetto.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nomeprogetto");
        difficolta.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficolta") + "--";
        repartiCoinvolti.text = "--";
        durata.text = " --";
        durataRimanente.SetActive(false);
        lavoro.text = " --";
        lavoroMancante.SetActive(false);
        anticipo.text = "--";
        settimanale.text = "--";
        finale.text = "--";
        detrazione.text = "--";
        rescissione.text = "--";
        percentuale.text = "--";
        ritardi.text = "--";
        terminaProgettoButton.gameObject.SetActive(false);
    }
}
