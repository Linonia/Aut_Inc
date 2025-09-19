using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CreaElencoDipendentiTeam : MonoBehaviour
{
    public TMP_Text nomeTeam;
    public Image background;
    public GameObject PannelloDipendentePrefab;

    public void Compila(Team team, int numeroTeam)
    {
        nomeTeam.text = "Team " + numeroTeam;
        var coloreScritte = "#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", team.reparto.codice + "text");
        nomeTeam.color = ColorUtility.TryParseHtmlString(coloreScritte, out var colore) ? colore : Color.white;
        var coloreSfondo = "#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", team.reparto.codice + "bg");
        background.color = ColorUtility.TryParseHtmlString(coloreSfondo, out colore) ? colore : Color.white;
        Transform contenitore = gameObject.transform;
        // Riempi la lista dei dipendenti
        foreach (var dipendente in team.dipendenti)
        {
            GameObject nuovoDip = Instantiate(PannelloDipendentePrefab, contenitore);
            nuovoDip.GetComponent<CompilatorePannelloDipendente>().Compila(dipendente);
        }
    }
}
