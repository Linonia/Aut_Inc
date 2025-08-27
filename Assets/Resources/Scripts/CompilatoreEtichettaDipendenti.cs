using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.UI;

public class CompilatoreEtichettaDipendenti : MonoBehaviour
{
    public Image foto;
    public TMP_Text nome;
    public TMP_Text reparto;
    public TMP_Text team;
    public GameObject umoreBar;
    public GameObject competenzaBar;
    public Button selezionaButton;
    
    public void Compila(Dipendente dipendente)
    {
        foto.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Foto/" + dipendente.foto);
        nome.text = dipendente.nome;
        if (dipendente.team != null)
        {
            reparto.text = LocalizationSettings.StringDatabase.GetLocalizedString("Departments", dipendente.team.reparto.codice);
            team.text = (dipendente.team.reparto.teams.IndexOf(dipendente.team) + 1).ToString();
            competenzaBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.competenza);
        }
        else
        {
            reparto.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunReparto");
            team.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunTeam");
        }
        
        umoreBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.umore);
        competenzaBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.competenza);
        

        GameObject infoDipendente = transform.parent.parent.parent.parent.Find("InfoDipendente").gameObject;

        selezionaButton.onClick.AddListener(() => infoDipendente.GetComponent<VisualizzaInformazioniDipendente>().Compila(dipendente));
    }
}
