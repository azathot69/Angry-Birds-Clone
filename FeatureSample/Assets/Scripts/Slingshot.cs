using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listen to mouse input, instrantiate a projectile 
/// </summary>
public class Slingshot : MonoBehaviour
{
    public static Slingshot Instance;

    //Variables for reference to the LaunchPoint which has our "Glow"
    public GameObject launchPoint;

    //Variable for our prefab
    public GameObject projectilePrefab;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    public float velocityMulti = 4f;

    private void Awake()
    {
        //Launch Point Initiate
        Transform launchPointTransform = transform.Find("LaunchPoint");
        launchPoint = launchPointTransform.gameObject;
        launchPoint.SetActive(false);

        //Projectile Initiate
        launchPos = launchPointTransform.position;

        Instance = this;
        
    }

    private void Update()
    {
        //If slingshot != true, dont run code
        if (!aimingMode) return;

        //Get current mouse position in 2D screen space
        Vector3 mousePos2D = Input.mousePosition;

        //Convert mouse pos to 3D game space
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //Find Dealta from launch position to whereever the mouse is
        //Delta = difference
        Vector3 mouseDelta = mousePos3D - launchPos;

        //Limit the mouseDelta value to some radius
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;

        //Calculate new position for projectiles
        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        //Move projectile to new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        //Fire Position
        if (Input.GetMouseButtonUp(0))
        {
            //LMB was released
            //No more aiming
            aimingMode = false;

            //Make projectile listen to physics system
            projectile.GetComponent<Rigidbody>().isKinematic = false;

            //Shott Projectile
            projectile.GetComponent<Rigidbody>().velocity = -mouseDelta * velocityMulti;

            //Tell camera to follow the projectile
            FollowCam.Instance.poi = projectile;

            projectile = null;
        }
    }

    private void OnMouseEnter()
    {
        //print("Slingshot.OnMouseEnter");
        launchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        //print("Slingshot.OnMouseExit");
        launchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        //Player has pressed the mouse button while over the slingshot
        aimingMode = true;

        //Instantiate a projectile
        projectile = Instantiate(projectilePrefab);

        //Move it to the launch point
        projectile.transform.position = launchPos;

        //Tell that projectile to be Kinematic
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

}
