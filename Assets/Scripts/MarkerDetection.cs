using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MarkerDetection : MonoBehaviour
{
    [SerializeField] private GameObject[] markerArray;
    private Dictionary<string, Marker> markerDict;
    private ArrayList markerOrderedArray;
    private string trackingMode = ParamConstants.Tracking_Modes.ZERO_MARKER_MODE;

    private void Awake()
    {
        markerDict = new Dictionary<string, Marker>(){
            {ParamConstants.Marker_Names.PEBBLES, new Marker(markerArray[0], ParamConstants.Marker_Names.PEBBLES, false)},
            {ParamConstants.Marker_Names.WOODCHIPS, new Marker(markerArray[1], ParamConstants.Marker_Names.WOODCHIPS, false)},
            {ParamConstants.Marker_Names.GRASS, new Marker(markerArray[2], ParamConstants.Marker_Names.GRASS, false)},
            {ParamConstants.Marker_Names.ASPHALT, new Marker(markerArray[3], ParamConstants.Marker_Names.ASPHALT, false)}
        };

        markerOrderedArray = new ArrayList();

        EventBroadcaster.Instance.AddObserver(EventNames.Anamorphosis_Events.ON_MARKER_FOUND, this.OnMarkerChange);
        EventBroadcaster.Instance.AddObserver(EventNames.Anamorphosis_Events.ON_MARKER_LOST, this.OnMarkerChange);
    }

    private void OnMarkerChange(Parameters parameters)
    {
        string marker = parameters.GetStringExtra(ParamConstants.Extra_Keys.MARKER_NAME, "");
        bool status = parameters.GetBoolExtra(ParamConstants.Extra_Keys.MARKER_STATUS, false);
        //GameObject markerObject = parameters.GetObjectExtra(ParamConstants.Extra_Keys.MARKER_OBJECT);

        Debug.Log(marker);

        if(markerDict.ContainsKey(marker) == false) return;

        markerDict[marker].SetStatus(status);

        string newTrackingMode = CheckMarkerMode();

        Debug.Log(newTrackingMode);

        if(trackingMode != newTrackingMode)
        {
            trackingMode = newTrackingMode;

            SetMarkerOrderedArray();

            MarkerModeChangeSignal(newTrackingMode);
        }
    }

    private string CheckMarkerMode()
    {
        int count = 0;
        foreach(Marker marker in markerDict.Values)
            if (marker.GetStatus()) count++;

        if(count >= 4)
            return ParamConstants.Tracking_Modes.FOUR_MARKER_MODE;
        else if (count >= 2)
            return ParamConstants.Tracking_Modes.TWO_MARKER_MODE;
        else if (count == 1)
            return ParamConstants.Tracking_Modes.ONE_MARKER_MODE;
        else
            return ParamConstants.Tracking_Modes.ZERO_MARKER_MODE;
    }

    private void SetMarkerOrderedArray()
    {
        if(trackingMode == ParamConstants.Tracking_Modes.ONE_MARKER_MODE)
        {
            // find true status marker
            foreach(Marker marker in markerDict.Values)
                if (marker.GetStatus())
                {
                    markerOrderedArray = new ArrayList(){marker};
                    break;
                }
        }
        else if (trackingMode == ParamConstants.Tracking_Modes.TWO_MARKER_MODE)
        {
            // find true status marker
            string[] names = new string[2];
            int i = 0;
            foreach(Marker marker in markerDict.Values)
                if (marker.GetStatus())
                {
                    names[i] = marker.GetName();
                    i++;
                    if(i==2) break;
                }

            if(markerDict[names[0]].GetMarkerObject().transform.position.x >= markerDict[names[1]].GetMarkerObject().transform.position.x)
                markerOrderedArray = new ArrayList(){markerDict[names[0]], markerDict[names[1]]};
            else
                markerOrderedArray = new ArrayList(){markerDict[names[1]], markerDict[names[0]]};
        }
        else if (trackingMode == ParamConstants.Tracking_Modes.FOUR_MARKER_MODE)
        {
            List<Vector3> targetPositions = new List<Vector3>();
            int topLeft = 0, botLeft = 0, topRight = 0, botRight = 0;

            foreach(Marker marker in markerDict.Values)
                targetPositions.Add(marker.GetMarkerObject().transform.position);

            Vector3 center = getCenterPosition(targetPositions);

            for(int i = 0; i < 4; i++)
            {
                Vector3 pos = markerDict.ElementAt(i).Value.GetMarkerObject().transform.position;
                if(pos.x < center.x && pos.z > center.z) topLeft = i;
                else if(pos.x < center.x && pos.z < center.z) botLeft = i;
                else if(pos.x > center.x && pos.z > center.z) topRight = i;
                else botRight = i;
            }

            Debug.Log("TL: " + markerDict.ElementAt(topLeft).Value.GetName());
            Debug.Log("BL: " + markerDict.ElementAt(botLeft).Value.GetName());
            Debug.Log("TR: " + markerDict.ElementAt(topRight).Value.GetName());
            Debug.Log("BR: " + markerDict.ElementAt(botRight).Value.GetName());

            markerOrderedArray = new ArrayList(){
                markerDict.ElementAt(topLeft).Value,
                markerDict.ElementAt(botLeft).Value,
                markerDict.ElementAt(topRight).Value,
                markerDict.ElementAt(botRight).Value
            };
        }
        else
            markerOrderedArray = new ArrayList(){};
    }

    private Vector3 getCenterPosition(List<Vector3> v)
    {
        Vector3 temp = new Vector3();
        foreach (Vector3 vec in v)
            temp += vec;
        return temp / v.Count;
    }

    private void MarkerModeChangeSignal(string mode)
    {
        Parameters markerParams = new Parameters();

        markerParams.PutExtra(ParamConstants.Extra_Keys.MARKER_MODE, mode);
        markerParams.PutExtra(ParamConstants.Extra_Keys.MARKER_ARRAY, markerOrderedArray);

        EventBroadcaster.Instance.PostEvent(EventNames.Anamorphosis_Events.ON_MARKER_MODE_CHANGE, markerParams);
    }

    private void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(EventNames.Anamorphosis_Events.ON_MARKER_FOUND);
        EventBroadcaster.Instance.RemoveObserver(EventNames.Anamorphosis_Events.ON_MARKER_LOST);
    }
}