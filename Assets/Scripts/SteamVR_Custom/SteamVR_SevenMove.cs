using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVR_SevenMove : MonoBehaviour {
    // https://www.youtube.com/watch?v=ycCBzwjOD70

    public SteamVR_NewController steamCtlMain;
    public SteamVR_NewController steamCtlAlt;
    public GameObject target;
    public GameObject center;
    public float minScale = 0.1f;
    public float moveSpeed = 4f;
    public enum TrackMode { AVG, MAIN, ALT};
    public TrackMode trackMode = TrackMode.AVG;

    private Renderer centerRen;
    private Vector3 prevPosMain = Vector3.zero;
    private Vector3 prevPosAlt = Vector3.zero;
    private Vector3 centerPos = Vector3.zero;
    private float dist = 0f;
    private float delta = 0f;
    private float prevDist = 0f;

    private float angle = 0f;
    private float prevAngle = 0f;
    private float angleDelta = 0f;

    private float deltaThreshold = 0.01f;
    private float scaleDelta = 5f;

    private void Awake() {
        centerRen = center.GetComponent<Renderer>();
        if (centerRen != null) centerRen.enabled = false;
    }

    private void Update() {
        dist = (steamCtlMain.transform.position - steamCtlAlt.transform.position).magnitude;
        delta = dist - prevDist;

        Vector3 deltaPosMain = steamCtlMain.transform.position - prevPosMain;
        Vector3 deltaPosAlt = steamCtlAlt.transform.position - prevPosAlt;
        Vector3 deltaPosAvg = (deltaPosMain + deltaPosAlt) / 2f;
        deltaPosAvg = new Vector3(deltaPosAvg.x, -deltaPosAvg.y, deltaPosAvg.z);

        centerPos = (steamCtlMain.transform.position + steamCtlAlt.transform.position) / 2f;
        center.transform.position = centerPos;

        if (trackMode == TrackMode.AVG) {
            angle = (getAngle(steamCtlMain.transform, centerPos) + getAngle(steamCtlAlt.transform, centerPos)) / 2f;
        } else if (trackMode == TrackMode.MAIN) {
            angle = getAngle(steamCtlMain.transform, centerPos);
        } else if (trackMode == TrackMode.ALT) {
            angle = getAngle(steamCtlAlt.transform, centerPos);
        }
        angleDelta = angle - prevAngle;

        if (steamCtlMain.gripped && steamCtlAlt.gripped) {
            if (centerRen != null) centerRen.enabled = true;

            if (Mathf.Abs(delta) > Mathf.Abs(deltaThreshold * target.transform.localScale.x)) {
                target.transform.localScale -= Vector3.one * Time.deltaTime * Mathf.Sign(delta) * scaleDelta;
            }
            if (target.transform.localScale.x < minScale) target.transform.localScale = new Vector3(minScale, minScale, minScale);

            target.transform.Rotate(0f, -angleDelta, 0f);

            Vector3 move = new Vector3(-deltaPosAvg.x, deltaPosAvg.y, -deltaPosAvg.z) * moveSpeed;
            Vector3 dir = target.transform.InverseTransformDirection(move);
            Vector3 finalMove = new Vector3(move.x * dir.x, move.y, move.z * dir.z);
            target.transform.Translate(dir);
        } else {
            if (centerRen != null) centerRen.enabled = false;
        }

        prevPosMain = steamCtlMain.transform.position;
        prevPosAlt = steamCtlAlt.transform.position;
        prevDist = dist;
        prevAngle = angle;
    }

    public float getAngle(Transform t1, Vector3 v1) {
        Vector3 relative = t1.InverseTransformPoint(v1);
        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        return angle;
    }

}
