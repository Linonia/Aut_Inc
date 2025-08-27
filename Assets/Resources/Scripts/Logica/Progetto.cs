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
        public static Progetto CreaProgetto()
        {
            var progettoInit = NomeRepartiProgetto();
            var nome = progettoInit.Key;
            var reparti = progettoInit.Value;
            
            int roll = UnityEngine.Random.Range(0, 100);
            int difficolta = roll < 50 ? 0 : (roll < 75 ? 1 : 2);
            
            int produzioneDip = difficolta switch
            {
                0 => 70,
                1 => 85,
                2 => 95,
                _ => throw new System.ArgumentOutOfRangeException()
            };
            
            var produzioneSettimanale = 0;
            var costoTotaleSettimanale = 0;
            
            foreach (var reparto in reparti)
            {
                var nDip = Azienda.reparti[reparto].numeroMaxDipendenti;
                var costoDip = Azienda.reparti[reparto].CostoDipendenti();
                produzioneSettimanale += nDip * produzioneDip;
                costoTotaleSettimanale += costoDip * nDip;
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
                0 => 1.10f, // Facile → guadagno modesto
                1 => 1.35f, // Medio
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
                forcedEnd
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
            bool forcedEnd)
        {
            this.nome = nome;
            this.repartiCoinvolti = repartiCoinvolti;
            this.durata = durata;
            this.durataRimanente = durata;
            this.lavoroRichiesto = lavoroRichiesto;
            this.lavoroMancante = lavoroRichiesto;
            this.anticipo = anticipo;
            this.settimanale = settimanale;
            this.finale = finale;
            this.finaleDetrazione = -finaleDetrazione;
            this.detrazioneRescissione = -detrazioneRescissione;
            this.percentualeDetrazione = -percentualeDetrazione;
            this.forcedEnd = forcedEnd;
        }
        
        // Funzione di aggiornamento del progetto
        public int AggiornaProgetto()
        {
            var guadagno = 0;
            var lavoroSvolto = 0;

            foreach (var reparto in repartiCoinvolti)
            {
                lavoroSvolto += Azienda.reparti[reparto].produzionePerProgetto;
            }
            
            lavoroMancante -= lavoroSvolto;
            durataRimanente--;

            if (lavoroMancante <= 0)
            {
                if (durataRimanente > 0)
                {
                    guadagno += settimanale * durataRimanente;
                }
                guadagno += finale - (finale * percentualeDetrazione / 100 * (-durataRimanente));
                ChiudiProgetto();
            }
            else
            {
                if (durataRimanente == 0)
                {
                    if (forcedEnd)
                    {
                        guadagno += finaleDetrazione;
                        ChiudiProgetto();
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
            Azienda.progettiInCorso.Remove(this);
            ChiudiProgetto();
            return detrazioneRescissione;
        }
        
        // Chiude il progetto
        public void ChiudiProgetto()
        {
            foreach (var reparto in repartiCoinvolti)
            {
                Azienda.reparti[reparto].RimuoviProgetto();
            }
            Azienda.progettiInCorso.Remove(this);
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
        public static KeyValuePair<string, List<NomiReparti>> NomeRepartiProgetto()
        {
            if (jsonProgetti == null)
            {
                CaricaJsonProgetti();
            }

            List<NomiReparti> repartiDisponibili = Azienda.RepartiSbloccati();
            
            var progettiValidi = jsonProgetti.
                Where(p => p.Value.
                    All(r => repartiDisponibili.Contains(r))).ToList();
            
            var index = UnityEngine.Random.Range(0, (int) progettiValidi.Count);
            return progettiValidi[index];
        }
    }
}