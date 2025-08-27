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
    public TMP_Text repartiCoinvolti;
    public TMP_Text durata;
    public GameObject durataRimanente;
    public GameObject lavoroMancante;
    public TMP_Text cifreIniziali;
    public TMP_Text cifreVarianti;
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
        
        repartiCoinvolti.text = "";
        foreach (var reparto in progetto.repartiCoinvolti)
        {
            repartiCoinvolti.text += " - " + LocalizationSettings.StringDatabase.GetLocalizedString("Departments", reparto.ToString()) + "\n";
        }

        durata.text = progetto.durataRimanente + "/" + progetto.durata;
        durataRimanente.GetComponent<GestioneProgressBar>().ShowValue( Math.Max( (float) progetto.durataRimanente * 100 / progetto.durata, 0) );
        
        lavoroMancante.GetComponent<GestioneProgressBar>().ShowValue( Math.Max( (float) (progetto.lavoroRichiesto - progetto.lavoroMancante) * 100 / progetto.lavoroRichiesto, 0) );
        
        cifreIniziali.text =
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "anticipo") + ": <color=green>" + progetto.anticipo.ToString("F2") + "$</color>\n" +
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "settimanale") + ": <color=green>" + progetto.settimanale.ToString("F2") + "$</color>\n" +
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "finale") + ": <color=green>" + progetto.finale.ToString("F2") + "$</color>\n";

        cifreVarianti.text =
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "detrazione") + ": <color=red>-" + progetto.finaleDetrazione.ToString("F2") + "$</color>\n" +
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "rescissione") + ": <color=red>-" + progetto.detrazioneRescissione.ToString("F2") + "$</color>\n" +
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "percentuale") + ": <color=red>" + progetto.percentualeDetrazione.ToString("F0") + "%</color>\n";


           
        terminaProgettoButton.GetComponent<Button>().onClick.RemoveAllListeners();
        terminaProgettoButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            azienda.OnTerminaProgetto(progetto, Clear, elencoProgetti.GetComponent<CompilatoreElencoProgetti>().OnEnable);
        });
    }

    public void Clear()
    {
        
    }
}
