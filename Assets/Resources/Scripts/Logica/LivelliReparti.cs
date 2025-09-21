using System.Collections.Generic;

namespace Scripts.Logica
{
    public class LivelliReparti
    {
        // Struttura per i livelli dei reparti
        // Contiene: minimo team, massimo team, massimo dipendenti, costo dipendente
        public static readonly Dictionary<int, (int, int, int, int, int) > LivelloReparto = new Dictionary<int, (int, int, int, int, int)>()
        {
            {1, (2, 3, 6, 1500, 60000)},
            {2, (3, 4, 9, 2500, 90000)},
            {3, (4, 5, 12, 3500, 150000)},
            {4, (4, 5, 14, 5000, 0)}
        };
    
    
    }
}