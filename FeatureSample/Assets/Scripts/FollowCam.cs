using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follow Cam kinda like angry birds - follow the projectile
/// 1. Cam will sit at an initial position. Doesnt move durring aiming mode
/// 2. Once projectile has been launched, cam should follow (We need some easing, aka Interpolation)
/// 3. As camera moves through the air, zoom out so we can still see the ground
/// 4. When projectile is at rest, stop following, and return to initial position
/// 
/// Since there will be only 1 camera, make it a singleton
/// </summary>
public class FollowCam : MonoBehaviour
{
    //Need a reference to self
    public static FollowCam Instance;

    // Point Of Interest - Its public so anywhere we want we can access
    public GameObject poi;

    //Initial position
    public float camZ;

    public float easing = 0.05f;

    public Vector2 minXY;

    // Start is called before the first frame update
    void Awake()
    {
        /* Because Instance variable is static, 
         * the singleton Instance can be accessed anywhere in our project
         * REMINDER - This is a quicky/dirty version of a Singleton
         *  We are nit checking for other versions.
         */

        //Initialize Singleton
        Instance = this;
        camZ = this.transform.position.z;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Don't run update when game is not in aiming mode
        if (poi == null) return;

        //Get the position of the thing to follow (poi)
        Vector3 destination = poi.transform.position;

        //Limit XY Values that the camera can move on
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        destination = Vector3.Lerp(transform.position, destination, easing);

        //Retain the destination.z of camZ
        destination.z = camZ;

        //Move the camera to that new destination
        this.transform.position = destination;

        //Fake Zoom and change size of camera
        this.GetComponent<Camera>().orthographicSize = destination.y + 10;
    }
}
