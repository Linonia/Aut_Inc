namespace Scripts.Logica
{
    public enum TipoVarianteProgetto
    {
        Nessuna,
        MoltoSemplice,
        MoltoDifficile,
        PocoRemunerativo,
        MoltoRemunerativo
    }

    public static class ParametriContratto
    {
        // Moltiplicatori sulla produzione settimanale per calcolare il valore totale del contratto
        public static float MoltiplicatorePagaFacile { get; set; } = 0.8f;
        public static float MoltiplicatorePagaMedia { get; set; } = 1.0f;
        public static float MoltiplicatorePagaDifficile { get; set; } = 1.30f;

        // Percentuali dei pagamenti (rispetto al valore totale)
        public static float PercentualeAnticipo { get; set; } = 0.15f;  // 15% all'inizio
        public static float PercentualeFinale { get; set; } = 0.25f;    // 25% alla fine

        // Range di settimane per la durata dei contratti per difficoltà
        public static int MinSettimaneFacile { get; set; } = 4;
        public static int MaxSettimaneFacile { get; set; } = 10;

        public static int MinSettimaneMedia { get; set; } = 4;
        public static int MaxSettimaneMedia { get; set; } = 12;

        public static int MinSettimaneDifficile { get; set; } = 7;
        public static int MaxSettimaneDifficile { get; set; } = 16;

        // Penalità / bonus (% applicata al finale per anticipo o ritardo)
        public static int DetrazioneFacile { get; set; } = 8;
        public static int DetrazioneMedia { get; set; } = 5;
        public static int DetrazioneDifficile { get; set; } = 3;

        // Penalità fissa per rescindere un contratto
        public static float PenaleRescissione { get; set; } = 0.35f; // 35% del valore totale


        // ===== NUOVE PROBABILITÀ VARIANTI =====
        // Facile
        public static int ProbFacile_MoltoSemplice { get; set; } = 5;
        public static int ProbFacile_MoltoDifficile { get; set; } = 2;
        public static int ProbFacile_PocoRemunerativo { get; set; } = 5;
        public static int ProbFacile_MoltoRemunerativo { get; set; } = 5;

        // Medio
        public static int ProbMedia_MoltoSemplice { get; set; } = 5;
        public static int ProbMedia_MoltoDifficile { get; set; } = 5;
        public static int ProbMedia_PocoRemunerativo { get; set; } = 5;
        public static int ProbMedia_MoltoRemunerativo { get; set; } = 8;

        // Difficile
        public static int ProbDifficile_MoltoSemplice { get; set; } = 3;
        public static int ProbDifficile_MoltoDifficile { get; set; } = 5;
        public static int ProbDifficile_PocoRemunerativo { get; set; } = 5;
        public static int ProbDifficile_MoltoRemunerativo { get; set; } = 8;
        
        // Percentuaoli di impatto delle varianti
        public static float ImpattoVariante_MoltoSemplice { get; set; } = 0.75f;      // -25% di difficoltà
        public static float ImpattoVariante_MoltoDifficile { get; set; } = 1.20f;      // +20% di difficoltà
        
        public static float ImpattoVariante_PocoRemunerativo { get; set; } = 0.80f;     // -20% di paga
        public static float ImpattoVariante_MoltoRemunerativo { get; set; } = 1.30f;    // +30% di paga


        // ===== FUNZIONE PER OTTENERE LA VARIANTE =====
        public static TipoVarianteProgetto GetVariante(int difficolta)
        {
            int roll = UnityEngine.Random.Range(0, 100); // 0–99
            int accumulatore = 0;

            switch (difficolta)
            {
                case 0: // FACILE
                    accumulatore += ProbFacile_MoltoSemplice;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoSemplice;

                    accumulatore += ProbFacile_MoltoDifficile;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoDifficile;

                    accumulatore += ProbFacile_PocoRemunerativo;
                    if (roll < accumulatore) return TipoVarianteProgetto.PocoRemunerativo;

                    accumulatore += ProbFacile_MoltoRemunerativo;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoRemunerativo;
                    break;

                case 1: // MEDIO
                    accumulatore += ProbMedia_MoltoSemplice;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoSemplice;

                    accumulatore += ProbMedia_MoltoDifficile;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoDifficile;

                    accumulatore += ProbMedia_PocoRemunerativo;
                    if (roll < accumulatore) return TipoVarianteProgetto.PocoRemunerativo;

                    accumulatore += ProbMedia_MoltoRemunerativo;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoRemunerativo;
                    break;

                case 2: // DIFFICILE
                    accumulatore += ProbDifficile_MoltoSemplice;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoSemplice;

                    accumulatore += ProbDifficile_MoltoDifficile;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoDifficile;

                    accumulatore += ProbDifficile_PocoRemunerativo;
                    if (roll < accumulatore) return TipoVarianteProgetto.PocoRemunerativo;

                    accumulatore += ProbDifficile_MoltoRemunerativo;
                    if (roll < accumulatore) return TipoVarianteProgetto.MoltoRemunerativo;
                    break;
            }

            return TipoVarianteProgetto.Nessuna; // se non rientra in nessuna percentuale
        }
    }
}
