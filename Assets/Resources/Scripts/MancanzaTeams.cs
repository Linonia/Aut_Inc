using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MancanzaTeams : MonoBehaviour
{
    public void Compila(Reparto reparto)
    {
        gameObject.GetComponent<TMP_Text>().color = ColorUtility.TryParseHtmlString("#" +
            LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", reparto.codice + "bg"),
            out var colore) ? colore : Color.white;
    }
}
