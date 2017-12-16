using UnityEngine;

public class VRTK_DragWorld : MonoBehaviour {

    public SteamVR_NewController ctlMain;
    public SteamVR_NewController ctlAlt;

    [Header("* Movement Settings")]
    public bool movementActivated = true;
    public float movementMultiplier = 3f;

    [Header("* Rotation Settings")]
    public bool rotationActivated = true;
    public float rotationMultiplier = 0.75f;
    public float rotationActivationThreshold = 0.1f;

    [Header("* Scale Settings")]
    public bool scaleActivated = true;
    public float scaleMultiplier = 3f;
    public float scaleActivationThreshold = 0.002f;
    public float minimumScale = 0.1f;
    public float maximumScale = Mathf.Infinity;

    [Header("* Transform Settings")]
    public Transform controllingTransform;
    public bool useOffsetTransform = true;
    public Transform offsetTransform;

    private Vector3 previousAltCtlPosition = Vector3.zero;
    private Vector3 previousMainCtlPosition = Vector3.zero;
    private Vector2 previousRotationAngle = Vector2.zero;
    private float previousControllerDistance = 0f;

    private void FixedUpdate() {
        if (ctlMain.gripped && ctlAlt.gripped) {
            Scale();
            Rotate();
            Move();
        }
    }

    private Vector3 GetAltCtlPosition() {
        return ctlAlt.transform.localPosition;
    }

    private Vector3 GetMainCtlPosition() {
        return ctlMain.transform.localPosition;
    }

    private Vector3 GetControllerAngularVelocity(SteamVR_NewController ctl) {
        Debug.Log(ctl.device.angularVelocity);
        return ctl.device.angularVelocity;
    }

    private void SetControllerPositions() {
        previousAltCtlPosition = GetAltCtlPosition();
        previousMainCtlPosition = GetMainCtlPosition();
    }

    private Vector2 GetControllerRotation() {
        return new Vector2((GetAltCtlPosition() - GetMainCtlPosition()).x, (GetAltCtlPosition() - GetMainCtlPosition()).z);
    }

    private float GetControllerDistance() {
         return Vector3.Distance(GetAltCtlPosition(), GetMainCtlPosition());
    }

    private void Move() {
        if (!movementActivated) return;

        Vector3 altMovementOffset = GetAltCtlPosition() - previousAltCtlPosition;
        Vector3 mainMovementOffset = GetMainCtlPosition() - previousMainCtlPosition;
        Vector3 movementOffset = controllingTransform.localRotation * (altMovementOffset + mainMovementOffset);
        Vector3 newPosition = controllingTransform.localPosition - Vector3.Scale((movementOffset * movementMultiplier), controllingTransform.localScale);
        controllingTransform.localPosition = new Vector3(newPosition.x, newPosition.y, newPosition.z);
        SetControllerPositions();
    }

    private void Rotate() {
        if (!rotationActivated) return;

        Vector2 currentRotationAngle = GetControllerRotation();
        float newAngle = Vector2.Angle(currentRotationAngle, previousRotationAngle) * Mathf.Sign(currentRotationAngle.x * currentRotationAngle.y - previousRotationAngle.x * previousRotationAngle.y);
        RotateByAngle(newAngle);
        previousRotationAngle = currentRotationAngle;
    }

    private void RotateByAngle(float angle) {
        if (Mathf.Abs(angle) >= rotationActivationThreshold) {
            if (useOffsetTransform) {
                controllingTransform.RotateAround(offsetTransform.position, Vector3.up, angle * rotationMultiplier);
            } else {
                controllingTransform.Rotate(Vector3.up * (angle * rotationMultiplier));
            }
        }
    }

    private void Scale() {
        if (!scaleActivated) return;

        float currentDistance = GetControllerDistance();
        float distanceDelta = currentDistance - previousControllerDistance;
        if (Mathf.Abs(distanceDelta) >= scaleActivationThreshold) {
            controllingTransform.localScale += (Vector3.one * Time.deltaTime * Mathf.Sign(distanceDelta) * scaleMultiplier);
            controllingTransform.localScale = new Vector3(Mathf.Clamp(controllingTransform.localScale.x, minimumScale, maximumScale), Mathf.Clamp(controllingTransform.localScale.y, minimumScale, maximumScale), Mathf.Clamp(controllingTransform.localScale.z, minimumScale, maximumScale));
        }
        previousControllerDistance = currentDistance;
    }

}

