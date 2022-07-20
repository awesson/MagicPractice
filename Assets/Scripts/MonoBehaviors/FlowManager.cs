using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameView : MonoBehaviour
{
    public abstract void Display();
}

public class FlowManager : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenu = default;

    [SerializeReference]
    private GameView MagiciansView = default;

    [SerializeReference]
    private GameView AssistantsView = default;

    private void Start()
    {
        ShowMainMenu();
    }

    public void OnBackToMainMenuClicked()
    {
        ShowMainMenu();
    }

    public void OnPracticeMagicianClicked()
    {
        MainMenu.SetActive(false);
        MagiciansView.gameObject.SetActive(true);
        MagiciansView.Display();
    }

    public void OnPracticeAssistantClicked()
    {
        MainMenu.SetActive(false);
        AssistantsView.gameObject.SetActive(true);
        AssistantsView.Display();
    }

    private void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        MagiciansView.gameObject.SetActive(false);
        AssistantsView.gameObject.SetActive(false);
    }
}
