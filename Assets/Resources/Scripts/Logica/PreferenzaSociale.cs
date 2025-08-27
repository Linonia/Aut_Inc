using System.Collections.Generic;

namespace Scripts.Logica
{
    public static class PreferenzaSociale
    {
        // Curva di socialità per i dipendenti in base alla loro preferenza sociale
        public static readonly Dictionary<int, (int, int) > CurvaSocialita = new Dictionary<int, (int, int)>()
        {
            {1, (1, 1)},  // socialità 1
            {2, (1, 2)},  // 2
            {3, (1, 3)},  // 3
            {4, (2, 4)},  // 4
            {5, (2, 5)},  // 5
            {6, (2, 5)},  // 6
            {7, (2, 5)},  // 7
            {8, (3, 6)},  // 8
            {9, (3, 6)},  // 9
            {10, (4, 6)}   // 10
        };
    
        // Penalità per l'abbandono del team in base alla stabilità emotiva in team
        public static readonly Dictionary<int, int> PenalitaAbbandonoTeam = new Dictionary<int, int>
        {
            {1, 0},
            {2, 1},
            {3, 3},
            {4, 6},
            {5, 9},
            {6, 13},
            {7, 16},
            {8, 18},
            {9, 19},
            {10, 20}
        };
    }
}