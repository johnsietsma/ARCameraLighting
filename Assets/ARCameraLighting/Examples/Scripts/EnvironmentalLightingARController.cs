
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using GoogleARCore;

/// <summary>
/// Controller for the environment AR lighting app. Mostly copied from HelloAR.
/// </summary>
public class EnvironmentalLightingARController : MonoBehaviour
{
    public Camera m_firstPersonCamera;
    public GameObject m_trackedPlanePrefab;
    public GameObject m_worldRoot;
    public GameObject[] m_environments;

    public UnityEvent planeTapped;


    private GoogleARCore.HelloAR.PlaneAttachment m_planeAttachment;
    private List<TrackedPlane> m_newPlanes = new List<TrackedPlane>();
    private List<TrackedPlane> m_allPlanes = new List<TrackedPlane>();

    private Color[] m_planeColors = new Color[] {
            new Color(1.0f, 1.0f, 1.0f),
            new Color(0.956f, 0.262f, 0.211f),
            new Color(0.913f, 0.117f, 0.388f),
            new Color(0.611f, 0.152f, 0.654f),
            new Color(0.403f, 0.227f, 0.717f),
            new Color(0.247f, 0.317f, 0.709f),
            new Color(0.129f, 0.588f, 0.952f),
            new Color(0.011f, 0.662f, 0.956f),
            new Color(0f, 0.737f, 0.831f),
            new Color(0f, 0.588f, 0.533f),
            new Color(0.298f, 0.686f, 0.313f),
            new Color(0.545f, 0.764f, 0.290f),
            new Color(0.803f, 0.862f, 0.223f),
            new Color(1.0f, 0.921f, 0.231f),
            new Color(1.0f, 0.756f, 0.027f)
        };


    public void Start()
    {
        m_planeAttachment = m_worldRoot.GetComponent<GoogleARCore.HelloAR.PlaneAttachment>();
    }

    public void Update()
    {
        if (Session.ConnectionState != SessionConnectionState.Connected)
            return;

        // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
        if (Frame.TrackingState != FrameTrackingState.Tracking)
        {
            const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
            Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Frame.GetNewPlanes(ref m_newPlanes);

        // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
        for (int i = 0; i < m_newPlanes.Count; i++)
        {
            // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
            // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
            // coordinates.
            GameObject planeObject = Instantiate(m_trackedPlanePrefab, Vector3.zero, Quaternion.identity,
                transform);
            planeObject.GetComponent<GoogleARCore.HelloAR.TrackedPlaneVisualizer>().SetTrackedPlane(m_newPlanes[i]);

            // Apply a random color and grid rotation.
            planeObject.GetComponent<Renderer>().material.SetColor("_GridColor", m_planeColors[Random.Range(0,
                m_planeColors.Length - 1)]);
            planeObject.GetComponent<Renderer>().material.SetFloat("_UvRotation", Random.Range(0.0f, 360.0f));
        }

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;

        if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
        {
            Debug.Log("Hit: " + hit.Point);

            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
            // world evolves.
            var anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);

            m_worldRoot.transform.position = hit.Point;
            m_worldRoot.transform.parent = anchor.transform;

            // Andy should look at the camera but still be flush with the plane.
            m_worldRoot.transform.LookAt(m_firstPersonCamera.transform);
            m_worldRoot.transform.rotation = Quaternion.Euler(0.0f,
                m_worldRoot.transform.rotation.eulerAngles.y, m_worldRoot.transform.rotation.z);

            m_planeAttachment.Attach(hit.Plane);

            planeTapped.Invoke();
        }
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    /// <param name="length">Toast message time length.</param>
    private static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
