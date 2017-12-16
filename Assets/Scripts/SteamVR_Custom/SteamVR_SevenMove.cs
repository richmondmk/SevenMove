using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVR_SevenMove : MonoBehaviour {
    // https://www.youtube.com/watch?v=ycCBzwjOD70

    public SteamVR_NewController steamCtlMain;
    public SteamVR_NewController steamCtlAlt;
    public Transform target;
    public GameObject centerObj;
    public float minScale = 0.1f;
    public float maxScale = 100f;
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
    private bool useCenterObj = false;

    private void Awake() {
        if (centerObj == null) return;
        useCenterObj = true;
        centerRen = centerObj.GetComponent<Renderer>();
        centerRen.enabled = false;
    }

    private void Update() {
        dist = (steamCtlMain.transform.position - steamCtlAlt.transform.position).magnitude;
        delta = dist - prevDist;

        Vector3 deltaPosMain = steamCtlMain.transform.position - prevPosMain;
        Vector3 deltaPosAlt = steamCtlAlt.transform.position - prevPosAlt;
        Vector3 deltaPosAvg = (deltaPosMain + deltaPosAlt) / 2f;
        deltaPosAvg = new Vector3(deltaPosAvg.x, -deltaPosAvg.y, deltaPosAvg.z);

        centerPos = (steamCtlMain.transform.position + steamCtlAlt.transform.position) / 2f;
        if (useCenterObj) centerObj.transform.position = centerPos;

        if (trackMode == TrackMode.AVG) {
            angle = (getAngle(steamCtlMain.transform, centerPos) + getAngle(steamCtlAlt.transform, centerPos)) / 2f;
        } else if (trackMode == TrackMode.MAIN) {
            angle = getAngle(steamCtlMain.transform, centerPos);
        } else if (trackMode == TrackMode.ALT) {
            angle = getAngle(steamCtlAlt.transform, centerPos);
        }
        angleDelta = angle - prevAngle;

        if (steamCtlMain.gripped && steamCtlAlt.gripped) {
            if (useCenterObj) centerRen.enabled = true;

            doScale();
            doRotate();
            doTranslate(deltaPosAvg);
        } else {
            if (useCenterObj) centerRen.enabled = false;
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

    public void doTranslate(Vector3 deltaPosAvg) {
        Vector3 move = new Vector3(-deltaPosAvg.x, deltaPosAvg.y, -deltaPosAvg.z) * moveSpeed;
        Vector3 dir = target.InverseTransformDirection(move);
        Vector3 finalMove = new Vector3(move.x * dir.x, move.y, move.z * dir.z);
        target.Translate(dir);
    }

    public void doRotate() {
        target.Rotate(0f, -angleDelta, 0f);
    }

    public void doScale() {
        if (Mathf.Abs(delta) > Mathf.Abs(deltaThreshold * target.localScale.x)) {
            target.localScale -= (Vector3.one * Time.deltaTime * Mathf.Sign(delta) * scaleDelta);
            target.localScale = new Vector3(Mathf.Clamp(target.localScale.x, minScale, maxScale), Mathf.Clamp(target.localScale.y, minScale, maxScale), Mathf.Clamp(target.localScale.z, minScale, maxScale));
        }
    }

}
