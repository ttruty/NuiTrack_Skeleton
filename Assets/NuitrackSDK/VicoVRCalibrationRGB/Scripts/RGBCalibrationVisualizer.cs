﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBCalibrationVisualizer : MonoBehaviour {
	
	[SerializeField] MeshRenderer sprt;
	[SerializeField] MeshRenderer sprtColor;

	[SerializeField] GameObject BackGround;
	[SerializeField] GameObject ConnectionLostItems;
	[SerializeField] GameObject VisualiserItems;

	[SerializeField] Transform headAnchor;

	[SerializeField] TextMesh progressText;

    [Header("Controller Calibration")]
    [Tooltip("VicoVR controller")]
    [SerializeField] bool needController;
    [SerializeField] GameObject controllerCalibrationUI;
    [SerializeField] UnityEngine.UI.Image controllerProgressbar;

	[SerializeField] bool autoFindGvrHead = false;

	Transform stackPoint;

	bool streamingEnabled = false;

    bool calibratedOnce = false;
    
	bool firstCalibrationEvent = true;

    nuitrack.PublicNativeImporter.ControllerCalibrationCallback controllerCalibrationCallback;

    void OnEnable()
	{

#if UNITY_ANDROID
        if (NuitrackLoader.initState != NuitrackInitState.INIT_OK)
        {
			gameObject.SetActive (false);
			return;
		}
		#endif
		if (autoFindGvrHead) {
			progressText.GetComponent<MeshRenderer> ().enabled = false;

			stackPoint = transform.GetChild (0);
		}
		BackTextureCreator.newTextureEvent += UpdateTexture;
        //TPoseCalibration.onStart += StartStream;
        //TPoseCalibration tpc = FindObjectOfType<TPoseCalibration>();
        TPoseCalibration.Instance.onStart += StartStream;
        TPoseCalibration.Instance.onSuccess += CloseStream;
        TPoseCalibration.Instance.onProgress += ChangeProgress;
        TPoseCalibration.Instance.onFail += OnCalibrationFail;

        SensorDisconnectChecker.SensorConnectionTimeOut += ShowConnectionProblem;
		SensorDisconnectChecker.SensorReconnected += HideConnectionProblem;

		StartCoroutine (StartStreamingC ());
        PointerPassing.OnCalibration += ControllerCalibration;
    }

    private void OnCalibrationFail()
    {
        if(!calibratedOnce)
        {
            ChangeProgress(0);
        }
    }

    void ControllerCalibration(int handID, float progress)
    {
        //Debug.Log("CONTROLLER CALIBRATION: " + handID + " " + progress);

        if (handID == -1) //Controller not found
        {
            controllerCalibrationUI.SetActive(false);
            needController = false;
        }

        if (handID < 0 || progress < 0.001f)
        {
            return;
        }

        controllerProgressbar.fillAmount = progress / 100;
        controllerCalibrationUI.SetActive(true);

        if (progress > 99.999f)
        {
            controllerCalibrationUI.SetActive(false);
        }
    }

    void ShowConnectionProblem()
	{
		BackGround.SetActive (true);
		ConnectionLostItems.SetActive (true);
		VisualiserItems.SetActive (false);
	}

	void HideConnectionProblem()
	{
		ConnectionLostItems.SetActive (false);
		if (streamingEnabled) {
			BackGround.SetActive (true);
			VisualiserItems.SetActive (true);
		} else {
			BackGround.SetActive (false);
			VisualiserItems.SetActive (false);
		}
	}

	void ChangeProgress(float progress)
	{
        progressText.text = "CALIBRATION  " + (100 * progress).ToString("0") + "%";

        if (100*progress > 1) {
			calibrationTimeOut = 0;
			StartStream ();
		}
    }

	void UpdateTexture(Texture txtr,Texture txtrColor)
	{
        //Debug.Log ("textureUpdated");
		sprt.material.mainTexture = txtr;
		sprtColor.material.mainTexture = txtrColor;

        //BackTextureCreator.newTextureEvent -= UpdateTexture;

	}

	IEnumerator StartStreamingC()
	{
		yield return new WaitForSeconds (0.1f);
		StartStream();
		firstCalibrationEvent = true;
	}

	public void StartStream () {
        //progressText.text = "CALIBRATION\n" + 0 + "%";
		firstCalibrationEvent = false;

		//adbDebug.Log ("startStream");
        //sprt.material.mainTexture = FindObjectOfType<BackTextureCreator>().GetRGBTexture;
		NuitrackManager.Instance.DepthModuleStart ();
		sprt.enabled = true;
		sprtColor.enabled = true;
		streamingEnabled = true;
		if (autoFindGvrHead) {
			stackPoint.SetParent (headAnchor);
			stackPoint.transform.localRotation = Quaternion.identity;
			stackPoint.transform.localPosition = Vector3.zero;
		}
		BackGround.SetActive (true);
        ConnectionLostItems.SetActive(false);
        VisualiserItems.SetActive (true);
        //ConnectionLostItems.SetActive (true);
	}

	float calibrationTimeOut = 0;
	void Update()
	{
		if (!firstCalibrationEvent) {
			calibrationTimeOut += Time.deltaTime;
			if (calibrationTimeOut > 0.5f && calibratedOnce) {
				CloseStream (Quaternion.identity);
				calibrationTimeOut = 0;
				firstCalibrationEvent = true;
			}
		}

	}

	public void CloseStream (Quaternion a) {
		Debug.Log ("closeStream: start");
		NuitrackManager.Instance.DepthModuleClose ();
		sprt.enabled = false;
		sprtColor.enabled = false;
		streamingEnabled = false;

		BackGround.SetActive (false);
        ConnectionLostItems.SetActive(false);
        VisualiserItems.SetActive (false);
		ConnectionLostItems.SetActive (false);
        calibratedOnce = true;

        if (!calibratedOnce && needController)
        {
            controllerCalibrationUI.SetActive(true);
        }
		Debug.Log ("closeStream: end");
    }
	void OnDisable()
	{
		CloseStream (Quaternion.identity);
		BackTextureCreator.newTextureEvent -= UpdateTexture;
		//TPoseCalibration tpc = FindObjectOfType<TPoseCalibration>();

        //TPoseCalibration.onStart -= StartStream;
        TPoseCalibration.Instance.onStart -= StartStream;
        TPoseCalibration.Instance.onSuccess -= CloseStream;
        TPoseCalibration.Instance.onProgress -= ChangeProgress;
        TPoseCalibration.Instance.onFail -= OnCalibrationFail;

        SensorDisconnectChecker.SensorConnectionTimeOut -= ShowConnectionProblem;
		SensorDisconnectChecker.SensorReconnected -= HideConnectionProblem;
	}
}
