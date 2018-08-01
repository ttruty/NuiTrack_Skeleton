using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorDisconnectChecker : MonoBehaviour {
	public delegate void ConnectionStatusChange();
	static public event ConnectionStatusChange SensorConnectionTimeOut;
	static public event ConnectionStatusChange SensorReconnected;

	bool connection = true;

	void OnEnable () {
        NuitrackManager.onSkeletonTrackerUpdate += ClearTimer;
        nuitrack.Nuitrack.onIssueUpdateEvent += NoConnectionIssue;
        BackTextureCreator.newTextureEvent += UpdateTexture;
    }

    void OnDestroy()
	{
		NuitrackManager.onSkeletonTrackerUpdate -= ClearTimer;
	}

	void ClearTimer(nuitrack.SkeletonData sd)
	{
		if (!connection) {
			connection = true;
			if(SensorReconnected != null)
				SensorReconnected ();
		}
	}

	bool connectionProblem = false;
	void NoConnectionIssue(nuitrack.issues.IssuesData issData)
	{
		if (issData.GetIssue<nuitrack.issues.SensorIssue> () != null) {
			if (SensorConnectionTimeOut != null)
				SensorConnectionTimeOut ();
			connectionProblem = true;
		} else {
			if(connectionProblem && SensorReconnected != null)
				SensorReconnected ();
			connectionProblem = false;
		}
	}

    void OnDisable()
    {
        NuitrackManager.onSkeletonTrackerUpdate -= ClearTimer;
        nuitrack.Nuitrack.onIssueUpdateEvent -= NoConnectionIssue;
        BackTextureCreator.newTextureEvent -= UpdateTexture;
    }

    private void UpdateTexture(Texture txtr, Texture userTxtr)
    {
        if (connectionProblem && SensorReconnected != null)
            SensorReconnected();
        connectionProblem = false;
    }
}
