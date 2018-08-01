using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchSpeedSender : MonoBehaviour {

    [SerializeField] PunchSpeedMeter punchSpeedMeter;

    private void OnCollisionEnter(Collision collision)
    {
        punchSpeedMeter.CalculateMaxPunchSpeed(collision.relativeVelocity.magnitude);
    }
}
