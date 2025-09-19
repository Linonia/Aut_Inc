using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class VisualizzaInformazioniDipartimento : MonoBehaviour
{
    public Azienda azienda;
    
    [HideInInspector] public Reparto reparto;
    
    public TMP_Text nome,
        descrizione,
        livello,
        dipendenti,
        teamConsigliato,
        teamNumeroConsigliato,
        teamAttuale,
        teamNumeroAttuale,
        costoDipendente,
        numeroCostoDipendente,
        miglioramento,
        numeroMiglioramento;

    public Image[] linee;
    
    public Button miglioramentoButton;

    public Image sfondoTeams;
    
    public CreaElencoTeamReparto elencoTeamReparto;

    public void Compila(NomiReparti repartoNome)
    {
        reparto = azienda.reparti[repartoNome];
        var coloreScritte = "#" +
            LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", reparto.codice + "text");
        var coloreSfondo = "#" +
            LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", reparto.codice + "bg");
        
        gameObject.GetComponent<Image>().color = ColorUtility.TryParseHtmlString(coloreSfondo, out var colore) ? colore : Color.white;
        gameObject.GetComponent<Outline>().effectColor = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        nome.text = LocalizationSettings.StringDatabase.GetLocalizedString("Departments", reparto.codice);
        nome.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        descrizione.text = LocalizationSettings.StringDatabase.GetLocalizedString("Departments", reparto.descrizione);
        descrizione.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        livello.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "livello") + ": " + reparto.livelloReparto + "/" + reparto.livelloRepartoMax;
        livello.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        dipendenti.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "dipendenti") + reparto.NumeroDipendenti() + "/" + reparto.numeroMaxDipendenti;
        dipendenti.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        teamConsigliato.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        teamNumeroConsigliato.text = reparto.numeroMinimoTeam + " / " + reparto.numeroMaxTeam;
        teamNumeroConsigliato.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        teamAttuale.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;

        teamNumeroAttuale.text = "" + reparto.NumeroTeamAttivi();
        teamNumeroAttuale.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        costoDipendente.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        numeroCostoDipendente.text = reparto.costoDipendente + " $";
        numeroCostoDipendente.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        if (reparto.livelloReparto >= reparto.livelloRepartoMax)
        {
            miglioramento.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
            numeroMiglioramento.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "massimoRaggiunto");
            numeroMiglioramento.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
            miglioramentoButton.gameObject.SetActive(false);
        }
        else
        {
            miglioramento.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
            numeroMiglioramento.text = reparto.costoPotenziamento + " $";
            numeroMiglioramento.color = ColorUtility.TryParseHtmlString("#FF0000", out colore) ? colore : Color.white;
            miglioramentoButton.gameObject.SetActive(true);
            miglioramentoButton.onClick.RemoveAllListeners();
            miglioramentoButton.onClick.AddListener(() =>
            {
                azienda.PotenziaReparto(repartoNome);
            });
        }
        
        foreach (var linea in linee)
        {
            linea.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        }
        
        sfondoTeams.color = ColorUtility.TryParseHtmlString(coloreScritte, out colore) ? colore : Color.white;
        
        elencoTeamReparto.Compila(reparto);
        
        //Canvas.ForceUpdateCanvases();
    }
}
