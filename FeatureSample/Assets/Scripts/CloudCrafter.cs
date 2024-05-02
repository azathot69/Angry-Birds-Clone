using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// CloudCrafter will generate as many cloud prefabs as we want.
/// It will then scale and move the clouds to simulate vection
/// </summary>
public class CloudCrafter : MonoBehaviour
{
    #region Variables
    //# of clouds to make
    public int numClouds = 45;

    //Array to hold the cloud prefabs
    public GameObject[] cloudPrefabs;

    //Min Height Position we want to use for the clouds.
    public Vector3 cloudsPosMin;

    //Max Height Position we want for our clouds.
    public Vector3 cloudsPosMax;

    //Min Scales for clouds far away
    public float cloudsScaleMin = 1f;

    //Max scale for close up clouds
    public float cloudsScaleMax = 5f;

    //Speed adjuster
    public float cloudsSpeedMult = 0.5f;

    //References for each cloud we make
    public GameObject[] cloudInstance;
    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        //Make the array large enough to hold all cloud instances
        cloudInstance = new GameObject[numClouds];

        //Find the cloudAnchor parent obj
        GameObject anchor = GameObject.Find("CloudAnchor");

        GameObject cloud;

        //Iterate through and  make cloud#1, 2, 3, ..., n
        for (int index = 0; index < numClouds; index++)
        {
            //We need a random cloud to generate from our cloudPrefab array
            //Pick an int between 0 and cloudPrefab.length
            //NOTE: Random.Range will NEVER return the highest number if you use the INT version
            int prefabNum = Random.Range(0, cloudPrefabs.Length);

            //Make a cloud instance
            cloud = Instantiate(cloudPrefabs[prefabNum]) as GameObject;

            //Position Cloud
            Vector3 cPos = Vector3.zero;
            cPos.x = Random.Range(cloudsPosMin.x, cloudsPosMax.x);
            cPos.y = Random.Range(cloudsPosMin.y, cloudsPosMax.y);

            //Scale the cloud
            float scaleU = Random.value;
            float scaleVal = Mathf.Lerp(cloudsScaleMin, cloudsScaleMax, scaleU);

            //Smaller xlouds (with smaller U value) should never be near the ground
            cPos.y = Mathf.Lerp(cloudsPosMin.y, cPos.y, scaleU);

            //Lets also make sure that smaller clouds are also further away from the camera
            cPos.z = 100 - 90 * scaleU;

            //Apply to cloud
            cloud.transform.position = cPos;

            //Scale the clouds
            cloud.transform.localScale = Vector3.one * scaleVal;

            //Make cloud a child of anchor
            cloud.transform.parent = anchor.transform;

            //Store this cloud with our array of all clouds
            cloudInstance[index] = cloud;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject cloud in cloudInstance)
        {
            //Get cloud scale and position
            float scaleVal = cloud.transform.localScale.x;
            Vector3 cPos = cloud.transform.position;

            //move Clouds (To the Left)
            cPos.x -= scaleVal * Time.deltaTime * cloudsSpeedMult;

            //If cloud moves too far to the left; reset it to he far right
            if (cPos.x <= cloudsPosMin.x)
            {
                cPos.x = cloudsPosMax.x;
            }

            //Apply this new position to each cloud, each frame
            cloud.transform.position = cPos;
        }
    }
}
