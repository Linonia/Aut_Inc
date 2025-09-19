using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.Logica
{
    [System.Serializable]
    public class Team
    {
    // Variabili del team
        // Lista dei dipendenti del team
        public List<Dipendente> dipendenti;
        
        // Reparto a cui appartiene il team
        [JsonIgnore][SerializeReference] public Reparto reparto;
        
        public Team(){}
        // Costruttore del team
        public Team(Reparto reparto)
        {
            this.reparto = reparto;
            dipendenti = new List<Dipendente>();
        }
        
        // Aggiunge un dipendente al team e aggiorna il morale di tutti i membri
        public void AggiungiDipendente(Dipendente dipendente)
        {
            dipendenti.Add(dipendente);
            dipendente.EntraNelTeam(this);
            // Calcola l'umore ideale per tutti i dipendenti
            foreach (var dip in dipendenti)
            {
                dip.CalcolaUmoreIdeale();
            }
        }

        // Rimuove un dipendente dal team e diminuisce l'umore di tutti i membri
        public void RimuoviDipendente(Dipendente dipendente)
        {
            foreach (var dip in dipendenti)
            {
                dip.DiminuzioneUmore();
            }
            dipendente.EsciDalTeam();
            dipendenti.Remove(dipendente);
        }
        
        
        // Rimuove tutti i dipendenti dal team
        public void RimuoviTuttiDipendenti()
        {
            foreach (var dipendente in dipendenti)
            {
                dipendente.EsciDalTeam();
            }
            dipendenti.Clear();
        }
        
        // Restituisce la produzione settimanale del team
        public int GetProduzioneSettimanale()
        {
            int produzioneTotale = 0;
            foreach (var dipendente in dipendenti)
            {
                produzioneTotale += dipendente.produzioneSettimanale;
            }
            return produzioneTotale;
        }
        
        // Restituisce il numero di dipendenti nel team
        public int NumeroDipendenti()
        {
            return dipendenti.Count;
        }
        
        public bool ContieneDipendente(Dipendente dipendente)
        {
            return dipendenti.Contains(dipendente);
        }
        
        // Aggiorna il team
        public void Aggiorna()
        {
            foreach (var dipendente in dipendenti)
            {
                dipendente.Aggiorna();
            }
        }
        
        // Verifica se il team non può ospitare altri dipendenti
        public bool PostiDisponibiliEsistenti()
        {
            return reparto.numeroPostiLiberi > 0;
        }

        // Funzione di caricamento del salvataggio
        public void OnAfterLoad(Reparto reparto)
        {
            this.reparto = reparto;
            foreach (var dipendente in dipendenti)
            {
                dipendente.OnAfterLoad(this);
            }
        }
    }
}