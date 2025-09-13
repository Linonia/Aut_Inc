using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Logica
{
    [System.Serializable]
    public class Reparto
    {
    // Variabili del reparto
        // Informazioni riguardanti il reparto
        public string codice;
        public int livelloReparto = 1;
        public int livelloRepartoMax;
        public int costoPotenziamento;
        
        // Informazioni riguardanti le categorie del reparto
        public List<Categorie> categorie;
        
        // Informaizioni riguardanti i team che compongono il reparto
        public List<Team> teams = new List<Team>();
        public int numeroMaxDipendenti;
        public int numeroPostiLiberi;
        public int numeroMinimoTeam;
        public int numeroMaxTeam;
        
        // Informazioni riguardanti la produzione del reparto
        public int produzioneSettimanale;
        public int produzionePerProgetto;
        public int numeroProgetti = 0;
        
        // Costo per ogni dipendente del reparto
        public int costoDipendente;
        
        // Stato del reparto
        public bool aperto = false;
        
        // Descrizione testuale del reparto
        public string descrizione;
        
        // Riferimento all'azienda a cui appartiene il reparto
        [SerializeReference] public Azienda azienda;
        
    // Funzioni del reparto
        // Costruttore del reparto
        public Reparto(string codice, List<Categorie> categorie, string descrizione, Azienda azienda)
        {
            this.codice = codice;
            this.descrizione = descrizione;
            this.categorie = new List<Categorie>(categorie);
            (var min, var max, var maxDipendenti, var costo, var costoPot) = LivelliReparti.LivelloReparto[livelloReparto];
            numeroMinimoTeam = min;
            numeroMaxTeam = max;
            numeroMaxDipendenti = maxDipendenti;
            numeroPostiLiberi = maxDipendenti;
            costoDipendente = costo;
            costoPotenziamento = costoPot;
            livelloRepartoMax = LivelliReparti.LivelloReparto.Count;
            this.azienda = azienda;
        }

        // Aumenta il livello del reparto
        public void AumentaLivello()
        {
            if (livelloReparto < livelloRepartoMax)
            {
                livelloReparto++;
                (var min, var max, var maxDipendenti, var costo, var costoPot) = LivelliReparti.LivelloReparto[livelloReparto];
                numeroMinimoTeam = min;
                numeroMaxTeam = max;
                numeroPostiLiberi += maxDipendenti - numeroMaxDipendenti;
                numeroMaxDipendenti = maxDipendenti;
                costoDipendente = costo;
                costoPotenziamento = costoPot;
            }
        }
        
        // Rimuove il dipendente dal team selezionato
        public void RimuoviDipendente(Team team, Dipendente dipendente)
        {
            if (teams.Contains(team) && team.ContieneDipendente(dipendente))
            {
                team.RimuoviDipendente(dipendente);
                numeroPostiLiberi++;
                Debug.Log("Numero dipendenti del team dopo la rimozione: " + team.NumeroDipendenti());
                if (team.NumeroDipendenti() == 0)
                {
                    RimuoviTeam(team);
                }
            }
        }
        
        // Aggiunge un dipendente al team selezionato
        public void AggiungiDipendente(Team team, Dipendente dipendente)
        {
            if (teams.Contains(team) && numeroPostiLiberi > 0)
            {
                team.AggiungiDipendente(dipendente);
                numeroPostiLiberi--;
            }
        }

        // Rimuove un team dal reparto
        public void RimuoviTeam(Team team)
        {
            team.RimuoviTuttiDipendenti();
            numeroPostiLiberi = numeroPostiLiberi + team.NumeroDipendenti();
            teams.Remove(team);
        }

        // Aggiunge un nuovo team vuoto al reparto
        public void AggiungiTeam()
        {
            teams.Add(new Team(this));
        }

        // Restituisce il numero di team attivi nel reparto
        public int NumeroTeamAttivi()
        {
            int count = 0;
            foreach (var team in teams)
            {
                if (team.NumeroDipendenti() > 0)
                {
                    count++;
                }
            }
            return count;
        }
        
        // Calcola il costo totale dei dipendenti del reparto
        public int CostoDipendenti()
        {
            int numeroDipendenti = 0;
            foreach (var team in teams)
            {
                numeroDipendenti += team.dipendenti.Count;
            }
            return numeroDipendenti * costoDipendente;
        }

        // Calcola la produzione settimanale del reparto e quella effettiva per progetto e per reparto
        public void CalcoloProduzione()
        {
            var lavoroTotale = 0;
            foreach (var team in teams)
            {
                lavoroTotale += team.GetProduzioneSettimanale();
            }
            
            var numeroTeamAttivi = NumeroTeamAttivi();
            var distanzaTeam = 0;
            if(numeroTeamAttivi < numeroMinimoTeam)
            {
                distanzaTeam = numeroMinimoTeam - numeroTeamAttivi;
            }
            else if(numeroTeamAttivi > numeroMaxTeam)
            {
                distanzaTeam = numeroTeamAttivi - numeroMaxTeam;
            }
            
            lavoroTotale -= (int) (lavoroTotale * 0.1 * distanzaTeam);
            double variazionePercentuale = 0.05;
            int variazioneMassima = (int)(lavoroTotale * variazionePercentuale);
            int delta = UnityEngine.Random.Range(-variazioneMassima, variazioneMassima + 1);
            produzioneSettimanale = lavoroTotale + delta;
            produzionePerProgetto = (numeroProgetti > 0) ? produzioneSettimanale / numeroProgetti : 0;
        }
        
        // Aggiunge un progetto al reparto
        public void AggiungiProgetto()
        {
            numeroProgetti++;
        }
        
        // Rimuove un progetto dal reparto
        public void RimuoviProgetto()
        {
            if (numeroProgetti > 0)
            {
                numeroProgetti--;
            }
        }
        
        // Apre il reparto se non è già aperto
        public void ApriReparto()
        {
            if (!aperto)
            {
                aperto = true;
                numeroProgetti = 0;
            }
        }
        
        // Aggiornamento del reparto
        public void Aggiorna()
        {
            CalcoloProduzione();
            foreach (var team in teams)
            {
                team.Aggiorna();
            }
        }
        
        
    }
}