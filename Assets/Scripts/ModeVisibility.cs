using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeVisibility : MonoBehaviour
{
    [SerializeField] string activeMode;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        EventBroadcaster.Instance.AddObserver(EventNames.Anamorphosis_Events.ON_MARKER_MODE_CHANGE, this.OnMarkerModeChange);
    }

    private void OnMarkerModeChange(Parameters parameters)
    {
        string newMode = parameters.GetStringExtra(ParamConstants.Extra_Keys.MARKER_MODE, "");
        if(newMode == activeMode)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(EventNames.Anamorphosis_Events.ON_MARKER_MODE_CHANGE);
    }
}
