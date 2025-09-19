using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CompilatorePannelloDipendente : MonoBehaviour
{
    public Image foto;
    public TMP_Text nome;

    public GestioneProgressBar umore;
    public GestioneProgressBar competenza;

    public void Compila(Dipendente dipendente)
    {
        var coloreScritte = "#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", dipendente.repartoCodice + "text");
        foto.sprite = UnityEngine.Resources.Load<Sprite>("Images/Foto/" + dipendente.foto);

        nome.text = dipendente.nome;
        nome.color = ColorUtility.TryParseHtmlString(coloreScritte, out var colore) ? colore : Color.white;
        umore.ShowValue(dipendente.umore);
        if(dipendente.attesaCompetenza > 0)
            competenza.ShowValue(-1);
        else
            competenza.ShowValue(dipendente.competenza);
    }
}
