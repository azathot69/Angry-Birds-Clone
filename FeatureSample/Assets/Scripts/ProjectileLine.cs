using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Draw a line behind the projectile to give the player a way to track their shot,
/// improving their next shot
/// </summary>
public class ProjectileLine : MonoBehaviour
{
    //Only one of these so lets use a singleton
    public static ProjectileLine Instance;

    //Reference to the LineRanderer
    public LineRenderer line;

    //Minimum Distance to draw
    public float minDist = 0.1f;

    //Need to know what were drawing a line behind
    private GameObject _poi;

    //Need a list of all the points we are drawing lines between
    public List<Vector3> points;

    //A property is a function thats pretending to be a variable
    public GameObject poi
    {
        get { return _poi; }
        set
        {
            _poi = value;

            //Reset Everything when POI is set to some new value
            if (_poi != null)
            {
                line.enabled = false;

                //Re-Initialize list to make it empty
                points = new List<Vector3>();

                AddPoint();
            }
        }
    }

    //Need a function to clear the line
    public void Clear()
    {
        _poi = null;
        line.enabled = false;
        points = new List<Vector3>();
    }

    //We need to add points to the line
    public void AddPoint()
    {
        Vector3 pt = _poi.transform.position;

        //If the pooint isnt far enough from the last point, return
        if (points.Count > 0 && (pt-lastPoint).magnitude < minDist) return;

            //If this is the launch Point 
            if (points.Count == 0)
            {
                Vector3 launchPos = Slingshot.Instance.launchPoint.transform.position;
            Vector3 launchPosDiff = pt - launchPos;

            //We want to add an extra bit of line to aid player aiming
            points.Add(pt + launchPosDiff);
            points.Add(pt);

            //Its ok to use depricated code - Unity may take it away one day though...\
            line.SetVertexCount(2);

            //Now we tell the line renderer what two points to use
            line.SetPosition(0, points[0]);
            line.SetPosition(1, points[1]);

            //Draw
            line.enabled = true;
        }
        else
        {
            //Normal "Add a Point" Behavior
            points.Add(pt);
            line.SetVertexCount(points.Count);
            line.SetPosition(points.Count - 1, lastPoint);
            line.enabled = true;
        }
        
    }

    //Property to return the most recently added point
    public Vector3 lastPoint
    {
        get {
                //If points == null, return (0, 0, 0) vector
                if (points == null)
                {
                    return Vector3.zero;
                }
            return points[points.Count - 1];
            }


    }

    private void FixedUpdate()
    {
        //If no poi we need to find something
        if (poi == null)
        {
            //Check the follow for its POI
            if (FollowCam.Instance.poi != null)
            {
                //Make sure it's POI is a projectile
                if (FollowCam.Instance.poi.tag == "projectile")
                {
                    //Found projectile to follow and draw lines
                    poi = FollowCam.Instance.poi;
                }
                else
                {
                    //Found jack-shit; don't draw anything else
                    return;
                }
            }
            else
            {
                //Found jack-shit; don't draw anything else
                return;
            }
        }
        //Now we have something to follow and draw a line behind
        AddPoint();

        //Stop when projectile stops moving
        //IsSleeping checks to see if object is still affected by physics
        if (poi.GetComponent<Rigidbody>().IsSleeping())
        {
            //poi is sleeping
            poi = null;
        }
    }

    private void Awake()
    {
        //Instatiate Singleton
        Instance = this;

        //Set the reference for the line renderer
        line = this.GetComponent<LineRenderer>();

        //Disable the line Renderer until I need it
        line.enabled = false;

        //Initialize the points list
        points = new List<Vector3>();

        
    }
}
