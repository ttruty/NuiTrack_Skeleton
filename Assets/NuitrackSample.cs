using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nuitrack;
using UnityEngine.UI;
using System;

public class NuitrackSample : MonoBehaviour {

    //game objects
    [SerializeField]
    private RawImage DepthView;
    private Texture2D texture;
    private byte[] colorData;
    private ulong previousTimestamp;
    private readonly ushort MaxDepth = 8000;


    //NuiTrack objects
    private DepthSensor depthSensor;
    private SkeletonTracker skeletonTracker;
    private GameObject CreatedHead;

    public GameObject PrefabJoint;

    // Use this for initialization
    void Start () {
        Nuitrack.Init();

        depthSensor = DepthSensor.Create();
        depthSensor.OnUpdateEvent += DepthSensor_OnUpdateEvent;

        skeletonTracker = SkeletonTracker.Create();
        skeletonTracker.OnSkeletonUpdateEvent += SkeletonTracker_OnSkeletonUpdateEvent;

        Nuitrack.Run();
    }
	
	// Update is called once per frame
	void Update () {
        Nuitrack.Update();
	}

    private void OnApplicationQuit()
    {
        if (depthSensor != null)
        {
            depthSensor.OnUpdateEvent -= DepthSensor_OnUpdateEvent;
        }

        if (skeletonTracker != null)
        {
            skeletonTracker.OnSkeletonUpdateEvent -= SkeletonTracker_OnSkeletonUpdateEvent;
        }

        Nuitrack.Release();
    }

    private void DepthSensor_OnUpdateEvent(DepthFrame frame)
    {
        if (frame != null)
        {
            if (frame.Timestamp != previousTimestamp)
            {
                previousTimestamp = frame.Timestamp;

                if (texture == null)
                {
                    texture = new Texture2D(frame.Cols, frame.Rows, TextureFormat.RGBA32, false);
                    colorData = new byte[frame.Cols * frame.Rows * 4];
                    DepthView.texture = texture;
                }

                int index = 0;

                for (int i = 0; i < frame.Rows; i++)
                {
                    for (int j = 0; j < frame.Cols; j++)
                    {
                        ushort depth = frame[i, j];

                        byte color = (byte)(depth * 255 / MaxDepth);

                        colorData[index + 0] = (byte)(255 * color);
                        colorData[index + 1] = (byte)(255 * color);
                        colorData[index + 2] = 0;
                        colorData[index + 3] = 255;

                        index += 4;
                    }
                }

                texture.LoadRawTextureData(colorData);
                texture.Apply();
            }
        }
    }

    private void SkeletonTracker_OnSkeletonUpdateEvent(SkeletonData skeletonData)
    {
        if (skeletonData != null)
        {
            Debug.Log("Tracked users: " + skeletonData.NumUsers);

            Skeleton body = skeletonData.Skeletons.Closest();

            if (body != null)
            {
                var head3D = body.Joints[(int)JointType.Head].Real;
                var head2D = depthSensor.ConvertRealToProjCoords(head3D);

                var neck3D = body.Joints[(int)JointType.Neck].Real;
                var neck2D = depthSensor.ConvertRealToProjCoords(neck3D);

                var torso3D = body.Joints[(int)JointType.Torso].Real;
                var torso2D = depthSensor.ConvertRealToProjCoords(torso3D);

            }
        }

    }
}

public static class NuitrackExtensions
{
    public static Skeleton Closest(this Skeleton[] skeletons)
    {
        Skeleton body = null;

        float minDistance = 0f;

        foreach (Skeleton current in skeletons)
        {
            if (body == null)
            {
                body = current;
            }
            else
            {
                float distance = body.Joints[(int)JointType.Waist].Real.Z;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    body = current;
                }
            }
        }

        return body;
    }
}
