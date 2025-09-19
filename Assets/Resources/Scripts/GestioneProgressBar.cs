using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GestioneProgressBar : MonoBehaviour
{
    public GameObject IgnotoText;
    public GameObject Bar;
    
    public void ShowValue(float value)
    {
        // Prendi il children "Image" e cambia lo scale in base al valore compreso tra 0 e 1 su un range di 0 a 100
        if (value < 0)
        {
            IgnotoText.SetActive(true);
            gameObject.GetComponent<Image>().enabled = false;
            Bar.gameObject.SetActive(false);
        }
        else
        {
            if(IgnotoText != null)
                IgnotoText.SetActive(false);
            Bar.SetActive(true);
            Bar.transform.localScale = new Vector3(value / 100f, 1, 1);
            gameObject.GetComponent<Image>().enabled = true;
        }
    }
}
