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

    private Matrix4x4 transformMatrix;
    private Matrix4x4 cameraTransformMatrix;

    public void updateTransformMatrix() {
        transformMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
    }

    public void updateCameraTransformMatrix() {
        cameraTransformMatrix = Matrix4x4.TRS(Camera.main.transform.position, Camera.main.transform.rotation, Camera.main.transform.localScale);
    }

    public Vector3 applyTransformMatrix(Vector3 p) {
        return transformMatrix.MultiplyPoint3x4(p);
    }

    public Vector3 applyCameraTransformMatrix(Vector3 p) {
        return cameraTransformMatrix.MultiplyPoint3x4(p);
    }

    void Awake() {
        updateTransformMatrix();
        updateCameraTransformMatrix();

        centerRen = center.GetComponent<Renderer>();
        centerRen.enabled = false;
    }

    void Update() {
        dist = (steamCtlMain.transform.position - steamCtlAlt.transform.position).magnitude;
        delta = dist - prevDist;

        Vector3 deltaPosMain = steamCtlMain.transform.position - prevPosMain;
        Vector3 deltaPosAlt = steamCtlAlt.transform.position - prevPosAlt;
        Vector3 deltaPosAvg = (deltaPosMain + deltaPosAlt) / 2f;
        deltaPosAvg = new Vector3(deltaPosAvg.x, -deltaPosAvg.y, deltaPosAvg.z);

        centerPos = (steamCtlMain.transform.position + steamCtlAlt.transform.position) / 2f;
        center.transform.position = centerPos;

        angle = getAngle(steamCtlMain.transform, centerPos);// (getAngle(steamCtlMain.transform, centerPos) + getAngle(steamCtlAlt.transform, centerPos)) / 2f;
        angleDelta = angle - prevAngle;

        if (steamCtlMain.gripped && steamCtlAlt.gripped) {
            centerRen.enabled = true;

            if (Mathf.Abs(delta) > Mathf.Abs(deltaThreshold * target.transform.localScale.x)) {
                target.transform.localScale -= Vector3.one * Time.deltaTime * Mathf.Sign(delta) * scaleDelta;
            }
            if (target.transform.localScale.x < minScale) target.transform.localScale = new Vector3(minScale, minScale, minScale);

            target.transform.Rotate(0f, -angleDelta, 0f);

            target.transform.Translate(deltaPosAvg * 4f);

        } else {
            centerRen.enabled = false;
        }

        prevPosMain = steamCtlMain.transform.position;
        prevPosAlt = steamCtlAlt.transform.position;
        prevDist = dist;
        prevAngle = angle;
    }

    float getAngle(Transform t1, Vector3 v1) {
        Vector3 relative = t1.InverseTransformPoint(v1);
        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        return angle;
    }

}
