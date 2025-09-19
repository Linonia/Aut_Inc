using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Scripts.Logica;
using UnityEngine;

[Serializable]
public class SalvaAzienda
{
    public int capitale;
    public int costoDipendenteLibero;
    public int tasseMensile;
    public int tempoDiminuzioneGuadagno;

    public int anno;
    public int mese;
    public int settimana;

    public int costoReparto;

    public List<NomiReparti> repartiDaSbloccare;
    public List<Dipendente> dipendentiLiberi;
    public Dictionary<NomiReparti, Reparto> reparti; // chiavi come stringhe
    public List<Progetto> progettiInCorso;
    public List<Progetto> progettiCompletatiInSettimana;
    public List<Progetto> progettiProposti;

    public Dictionary<string, bool> flags;
}

public static class SalvataggioAzienda
{
    private static string GetPath() => Application.persistentDataPath + "/savegame.json";

    public static void Salva(Azienda azienda)
    {
        SalvaAzienda data = new SalvaAzienda
        {
            capitale = azienda.capitale,
            costoDipendenteLibero = azienda.costoDipendenteLibero,
            tasseMensile = azienda.tasseMensile,
            tempoDiminuzioneGuadagno = azienda.tempoDiminuzioneGuadagno,

            anno = azienda.anno,
            mese = azienda.mese,
            settimana = azienda.settimana,

            costoReparto = azienda.costoReparto,

            reparti = azienda.reparti,
            repartiDaSbloccare = azienda.repartiDaSbloccare,
            dipendentiLiberi = azienda.dipendentiLiberi,
            progettiInCorso = azienda.progettiInCorso,
            progettiCompletatiInSettimana = azienda.progettiCompletatiInSettimana,
            progettiProposti = azienda.progettiProposti,

            flags = azienda.flags
        };

        // convertiamo i dizionari con enum come chiave in stringhe
        

        string json = JsonConvert.SerializeObject(data, Formatting.Indented,
            new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        File.WriteAllText(GetPath(), json);
        Debug.Log("Salvataggio completato: " + GetPath());
    }

    public static void Carica(Azienda azienda)
    {
        string path = GetPath();
        if (!File.Exists(path)) return;

        try
        {
            string json = File.ReadAllText(path);
            SalvaAzienda data = JsonConvert.DeserializeObject<SalvaAzienda>(json);

            // Inizializza dizionari/liste
            data.reparti ??= new Dictionary<NomiReparti, Reparto>();
            data.dipendentiLiberi ??= new List<Dipendente>();
            data.repartiDaSbloccare ??= new List<NomiReparti>();
            data.progettiInCorso ??= new List<Progetto>();
            data.progettiCompletatiInSettimana ??= new List<Progetto>();
            data.progettiProposti ??= new List<Progetto>();
            data.flags ??= new Dictionary<string,bool>();

            // Assegna valori ad Azienda
            azienda.capitale = data.capitale;
            azienda.costoDipendenteLibero = data.costoDipendenteLibero;
            azienda.tasseMensile = data.tasseMensile;
            azienda.tempoDiminuzioneGuadagno = data.tempoDiminuzioneGuadagno;

            azienda.anno = data.anno;
            azienda.mese = data.mese;
            azienda.settimana = data.settimana;

            azienda.costoReparto = data.costoReparto;

            azienda.reparti = data.reparti;
            azienda.repartiDaSbloccare = data.repartiDaSbloccare;
            azienda.dipendentiLiberi = data.dipendentiLiberi;
            azienda.progettiInCorso = data.progettiInCorso;
            azienda.progettiCompletatiInSettimana = data.progettiCompletatiInSettimana;
            azienda.progettiProposti = data.progettiProposti;

            azienda.flags = data.flags;

            Debug.Log("Caricamento completato: " + path);
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante il caricamento: " + e);
        }
    }

}
