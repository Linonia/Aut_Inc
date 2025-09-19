using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CompilaTutorialPanel : MonoBehaviour
{

    public TMP_Text titolo;
    public TMP_Text testo;

    public void MostraTutorial(string tit, Azienda azienda)
    {
        titolo.text = "Tutorial: " + LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TITOLO");
        testo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TESTO");
        azienda.flags[tit] = true;
        gameObject.SetActive(true);
    }
}
