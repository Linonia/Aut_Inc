namespace Scripts.Logica
{
    public static class ParametriContratto
    {
        // Moltiplicatori sulla produzione settimanale per calcolare il valore totale del contratto
        public static float MoltiplicatorePagaFacile { get; set; } = 0.9f;
        public static float MoltiplicatorePagaMedia { get; set; } = 1.1f;
        public static float MoltiplicatorePagaDifficile { get; set; } = 1.35f;

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
    }
}