using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Scripts.Logica;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Azienda : MonoBehaviour
{
    // Istanza singleton
    [JsonIgnore] public Azienda instance;
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
    [HideInInspector] public int capitale = 50000; // capitale attuale
    [HideInInspector] public int costoDipendenteLibero = 1500; // costo di un dipendente libero mensile
    [HideInInspector] public int tasseMensile = 8000; // tasse mensili da pagare
    [HideInInspector] public int tempoDiminuzioneGuadagno = 18; // mesi dopo i quali il guadagno diminuisce se non si fanno upgrades
    
    // informazioni sul tempo
    [HideInInspector] public int anno = 1; // anno attuale
    [HideInInspector] public int mese = 1; // mese attuale
    [HideInInspector] public int settimana = 1; // settimana attuale
    [JsonIgnore][SerializeReference] public float timer = 8f;
    [JsonIgnore][SerializeReference] public float currentTimer = 8f;
    [JsonIgnore][SerializeReference] public bool pausa = true;
    [JsonIgnore][SerializeReference] public bool inPausa = true;
    
    // informazioni sui reparti
    [HideInInspector] public Dictionary<NomiReparti, Reparto> reparti = new Dictionary<NomiReparti, Reparto>(); // reparti dell'azienda
    [HideInInspector] public List<NomiReparti> repartiDaSbloccare = new List<NomiReparti>(); // reparti che si possono sbloccare
    [HideInInspector] public int costoReparto = 40000; // costo per sbloccare un reparto
    [HideInInspector] public int costoAssunzioneDipendente = 5000;
    [HideInInspector] public int costoLicenziamento = 3000;
    [HideInInspector] public int ricercheDipendentiGratuite = 2; // (sono in realtà 3, parte da 0)
    
    // informazioni sui dipendenti
    [HideInInspector] public List<Dipendente> dipendentiLiberi = new List<Dipendente>(); // dipendenti non assegnati a nessun team
    
    // informazioni sui progetti
    [HideInInspector] public List<Progetto> progettiInCorso = new List<Progetto>(); // progetti attualmente in corso
    [HideInInspector] public List<Progetto> progettiCompletatiInSettimana = new List<Progetto>(); // progetti completati nella settimana corrente
    [HideInInspector] public List<Progetto> progettiProposti = new List<Progetto>(); // progetti che si possono iniziare
    
    // oggetti di gestione della UI in game
    [JsonIgnore] public TMP_Text tempo;
    [JsonIgnore] public TMP_Text capitaleText;
    [JsonIgnore] public TMP_Text dipendentiText;
    [JsonIgnore] public Image pausaImage;
    [JsonIgnore] public Image playImage;
    [JsonIgnore] public GameObject weekSlider;
    
    [JsonIgnore] public GameObject capitalePanel;
    [JsonIgnore] private Queue<(int, List<string>)> codaEventi = new Queue<(int, List<string>)>();
    [JsonIgnore] private bool animazioneInCorso = false;

    [JsonIgnore] public GameObject salvataggioCompletato;

    [JsonIgnore] public CompilatoreEtichettaDipartimenti etichettaAssistenza;
    [JsonIgnore] public CompilatoreEtichettaDipartimenti etichettaSviluppo;
    [JsonIgnore] public CompilatoreEtichettaDipartimenti etichettaUxE;
    [JsonIgnore] public CompilatoreEtichettaDipartimenti etichettaQualita;
    [JsonIgnore] public CompilatoreEtichettaDipartimenti etichettaRicerca;
    [JsonIgnore] public CompilatoreEtichettaDipartimenti etichettaMarketing;
    
    [JsonIgnore] public CompilaTutorialPanel tutorialPanel;
    [JsonIgnore] public GameObject bancarottaPanel;
    
    [HideInInspector]public Dictionary<string, bool> warningFlags = new Dictionary<string, bool>();
    [HideInInspector]public Dictionary<string, bool> tutorialFlags = new Dictionary<string, bool>();
    
    public Azienda(){}
    // Metodi di gestione della memoria dell'azienda
    public void CreazioneAzienda()
    {
        dipendentiLiberi = new List<Dipendente>();
        reparti = new Dictionary<NomiReparti, Reparto>();
        
        reparti.Add(NomiReparti.AssistenzaESupportoTecnico, new Reparto("AssistenzaESupportoTecnico" ,new List<Categorie>
        {
            Categorie.ComunicazioneChiarezzaEspressiva,
            Categorie.ComunicazioneAdattabilitaComunicativa
        }, "desc1", this, etichettaAssistenza));
        
        reparti.Add(NomiReparti.SviluppoSoftware, new Reparto("SviluppoSoftware", new List<Categorie>
        {
            Categorie.CapacitaCognitiveProblemSolvingCreativo,
            Categorie.CapacitaCognitiveFlessibilitaCognitiva
        }, "desc4", this, etichettaSviluppo));
        
        reparti.Add(NomiReparti.UxEDesign, new Reparto("UxEDesign", new List<Categorie>
        {
            Categorie.SensibilitaSensorialePercezioneSensorialeFina,
            Categorie.SensibilitaSensorialeComfortAmbientale
        }, "desc3", this, etichettaUxE));
        
        reparti.Add(NomiReparti.ControlloQualita, new Reparto("ControlloQualita", new List<Categorie>
        {
            Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica
        }, "desc2", this, etichettaQualita));
        
        reparti.Add(NomiReparti.RicercaESviluppo, new Reparto("RicercaESviluppo", new List<Categorie>
        {
            Categorie.CapacitaCognitiveProblemSolvingCreativo,
            Categorie.CapacitaCognitiveFlessibilitaCognitiva,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica
        }, "desc5", this, etichettaRicerca));
        
        reparti.Add(NomiReparti.Marketing, new Reparto("Marketing", new List<Categorie>
        {
            Categorie.SensibilitaSensorialePercezioneSensorialeFina,
            Categorie.SensibilitaSensorialeComfortAmbientale,
            Categorie.ComunicazioneChiarezzaEspressiva,
            Categorie.ComunicazioneAdattabilitaComunicativa
        }, "desc6", this, etichettaMarketing));
        
        // Reparti da sbloccare
        repartiDaSbloccare = new List<NomiReparti>
        {
            NomiReparti.UxEDesign,
            NomiReparti.ControlloQualita,
            NomiReparti.RicercaESviluppo,
            NomiReparti.Marketing,
            //NomiReparti.SviluppoHardwareEInnovazioniTecnologiche
        };
        
        // Sblocco il primo reparto
        AperturaNuovoReparto(NomiReparti.AssistenzaESupportoTecnico);
        AperturaNuovoReparto(NomiReparti.SviluppoSoftware);
        
        progettiInCorso = new List<Progetto>();
        
        capitale = 50000;
        costoDipendenteLibero = 1500;
        tasseMensile = 8000;
        tempoDiminuzioneGuadagno = 18;
        settimana = 1;
        mese = 1;
        anno = 1;
        timer = 8f;
        currentTimer = 8f;
        pausa = true;
        inPausa = true;
        costoReparto = 40000;
        costoAssunzioneDipendente = 5000;
        costoLicenziamento = 3000;
        ricercheDipendentiGratuite = 2;
        
        //creazioneFlags
        warningFlags = new Dictionary<string, bool>();
        warningFlags.Add("licenziamento", false);
        warningFlags.Add("rescissione", false);
        warningFlags.Add("cambioTeam", false);
        warningFlags.Add("acquistoReparto", false);
        warningFlags.Add("avvisoRicaricaDipendenti", false);
        warningFlags.Add("potenziaReparto", false);
        
        //tutorial
        tutorialFlags.Add("introduzione1", false);
        tutorialFlags.Add("introduzione2", false);
        tutorialFlags.Add("introduzione3", false);
        tutorialFlags.Add("introduzione4", false);
        tutorialFlags.Add("introduzione5", false);
        tutorialFlags.Add("introduzione6", false);
        tutorialFlags.Add("dipartimenti1", false);
        tutorialFlags.Add("dipartimenti2", false);
        tutorialFlags.Add("dipartimenti3", false);
        tutorialFlags.Add("dipendenti1", false);
        tutorialFlags.Add("dipendenti2", false);
        tutorialFlags.Add("nuoviDipendenti1", false);
        tutorialFlags.Add("progetti1", false);
        tutorialFlags.Add("progetti2", false);
        tutorialFlags.Add("progetti3", false);
        tutorialFlags.Add("nuoviProgetti1", false);
    }

    public void CompraNuovoReparto(NomiReparti nomeReparto)
    {
        if (repartiDaSbloccare.Contains(nomeReparto) && capitale >= costoReparto)
        {
            ShowWarningMessage("acquistoRepartoAvviso", () =>
            {
                //capitale -= costoReparto;
                aggiornaCapitale(-costoReparto, new List<string>{"acquistoReparto"});
                tasseMensile += 5000;
                costoReparto += costoReparto;
                tempoDiminuzioneGuadagno = Math.Min(tempoDiminuzioneGuadagno + 18, 18);
                AperturaNuovoReparto(nomeReparto);
                aggiornaDipendenti();
            }, () => 
            {
                
            }, "conferma", "annulla", "acquistoReparto", new Dictionary<string, string>
            {
                { "costo", "<color=red> " +  string.Format("{0:N2}", costoReparto) + "$</color>" }
            });
        }
        else
        {
            ShowErrorMessage("erroreAcquistoReparto", () =>
            {
                
            }, "chiudi");
        }
    }
    
    public void AperturaNuovoReparto(NomiReparti nomeReparto)
    {
        
        reparti[nomeReparto].ApriReparto();
        repartiDaSbloccare.Remove(nomeReparto);
        if (repartiDaSbloccare.Count > 0)
        {
            var prossimoReparto = repartiDaSbloccare[0]; // il primo della lista
            reparti[prossimoReparto].SbloccaAcquisto();
        }
        // Aggiorna la UI
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
            ShowErrorMessage("erroreDipartimentoPieno", () => {});
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
        if (team.PostiDisponibiliEsistenti() || dipendente.team != null && dipendente.team.reparto == team.reparto )
        {
            if (dipendente.team != null)
            {
                RimuoviDipendente(dipendente);
            }
            AggiungiDipendente(dipendente, team);
        }
        else
        {
            ShowErrorMessage("erroreDipartimentoPieno", () => {});
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
        List<string> motiv = new List<string>();
        
        foreach (var reparto in reparti.Values)
        {
            if (reparto.aperto)
            {
                reparto.Aggiorna();
            }
        }

        foreach (var dip in dipendentiLiberi)
        {
            dip.Aggiorna();
        }

        var guadagniProgetto = 0;
        foreach (var progetto in progettiInCorso)
        {
            guadagniProgetto += progetto.AggiornaProgetto2();
        }
        
        motiv.Add("compensoSettimanale");
        
        guadagnoTotale += guadagniProgetto;

        var chiusureProgetti = 0;
        var almenoUnoChiuso = false;
        foreach (var progetto in progettiInCorso)
        {
            var singoloProgetto = progetto.ChiudiProgetto2();
            chiusureProgetti += singoloProgetto;
            almenoUnoChiuso = singoloProgetto != 0 || almenoUnoChiuso;
        }

        if (almenoUnoChiuso)
        {
            if (chiusureProgetti > 0)
            {
                motiv.Add("pagamentoFinaleBuono");
            }
            else if (chiusureProgetti < 0)
            {
                motiv.Add("pagamentoFinaleBrutto");
            }
        }
        
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
        tempoDiminuzioneGuadagno = Math.Max(-12, tempoDiminuzioneGuadagno - 1);
        GeneraProgettiSettimanali();
        
        if (settimana == 1)
        {
            guadagnoTotale -= PagaDipendenti();
            guadagnoTotale -= tasseMensile;
            motiv.Add("tasse");
        }
        
        aggiornaCapitale(guadagnoTotale, motiv);
    }

    public void Bancarotta()
    {
        bancarottaPanel.SetActive(true);
        PauseClick();
        codaEventi.Clear();
        StopAllCoroutines();
        animazioneInCorso = false;
    }

    // Potenzia un reparto
    public void PotenziaReparto(NomiReparti nomeReparto)
    {
        var reparto = reparti[nomeReparto];
        if (capitale < reparto.costoPotenziamento)
        {
            ShowErrorMessage("errorePotenziamentoReparto", () => 
            {
                
            }, "chiudi");
        }
        else
        {
            ShowWarningMessage(
                "potenziamentoRepartoAvviso", 
                () =>
                {
                    //capitale -= reparto.costoPotenziamento;
                    aggiornaCapitale(-reparto.costoPotenziamento, new List<string>{"potenziamentoReparto"});
                    tempoDiminuzioneGuadagno = Math.Min(tempoDiminuzioneGuadagno + 18, 18);
                    reparto.AumentaLivello();
                    tasseMensile += 1000;
                }, 
                () => 
                {
                    
                }, 
                "conferma", 
                "annulla", 
                "potenziaReparto", 
                new Dictionary<string, string>
                {
                    { "costo", "<color=red> " +  string.Format("{0:N2}", reparto.costoPotenziamento) + "$</color>" }
                });
        }
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
    
    // Funzione per generare nuovi progetti settimanali
    public void GeneraProgettiSettimanali()
    {
        progettiProposti.Clear();
        int numeroProgetti = 3 + RepartiSbloccati().Count;
        for (int i = 0; i < numeroProgetti; i++)
        {   
            Progetto progetto = Progetto.CreaProgetto(this);
            progettiProposti.Add(progetto);
        }
    }
    
    // Funzione per firmare un progetto e aggiungerlo alla lista dei progetti in corso
    public void OnFirmaProgetto(Progetto progetto, Action clearAction)
    {
        clearAction();
        progetto.ApriProgetto();
    }
    
    // Funzione per terminare un progetto e rimuoverlo dalla lista dei progetti in corso con flag
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
            
        }, "conferma", "annulla", "rescissione");
    }
    
    // Funzione di rescissione di un contratto
    public void RescindiContratto(Progetto progetto)
    {
        if (progetto == null) return;
        
        // Aggiungo la penale al capitale
        var costo = progetto.rescissioneProgetto();
        aggiornaCapitale(costo, new List<string>{"progettoRescisso"});
        // Messaggio di successo per la rescissione del contratto
    }
    
    // Funzione per licenziare un dipendente con flag
    public void OnLicenziaDipendente(Dipendente dipendente, Action clearAction, Action reloadEmployeeList)
    {
        if (dipendente == null) return;
        
        // Chiedo conferma per il licenziamento
        ShowWarningMessage("licenziamentoAvviso",
            () =>
                {
                    this.LicenziaDipendente(dipendente);
                    clearAction();
                    reloadEmployeeList();
                },
            () => 
        {
            
        },
            "conferma",
            "annulla",
            "licenziamento",
            new Dictionary<string, string>{
                    {"costo", "<color=red>-"+ costoLicenziamento + "$</color>"}
                }
            );
        
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
        //capitale -= costoLicenziamento;
        aggiornaDipendenti();
        aggiornaCapitale(-costoLicenziamento, new List<string>{"licenziamento"});
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
        tutorialPanel.MostraTutorial("dipendenti1");
    }

    public void OpenProjectPanel()
    {
        GameObject ProgettiPanel = gameObject.transform.Find("ProgettiPanel").gameObject;
        ProgettiPanel.SetActive(true);
        tutorialPanel.MostraTutorial("progetti1");
    }
    
    public void OpenDepartmentPanel(NomiReparti reparto)
    {
        GameObject RepartiPanel = gameObject.transform.Find("Dipartimento").gameObject;
        RepartiPanel.SetActive(true);
        RepartiPanel.GetComponent<VisualizzaInformazioniDipartimento>().Compila(reparto);
        tutorialPanel.MostraTutorial("dipartimenti1");
        //RepartiPanel.GetComponent<CompilatorePannelloReparti>().setDepartment(reparto);
        //RepartiPanel.GetComponent<CompilatorePannelloReparti>().AggiornaUI();
    }

    public void RicaricaDepartimentPanel()
    {
        GameObject RepartiPanel = gameObject.transform.Find("Dipartimento").gameObject;
        RepartiPanel.SetActive(true);
        RepartiPanel.GetComponent<VisualizzaInformazioniDipartimento>().Ricarica();
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
        SalvataggioAzienda.Carica(this);

        // Inizializza eventuali null
        reparti ??= new Dictionary<NomiReparti, Reparto>();
        dipendentiLiberi ??= new List<Dipendente>();
        repartiDaSbloccare ??= new List<NomiReparti>();
        progettiInCorso ??= new List<Progetto>();
        progettiCompletatiInSettimana ??= new List<Progetto>();
        progettiProposti ??= new List<Progetto>();
        warningFlags ??= new Dictionary<string,bool>();

        // Ripristina UI dei reparti solo se esistono
        if (reparti.TryGetValue(NomiReparti.AssistenzaESupportoTecnico, out var r1))
            r1.OnAfterLoad(this, etichettaAssistenza);
        if (reparti.TryGetValue(NomiReparti.SviluppoSoftware, out var r2))
            r2.OnAfterLoad(this, etichettaSviluppo);
        if (reparti.TryGetValue(NomiReparti.UxEDesign, out var r3))
            r3.OnAfterLoad(this, etichettaUxE);
        if (reparti.TryGetValue(NomiReparti.ControlloQualita, out var r4))
            r4.OnAfterLoad(this, etichettaQualita);
        if (reparti.TryGetValue(NomiReparti.RicercaESviluppo, out var r5))
            r5.OnAfterLoad(this, etichettaRicerca);
        if (reparti.TryGetValue(NomiReparti.Marketing, out var r6))
            r6.OnAfterLoad(this, etichettaMarketing);

        Dipendente.CaricaJsonCategorie();
        Progetto.CaricaJsonProgetti();
        // Aggiorna la UI generale
        AggiornaUIIniziale();
    }


    public void AggiornaUIIniziale()
    {
        capitaleText.text = capitale >= 0
            ? "<color=green>$" + string.Format("{0:N2}", capitale) + "</color>"
            : "<color=red>$" + string.Format("{0:N2}", capitale) + "</color>";
        aggiornaDipendenti();
        aggiornaTempo();
        weekSlider.GetComponent<GestioneProgressBar>().ShowValue(0);
    }
    
    // Funzioni di salvataggio del gioco
    public void OnSaveGame()
    {
        try
        {
            SalvataggioAzienda.Salva(this);
            StartCoroutine(MostraAvvisoSalvataggioCompletato());
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante il salvataggio: " + e.Message);
            ShowErrorMessage("erroreSalvataggio", () =>
            {
                
            }, "chiudi");
        }
    }
    
    private IEnumerator MostraAvvisoSalvataggioCompletato()
    {
        salvataggioCompletato.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f); // rimane visibile 3 secondi
        salvataggioCompletato.gameObject.SetActive(false);
    }

    // Funzioni di creazione di una nuova partita
    public void OnNewGame()
    {
        // Inizializzo le variabili dell'azienda
        CreazioneAzienda();
        PauseClick();
        currentTimer = timer;
        Dipendente.CaricaJsonCategorie();
        Progetto.CaricaJsonProgetti();
        GeneraProgettiSettimanali();
        AggiornaUIIniziale();
        tutorialPanel.MostraTutorial("introduzione1");
    }
    
    // Funzione di show del pannello di errore
    public void ShowErrorMessage(string message, Action onClose, string confirmText = "chiudi", string flag = "")
    {
        GameObject ErrorPanel = gameObject.transform.Find("ErrorPanel").gameObject;
        // Gestisco il messaggio di errore
        string text = LocalizationSettings.StringDatabase.GetLocalizedString("ErrorTable", message);
        ErrorPanel.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        
        GameObject CloseButton = ErrorPanel.transform.Find("Close").gameObject;
        CloseButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", confirmText);
        CloseButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            onClose();
            ErrorPanel.SetActive(false);
        });
        ErrorPanel.SetActive(true);
    }
    
    // Funzione di show del pannello di avviso
    public void ShowWarningMessage(string message, Action onConfirm, Action onRetry, string confirmText = "continua", string retryText = "annulla", string flag = "", Dictionary<string, string> replacements = null)
    {
        GameObject WarningPanel = gameObject.transform.Find("WarningPanel").gameObject;
        Toggle toggle = WarningPanel.transform.Find("Toggle").GetComponent<Toggle>();

        if (!string.IsNullOrEmpty(flag) && warningFlags.ContainsKey(flag) && warningFlags[flag])
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
        
        // Se ci sono replacements, sostituisco i placeholder
        if (replacements != null)
        {
            foreach (var kv in replacements)
            {
                // Sostituisce {chiave} con il valore corrispondente
                text = text.Replace("{" + kv.Key + "}", kv.Value);
            }
        }
        
        WarningPanel.transform.Find("Text").GetComponent<TMP_Text>().text = text;

        GameObject ConfirmButton = WarningPanel.transform.Find("Confirm").gameObject;
        ConfirmButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", confirmText);
        ConfirmButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ConfirmButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(flag) && toggle.isOn)
                warningFlags[flag] = true;

            onConfirm();
            WarningPanel.SetActive(false);
        });

        GameObject RetryButton = WarningPanel.transform.Find("Retry").gameObject;
        RetryButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", retryText);
        RetryButton.GetComponent<Button>().onClick.RemoveAllListeners();
        RetryButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(flag) && toggle.isOn)
                warningFlags[flag] = true;

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
    
    public void aggiornaCapitale(int ammontareDifferenza, List<string> motivazioni)
    {
        // Accodo direttamente tupla (differenza, motivazioni)
        codaEventi.Enqueue((ammontareDifferenza, motivazioni));

        if (!animazioneInCorso)
            StartCoroutine(EseguiAnimazioni());
    }
    
    IEnumerator EseguiAnimazioni()
    {
        animazioneInCorso = true;

        while (codaEventi.Count > 0)
        {
            // Dequeue della tupla
            var evento = codaEventi.Dequeue();
            int differenza = evento.Item1;
            List<string> motivazioni = evento.Item2;

            // --- Testo motivazioni ---
            TMP_Text motivazioniText = capitalePanel.transform.Find("TestoCapitale").GetComponent<TMP_Text>();
            motivazioniText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Transizioni", motivazioni[0]);
            for (int i = 1; i < motivazioni.Count; i++)
            {
                motivazioniText.text += " + " + LocalizationSettings.StringDatabase.GetLocalizedString("Transizioni", motivazioni[i]);
            }

            // --- Testo ammontare ---
            TMP_Text ammontare = capitalePanel.transform.Find("CapitaleText").GetComponent<TMP_Text>();
            ammontare.text = differenza < 0
                ? "<color=red>- $" + string.Format("{0:N2}", -differenza) + "</color>"
                : "<color=green>+ $" + string.Format("{0:N2}", differenza) + "</color>";

            // aggiorno il capitale effettivo con la cifra
            capitale += differenza;
            capitaleText.text = capitale >= 0
                ? "<color=green>$" + string.Format("{0:N2}", capitale) + "</color>"
                : "<color=red>$" + string.Format("{0:N2}", capitale) + "</color>";
            
            // --- Animazione ---
            yield return StartCoroutine(MuoviPanel());
            
            if (capitale < 0)
            {
                Bancarotta();
            }
        }

        animazioneInCorso = false;
    }
    
    // Animazione del cambiamento del capitale
    IEnumerator MuoviPanel(float durata = 0.15f, float attesa = 2f, float xTarget = 490f)
    {
        Vector2 posIniziale = capitalePanel.GetComponent<RectTransform>().anchoredPosition;
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
            capitalePanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(start, end, t);
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
            Image targetImage = weekSlider.transform.Find("Image").GetComponent<Image>();
            targetImage.color = ColorUtility.TryParseHtmlString("#249E59", out var fallback) ? fallback : Color.green;
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
        Image targetImage = weekSlider.transform.Find("Image").GetComponent<Image>();
        targetImage.color = ColorUtility.TryParseHtmlString("#B32F3C", out var fallback) ? fallback : Color.darkRed;
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
        OnNewGame();
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
        weekSlider.GetComponent<GestioneProgressBar>().ShowValue((timer - currentTimer)/ timer * 100);
    }
}
