using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Scripts.Logica
{
    [System.Serializable]
    public class Progetto
    {
    // Variabili del progetto
        public string nome;
        public List<NomiReparti> repartiCoinvolti;
        public bool forcedEnd;
        public string difficolta;
        [SerializeReference] public Azienda azienda;
        
        // Informazioni riguardanti la durata del progetto
        public int durata;
        public int durataRimanente;
        public int lavoroRichiesto;
        public int lavoroMancante;
        
        // Informazioni riguardanti i pagamenti del progetto
        public int anticipo;
        public int settimanale;
        public int finale;
        public int finaleDetrazione; // Pagamento da fare se il progetto viene concluso senza essere completato in tempo
        public int detrazioneRescissione; // Pagamento da fare se il progetto viene rescisso
        public int percentualeDetrazione; // Percentuale di bonus o detrazione sul pagamento finale in base alle settimane di ritardo o anticipo
        
        // json per la creazione del progetto
        public static Dictionary<string, List<NomiReparti>> jsonProgetti;
    
        
    // Funzioni del progetto
        //Costruttore del orogetto
        public static Progetto CreaProgetto(Azienda azienda)
        {
            var progettoInit = NomeRepartiProgetto(azienda);
            var nome = progettoInit.Key;
            var reparti = progettoInit.Value;
            
            int roll = UnityEngine.Random.Range(0, 100);
            int difficolta = roll < 50 ? 0 : (roll < 80 ? 1 : 2);
            
            int produzioneDip = difficolta switch
            {
                0 => 70,
                1 => 85,
                2 => 95,
                _ => throw new System.ArgumentOutOfRangeException()
            };
            
            float percentualeReparto = difficolta switch
            {
                0 => 0.33f,  // Facile → 1/3
                1 => 0.5f,   // Medio → 1/2
                2 => 0.8f,   // Difficile → 80%
                _ => 1f
            };
            
            var produzioneSettimanale = 0;
            var costoTotaleSettimanale = 0;
            
            foreach (var reparto in reparti)
            {
                var nDip = azienda.reparti[reparto].numeroMaxDipendenti;
                var costoDip = azienda.reparti[reparto].costoDipendente;
                produzioneSettimanale += (int) (nDip * produzioneDip * percentualeReparto);
                costoTotaleSettimanale += (int) (costoDip * nDip * percentualeReparto);
            }
            
            int durata = difficolta switch
            {
                0 => UnityEngine.Random.Range(ParametriContratto.MinSettimaneFacile, ParametriContratto.MaxSettimaneFacile + 1),
                1 => UnityEngine.Random.Range(ParametriContratto.MinSettimaneMedia, ParametriContratto.MaxSettimaneMedia + 1),
                2 => UnityEngine.Random.Range(ParametriContratto.MinSettimaneDifficile, ParametriContratto.MaxSettimaneDifficile + 1),
                _ => throw new System.ArgumentOutOfRangeException()
            };
            
            int lavoroRichiesto = produzioneSettimanale * durata;

            int costoTotaleContratto = costoTotaleSettimanale * durata;
        
            float margine = difficolta switch
            {
                0 => 1.20f, // Facile → guadagno modesto
                1 => 1.40f, // Medio
                2 => 1.70f, // Difficile → guadagno alto ma rischioso
                _ => throw new ArgumentOutOfRangeException()
            };

            // Valore economico totale del contratto
            int valoreTotale = (int)(costoTotaleContratto * margine);
        
            // Pagamenti
            int anticipo = (int)(valoreTotale * ParametriContratto.PercentualeAnticipo);
            int finale = (int)(valoreTotale * ParametriContratto.PercentualeFinale);
            int settimanale = (valoreTotale - anticipo - finale) / durata;

            // Penalità e bonus
            int percentualeDetrazione = difficolta switch
            {
                0 => ParametriContratto.DetrazioneFacile,
                1 => ParametriContratto.DetrazioneMedia,
                2 => ParametriContratto.DetrazioneDifficile,
                _ => throw new ArgumentOutOfRangeException()
            };
        
            int finaleDetrazione = finale;
            int detrazioneRescissione = (int)(valoreTotale * ParametriContratto.PenaleRescissione);

            bool forcedEnd = difficolta switch
            {
                0 => UnityEngine.Random.value < 0.3f, // 30%
                1 => UnityEngine.Random.value < 0.5f, // 50%
                2 => UnityEngine.Random.value < 0.7f, // 70%
                _ => throw new ArgumentOutOfRangeException()
            };

            Progetto progetto = new Progetto(
                nome,
                reparti,
                durata,
                lavoroRichiesto,
                anticipo,
                settimanale,
                finale,
                finaleDetrazione,
                detrazioneRescissione,
                percentualeDetrazione,
                forcedEnd,
                difficolta,
                azienda
            );
            return progetto;
        }
        
        // Costruttore del progetto
        public Progetto(
            string nome,
            List<NomiReparti> repartiCoinvolti,
            int durata,
            int lavoroRichiesto,
            int anticipo,
            int settimanale,
            int finale,
            int finaleDetrazione,
            int detrazioneRescissione,
            int percentualeDetrazione,
            bool forcedEnd,
            int difficolta,
            Azienda azienda)
        {
            this.nome = nome;
            this.repartiCoinvolti = repartiCoinvolti;
            this.durata = durata;
            this.durataRimanente = durata;
            this.lavoroRichiesto = (int)(Math.Round(lavoroRichiesto / 100.0) * 100);
            this.lavoroMancante = this.lavoroRichiesto;
            this.anticipo = anticipo;
            this.settimanale = settimanale;
            this.finale = finale;
            this.finaleDetrazione = -finaleDetrazione;
            this.detrazioneRescissione = -detrazioneRescissione;
            this.percentualeDetrazione = -percentualeDetrazione;
            this.forcedEnd = forcedEnd;
            this.difficolta = difficolta switch
            {
                0 => "bassa",
                1 => "media",
                2 => "alta",
                _ => throw new ArgumentOutOfRangeException()
            };
            this.azienda = azienda;
        }
        
        // Funzione di aggiornamento del progetto
        public int AggiornaProgetto()
        {
            var guadagno = 0;
            var lavoroSvolto = 0;

            foreach (var reparto in repartiCoinvolti)
            {
                lavoroSvolto += azienda.reparti[reparto].produzionePerProgetto;
            }
            
            lavoroMancante -= lavoroSvolto;
            durataRimanente--;

            if (lavoroMancante <= 0)
            {
                if (durataRimanente > 0)
                {
                    Debug.Log("Progetto " + nome + " completato in anticipo! Pagamento settimane arretrate: " + settimanale * durataRimanente);
                    guadagno += settimanale * durataRimanente;
                }
                Debug.Log("Finale: " + finale + " con detrazione/bonus di percentuale " + percentualeDetrazione + "% per " + durataRimanente + " settimane di anticipo.");
                guadagno += finale - (finale * percentualeDetrazione / 100 * (durataRimanente));
                Debug.Log("Guadagno totale progetto " + nome + ": " + guadagno);
                azienda.progettiCompletatiInSettimana.Add(this);
            }
            else
            {
                if (durataRimanente == 0)
                {
                    if (forcedEnd)
                    {
                        guadagno += finaleDetrazione;
                        azienda.progettiCompletatiInSettimana.Add(this);
                    }
                }
                else
                {
                    guadagno += settimanale;
                }
            }

            return guadagno;
        }
        
        //Rescinde il progetto
        public int rescissioneProgetto()
        {
            azienda.progettiInCorso.Remove(this);
            ChiudiProgetto();
            return detrazioneRescissione;
        }
        
        // Chiude il progetto
        public void ChiudiProgetto()
        {
            foreach (var reparto in repartiCoinvolti)
            {
                azienda.reparti[reparto].RimuoviProgetto();
            }
            azienda.progettiInCorso.Remove(this);
        }
        
        // Apre il progetto
        public void ApriProgetto()
        {
            foreach (var reparto in repartiCoinvolti)
            {
                azienda.reparti[reparto].AggiungiProgetto();
            }

            azienda.capitale += anticipo;
            azienda.progettiInCorso.Add(this);
            azienda.aggiornaCapitale();
        }

        public static void CaricaJsonProgetti()
        {
            if (jsonProgetti != null) return;

            TextAsset file = UnityEngine.Resources.Load<TextAsset>("Json/ProgettiCreazione");

            string contenuto = file.text;
            
            JObject root = JObject.Parse(contenuto);

            jsonProgetti = new Dictionary<string, List<NomiReparti>>();

            foreach (var propProgetto in root.Properties())
            {
                string nome = propProgetto.Name;
                var repartiArray = (JArray) propProgetto.Value;
                
                var listaReparti = new List<NomiReparti>();

                foreach (var repartoToken in repartiArray)
                {
                    string repartoString = repartoToken.Value<string>();
                    
                    if(Enum.TryParse<NomiReparti>(repartoString, out var repartoEnum))
                        listaReparti.Add(repartoEnum);
                    else
                        Console.WriteLine("Reparto non valido: " + repartoString);
                }
                
                jsonProgetti[nome] = listaReparti;
            }
        }
        
        // Restituisce un nome di progetto casuale e i reparti coinvolti
        public static KeyValuePair<string, List<NomiReparti>> NomeRepartiProgetto(Azienda azienda)
        {
            if (jsonProgetti == null)
            {
                CaricaJsonProgetti();
            }

            List<NomiReparti> repartiDisponibili = azienda.RepartiSbloccati();
            
            var progettiValidi = jsonProgetti.
                Where(p => p.Value.
                    All(r => repartiDisponibili.Contains(r))).ToList();
            
            var index = UnityEngine.Random.Range(0, (int) progettiValidi.Count);
            return progettiValidi[index];
        }
    }
}