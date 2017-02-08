using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVR_NineMove : MonoBehaviour {

	public SteamVR_NewController steamCtlMain;
	public SteamVR_NewController steamCtlAlt;
	public Transform world_root;

	private Transform lastMain;
	private Transform lastAlt;

	private Vector3 deltaPos = Vector3.zero;
	private Vector3 deltaRot = Vector3.zero;
	private Vector3 deltaScale = Vector3.zero;

	void Update() {
		if (steamCtlMain.triggerDown) {
			lastMain = steamCtlMain.transform;
			lastAlt = steamCtlAlt.transform;
		} else if (steamCtlMain.triggerPressed) {
			deltaPos = ((steamCtlMain.transform.position - lastMain.position) + (steamCtlAlt.transform.position - lastAlt.position)) / 2f;
			//deltaRot;
			float getDeltaScale =  Vector3.Distance(steamCtlMain.transform.position, steamCtlAlt.transform.position) - Vector3.Distance(lastMain.position, lastAlt.position);
			deltaScale = new Vector3(getDeltaScale, getDeltaScale, getDeltaScale);

			world_root.Translate(deltaPos);
			world_root.Rotate(deltaRot);
			world_root.localScale += deltaScale;

			lastMain = steamCtlMain.transform;
			lastAlt = steamCtlAlt.transform;
		}
	}

	float getAngle(Transform target) {
		Vector3 relative  = transform.InverseTransformPoint(target.position);
		float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
		return angle;
	}

}
