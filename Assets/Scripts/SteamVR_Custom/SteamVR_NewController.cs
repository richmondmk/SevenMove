using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class SteamVR_NewController : MonoBehaviour {

	public bool triggerPressed = false;
	public bool padPressed = false;
	public bool gripped = false;
	public bool menuPressed = false;
	public bool triggerDown = false;
	public bool padDown = false;
	public bool gripDown = false;
	public bool menuDown = false;
	public bool triggerUp = false;
	public bool padUp = false;
	public bool gripUp = false;
	public bool menuUp = false;

	private SteamVR_TrackedObject trackedObj;
	//private bool blockTriggerButton = false;
	//private bool blockGripButton = false;
	//private bool blockPadButton = false;

	void Awake() {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	//void FixedUpdate() {
	void Update() {
		var device = SteamVR_Controller.Input((int) trackedObj.index);

		// ~ ~ ~ ~ ~

		triggerDown = false;
		padDown = false;
		gripDown = false;
		menuDown = false;
		triggerUp = false;
		padUp = false;
		gripUp = false;
		menuUp = false;

		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
			triggerPressed = true;
			triggerDown = true;
		} else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
			triggerPressed = false;
			triggerUp = true;
		}

		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
			padPressed = true;
			padDown = true;
		} else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) {
			padPressed = false;
			padUp = true;
		}

		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip)) {
			gripped = true;
			gripDown = true;
		} else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip)) {
			gripped = false;
			gripUp = true;
		}

		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			menuPressed = true;
			menuDown = true;
		} else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			menuPressed = false;
			menuUp = true;
		}

		// ~ ~ ~ ~ ~

		/*
		triggerOneShot = false;
		padOneShot = false;
		gripOneShot = false;

		if ((triggerPressed && !blockTriggerButton)) {
			triggerOneShot = true;
			blockTriggerButton = true;
		} else if ((!triggerPressed && blockTriggerButton)) {
			blockTriggerButton = false;
		}

		if ((padPressed && !blockPadButton)) {
			padOneShot = true;
			blockPadButton = true;
		} else if ((!padPressed && blockPadButton)) {
			blockPadButton = false;
		}

		if ((gripped && !blockGripButton)) {
			gripOneShot = true;
			blockGripButton = true;
		} else if ((!gripped && blockGripButton)) {
			blockGripButton = false;
		}
		*/
	}

}
