using Scripts.Logica;
using UnityEngine;

public class CreaElencoTeamReparto : MonoBehaviour
{
    public GameObject TeamPrefab;
    public GameObject MancanzaTeamPrefab;

    public void Compila(Reparto reparto)
    {
        Transform contenitore = gameObject.transform;
        
        foreach (Transform child in contenitore)
        {
            Destroy(child.gameObject);
        }

        if (reparto.teams.Count <= 0)
        {
            GameObject nuovoTeam = Instantiate(MancanzaTeamPrefab, contenitore);
            nuovoTeam.GetComponent<MancanzaTeams>().Compila(reparto);
            return;
        }
        
        int numeroTeam = 1;
        // Riempi la lista dei team
        foreach (var team in reparto.teams)
        {
            GameObject nuovoTeam = Instantiate(TeamPrefab, contenitore);
            nuovoTeam.GetComponent<CreaElencoDipendentiTeam>().Compila(team, numeroTeam);
            numeroTeam++;
        }
        
        Canvas.ForceUpdateCanvases();
        
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        if (rt == null) return;

        // salva il vecchio valore
        Vector2 oldMin = rt.anchorMin;

        // fai una piccola variazione
        rt.anchorMin = new Vector2(oldMin.x, oldMin.y + 0.00001f);

        // ritorna al valore originale
        rt.anchorMin = oldMin;
    }
}
