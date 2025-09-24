using UnityEngine;

public class VisualizzaCrediti : MonoBehaviour
{
    public CompilaTutorialPanelMenuPrincipale tutorialPanelObject;
    
    public void MostraCrediti()
    {
        tutorialPanelObject.MostraTutorial("aboutus1", null, true);
    }
}
