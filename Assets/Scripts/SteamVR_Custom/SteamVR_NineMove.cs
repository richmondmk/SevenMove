using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVR_NineMove : MonoBehaviour {
    // https://www.youtube.com/watch?v=ycCBzwjOD70

    public SteamVR_NewController steamCtlMain;
	public SteamVR_NewController steamCtlAlt;
    public GameObject target;
    public GameObject center;

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

    void Awake() {
        centerRen = center.GetComponent<Renderer>();
        centerRen.enabled = false;
    }

    void Update() {
        if (steamCtlMain.gripped && steamCtlAlt.gripped) {
            centerRen.enabled = true;

            dist = (steamCtlMain.transform.position - steamCtlAlt.transform.position).magnitude;
            delta = dist - prevDist;
            if (Mathf.Abs(delta) > deltaThreshold) {
                target.transform.localScale -= Vector3.one * Time.deltaTime * Mathf.Sign(delta) * scaleDelta;

                Vector3 deltaPosMain = steamCtlMain.transform.position - prevPosMain;
                Vector3 deltaPosAlt = steamCtlAlt.transform.position - prevPosAlt;
                Vector3 deltaPosAvg = (deltaPosMain + deltaPosAlt) / 2f;
                deltaPosAvg = new Vector3(deltaPosAvg.x, -deltaPosAvg.y, deltaPosAvg.z);
                target.transform.Translate(deltaPosAvg * 4f);

                centerPos = (steamCtlMain.transform.position + steamCtlAlt.transform.position) / 2f;
                center.transform.position = centerPos;

                angle = (getAngle(steamCtlMain.transform, centerPos) + getAngle(steamCtlAlt.transform, centerPos)) / 2f;
                angleDelta = angle - prevAngle;
                target.transform.Rotate(0f, -angleDelta, 0f);
            }
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
