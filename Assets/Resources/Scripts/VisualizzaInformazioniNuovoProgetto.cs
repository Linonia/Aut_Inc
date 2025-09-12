using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class VisualizzaInformazioniNuovoProgetto : MonoBehaviour
{
    private Progetto progetto;
    
    public TMP_Text nomeProgetto;
    public TMP_Text difficolta;
    public TMP_Text repartiCoinvolti;
    public TMP_Text durata;
    public TMP_Text lavoro;
    public TMP_Text anticipo;
    public TMP_Text settimanale;
    public TMP_Text finale;
    public TMP_Text detrazione;
    public TMP_Text rescissione;
    public TMP_Text percentuale;
    public TMP_Text ritardi;
    public Button FirmaProgettoButton;
    
    public Azienda azienda;
    
    public void OnEnable()
    {
        Clear();
    }

    public void Compila(Progetto progetto, GameObject infoPanel)
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
            repartiCoinvolti.text += " - " + "<color=#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", reparto.ToString()) + ">"+ LocalizationSettings.StringDatabase.GetLocalizedString("Departments", reparto.ToString()) + "</color>" + "\n";
        }
        
        durata.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "settimaneRichieste") + $" <color=#D17A22>{progetto.durata}</color>";
        lavoro.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "lavoroRichiesto") + $" <color=#2F6F4E>{progetto.lavoroRichiesto.ToString("F0")}</color>";
        
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

        FirmaProgettoButton.gameObject.SetActive(true);
        FirmaProgettoButton.onClick.RemoveAllListeners();
        FirmaProgettoButton.onClick.AddListener(() =>
        {
            Destroy(infoPanel);
            azienda.OnFirmaProgetto(this.progetto, Clear);
        });
    }

    public void Clear()
    {
        progetto = null;
        nomeProgetto.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nomeprogetto");
        difficolta.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficolta") + "--";
        repartiCoinvolti.text = "--";
        durata.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "settimaneRichieste") + " --";
        lavoro.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "lavoroRichiesto") + " --";
        anticipo.text = "--";
        settimanale.text = "--";
        finale.text = "--";
        detrazione.text = "--";
        rescissione.text = "--";
        percentuale.text = "--";
        ritardi.text = "--";
        FirmaProgettoButton.gameObject.SetActive(false);
    }
}
