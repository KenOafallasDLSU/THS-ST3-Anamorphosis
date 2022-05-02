using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image title;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button helpBtn;
    [SerializeField] private Image instructions;
    [SerializeField] private Button closeBtn;

    void Start()
    {
        instructions.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
    }

    public void showSelection()
    {
        startBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        helpBtn.gameObject.SetActive(false);
    }

    public void showInst()
    {
        instructions.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(true);
    }

    public void hideInst()
    {
        instructions.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
    }

}
