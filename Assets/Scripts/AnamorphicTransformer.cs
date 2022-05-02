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
    string newMode = ParamConstants.Tracking_Modes.ZERO_MARKER_MODE;
    //int tracker;

    // repositioner/scaler
    [SerializeField] public GameObject model;
    private float markerdist;
    private float scale;
    private Vector3 centerPosition;
    List<Vector3> targetPosition = new List<Vector3>();

    private bool firstTrack = true;
    // solution checking
    private bool tracking;


    // Start is called before the first frame update
    void Start()
    {
        EventBroadcaster.Instance.AddObserver(EventNames.Anamorphosis_Events.ON_MARKER_MODE_CHANGE, this.OnMarkerModeChange);
        tracking = false;
        Debug.Log("INIT: " + newMode);
    }

    void Update()
    {
        if(tracking)
        {
            //store values for easier referencing
            float cam_x = camera.transform.position.x;
            float cam_y = camera.transform.position.y;
            float cam_z = camera.transform.position.z;

            float sol_x = solutionPoint.transform.position.x;
            float sol_y = solutionPoint.transform.position.y;
            float sol_z = solutionPoint.transform.position.z;

            // Debug.Log("Positions");
            // Debug.Log(camera.transform.position);
            // Debug.Log(solutionPoint.transform.position);

            bool goodX = cam_x > sol_x-0.02 && cam_x < sol_x+0.02;
            bool goodY = cam_y > sol_y-0.02 && cam_y < sol_y+0.02;
            bool goodZ = cam_z > sol_z-0.02 && cam_z < sol_z+0.02;

            if(goodX && goodY && goodZ)
            {
                EventBroadcaster.Instance.PostEvent(EventNames.Anamorphosis_Events.ON_WIN);
                tracking = false;
            }
                
        }
    }

    private void OnMarkerModeChange(Parameters parameters)
    {
        // delete all marker children
        foreach (GameObject marker in markerArray)
        {
            foreach (Transform child in marker.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // get new marker mode
        newMode = parameters.GetStringExtra(ParamConstants.Extra_Keys.MARKER_MODE, ParamConstants.Tracking_Modes.ZERO_MARKER_MODE);
        Debug.Log("NEW: " + newMode);

        // reposition model holder
        Reposition();

        // update correct values, anamorphosize and redistribute slices
        if(newMode != ParamConstants.Tracking_Modes.ZERO_MARKER_MODE)
        {
            //correctDistance = Vector3.Distance(gameHolder.transform.position, solutionPoint.transform.position);

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
            //Debug.Log("Original" + origslice.transform.position);
            GameObject slice = GameObject.Instantiate(origslice, origslice.transform.position, origslice.transform.rotation);
            
            //Debug.Log("Copy" + slice.transform.position);
            slice.SetActive(true);

            Debug.Log("OLD SLICE: " + slice.transform.position.ToString("F3"));
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
            Debug.Log("NEW SLICE: " + slice.transform.position.ToString("F3"));

            /**
            * Change parent based on scale
            */

            // for one marker, parent everything to that marker
            if(newMode == ParamConstants.Tracking_Modes.ONE_MARKER_MODE)
            {
                slice.transform.parent = markerArray[0].transform;
            }
            // for two markers, parent larger slices to farther marker, smmaller to closer marker
            else if(newMode == ParamConstants.Tracking_Modes.TWO_MARKER_MODE)
            {
                if(s > 1.0f) //check if father/closer to the solution point
                    slice.transform.parent = markerArray[0].transform;
                else
                    slice.transform.parent = markerArray[1].transform;
            }
            // for four markers, determine if smaller or larger
            // then parent to left marker if closer to left, accordingly for right marker
            else if(newMode == ParamConstants.Tracking_Modes.FOUR_MARKER_MODE)
            {
                if(s > 1.0f) //check if father/closer to the solution point
                {
                    float xmid = (markerArray[0].transform.position.x + markerArray[1].transform.position.x)/2;
                    if(slice.transform.position.x < xmid)
                        slice.transform.parent = markerArray[0].transform;
                    else
                        slice.transform.parent = markerArray[2].transform;
                }
                else
                {
                    float xmid = (markerArray[2].transform.position.x + markerArray[3].transform.position.x)/2;
                    if(slice.transform.position.x < xmid)
                        slice.transform.parent = markerArray[1].transform;
                    else
                        slice.transform.parent = markerArray[3].transform;
                }
            }
            else
            {
                Destroy(slice);
            }
        }

        Debug.Log("ANA DONE");
    }

    private void Reposition()
    {
        model.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        model.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        if(newMode != ParamConstants.Tracking_Modes.ZERO_MARKER_MODE)
        {
            this.targetPosition.Add(markerArray[0].transform.position);

            if(newMode == ParamConstants.Tracking_Modes.TWO_MARKER_MODE)
                this.targetPosition.Add(markerArray[1].transform.position);
            
            if(newMode == ParamConstants.Tracking_Modes.FOUR_MARKER_MODE)
            {
                this.targetPosition.Add(markerArray[2].transform.position);
                this.targetPosition.Add(markerArray[3].transform.position);
            }
            
            // get the center of all markers in the list
            centerPosition = getCenterPosition(this.targetPosition);
            Debug.Log("CENTER: " + centerPosition.ToString("F3"));
            Debug.Log("PEBBLES: " + markerArray[0].transform.position.ToString("F3"));
            Debug.Log("MODEL: " + model.transform.position.ToString("F3"));
            Debug.Log("SOL: " + solutionPoint.transform.position.ToString("F3"));

            // contains the maximum x and y distances among the marker/s
            Vector3 max = getMaxVector(this.targetPosition);
            this.markerdist = max.z > max.x ? max.z : max.x;

            // scale starts at 1, and scales linearly depending on the distance of the two markers
            this.scale = 1f + (float)markerdist;

            if(newMode == ParamConstants.Tracking_Modes.TWO_MARKER_MODE)
                this.scale = this.scale*1.5f;
            else if(newMode == ParamConstants.Tracking_Modes.FOUR_MARKER_MODE)
                this.scale = this.scale*2f;

            // if (this.scale > 1.5) // refers to the length of 2 markers
            //     this.model.SetActive(false); // deactivates the model
            // else if (this.model.activeInHierarchy == false)
            //     this.model.SetActive(true); // activates the model

            // the positions also needs to scale to preserve anamorphism
            // the rotation follows the main target's rotation
            
            model.transform.localScale = new Vector3(scale, scale, scale);
            model.transform.position = centerPosition;

            Debug.Log("MODEL NEW: " + model.transform.position.ToString("F3"));
            Debug.Log("SOL NEW: " + solutionPoint.transform.position.ToString("F3"));

            this.targetPosition.Clear();
        }

        Debug.Log("REPO DONE");
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
    }
}