using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Logica;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Azienda : MonoBehaviour
{
    // Istanza singleton
    [HideInInspector] public Azienda instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Meomoria dell'azienda
    // informazioni di economia dell'azienda
    [HideInInspector] public int capitale; // capitale attuale
    [HideInInspector] public int costoDipendenteLibero = 1500; // costo di un dipendente libero mensile
    [HideInInspector] public int tasseMensile; // tasse mensili da pagare
    [HideInInspector] public int tempoDiminuzioneGuadagno = 12; // mesi dopo i quali il guadagno diminuisce se non si fanno upgrades
    
    // informazioni sul tempo
    [HideInInspector] public int anno = 1; // anno attuale
    [HideInInspector] public int mese = 1; // mese attuale
    [HideInInspector] public int settimana = 1; // settimana attuale
    [HideInInspector] public float timer = 4f;
    [HideInInspector] public float currentTimer = 4f;
    [HideInInspector] public bool pausa = true;
    [HideInInspector] private bool inPausa = true;
    
    // informazioni sui reparti
    [HideInInspector] public Dictionary<NomiReparti, Reparto> reparti = new Dictionary<NomiReparti, Reparto>(); // reparti dell'azienda
    [HideInInspector] public List<NomiReparti> repartiDaSbloccare = new List<NomiReparti>(); // reparti che si possono sbloccare
    [HideInInspector] public int costoReparto = 20000; // costo per sbloccare un reparto
    
    // informazioni sui dipendenti
    [HideInInspector] public List<Dipendente> dipendentiLiberi = new List<Dipendente>(); // dipendenti non assegnati a nessun team
    
    // informazioni sui progetti
    [HideInInspector] public List<Progetto> progettiInCorso = new List<Progetto>(); // progetti attualmente in corso
    [HideInInspector] public List<Progetto> progettiCompletatiInSettimana = new List<Progetto>(); // progetti completati nella settimana corrente
    [HideInInspector] public List<Progetto> progettiProposti = new List<Progetto>(); // progetti che si possono iniziare
    
    // oggetti di gestione della UI in game
    public TMP_Text tempo;
    public TMP_Text capitaleText;
    public TMP_Text dipendentiText;
    public Image pausaImage;
    public Image playImage;
    public TMP_Text cambioCapitale;
    public RectTransform capitalePanel;
    
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();
    
    // Metodi di gestione della memoria dell'azienda
    public void CreazioneAzienda()
    {
        dipendentiLiberi = new List<Dipendente>();
        reparti = new Dictionary<NomiReparti, Reparto>();
        
        reparti.Add(NomiReparti.AssistenzaESupportoTecnico, new Reparto("AssistenzaESupportoTecnico" ,new List<Categorie>
        {
            Categorie.ComunicazioneChiarezzaEspressiva,
            Categorie.ComunicazioneAdattabilitaComunicativa
        }, "desc1", this));
        
        reparti.Add(NomiReparti.SviluppoSoftware, new Reparto("SviluppoSoftware", new List<Categorie>
        {
            Categorie.CapacitaCognitiveProblemSolvingCreativo,
            Categorie.CapacitaCognitiveFlessibilitaCognitiva
        }, "desc4", this));
        
        reparti.Add(NomiReparti.UxEDesign, new Reparto("UxEDesign", new List<Categorie>
        {
            Categorie.SensibilitaSensorialePercezioneSensorialeFina,
            Categorie.SensibilitaSensorialeComfortAmbientale
        }, "desc3", this));
        
        reparti.Add(NomiReparti.ControlloQualita, new Reparto("ControlloQualita", new List<Categorie>
        {
            Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica
        }, "desc2", this));
        
        reparti.Add(NomiReparti.RicercaESviluppo, new Reparto("RicercaESviluppo", new List<Categorie>
        {
            Categorie.CapacitaCognitiveProblemSolvingCreativo,
            Categorie.CapacitaCognitiveFlessibilitaCognitiva,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica
        }, "desc5", this));
        
        reparti.Add(NomiReparti.Marketing, new Reparto("Marketing", new List<Categorie>
        {
            Categorie.SensibilitaSensorialePercezioneSensorialeFina,
            Categorie.SensibilitaSensorialeComfortAmbientale,
            Categorie.ComunicazioneChiarezzaEspressiva,
            Categorie.ComunicazioneAdattabilitaComunicativa
        }, "desc6", this));
        
        // Questi sono i reparti iniziali
        reparti[NomiReparti.SviluppoSoftware].ApriReparto();
        reparti[NomiReparti.AssistenzaESupportoTecnico].ApriReparto();
        
        // Reparti da sbloccare
        repartiDaSbloccare = new List<NomiReparti>
        {
            NomiReparti.UxEDesign,
            NomiReparti.ControlloQualita,
            NomiReparti.RicercaESviluppo,
            NomiReparti.Marketing,
            //NomiReparti.SviluppoHardwareEInnovazioniTecnologiche
        };

        tasseMensile = 10000; // Tasse mensili dell'azienda
        
        capitale = 100000; // Capitale iniziale dell'azienda
        
        anno = 1;
        mese = 1;
        settimana = 1;
        
        progettiInCorso = new List<Progetto>();
        
        //creazioneFlags
        flags = new Dictionary<string, bool>();
        flags.Add("licenziamento", false);
    }
    
    public void AperturaNuovoReparto(NomiReparti nomeReparto)
    {
        if (repartiDaSbloccare.Contains(nomeReparto) && capitale >= costoReparto)
        {
            reparti[nomeReparto].ApriReparto();
            repartiDaSbloccare.Remove(nomeReparto);
            tasseMensile += 1000;
            capitale -= costoReparto;
            costoReparto += 10000;
            // Aggiorna la UI
        }
    }

    // Aggiunge un dipendente al team se ci sono posti liberi nel reparto
    public void AggiungiDipendente(Dipendente dipendente, Team team)
    {
        if (team.reparto.numeroPostiLiberi > 0)
        {
            team.reparto.AggiungiDipendente(team, dipendente);
            dipendentiLiberi.Remove(dipendente);
        }
        else
        {
            Debug.Log("Non ci sono posti liberi nel reparto " + team.reparto.codice);
        }
    }

    // Rimuove un dipendente dal team e lo rende libero
    public void RimuoviDipendente(Dipendente dipendente)
    {
        dipendente.team.reparto.RimuoviDipendente(dipendente.team, dipendente);
        dipendentiLiberi.Add(dipendente);
    }

    public void SpostaDipendente(Dipendente dipendente, Team team)
    {
        if (team.PostiDisponibiliEsistenti())
        {
            if (dipendente.team != null)
            {
                RimuoviDipendente(dipendente);
            }
            AggiungiDipendente(dipendente, team);
        }
        else
        {
            Debug.Log("Non ci sono posti liberi nel reparto " + team.reparto.codice);
            ShowErrorMessage("teamPienoErrore", () => 
            {
                
            }, "chiudi");
        }
    }

    public int PagaDipendenti()
    {
        var costoTotale = 0;
        foreach (var reparto in reparti.Values)
        {
            if (!reparto.aperto) continue;
            costoTotale += reparto.CostoDipendenti();
        }

        costoTotale += dipendentiLiberi.Count * costoDipendenteLibero;
        return costoTotale;

        // eventuale aggiornamento UI
    }

    public void Aggiorna()
    {
        var guadagnoTotale = 0;
        
        foreach (var reparto in reparti.Values)
        {
            if (reparto.aperto)
            {
                reparto.Aggiorna();
            }
        }

        foreach (var progetto in progettiInCorso)
        {
            guadagnoTotale += progetto.AggiornaProgetto();
        }

        foreach (var progetto in progettiCompletatiInSettimana)
        {
            progetto.ChiudiProgetto();
        }
        progettiCompletatiInSettimana.Clear();
        
        settimana++;
        if (settimana > 4)
        {
            settimana = 1;
            mese++;
            if (mese > 12)
            {
                mese = 1;
                anno++;
            }
        }
        instance.aggiornaTempo();
        
        if (settimana == 1)
        {
            guadagnoTotale -= PagaDipendenti();
            guadagnoTotale -= tasseMensile;
        }
        
        if (capitale < 0)
        {
            //Debug.Log("Capitale insufficiente per pagare le tasse");
            // Messaggio di errore per capitale insufficiente
            // Game over
        }
        
        capitale += guadagnoTotale;
        aggiornaCapitale(guadagnoTotale, true);
    }

    // Potenzia un reparto
    public void PotenziaReparto(NomiReparti nomeReparto)
    {
        var reparto = reparti[nomeReparto];
        if (capitale < reparto.costoPotenziamento)
        {
            // Messaggio di errore per capitale insufficiente
            Debug.Log("Capitale insufficiente per potenziare il reparto");
        }
        capitale -= reparto.costoPotenziamento;
        reparto.AumentaLivello();
        tasseMensile += 500;
    }
    
    // Funzione per ottenere i reparti sbloccati
    public List<NomiReparti> RepartiSbloccati()
    {
        List<NomiReparti> repartiSbloccati = new List<NomiReparti>();
        foreach (var reparto in reparti)
        {
            if (reparto.Value.aperto)
            {
                repartiSbloccati.Add(reparto.Key);
            }
        }
        return repartiSbloccati;
    }
    
    // Funzione per firmare un progetto e aggiungerlo alla lista dei progetti in corso
    public void OnFirmaProgetto(Progetto progetto, Action clearAction)
    {
        clearAction();
        progetto.ApriProgetto();
    }
    
    // Funzione per terminare un progetto e rimuoverlo dalla lista dei progetti in corso
    public void OnTerminaProgetto(Progetto progetto, Action clearAction, Action reloadProjectList)
    {
        if (progetto == null) return;
        
        // Chiedo conferma per la terminazione del progetto
        ShowWarningMessage("terminaProgettoAvviso", () =>
        {
            this.RescindiContratto(progetto);
            clearAction();
            reloadProjectList();
        }, () => 
        {
            
        }, "conferma", "annulla");
    }
    
    // Funzione di rescissione di un contratto
    public void RescindiContratto(Progetto progetto)
    {
        if (progetto == null) return;
        
        // Aggiungo la penale al capitale
        capitale += progetto.rescissioneProgetto();
        
        // Messaggio di successo per la rescissione del contratto
    }
    
    // Funzione per licenziare un dipendente
    public void OnLicenziaDipendente(Dipendente dipendente, Action clearAction, Action reloadEmployeeList)
    {
        if (dipendente == null) return;
        
        // Chiedo conferma per il licenziamento
        ShowWarningMessage("licenziamentoAvviso", () =>
        {
            this.LicenziaDipendente(dipendente);
            clearAction();
            reloadEmployeeList();
        }, () => 
        {
            
        }, "conferma", "annulla", "licenziamento");
    }
    
    // Funzione per licenziare un dipendente
    public void LicenziaDipendente(Dipendente dipendente)
    {
        if ( dipendente.team != null)
        {
            dipendente.team.reparto.RimuoviDipendente(dipendente.team, dipendente);
        }
        // Rimuovo il dipendente dalla lista dei dipendenti non assegnati
        dipendentiLiberi.Remove(dipendente);
        aggiornaDipendenti();
        // Messaggio di successo per il licenziamento
    }
    
    // Funzione per assumere un dipendente
    public void OnAssumiDipendente(Dipendente dipendente, Action clearAction)
    {
        clearAction();
        this.AssumiDipendente(dipendente);
    }
    
    public void AssumiDipendente(Dipendente dipendente)
    {
        dipendentiLiberi.Add(dipendente);
        aggiornaDipendenti();
        // Messaggio di successo per l'assunzione
    }
    
    
    // Apertura dei pannelli dei dipendenti e dei progetti
    public void OpenEmployeePanel()
    {
        GameObject DipendentiPanel = gameObject.transform.Find("DipendentiPanel").gameObject;
        DipendentiPanel.SetActive(true);
    }

    public void OpenProjectPanel()
    {
        GameObject ProgettiPanel = gameObject.transform.Find("ProgettiPanel").gameObject;
        ProgettiPanel.SetActive(true);
    }
    
    // Funzioni di uscita del gioco
    public void OnExitGame()
    {
        ShowWarningMessage("uscitaDalGiocoAvvisoSalvataggio", ExitGame, () =>
        {
            GameObject WarningPanel = gameObject.transform.Find("WarningPanel").gameObject;
            WarningPanel.SetActive(false);
        }, "conferma", "annulla");
        
    }

    public void ExitGame()
    {
        var SceneManagerInstance = SceneManagerScript.instance;
        SceneManagerInstance.UnloadGameScene();
    }
    
    // Funzioni di caricamento del salvataggio
    public void OnLoadGame()
    {
        
    }
    
    // Funzioni di salvataggio del gioco
    public void OnSaveGame()
    {
        
    }

    // Funzioni di creazione di una nuova partita
    public void OnNewGame()
    {
        // Inizializzo le variabili dell'azienda
        CreazioneAzienda();
        
        Dipendente.CaricaJsonCategorie();
        Progetto.CaricaJsonProgetti();
        
        // Crea 4 dipendenti di prova
        for(int i = 0; i < 4; i++)
        {
            Dipendente dipendente = Dipendente.GeneraDipendente2();
            dipendentiLiberi.Add(dipendente);
        }
        
        
        // Creo dei progetti di prova
        for(int i = 0; i < 1; i++)
        {
            var progetto = Progetto.CreaProgetto(this);
            progettiInCorso.Add(progetto);
        }
    }
    
    // Funzione di show del pannello di errore
    public void ShowErrorMessage(string message, Action onClose, string confirmText = "chiudi", string flag = "")
    {
        GameObject ErrorPanel = gameObject.transform.Find("ErrorPanel").gameObject;
        // Gestisco il messaggio di errore
        string text = LocalizationSettings.StringDatabase.GetLocalizedString("ErrorTable", message);
        ErrorPanel.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        
        GameObject ConfirmButton = ErrorPanel.transform.Find("Confirm").gameObject;
        ConfirmButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", confirmText);
        ConfirmButton.GetComponent<Button>().onClick.AddListener(() => onClose());
    }
    
    // Funzione di show del pannello di avviso
    public void ShowWarningMessage(string message, Action onConfirm, Action onRetry, string confirmText = "continua", string retryText = "annulla", string flag = "")
{
    GameObject WarningPanel = gameObject.transform.Find("WarningPanel").gameObject;
    Toggle toggle = WarningPanel.transform.Find("Toggle").GetComponent<Toggle>();

    if (!string.IsNullOrEmpty(flag) && flags.ContainsKey(flag) && flags[flag])
    {
        onConfirm();
        return;
    }

    if (!string.IsNullOrEmpty(flag))
    {
        WarningPanel.transform.Find("TextShow").gameObject.SetActive(true);
        toggle.gameObject.SetActive(true);
        toggle.isOn = false;
    }
    else
    {
        WarningPanel.transform.Find("TextShow").gameObject.SetActive(false);
        toggle.gameObject.SetActive(false);
        toggle.isOn = false;
    }

    // Gestisco il messaggio di avviso
    string text = LocalizationSettings.StringDatabase.GetLocalizedString("ErrorTable", message);
    WarningPanel.transform.Find("Text").GetComponent<TMP_Text>().text = text;

    GameObject ConfirmButton = WarningPanel.transform.Find("Confirm").gameObject;
    ConfirmButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", confirmText);
    ConfirmButton.GetComponent<Button>().onClick.RemoveAllListeners();
    ConfirmButton.GetComponent<Button>().onClick.AddListener(() =>
    {
        if (!string.IsNullOrEmpty(flag) && toggle.isOn)
            flags[flag] = true;

        onConfirm();
        WarningPanel.SetActive(false);
    });

    GameObject RetryButton = WarningPanel.transform.Find("Retry").gameObject;
    RetryButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", retryText);
    RetryButton.GetComponent<Button>().onClick.RemoveAllListeners();
    RetryButton.GetComponent<Button>().onClick.AddListener(() =>
    {
        if (!string.IsNullOrEmpty(flag) && toggle.isOn)
            flags[flag] = true;

        onRetry();
        WarningPanel.SetActive(false);
    });

    WarningPanel.SetActive(true);
}

    
    // Funzione di aggiornamento del tempo di gioco
    public void aggiornaTempo()
    {
        if (LocalizationSettings.SelectedLocale.Identifier.Code == "it")
        {
            tempo.text = anno + "A " + mese + "M " + settimana + "S";
        }
        else
        {
            tempo.text = anno + "Y " + mese + "M " + settimana + "W";
        }
    }
    
    // Funzione di aggiornamento del numero di dipendenti
    public void aggiornaDipendenti()
    {
        var numeroTotali = 0;
		var numeroDip = 0;
        foreach (var reparto in reparti.Values)
        {
            if (reparto.aperto)
            {
				numeroDip += reparto.NumeroDipendenti();
                numeroTotali += reparto.numeroMaxDipendenti;
            }
        }
		numeroDip += dipendentiLiberi.Count;
        dipendentiText.text = numeroDip + "/" + numeroTotali;
    }
    
    // Funzione di aggiornamento del capitale
    public void aggiornaCapitale(int ammontareDifferenza = 0, bool apparizione = false)
    {
        if (apparizione)
        {
            // rosso se negativo, verde se positivo
            if (ammontareDifferenza < 0)
                cambioCapitale.text = "<color=red>- $" + string.Format("{0:N2}", -ammontareDifferenza) + "</color>";
            else
                cambioCapitale.text = "<color=green>+ $" + string.Format("{0:N2}", ammontareDifferenza) + "</color>";
            StartCoroutine(MuoviPanel());
        }
        // Aggiorno il testo del capitale principale
        capitaleText.text = "<color=green>$" + string.Format("{0:N2}", capitale) + "</color>";
    }
    
    // Animazione del cambiamento del capitale
    IEnumerator MuoviPanel(float durata = 0.5f, float attesa = 2f, float xTarget = 325f)
    {
        Vector2 posIniziale = capitalePanel.anchoredPosition;
        Vector2 posTarget = new Vector2(xTarget, posIniziale.y);
        // Vai avanti
        yield return StartCoroutine(FaiTransizione(posIniziale, posTarget, durata));
        // Aspetta
        yield return new WaitForSeconds(attesa);
        // Torna indietro
        yield return StartCoroutine(FaiTransizione(posTarget, posIniziale, durata));
    }
    
    IEnumerator FaiTransizione(Vector2 start, Vector2 end, float tempo)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / tempo;
            capitalePanel.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }
    }
    
    // Funzione di gestione del tempo di gioco
    // playClick toglie la pausa e segna che il gioco deve essere ripreso
    public void PlayClick()
    {
        inPausa = false;
        Play();
    }
    
    public void Play()
    {
        if (!inPausa)
        {
            pausaImage.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Icons/pauseDisable");
            playImage.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Icons/playEnable2");
            pausa = false;
            
        }
        
    }

    // segna che il gioco deve essere messo in pausa e che non deve essere tolta la pausa
    public void PauseClick()
    {
        inPausa = true;
        Pause();
    }

    // pause mette sempre in pausa il gioco
    public void Pause()
    {
        pausaImage.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Icons/pauseEnable2");
        playImage.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Icons/playDisable");
        pausa = true;
    }
    
    public void disableBottoniTempo()
    {
        playImage.GetComponent<Button>().interactable = false;
        pausaImage.GetComponent<Button>().interactable = false;
    }
    
    public void enableBottoniTempo()
    {
        playImage.GetComponent<Button>().interactable = true;
        pausaImage.GetComponent<Button>().interactable = true;
    }


    public void Start()
    {
        PauseClick();
        instance = this;
        // Inizializzo l'azienda
        CreazioneAzienda();
        currentTimer = 4f;
        Dipendente.CaricaJsonCategorie();
        Progetto.CaricaJsonProgetti();
        
        // Crea 4 dipendenti di prova
        for(int i = 0; i < 4; i++)
        {
            Dipendente dipendente = Dipendente.GeneraDipendente2();
            dipendentiLiberi.Add(dipendente);
        }
        
        // Creo dei progetti di prova
        for(int i = 0; i < 1; i++)
        {
            var progetto = Progetto.CreaProgetto(this);
            progettiInCorso.Add(progetto);
        }
        
        aggiornaDipendenti();
        aggiornaTempo();
        aggiornaCapitale(0);
    }
    
    // Funzione di update dello scorrere del tempo
    void Update()
    {
        // Se il gioco è in pausa → blocco il timer
        if (pausa) return;

        // Aggiorno il timer
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0f)
        {
            // Richiamo la funzione Aggiorna
            Aggiorna();

            // Resetto il timer
            currentTimer = timer;
        }
    }
}
