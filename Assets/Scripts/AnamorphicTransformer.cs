using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Vuforia;

public class AnamorphicTransformer : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] GameObject[] markerArray;
    [SerializeField] GameObject[] sliceArray;
    [SerializeField] GameObject solutionPoint;
    private ArrayList markerOrderedArray;
    string newMode = ParamConstants.Tracking_Modes.ZERO_MARKER_MODE;

    // repositioner/scaler
    [SerializeField] public GameObject model;
    private float markerdist;
    private float scale;
    private Vector3 centerPosition;
    List<Vector3> targetPosition = new List<Vector3>();

    // solution checking
    private bool tracking;
    
    void Awake()
    {
        tracking = false;
        markerOrderedArray = new ArrayList();

        EventBroadcaster.Instance.AddObserver(EventNames.Anamorphosis_Events.ON_MARKER_MODE_CHANGE, this.OnMarkerModeChange);
    }

    void Update()
    {
        if(tracking) //check if solved
        {
            //store values for easier referencing
            float cam_x = camera.transform.position.x;
            float cam_y = camera.transform.position.y;
            float cam_z = camera.transform.position.z;

            float sol_x = solutionPoint.transform.position.x;
            float sol_y = solutionPoint.transform.position.y;
            float sol_z = solutionPoint.transform.position.z;

            bool goodX = cam_x > sol_x-0.02 && cam_x < sol_x+0.02;
            bool goodY = cam_y > sol_y-0.02 && cam_y < sol_y+0.02;
            bool goodZ = cam_z > sol_z-0.02 && cam_z < sol_z+0.02;

            if(goodX && goodY && goodZ)
            {
                EventBroadcaster.Instance.PostEvent(EventNames.Anamorphosis_Events.ON_WIN);
                tracking = false;

                // delete all marker children
                foreach (GameObject marker in markerArray)
                    foreach (Transform child in marker.transform)
                        Destroy(child.gameObject);
            }
                
        }
    }

    private void OnMarkerModeChange(Parameters parameters)
    {
        // delete all marker children
        foreach (GameObject marker in markerArray)
            foreach (Transform child in marker.transform)
                Destroy(child.gameObject);

        // get new marker mode
        newMode = parameters.GetStringExtra(ParamConstants.Extra_Keys.MARKER_MODE, ParamConstants.Tracking_Modes.ZERO_MARKER_MODE);
        markerOrderedArray = parameters.GetArrayListExtra(ParamConstants.Extra_Keys.MARKER_ARRAY);

        if(newMode != ParamConstants.Tracking_Modes.ZERO_MARKER_MODE)
        {
            // reposition model holder
            Reposition();

            Anamorphosize();

            tracking = true;
        }
        else
            tracking = false; 
    }

    private void Anamorphosize()
    {
        float cx = solutionPoint.transform.position.x; //Camera.main.transform.position.x;
        float cy = solutionPoint.transform.position.y; //Camera.main.transform.position.y;
        float cz = solutionPoint.transform.position.z; //Camera.main.transform.position.z;

        foreach (GameObject origslice in sliceArray)
        {
            /**
            * Randomizing scale
            */
            GameObject slice = GameObject.Instantiate(origslice, origslice.transform.position, origslice.transform.rotation);
            slice.SetActive(true);

            //randomly scale
            float s = UnityEngine.Random.value + 0.5f;
            slice.transform.localScale *= s;

            //reposition x
            float mx = slice.transform.position.x;
            float nx = cx - ((cx - mx) * s);

            //reposition y
            float my = slice.transform.position.y;
            float ny = cy - ((cy - my) * s);

            //reposition z
            float mz = slice.transform.position.z;
            float nz = cz - ((cz - mz) * s);

            slice.transform.position = new Vector3(nx, ny, nz);

            /**
            * Change parent based on scale
            */
            // for one marker, parent everything to that marker
            if(newMode == ParamConstants.Tracking_Modes.ONE_MARKER_MODE)
            {
                slice.transform.parent = ((Marker)markerOrderedArray[0]).GetMarkerObject().transform;
            }
            // for two markers, parent larger slices to farther marker, smmaller to closer marker
            else if(newMode == ParamConstants.Tracking_Modes.TWO_MARKER_MODE)
            {
                if(s > 1.0f) //check if father/closer to the solution point
                    slice.transform.parent = ((Marker)markerOrderedArray[0]).GetMarkerObject().transform;
                else
                    slice.transform.parent = ((Marker)markerOrderedArray[1]).GetMarkerObject().transform;
            }
            // for four markers, determine if smaller or larger
            // then parent to left marker if closer to left, accordingly for right marker
            else if(newMode == ParamConstants.Tracking_Modes.FOUR_MARKER_MODE)
            {
                if(s > 1.0f) //check if father/closer to the solution point
                {
                    float xmid = (((Marker)markerOrderedArray[0]).GetMarkerObject().transform.position.x + ((Marker)markerOrderedArray[2]).GetMarkerObject().transform.position.x)/2;
                    if(slice.transform.position.x < xmid)
                        slice.transform.parent = ((Marker)markerOrderedArray[0]).GetMarkerObject().transform;
                    else
                        slice.transform.parent = ((Marker)markerOrderedArray[2]).GetMarkerObject().transform;
                }
                else
                {
                    float xmid = (((Marker)markerOrderedArray[1]).GetMarkerObject().transform.position.x + ((Marker)markerOrderedArray[3]).GetMarkerObject().transform.position.x)/2;
                    if(slice.transform.position.x < xmid)
                        slice.transform.parent = ((Marker)markerOrderedArray[1]).GetMarkerObject().transform;
                    else
                        slice.transform.parent = ((Marker)markerOrderedArray[3]).GetMarkerObject().transform;
                }
            }
            else
            {
                Destroy(slice);
            }
        }
    }

    private void Reposition()
    {
        model.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        model.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        this.targetPosition.Add(((Marker)markerOrderedArray[0]).GetMarkerObject().transform.position);

        if(newMode == ParamConstants.Tracking_Modes.TWO_MARKER_MODE)
            this.targetPosition.Add(((Marker)markerOrderedArray[1]).GetMarkerObject().transform.position);
        
        if(newMode == ParamConstants.Tracking_Modes.FOUR_MARKER_MODE)
        {
            this.targetPosition.Add(((Marker)markerOrderedArray[2]).GetMarkerObject().transform.position);
            this.targetPosition.Add(((Marker)markerOrderedArray[3]).GetMarkerObject().transform.position);
        }
        
        // get the center of all markers in the list
        centerPosition = getCenterPosition(this.targetPosition);

        // contains the maximum x and y distances among the marker/s
        Vector3 max = getMaxVector(this.targetPosition);
        this.markerdist = max.z > max.x ? max.z : max.x;

        // scale starts at 1, and scales linearly depending on the distance of the two markers
        this.scale = 1f + (float)markerdist;

        // if (this.scale > 1.5) // refers to the length of 2 markers
        //     this.model.SetActive(false); // deactivates the model
        // else if (this.model.activeInHierarchy == false)
        //     this.model.SetActive(true); // activates the model

        // the positions also needs to scale to preserve anamorphism
        // the rotation follows the main target's rotation
        if(newMode == ParamConstants.Tracking_Modes.FOUR_MARKER_MODE)
            model.transform.localScale = new Vector3(scale*1.4f, scale*1.4f, scale*1.4f);
        else if(newMode == ParamConstants.Tracking_Modes.TWO_MARKER_MODE)
            model.transform.localScale = new Vector3(scale*1.2f, scale*1.2f, scale*1.2f);
        else
            model.transform.localScale = new Vector3(scale, scale, scale);

        model.transform.position = centerPosition;

        this.targetPosition.Clear();
    }

    // returns the Vector3 center position of the markers in the list
    public static Vector3 getCenterPosition(List<Vector3> v)
    {
        Vector3 temp = new Vector3();
        foreach (Vector3 vec in v)
            temp += vec;
        return temp / v.Count;
    }

    // returns a vector with maximum x and y distances
    public static Vector3 getMaxVector(List<Vector3> v)
    {
        float x = 0;
        float z = 0;
        foreach (Vector3 a in v)
            foreach (Vector3 b in v)
            {
                x = Math.Abs(a.x - b.x) > x ? Math.Abs(a.x - b.x) : x;
                z = Math.Abs(a.z - b.z) > z ? Math.Abs(a.z - b.z) : z;
            }
        return new Vector3(x, 0, z);
    }

    private void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(EventNames.Anamorphosis_Events.ON_MARKER_MODE_CHANGE);
        EventBroadcaster.Instance.RemoveAllObservers();
    }
}