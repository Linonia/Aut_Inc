using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Logica;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.UI;


public class Azienda : MonoBehaviour
{
// Variabili e funzioni per la parte logica del gioco
    // Informazioni sull'azienda
    public static int capitale;
    public static int costoDipendenteLibero = 1500;
    public static int tasseMensile;
    public static int tempoDiminuzioneGuadagno;
    
    // Informazioni sul tempo
    public static int anno;
    public static int mese;
    public static int settimana;
    
    //Informazioni riguardanti i reparti
    public static Dictionary<NomiReparti, Reparto> reparti = new Dictionary<NomiReparti, Reparto>();
    public static List<NomiReparti> repartiDaSbloccare = new List<NomiReparti>();
    public static int costoReparto = 20000;
    
    // Informazioni riguardanti i dipendenti
    public static List<Dipendente> dipendentiNonAssegnati = new List<Dipendente>();
    
    // Informazioni riguardanti i progetti
    public static List<Progetto> progettiInCorso = new List<Progetto>();
    public static List<Progetto> progettiProposti;
    
// Funzioni
    // Inizializzazione dell'azienda quando viene fatta una nuova partita
    public static void AziendaInit()
    {
        dipendentiNonAssegnati = new List<Dipendente>();
        reparti = new Dictionary<NomiReparti, Reparto>();
        
        reparti.Add(NomiReparti.AssistenzaESupportoTecnico, new Reparto("AssistenzaESupportoTecnico" ,new List<Categorie>
        {
            Categorie.ComunicazioneChiarezzaEspressiva,
            Categorie.ComunicazioneAdattabilitaComunicativa
        }, "desc1"));
        
        reparti.Add(NomiReparti.ControlloQualita, new Reparto("ControlloQualita", new List<Categorie>
        {
            Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica
        }, "desc2"));
        
        reparti.Add(NomiReparti.UxEDesign, new Reparto("UxEDesign", new List<Categorie>
        {
            Categorie.SensibilitaSensorialePercezioneSensorialeFina,
            Categorie.SensibilitaSensorialeComfortAmbientale
        }, "desc3"));
        
        reparti.Add(NomiReparti.SviluppoSoftware, new Reparto("SviluppoSoftware", new List<Categorie>
        {
            Categorie.CapacitaCognitiveProblemSolvingCreativo,
            Categorie.CapacitaCognitiveFlessibilitaCognitiva
        }, "desc4"));
        
        reparti.Add(NomiReparti.RicercaESviluppo, new Reparto("RicercaESviluppo", new List<Categorie>
        {
            Categorie.CapacitaCognitiveProblemSolvingCreativo,
            Categorie.CapacitaCognitiveFlessibilitaCognitiva,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa,
            Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica
        }, "desc5"));
        
        reparti.Add(NomiReparti.Marketing, new Reparto("Marketing", new List<Categorie>
        {
            Categorie.SensibilitaSensorialePercezioneSensorialeFina,
            Categorie.SensibilitaSensorialeComfortAmbientale,
            Categorie.ComunicazioneChiarezzaEspressiva,
            Categorie.ComunicazioneAdattabilitaComunicativa
        }, "desc6"));
        
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
    }

    // Funzione per la gestione dell'acquisto di un nuovo reparto
    public static void AperturaNuovoReparto()
    {
        // Parte logica per l'apertura di un nuovo reparto
        if (repartiDaSbloccare.Count == 0)
        {
            return;
        }
        var nomeReparto = repartiDaSbloccare.First();
        reparti[nomeReparto].ApriReparto();
        repartiDaSbloccare.Remove(nomeReparto);
        tasseMensile += 1000;
        capitale -= costoReparto;
        costoReparto += 10000;
        
        // Parte grafica per l'apertura di un nuovo reparto
        // Utilizza il nome del reparto per cercare cosa aggiornare
    }

    // Funzione per l'aggiunta del dipendente ad un team
    public static void AggiungiDipendente(Dipendente dipendente, Team team)
    {
        if(team.reparto.numeroPostiLiberi > 0)
        {
            team.reparto.AggiungiDipendente(team, dipendente);
            dipendentiNonAssegnati.Remove(dipendente);
        }
        else
        {
            Debug.Log("Il reparto non ha posti disponibili");
            // Messaggio di errore per il team pieno
        }
    }
    
    // Funzione per rimuovere un dipendente da un team
    public static void RimuoviDipendente(Dipendente dipendente)
    {
        dipendente.team.reparto.RimuoviDipendente(dipendente.team, dipendente);
        dipendentiNonAssegnati.Add(dipendente);
    }
    
