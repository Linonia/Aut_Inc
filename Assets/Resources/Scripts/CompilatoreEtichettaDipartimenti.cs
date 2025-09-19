using Newtonsoft.Json;
using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CompilatoreEtichettaDipartimenti : MonoBehaviour
{
    [JsonIgnore] public TMP_Text numeroDipendenti;
    [JsonIgnore] public Button informazioni;
    [JsonIgnore] public NomiReparti nomeReparto;
    [JsonIgnore] public GameObject image;

    [JsonIgnore] public Button compra;
    [JsonIgnore] public TMP_Text compraText;
    
    [JsonIgnore] public Azienda azienda;

    [JsonIgnore] public bool sbloccato = false;


    public void aggiornaDipendenti()
    {
        if (sbloccato)
        {
            numeroDipendenti.text = azienda.reparti[nomeReparto].NumeroDipendenti() + "/" + azienda.reparti[nomeReparto].numeroMaxDipendenti;
        }
        else
        {
            numeroDipendenti.text = "---/---";
        }
    }

    public void repartoComprato()
    {
        gameObject.SetActive(true);
        compra.gameObject.SetActive(false);
        informazioni.gameObject.SetActive(true);
        sbloccato = true;
        image.gameObject.SetActive(true);
        numeroDipendenti.gameObject.SetActive(true);
        informazioni.onClick.AddListener(() =>
        {
            azienda.Pause();
            azienda.disableBottoniTempo();
            azienda.OpenDepartmentPanel(nomeReparto);
        });
        aggiornaDipendenti();
    }

    public void CompraReparto()
    {
        azienda.CompraNuovoReparto(nomeReparto);
    }

    public void SbloccaPossibilitaCompra()
    {
        gameObject.SetActive(true);
        compra.gameObject.SetActive(true);
        informazioni.gameObject.SetActive(false);
        sbloccato = false;
        image.gameObject.SetActive(false);
        numeroDipendenti.gameObject.SetActive(false);
        var text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "compra") + "<color=red> " +  string.Format("{0:N2}", -azienda.costoReparto) + "$</color>";
        compraText.text = text;
    }

    public void NascondiEtichetta()
    {
        gameObject.SetActive(false);
    }
}
