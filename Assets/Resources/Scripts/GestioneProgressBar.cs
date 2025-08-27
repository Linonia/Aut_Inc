using UnityEngine;
using UnityEngine.UI;

public class GestioneProgressBar : MonoBehaviour
{
    public void ShowValue(float value)
    {
        // Prendi il children "Image" e cambia lo scale in base al valore compreso tra 0 e 1 su un range di 0 a 100
        gameObject.transform.Find("Image").gameObject.transform.localScale = new Vector3(value / 100f, 1, 1);
    }
}
