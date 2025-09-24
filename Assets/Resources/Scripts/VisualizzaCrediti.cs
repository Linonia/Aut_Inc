using UnityEngine;

public class VisualizzaCrediti : MonoBehaviour
{
    public CompilaTutorialPanelMenuPrincipale tutorialPanelObject;
    
    public void MostraCrediti()
    {
        Debug.Log("AAOAAOAOAOAO");
        tutorialPanelObject.MostraTutorial("aboutus1", null, true);
    }
}
