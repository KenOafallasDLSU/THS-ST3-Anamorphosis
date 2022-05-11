using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker
{
  private GameObject markerObject;
  private string name;
  private bool status;

  public Marker(GameObject markerObject, string name, bool status)
  {
    this.markerObject = markerObject;
    this.name = name;
    this.status = status;
  }

  public GameObject GetMarkerObject()
  {
    return markerObject;
  }

  public bool GetStatus()
  {
    return status;
  }

  public string GetName()
  {
    return name;
  }


  public void SetStatus(bool status)
  {
    this.status = status;
  }
}