using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Scripts.Logica
{
    [System.Serializable]
    public class Dipendente
    {
    // Variabili del dipendente
        // Informazioni riguardanti il dipendente
        public string nome;
        public string foto;
        
        // Categorie di autismo del dipendente
        public Dictionary<Categorie, int> categorie;
        public string[] codiciDescrizioni;
        
        // informazioni sull'umore del dipendente
        public int umore;
        public int minimoUmore = 20;
        public int massimoUmore = 100;
        
        public int umoreIdeale;
        public int aggiornamentoUmore = 5;
        
        public int minimoTeam;
        public int massimoTeam;

        public int diminuzioneUmoreMassimo;
        public int diminuzioneUmoreAttuale;
        
        // informazioni sulla competenza del dipendente
        public int competenza;
        public int competenzaIdeale;
        public int aggiornamentoCompetenza = 5;
        public int diminuzioneCompetenza = 15;
        public int attesaCompetenza = 4;
        
        // informazioni riguardanti la produzione del dipendente
        public int produzioneSettimanale;
        
        // Informazioni riguardanti il team del dipendente
        [JsonIgnore][SerializeReference] public Team team;
        
        // Informazioni riguardanti il reparto del dipendente
        //[SerializeReference]public Reparto reparto;
        public string repartoCodice;
        
        // stato del lavoro del dipendente
        public bool staLavorando = false;

        // Istanza statica del json per la creazione dei dipendenti
        public static Dictionary<string, Dictionary<int, List<(int, int, string)>>> jsonDipendenti;
        
        public Dipendente(){}
        // Costruttore del dipendente
        public Dipendente(Dictionary<Categorie, int> categorie, string[] codiciDescrizioni, string foto, string nome)
        {
            this.nome = nome;
            this.foto = foto;
            
            this.categorie = new Dictionary <Categorie, int>(categorie);
            this.codiciDescrizioni = codiciDescrizioni;
            
            // inizializza l'umore del dipendente
            umore = massimoUmore;
            umoreIdeale = massimoUmore;

            (var min, var max) =
                PreferenzaSociale.CurvaSocialita[categorie[Categorie.InterazioneSocialePreferenzaSociale]];
            minimoTeam = min;
            massimoTeam = max;
            diminuzioneUmoreMassimo =
                PreferenzaSociale.PenalitaAbbandonoTeam[categorie[Categorie.InterazioneSocialeStabilitaEmotivaInTeam]];
            diminuzioneUmoreAttuale = 0;
            
            repartoCodice = "";
            staLavorando = false;
            attesaCompetenza = 4;
        }
        
        // Aggiornamento dell'umore del dipendente
        public void AggiornaUmore()
        {
            if (diminuzioneUmoreAttuale > 0)
            {
                diminuzioneUmoreAttuale -= aggiornamentoUmore;
                if(diminuzioneUmoreAttuale < 0)
                {
                    diminuzioneUmoreAttuale = 0;
                }
            }

            if (umore < umoreIdeale)
            {
                umore += aggiornamentoUmore;
                if (umore > umoreIdeale)
                {
                    umore = umoreIdeale;
                }
            }
            else if (umore > umoreIdeale)
            {
                umore -= aggiornamentoUmore;
                if (umore < umoreIdeale)
                {
                    umore = umoreIdeale;
                }
            }

            umore = Math.Clamp(umore, minimoUmore, massimoUmore);
        }
        
        // Aggiornamento della competenza del dipendente
        public void AggiornaCompetenza()
        {
            if (competenza < competenzaIdeale)
            {
                competenza += aggiornamentoCompetenza;
                if (competenza > competenzaIdeale)
                {
                    competenza = competenzaIdeale;
                }
            }
            attesaCompetenza = attesaCompetenza > 0 ? attesaCompetenza - 1 : 0;
        }
        
        // Aggiornamento della produzione settimanale del dipendente
        public void AggiornaProduzioneSettimanale()
        {
            produzioneSettimanale = (int) Math.Sqrt((umore * umore + competenza * competenza) / 2.0);
        }
        
        // Calcola l'umore ideale del dipendente in base al team
        public void CalcolaUmoreIdeale()
        {
            var numeroTeam = team.NumeroDipendenti();
            var differenza =
                numeroTeam < minimoTeam ? minimoTeam - numeroTeam :
                numeroTeam > massimoTeam ? numeroTeam - massimoTeam :
                0;
            umoreIdeale = Math.Max(minimoUmore, massimoUmore - (differenza * 10));
        }
        
        // Calcolo della diminuzione dell'umore del dipendente
        public void DiminuzioneUmore()
        {
            umore -= diminuzioneUmoreMassimo - diminuzioneUmoreAttuale;
            diminuzioneUmoreAttuale = diminuzioneUmoreMassimo;
            if (umore < minimoUmore)
            {
                umore = minimoUmore;
            }
        }
        
        // Calcolo della competenza ideale del dipendente
        public void CalcolaCompetenzaIdeale()
        {
            var sommaCompetenze = 0;
            // Stampa informazioni per il debug
            // Stampa il codice del reparto, il codice del reparto del team e le categorie del reparto
            // Stampa una alla volta le informazioni nella riga successiva
            //Debug.Log($"Reparto codice: {repartoCodice}, Team reparto codice: {team.reparto.codice}, Categorie reparto: {string.Join(", ", team.reparto.categorie)}");
            
            foreach (var categoria in team.reparto.categorie)
            {
                sommaCompetenze += categorie[categoria];
            }

            var competenzaMassima = team.reparto.categorie.Count * 10;
            competenzaIdeale = (int) (100 * sommaCompetenze / competenzaMassima);
            competenza = competenzaIdeale - diminuzioneCompetenza;
            competenza = Math.Clamp(competenza, 0, 100);
        }
        
        // Funzione di uscita dal team
        public void EsciDalTeam()
        {
            team = null;
            umoreIdeale = 100;
            staLavorando = false;
            attesaCompetenza = 4;
        }
        
        // Funzione di ingresso nel team
        public void EntraNelTeam(Team team)
        {
            this.team = team;
            if (team.reparto != null && team.reparto.codice != repartoCodice)
            {
                repartoCodice = team.reparto.codice;
                CalcolaCompetenzaIdeale();
            }
            staLavorando = true;
            CalcolaUmoreIdeale();
            AggiornaProduzioneSettimanale();
        }
        
        // Funzione per aggiornare il dipendente
        public void Aggiorna()
        {
            AggiornaUmore();
            if (staLavorando)
            {
                AggiornaCompetenza();
                AggiornaProduzioneSettimanale();
            }
        }
        
        // Carica i dati del json dei dipendenti
        public static void CaricaJsonCategorie()
        {
            if (jsonDipendenti != null) return;

            string resourcePath = "Json/CategorieAutismo";
            
            TextAsset jsonFile = UnityEngine.Resources.Load<TextAsset>(resourcePath);
            
            if (jsonFile == null)
            {
                Debug.LogError($"Impossibile caricare il file JSON da {resourcePath}");
                return;
            }
            
            string contenutoJson = jsonFile.text;
            
            JObject json = JObject.Parse(contenutoJson);
            
            jsonDipendenti = new Dictionary<string, Dictionary<int, List<(int, int, string)>>>();

            foreach (var categoria in json.Properties())
            {
                string nomeCategoria = categoria.Name;
                var livelli = new Dictionary<int, List<(int, int, string)>>();

                foreach (var livello in (JObject) categoria.Value)
                {
                    string stringaLivello = livello.Key.Trim();
                    
                    if(!int.TryParse(stringaLivello, out int numeroLivello))
                        continue;
                    
                    var listaValori = new List<(int, int, string)>();

                    foreach (var elemento in (JArray) livello.Value)
                    {
                        int valore1 = elemento[0].Value<int>();
                        int valore2 = elemento[1].Value<int>();
                        string descrizione = elemento[2].Value<string>();
                        listaValori.Add((valore1, valore2, descrizione));
                    }
                    livelli[numeroLivello] = listaValori;
                }
                jsonDipendenti[nomeCategoria] = livelli;
            }
        }

        public static Dipendente GeneraDipendente(bool neurodivergente = true)
        {
            List<int> listaValoriCategoria = new List<int>();
        
            // Creo 5 valori da 1 a 10 per le categorie del dipendente
            if (neurodivergente)
            {
                int n1 = UnityEngine.Random.Range(9, 11);  // Da 9 a 10 inclusi
                int n2 = UnityEngine.Random.Range(8, 11);  // Da 7 a 10 inclusi
                int n3 = UnityEngine.Random.Range(3, 9);   // Da 3 a 8 inclusi
                int n4 = UnityEngine.Random.Range(1, 4);   // Da 1 a 3 inclusi
                int n5 = UnityEngine.Random.Range(1, 3);   // Da 1 a 2 inclusi
                listaValoriCategoria = new List<int> { n1, n2, n3, n4, n5 };
                listaValoriCategoria = listaValoriCategoria.OrderBy(x => UnityEngine.Random.value).ToList();
            }
            else
            {
                listaValoriCategoria = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    int numero = UnityEngine.Random.Range(6, 9); // Da 6 a 8 inclusi
                    listaValoriCategoria.Add(numero);
                }
                listaValoriCategoria = listaValoriCategoria.OrderBy(x => UnityEngine.Random.value).ToList();
            }
            
            // Creo un dizionario delle categorie del dipendente
            Dictionary<Categorie, int> categorie = new Dictionary<Categorie, int>();
            List<string> descrizioni = new List<string>();
            
            // Aggiungo le categorie al dizionario
            (var primo, var secondo, var stringa) = jsonDipendenti["Comunicazione"][listaValoriCategoria[0]][UnityEngine.Random.Range(0, jsonDipendenti["Comunicazione"][listaValoriCategoria[0]].Count)];
            categorie.Add(Categorie.ComunicazioneChiarezzaEspressiva, primo);
            categorie.Add(Categorie.ComunicazioneAdattabilitaComunicativa, secondo);
            descrizioni.Add(stringa);
            
            (primo, secondo, stringa) = jsonDipendenti["InterazioneSociale"][listaValoriCategoria[1]][UnityEngine.Random.Range(0, jsonDipendenti["InterazioneSociale"][listaValoriCategoria[1]].Count)];
            categorie.Add(Categorie.InterazioneSocialePreferenzaSociale, primo);
            categorie.Add(Categorie.InterazioneSocialeStabilitaEmotivaInTeam, secondo);
            descrizioni.Add(stringa);
            
            (primo, secondo, stringa) = jsonDipendenti["ComportamentiRipetitiviEInteressiRistretti"][listaValoriCategoria[2]][UnityEngine.Random.Range(0, jsonDipendenti["ComportamentiRipetitiviEInteressiRistretti"][listaValoriCategoria[2]].Count)];
            categorie.Add(Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa, primo);
            categorie.Add(Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica, secondo);
            descrizioni.Add(stringa);
            
            (primo, secondo, stringa) = jsonDipendenti["SensibilitaSensoriale"][listaValoriCategoria[3]][UnityEngine.Random.Range(0, jsonDipendenti["SensibilitaSensoriale"][listaValoriCategoria[3]].Count)];
            categorie.Add(Categorie.SensibilitaSensorialePercezioneSensorialeFina, primo);
            categorie.Add(Categorie.SensibilitaSensorialeComfortAmbientale, secondo);
            descrizioni.Add(stringa);
            
            (primo, secondo, stringa) = jsonDipendenti["CapacitaCognitive"][listaValoriCategoria[4]][UnityEngine.Random.Range(0, jsonDipendenti["CapacitaCognitive"][listaValoriCategoria[4]].Count)];
            categorie.Add(Categorie.CapacitaCognitiveProblemSolvingCreativo, primo);
            categorie.Add(Categorie.CapacitaCognitiveFlessibilitaCognitiva, secondo);
            descrizioni.Add(stringa);
            
            // Scrivo la descrizione del dipendente
            List<string> descrizioneCompleta = descrizioni.OrderBy(x => Guid.NewGuid()).ToList();
            
            
            // Genero un numero casuale da 0 a 1 per decidere se il dipendente è maschio o femmina
            int genere = UnityEngine.Random.Range(0, 2);
            
            TextAsset file = Resources.Load<TextAsset>("Json/NomiDipendenti");
            var nomi = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(file.text);
            string nome = genere == 0
                ? nomi["M"][UnityEngine.Random.Range(0, nomi["M"].Length)]
                : nomi["F"][UnityEngine.Random.Range(0, nomi["F"].Length)];
            
            Sprite[] foto = genere == 0
                ? UnityEngine.Resources.LoadAll<Sprite>("Employee/Maschietti")
                : UnityEngine.Resources.LoadAll<Sprite>("Employee/Femminucce");
            
            var fotoProfilo = foto[UnityEngine.Random.Range(0, foto.Length)].name;
           
            // Creo il dipendente
            Dipendente dipendente = new Dipendente(categorie, descrizioneCompleta.ToArray(), fotoProfilo, nome);

            return dipendente;
        }
        
        public static Dipendente GeneraDipendente2(bool neurodivergente = true)
        {
            List<int> listaValoriCategoria = new List<int>();
            
            // Creo 5 valori da 1 a 10 per le categorie del dipendente
            if (neurodivergente)
            {
                int n1 = UnityEngine.Random.Range(9, 11);  // Da 9 a 10 inclusi
                int n2 = UnityEngine.Random.Range(8, 11);  // Da 8 a 10 inclusi
                int n3 = UnityEngine.Random.Range(1, 4);   // Da 1 a 3 inclusi
                int n4 = UnityEngine.Random.Range(1, 3);   // Da 1 a 2 inclusi
                listaValoriCategoria = new List<int> { n1, n2, n3, n4};
                listaValoriCategoria = listaValoriCategoria.OrderBy(x => UnityEngine.Random.value).ToList();
            }
            else
            {
                listaValoriCategoria = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    // Genera un numero da 0 a 99
                    int chance = UnityEngine.Random.Range(0, 100);

                    int numero;
                    if (chance < 8)
                        numero = 4;           // 10% di probabilità
                    else if (chance < 18)
                        numero = 5;           // 15% di probabilità
                    else if (chance < 48)
                        numero = 6;           // 25% di probabilità
                    else if (chance < 80)
                        numero = 7;           // 30% di probabilità
                    else
                        numero = 8;           // 20% di probabilità

                    listaValoriCategoria.Add(numero);
                }
                listaValoriCategoria = listaValoriCategoria.OrderBy(x => UnityEngine.Random.value).ToList();
            }
            
            // Creo un dizionario delle categorie del dipendente
            Dictionary<Categorie, int> categorie = new Dictionary<Categorie, int>();
            List<string> descrizioni = new List<string>();
            
            // Aggiungo le categorie al dizionario
            (var primo, var secondo, var stringa) = jsonDipendenti["Comunicazione"][listaValoriCategoria[0]][UnityEngine.Random.Range(0, jsonDipendenti["Comunicazione"][listaValoriCategoria[0]].Count)];
            categorie.Add(Categorie.ComunicazioneChiarezzaEspressiva, primo);
            categorie.Add(Categorie.ComunicazioneAdattabilitaComunicativa, secondo);
            descrizioni.Add(stringa);
            
            (primo, secondo, stringa) = jsonDipendenti["ComportamentiRipetitiviEInteressiRistretti"][listaValoriCategoria[1]][UnityEngine.Random.Range(0, jsonDipendenti["ComportamentiRipetitiviEInteressiRistretti"][listaValoriCategoria[1]].Count)];
            categorie.Add(Categorie.ComportamentiRipetitiviEInteressiRistrettiPrecisioneOperativa, primo);
            categorie.Add(Categorie.ComportamentiRipetitiviEInteressiRistrettiFocalizzazioneTematica, secondo);
            descrizioni.Add(stringa);
            
            (primo, secondo, stringa) = jsonDipendenti["SensibilitaSensoriale"][listaValoriCategoria[2]][UnityEngine.Random.Range(0, jsonDipendenti["SensibilitaSensoriale"][listaValoriCategoria[2]].Count)];
            categorie.Add(Categorie.SensibilitaSensorialePercezioneSensorialeFina, primo);
            categorie.Add(Categorie.SensibilitaSensorialeComfortAmbientale, secondo);
            descrizioni.Add(stringa);
            
            (primo, secondo, stringa) = jsonDipendenti["CapacitaCognitive"][listaValoriCategoria[3]][UnityEngine.Random.Range(0, jsonDipendenti["CapacitaCognitive"][listaValoriCategoria[3]].Count)];
            categorie.Add(Categorie.CapacitaCognitiveProblemSolvingCreativo, primo);
            categorie.Add(Categorie.CapacitaCognitiveFlessibilitaCognitiva, secondo);
            descrizioni.Add(stringa);

            if (neurodivergente)
            {
                // Numero da 1 a 10, con una distribuzione che favorisce i numeri più alti o più bassi
                double r = UnityEngine.Random.value; 
                r = (r < 0.5) ? r * r : 1 - (1 - r) * (1 - r); 
                int numero = (int)(r * 10) + 1;
                (primo, secondo, stringa) = jsonDipendenti["InterazioneSociale"][numero][UnityEngine.Random.Range(0, jsonDipendenti["InterazioneSociale"][numero].Count)];
                categorie.Add(Categorie.InterazioneSocialePreferenzaSociale, primo);
                categorie.Add(Categorie.InterazioneSocialeStabilitaEmotivaInTeam, secondo);
                descrizioni.Add(stringa);
            }
            else
            {
                int numero = UnityEngine.Random.Range(6, 9);  // Da 6 a 8 inclusi
                (primo, secondo, stringa) = jsonDipendenti["InterazioneSociale"][numero][UnityEngine.Random.Range(0, jsonDipendenti["InterazioneSociale"][numero].Count)];
                categorie.Add(Categorie.InterazioneSocialePreferenzaSociale, primo);
                categorie.Add(Categorie.InterazioneSocialeStabilitaEmotivaInTeam, secondo);
                descrizioni.Add(stringa);
            }
            
            // Scrivo la descrizione del dipendente
            List<string> descrizioneCompleta = descrizioni.OrderBy(x => Guid.NewGuid()).ToList();
            
            // Genero un numero casuale da 0 a 1 per decidere se il dipendente è maschio o femmina
            int genere = UnityEngine.Random.Range(0, 2);
            
            
            TextAsset file = Resources.Load<TextAsset>("Json/NomiDipendenti");
            var nomi = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(file.text);
            string nome = genere == 0
                ? nomi["M"][UnityEngine.Random.Range(0, nomi["M"].Length)]
                : nomi["F"][UnityEngine.Random.Range(0, nomi["F"].Length)];
            
            Sprite[] foto = genere == 0
                ? UnityEngine.Resources.LoadAll<Sprite>("Images/Foto/Maschietti")
                : UnityEngine.Resources.LoadAll<Sprite>("Images/Foto/Femminucce");
            
            var fotoProfilo = genere == 0
                ? "Maschietti/" + foto[UnityEngine.Random.Range(0, foto.Length)].name
                : "Femminucce/" + foto[UnityEngine.Random.Range(0, foto.Length)].name;
           
            // Creo il dipendente
            Dipendente dipendente = new Dipendente(categorie, descrizioneCompleta.ToArray(), fotoProfilo, nome);

            return dipendente;
        }

        // Funzione di caricamento del salvataggio
        public void OnAfterLoad(Team team)
        {
            this.team = team;
        }
    }
}