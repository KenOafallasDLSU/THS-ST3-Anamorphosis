using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerSignaling : MonoBehaviour
{
    // Passed as function to target ObserverBehavior
    // OnFound, signals ON_MARKER_FOUND with marker type as param
    public void MarkerFoundSignal(string marker)
    {
        Parameters markerParams = new Parameters();
        markerParams.PutExtra(ParamConstants.Extra_Keys.MARKER_NAME, marker);
        markerParams.PutExtra(ParamConstants.Extra_Keys.MARKER_STATUS, true);
        EventBroadcaster.Instance.PostEvent(EventNames.Anamorphosis_Events.ON_MARKER_FOUND, markerParams);
    }

    // Passed as function to target ObserverBehavior
    // OnLost, signals ON_MARKER_LOST with marker type as param
    public void MarkerLostSignal(string marker)
    {
        Parameters markerParams = new Parameters();
        markerParams.PutExtra(ParamConstants.Extra_Keys.MARKER_NAME, marker);
        markerParams.PutExtra(ParamConstants.Extra_Keys.MARKER_STATUS, false);
        EventBroadcaster.Instance.PostEvent(EventNames.Anamorphosis_Events.ON_MARKER_LOST, markerParams);
    }
}
