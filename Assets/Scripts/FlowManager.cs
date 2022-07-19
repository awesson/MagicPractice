using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenu;

    [SerializeField]
    private GameObject MagiciansView;

    [SerializeField]
    private GameObject AssistantsView;

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
        MagiciansView.SetActive(true);
        MagiciansView.GetComponent<Magician>().Display();
    }

    public void OnPracticeAssistantClicked()
    {
        MainMenu.SetActive(false);
        MagiciansView.SetActive(true);
        MagiciansView.GetComponent<Assistant>().Display();
    }

    private void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        MagiciansView.SetActive(false);
        AssistantsView.SetActive(false);
    }
}