    // Funzione per spostare un dipendente da un team ad un altro
    public static void SpostaDipendente(Dipendente dipendente, Team team)
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
            // Messaggio di errore per il team pieno
            Debug.Log("Il team è pieno, impossibile spostare il dipendente");
        }
    }

    // Funzione per pagare i dipendenti dell'azienda
    public static void PagaDipendenti()
    {
        var costoTotaleDipendenti = 0;
        foreach (var tuple in reparti)
        {
            var reparto = tuple.Value;
            costoTotaleDipendenti += reparto.CostoDipendenti();
        }
        costoTotaleDipendenti += dipendentiNonAssegnati.Count * costoDipendenteLibero;
        capitale -= costoTotaleDipendenti;
    }

    // Funzione per aggiornare l'azienda ogni settimana
    public static void Aggiorna()
    {
        foreach (var reparto in reparti.Values)
        {
            if (reparto.aperto)
            {
                reparto.Aggiorna();
            }
        }

        var guadagnoSettimanale = 0;
        // Calcolo del guadagno settimanale da tutti i progetti
        foreach (var progetto in progettiInCorso)
        {
            guadagnoSettimanale += progetto.AggiornaProgetto();
        }

        capitale += guadagnoSettimanale;

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

        // Gestione delle tasse mensili
        if (settimana == 1)
        {
            PagaDipendenti();
            capitale -= tasseMensile;
            if (capitale < 0)
            {
                Debug.Log("Capitale insufficiente per pagare le tasse");
                // Messaggio di errore per capitale insufficiente
                // Game over
            }
        }
    }

    // Funzione per potenziare un reparto
    public static void PotenziaReparto(NomiReparti nomeReparto)
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
    public static List<NomiReparti> RepartiSbloccati()
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
    
    public void OnTerminaProgetto(Progetto progetto, Action clearAction, Action reloadProjectList)
    {
        if (progetto == null) return;
        
        // Chiedo conferma per la terminazione del progetto
        ShowWarningMessage("terminaProgettoAvviso", () =>
        {
            Azienda.RescindiContratto(progetto);
            clearAction();
            reloadProjectList();
        }, () => 
        {
            
        }, "conferma", "annulla");
    }
    
    // Funzione di rescissione di un contratto
    public static void RescindiContratto(Progetto progetto)
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
            Azienda.LicenziaDipendente(dipendente);
            clearAction();
            reloadEmployeeList();
        }, () => 
        {
            
        }, "conferma", "annulla");
    }
    
    public static void LicenziaDipendente(Dipendente dipendente)
    {
        if ( dipendente.team != null)
        {
            dipendente.team.reparto.RimuoviDipendente(dipendente.team, dipendente);
        }
        // Rimuovo il dipendente dalla lista dei dipendenti non assegnati
        dipendentiNonAssegnati.Remove(dipendente);
        // Messaggio di successo per il licenziamento
        Debug.Log($"Dipendente {dipendente.nome} licenziato con successo.");
    }

    public void OnAssumiDipendente(Dipendente dipendente, Action clearAction)
    {
        clearAction();
        Azienda.AssumiDipendente(dipendente);
    }
    
    public static void AssumiDipendente(Dipendente dipendente)
    {
        dipendentiNonAssegnati.Add(dipendente);
        // Messaggio di successo per l'assunzione
        Debug.Log($"Dipendente {dipendente.nome} assunto con successo.");
    }


// Funzioni per la gestione estetica e di funzionalità del gioco

    private void Start()
    {
        // Inizializzo l'azienda
        AziendaInit();
        Dipendente.CaricaJsonCategorie();
        Progetto.CaricaJsonProgetti();
        
        // Crea 4 dipendenti di prova
        for(int i = 0; i < 4; i++)
        {
            Dipendente dipendente = Dipendente.GeneraDipendente2();
            dipendentiNonAssegnati.Add(dipendente);
        }
        
        // Assegno i dipendenti ai reparti
        var dipendente1 = dipendentiNonAssegnati[0];
        var dipendente2 = dipendentiNonAssegnati[1];
        var dipendente3 = dipendentiNonAssegnati[2];
        var dipendente4 = dipendentiNonAssegnati[3];
        
        // Creo dei progetti di prova
        for(int i = 0; i < 3; i++)
        {
            var progetto = Progetto.CreaProgetto();
            progettiInCorso.Add(progetto);
        }
        
    }
    
    public void OnLoadGame()
    {
        
    }
    
    public void OnSaveGame()
    {
        
    }

    public void OnNewGame()
    {
        
    }
    
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

    public void ShowErrorMessage(string message, Action onClose, string confirmText = "chiudi")
    {
        GameObject ErrorPanel = gameObject.transform.Find("ErrorPanel").gameObject;
        // Gestisco il messaggio di errore
        string text = LocalizationSettings.StringDatabase.GetLocalizedString("ErrorTable", message);
        ErrorPanel.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        
        GameObject ConfirmButton = ErrorPanel.transform.Find("Confirm").gameObject;
        ConfirmButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", confirmText);
        ConfirmButton.GetComponent<Button>().onClick.AddListener(() => onClose());
    }
    
    public void ShowWarningMessage(string message, Action onConfirm, Action onRetry, string confirmText = "continua", string retryText = "annulla")
    {
        GameObject WarningPanel = gameObject.transform.Find("WarningPanel").gameObject;
        // Gestisco il messaggio di avviso
        string text = LocalizationSettings.StringDatabase.GetLocalizedString("ErrorTable", message);
        WarningPanel.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        
        GameObject ConfirmButton = WarningPanel.transform.Find("Confirm").gameObject;
        ConfirmButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", confirmText);
        ConfirmButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ConfirmButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            onConfirm();
            WarningPanel.SetActive(false);
        });     
        
        GameObject RetryButton = WarningPanel.transform.Find("Retry").gameObject;
        RetryButton.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation",retryText);
        RetryButton.GetComponent<Button>().onClick.RemoveAllListeners();
        RetryButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            onRetry();
            WarningPanel.SetActive(false);
        });
        
        WarningPanel.SetActive(true);
    }
}
