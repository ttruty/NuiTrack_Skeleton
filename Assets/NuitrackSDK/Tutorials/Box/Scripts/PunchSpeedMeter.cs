using UnityEngine.UI;
using UnityEngine;

public class PunchSpeedMeter : MonoBehaviour {

    [SerializeField] Text speedMeterText;
    [SerializeField] GameObject dummy;
    [SerializeField] Transform transformTarget;

    float maximumPunchSpeed = 0;

    private void Awake()
    {
        dummy.SetActive(false);
    }

    private void OnEnable()
    {
        TPoseCalibration.Instance.onSuccess += OnSuccessCalibration;
    }

    void OnSuccessCalibration(Quaternion rotation)
    {
        dummy.SetActive(true);
        transform.position = transformTarget.position + new Vector3(0, -1, 1);
    }

    public void CalculateMaxPunchSpeed(float speed)
    {
        if (maximumPunchSpeed < speed)
            maximumPunchSpeed = speed;
        speedMeterText.text = maximumPunchSpeed.ToString("f2") + " m/s";
    }

    private void OnDisable()
    {
        TPoseCalibration.Instance.onSuccess -= OnSuccessCalibration;
    }
}
