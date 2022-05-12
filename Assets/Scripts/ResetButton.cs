using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ResetButton : MonoBehaviour
{
    [SerializeField] private Button resetButton;

    // Start is called before the first frame update
    void Start()
    {
        resetButton.onClick.AddListener(TaskOnClick);
    }

    private void TaskOnClick()
    {
        VuforiaBehaviour.Instance.enabled = false;
        VuforiaBehaviour.Instance.enabled = true;
    }
}
