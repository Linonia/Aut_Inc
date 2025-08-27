using System;
using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CompilatoreEtichettaProgetti : MonoBehaviour
{

    public TMP_Text nome;
    public TMP_Text settimana;
    public GameObject SettimaneSlider;
    public GameObject LavoroSlider;
    public Button SelezionaButton;

    public void Compila(Progetto progetto)
    {
        nome.text = progetto.nome;
        //nome.text = LocalizationSettings.StringDatabase.GetLocalizedString("ProjectNames", progetto.nome);
        settimana.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "settimaneRimanenti") +" " + progetto.durataRimanente + "/" + progetto.durata;
        SettimaneSlider.GetComponent<GestioneProgressBar>().ShowValue(Math.Max((float)progetto.durataRimanente / progetto.durata * 100 , 0));
        LavoroSlider.GetComponent<GestioneProgressBar>().ShowValue(Math.Max((float)(progetto.lavoroRichiesto - progetto.lavoroMancante) / progetto.lavoroRichiesto * 100 , 0));
        GameObject infoProgetto = transform.parent.parent.parent.parent.Find("InfoProgetto").gameObject;
        
        SelezionaButton.onClick.AddListener(() => infoProgetto.GetComponent<VisualizzaInformazioniProgetto>().Compila(progetto));
    }
    
}
