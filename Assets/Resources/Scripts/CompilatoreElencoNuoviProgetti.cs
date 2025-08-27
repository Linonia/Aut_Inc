using Scripts.Logica;
using UnityEngine;

public class CompilatoreElencoNuoviProgetti : MonoBehaviour
{
    public GameObject prefabNuovoProgetto;
    public void OnEnable()
    {
        // Ripulisci la lista dei progetti
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        
        Transform contenitore = gameObject.transform;
        // Riempi la lista con dei nuovi progetti
        for (int i = 0; i < 4; i++)
        {   
            Progetto progetto = Progetto.CreaProgetto();
            GameObject nuovoProgetto = Instantiate(prefabNuovoProgetto, contenitore);
            nuovoProgetto.GetComponent<CompilatoreEtichettaNuoviProgetti>().Compila(progetto);
        }
    }
}
