using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDetection : MonoBehaviour
{
    string[] markerArray = {
        ParamConstants.Marker_Names.PEBBLES,
        ParamConstants.Marker_Names.WOODCHIPS,
        ParamConstants.Marker_Names.GRASS,
        ParamConstants.Marker_Names.ASPHALT
    };
    bool[] statusArray = {false, false, false, false};
    string trackingMode = ParamConstants.Tracking_Modes.ZERO_MARKER_MODE;

    // Start is called before the first frame update
    void Start()
    {
        EventBroadcaster.Instance.AddObserver(EventNames.Anamorphosis_Events.ON_MARKER_FOUND, this.OnMarkerChange);
        EventBroadcaster.Instance.AddObserver(EventNames.Anamorphosis_Events.ON_MARKER_LOST, this.OnMarkerChange);
    }

    private void OnMarkerChange(Parameters parameters)
    {
        string marker = parameters.GetStringExtra(ParamConstants.Extra_Keys.MARKER_NAME, "");
        bool status = parameters.GetBoolExtra(ParamConstants.Extra_Keys.MARKER_STATUS, false);

        UpdateMarkerStatus(marker, status);
        Debug.Log("PEBBLES: " + statusArray[0] + ", WOODCHIPS: " + statusArray[1] + ", GRASS: " + statusArray[2] + ", ASPHALT: " + statusArray[3]);

        string newTrackingMode = CheckMarkerMode();
        if(trackingMode != newTrackingMode)
        {
            trackingMode = newTrackingMode;
            MarkerModeChangeSignal(newTrackingMode);
        }
    }

    private void UpdateMarkerStatus(string marker, bool status)
    {
        for(int i = 0; i < markerArray.Length; i++)
        {
            if(marker == markerArray[i])
                statusArray[i] = status;
        }
    }

    private string CheckMarkerMode()
    {
        if(statusArray[0] && statusArray[1] && statusArray[2] && statusArray[3])
            return ParamConstants.Tracking_Modes.FOUR_MARKER_MODE;
        else if (statusArray[0] && statusArray[1])
            return ParamConstants.Tracking_Modes.TWO_MARKER_MODE;
        else if (statusArray[0] && !statusArray[1])
            return ParamConstants.Tracking_Modes.ONE_MARKER_MODE;
        else
            return ParamConstants.Tracking_Modes.ZERO_MARKER_MODE;
    }

    private void MarkerModeChangeSignal(string mode)
    {
        Parameters markerParams = new Parameters();
        markerParams.PutExtra(ParamConstants.Extra_Keys.MARKER_MODE, mode);
        EventBroadcaster.Instance.PostEvent(EventNames.Anamorphosis_Events.ON_MARKER_MODE_CHANGE, markerParams);
    }

    private void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(EventNames.Anamorphosis_Events.ON_MARKER_FOUND);
        EventBroadcaster.Instance.RemoveObserver(EventNames.Anamorphosis_Events.ON_MARKER_LOST);
    }
}
