using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button helpBtn;
    [SerializeField] private Button onBtn;
    [SerializeField] private Button offBtn;
    [SerializeField] private Image instructions;

    [SerializeField] private GameObject boundingBox;

    void Start()
    {
        instructions.gameObject.SetActive(false);
        offBtn.gameObject.SetActive(false);
    }

    public void showSelection()
    {
        helpBtn.gameObject.SetActive(false);
        offBtn.gameObject.SetActive(false);
    }

    public void showInst()
    {
        //helpBtn.gameObject.SetActive(false);
        instructions.gameObject.SetActive(true);
    }

    public void closeInst()
    {
        //helpBtn.gameObject.SetActive(true);
        instructions.gameObject.SetActive(false);
    }

    public void showBox()
    {
        onBtn.gameObject.SetActive(true);
        offBtn.gameObject.SetActive(false);
        boundingBox.gameObject.SetActive(true);
    }

    public void hideBox()
    {
        onBtn.gameObject.SetActive(false);
        offBtn.gameObject.SetActive(true);
        boundingBox.gameObject.SetActive(false);
    }
}

