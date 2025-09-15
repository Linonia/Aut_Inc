using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Logica;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.Localization.Settings;

public class VisualizzaInformazioniDipendente : MonoBehaviour
{
    Dipendente dipendente;
    
    public TMP_Text nome;
    public TMP_Text team;
    public TMP_Text reparto;
    public Image foto;

    public GameObject umoreBar;
    public GameObject competenzaBar;
    
    public Button licenziamentoButton;
    public TMP_Dropdown cambioTeamDropdown;
    
    public GameObject elencoDipendenti;

    public Azienda azienda;

    public GameObject clearPanel;
    
    public Dictionary<int, (Reparto, Team)> teamSelezionato;

    public void OnEnable()
    {
        Clear();
    }

    public void Compila(Dipendente dipendente)
    {
        clearPanel.SetActive(false);
        this.dipendente = dipendente;
        foto.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Foto/" + dipendente.foto);
        nome.text = dipendente.nome;
        if (dipendente.team != null)
        {
            reparto.text = "<color=#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", dipendente.team.reparto.codice + "text") + ">"+ LocalizationSettings.StringDatabase.GetLocalizedString("Departments", dipendente.team.reparto.codice) + "</color>";
            team.text = "Team: " +(dipendente.team.reparto.teams.IndexOf(dipendente.team) + 1);
            competenzaBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.competenza);
        }
        else
        {
            reparto.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunReparto");
            team.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunTeam");
        }
        
        umoreBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.umore);
        competenzaBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.competenza);

        /*
        for(int i = 0; i < dipendente.codiciDescrizioni.Length; i++)
        {
            string codice = dipendente.codiciDescrizioni[i];
            string descrizione = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", codice);
            gameObject.transform.Find("Des" + (i + 1)).GetComponent<TMP_Text>().text = descrizione;
        }*/

        
        licenziamentoButton.gameObject.SetActive(true);
        licenziamentoButton.onClick.AddListener(() =>
        {
            azienda.OnLicenziaDipendente(dipendente, Clear, elencoDipendenti.GetComponent<CompilatoreElencoDipendenti>().OnEnable);
        });

        
        licenziamentoButton.gameObject.SetActive(true);
        licenziamentoButton.onClick.AddListener(() =>
        {
            azienda.OnLicenziaDipendente(dipendente, Clear, elencoDipendenti.GetComponent<CompilatoreElencoDipendenti>().OnEnable);
        });
        gameObject.transform.Find("ChangeTeam").gameObject.SetActive(true);
        int index = 0;
        int selectedTeam = 0;
        cambioTeamDropdown.gameObject.SetActive(true);
        cambioTeamDropdown.ClearOptions();
        List<string> options = new List<string>{
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunTeamAssegnato")
        };
        teamSelezionato = new Dictionary<int, (Reparto, Team)>();
        teamSelezionato.Add(index, (null, null));
        if(dipendente.team == null)
            cambioTeamDropdown.GetComponent<Image>().color = new Color32(245, 245, 245, 255);
        foreach (var repartiNome in azienda.RepartiSbloccati())
        {
            var repNome = LocalizationSettings.StringDatabase.GetLocalizedString("Departments", azienda.reparti[repartiNome].codice);
            string colorHex = "#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", azienda.reparti[repartiNome].codice + "text");
            
            foreach (var team in azienda.reparti[repartiNome].teams)
            {
                index++;
                var teamNumber = team.reparto.teams.IndexOf(team) + 1;
                if (team == dipendente.team)
                {
                    cambioTeamDropdown.GetComponent<Image>().color =
                        ColorUtility.TryParseHtmlString("#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", azienda.reparti[repartiNome].codice + "bg"), out var c) ? c : new Color32(245, 245, 245, 255);
                    selectedTeam = index;
                }
                options.Add($"<color={colorHex}>{repNome} - {teamNumber}</color>");
                teamSelezionato.Add(index, (azienda.reparti[repartiNome], team));
            }
            index++;
            options.Add($"<color={colorHex}>{repNome} - {LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nuovoteam")}</color>");
            teamSelezionato.Add(index, (azienda.reparti[repartiNome], null));
        }
        cambioTeamDropdown.AddOptions(options);
        cambioTeamDropdown.onValueChanged.RemoveAllListeners();
        cambioTeamDropdown.value = selectedTeam;
        //cambioTeamDropdown.coloriTuple = teamSelezionato;
        cambioTeamDropdown.onValueChanged.AddListener(delegate { OnCambioTeam(); Ricarica(); });

    }
    
    private void SetLastOptionsBackground(Color color, int totalOptions)
    {
        Transform content = cambioTeamDropdown.template.Find("Viewport/Content");
        Debug.Log("Opzioni totali: " + content.childCount + " Mentre da colorare: " + totalOptions);
        if (content == null) return;

        int startIndex = Mathf.Max(0, content.childCount - totalOptions); // partiamo solo dalle ultime aggiunte

        for (int i = startIndex; i < content.childCount; i++)
        {
            Transform item = content.GetChild(i);
            Transform bgTransform = item.Find("Item Background"); // cerca il child
            if (bgTransform != null)
            {
                Image bg = bgTransform.GetComponent<Image>();
                if (bg != null)
                {
                    bg.color = color;
                }
            }
        }
    }

    
    public void Ricarica()
    {
        clearPanel.SetActive(false);
        foto.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Foto/" + dipendente.foto);
        nome.text = dipendente.nome;
        if (dipendente.team != null)
        {
            reparto.text = "<color=#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", dipendente.team.reparto.codice + "text") + ">"+ LocalizationSettings.StringDatabase.GetLocalizedString("Departments", dipendente.team.reparto.codice) + "</color>";
            team.text = "Team: " +(dipendente.team.reparto.teams.IndexOf(dipendente.team) + 1);
            competenzaBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.competenza);
        }
        else
        {
            reparto.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunReparto");
            team.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunTeam");
        }
        
        umoreBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.umore);
        competenzaBar.GetComponent<GestioneProgressBar>().ShowValue(dipendente.competenza);

        /*
        for(int i = 0; i < dipendente.codiciDescrizioni.Length; i++)
        {
            string codice = dipendente.codiciDescrizioni[i];
            string descrizione = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", codice);
            gameObject.transform.Find("Des" + (i + 1)).GetComponent<TMP_Text>().text = descrizione;
        }*/

        
        licenziamentoButton.gameObject.SetActive(true);
        licenziamentoButton.onClick.AddListener(() =>
        {
            azienda.OnLicenziaDipendente(dipendente, Clear, elencoDipendenti.GetComponent<CompilatoreElencoDipendenti>().OnEnable);
        });

        
        licenziamentoButton.gameObject.SetActive(true);
        licenziamentoButton.onClick.AddListener(() =>
        {
            azienda.OnLicenziaDipendente(dipendente, Clear, elencoDipendenti.GetComponent<CompilatoreElencoDipendenti>().OnEnable);
        });
        gameObject.transform.Find("ChangeTeam").gameObject.SetActive(true);
        int index = 0;
        int selectedTeam = 0;
        cambioTeamDropdown.gameObject.SetActive(true);
        cambioTeamDropdown.ClearOptions();
        List<string> options = new List<string>{
            LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nessunTeamAssegnato")
        };
        teamSelezionato = new Dictionary<int, (Reparto, Team)>();
        teamSelezionato.Add(index, (null, null));
        if(dipendente.team == null)
            cambioTeamDropdown.GetComponent<Image>().color = new Color32(245, 245, 245, 255);
        foreach (var repartiNome in azienda.RepartiSbloccati())
        {
            var repNome = LocalizationSettings.StringDatabase.GetLocalizedString("Departments", azienda.reparti[repartiNome].codice);
            string colorHex = "#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", azienda.reparti[repartiNome].codice + "text");
            
            foreach (var team in azienda.reparti[repartiNome].teams)
            {
                index++;
                var teamNumber = team.reparto.teams.IndexOf(team) + 1;
                if (team == dipendente.team)
                {
                    cambioTeamDropdown.GetComponent<Image>().color =
                        ColorUtility.TryParseHtmlString("#" + LocalizationSettings.StringDatabase.GetLocalizedString("DepartmentColor", azienda.reparti[repartiNome].codice + "bg"), out var c) ? c : new Color32(245, 245, 245, 255);
                    selectedTeam = index;
                }
                options.Add($"<color={colorHex}>{repNome} - {teamNumber}</color>");
                teamSelezionato.Add(index, (azienda.reparti[repartiNome], team));
            }
            index++;
            options.Add($"<color={colorHex}>{repNome} - {LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nuovoteam")}</color>");
            teamSelezionato.Add(index, (azienda.reparti[repartiNome], null));
        }
        cambioTeamDropdown.AddOptions(options);
        cambioTeamDropdown.onValueChanged.RemoveAllListeners();
        cambioTeamDropdown.value = selectedTeam;
        //cambioTeamDropdown.coloriTuple = teamSelezionato;
        cambioTeamDropdown.onValueChanged.AddListener(delegate { OnCambioTeam(); Ricarica(); });
    }
    
    public void Clear()
    {
        //dipendente = null;
        //nome.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nome");
        //team.text = "Team";
        //reparto.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "reparto");
        //foto.GetComponent<Image>().sprite = null;
        //// foto.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Foto/placeholder");
        //
        //umoreBar.GetComponent<GestioneProgressBar>().ShowValue(0);
        //competenzaBar.GetComponent<GestioneProgressBar>().ShowValue(0);
        //licenziamentoButton.gameObject.SetActive(false);
        //licenziamentoButton.onClick.RemoveAllListeners();
        //gameObject.transform.Find("ChangeTeam").gameObject.SetActive(false);
        //cambioTeamDropdown.ClearOptions();
        //cambioTeamDropdown.gameObject.SetActive(false);
        //teamSelezionato = new Dictionary<int, (Reparto, Team)>();
        //// Clear descriptions
        //for(int i = 0; i < 5; i++)
        //{
        //    gameObject.transform.Find("Des" + (i + 1)).GetComponent<TMP_Text>().text = "--";
        //}
        clearPanel.SetActive(true);
    }
    
    public void OnCambioTeam()
    {
        int selectedIndex = cambioTeamDropdown.value;
        if (teamSelezionato.ContainsKey(selectedIndex))
        {
            var (reparto, team) = teamSelezionato[selectedIndex];
            if (reparto == null)
            {
                if (dipendente.team == null) return; // No change needed, already not in a team
                azienda.RimuoviDipendente(dipendente);
            }
            else if (team != null)
            {
                if (dipendente.team == team) return; // No change needed
                if(team.PostiDisponibiliEsistenti())
                    azienda.SpostaDipendente(dipendente, team);
                else
                {
                    Debug.Log("Non puoi inserire in un team pieno");
                    // Non puoi inserire in un team pieno
                }
            }
            else
            {
                if (reparto.numeroPostiLiberi > 0)
                {
                    reparto.AggiungiTeam();
                    var newTeam = reparto.teams.Last();
                    azienda.SpostaDipendente(dipendente, newTeam);
                }
            }
        }
        
        Ricarica();

        elencoDipendenti.GetComponent<CompilatoreElencoDipendenti>().OnEnable();
    }
}
