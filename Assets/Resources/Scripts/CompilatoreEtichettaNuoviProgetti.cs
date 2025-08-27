using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CompilatoreEtichettaNuoviProgetti : MonoBehaviour
{
    public TMP_Text nomeProgetto;
    public TMP_Text settimane;
    public TMP_Text lavoro;
    public TMP_Text difficolta;
    public Button selezionaButton;
    
    public void Compila(Progetto progetto)
    {
        nomeProgetto.text = progetto.nome;
        //nomeProgetto.text = LocalizationSettings.StringDatabase.GetLocalizedString("ProjectNames", progetto.nome);
        settimane.text = progetto.durata.ToString();
        lavoro.text = progetto.lavoroRichiesto.ToString();
        
        difficolta.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficolta") + progetto.difficolta switch
        {
            "bassa" => "<color=green>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficoltabassa") + "</color>",
            "media" => "<color=yellow>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficoltamedia") + "</color>",
            "alta" => "<color=red>" + LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "difficoltaalta") + "</color>",
            _ => throw new System.ArgumentOutOfRangeException()
        };
        
        GameObject infoProgetto = transform.parent.parent.parent.parent.Find("InfoProgetto").gameObject;
        selezionaButton.onClick.AddListener(() => infoProgetto.GetComponent<VisualizzaInformazioniNuovoProgetto>().Compila(progetto, gameObject));
    }
}
