using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DistanceTesting : MonoBehaviour
{
  [SerializeField] private GameObject camera;
  [SerializeField] private GameObject solutionPoint;

  [SerializeField] private Text CameraPos;
  [SerializeField] private Text SolutionPos;

  void Update()
    {
      CameraPos.text = "Cam: " + camera.transform.position.ToString("F3");
      SolutionPos.text = "Sol: " + solutionPoint.transform.position.ToString("F3");
    }
}