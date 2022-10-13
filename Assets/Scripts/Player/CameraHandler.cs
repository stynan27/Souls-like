using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MOOSE
{
  public class CameraHandler : MonoBehaviour
  {
    public Transform targetTransform; // target transform the camer wil GO TO
    public Transform cameraTransform; // transform of the camera
    public Transform cameraPivotTransform; // how the camera will change on a swivel
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    private LayerMask ignoreLayers;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public static CameraHandler singleton;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -35;
    public float maximumPivot = 35;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    private void Awake()
    {
      singleton = this;
      myTransform = transform; // local transform equals the transform of the game object invoked
      defaultPosition = cameraTransform.localPosition.z;
      ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); // helps prevent camera collision with the environment
    }

    public void FollowTarget(float delta)
    {
      Vector3 targetPosition = Vector3.SmoothDamp
        (myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        //Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
      myTransform.position = targetPosition;

      HandleCameraCollisions(delta);
    }

    public void handleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
      lookAngle += (mouseXInput * lookSpeed) / delta;
      pivotAngle -= (mouseYInput * pivotSpeed) / delta;
      pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot); // camera can't go lower or higher than angles established

      Vector3 rotation = Vector3.zero;
      rotation.y = lookAngle;
      Quaternion targetRotation = Quaternion.Euler(rotation); // rotation in y based on look position
      myTransform.rotation = targetRotation;

      rotation = Vector3.zero;
      rotation.x = pivotAngle;
      targetRotation = Quaternion.Euler(rotation); // rotation in x based on pivot angle
      cameraPivotTransform.localRotation = targetRotation;
    }
    
    // bumps camera off of objects in the environment (prevents getting stuck behind walls etc.)
    private void HandleCameraCollisions(float delta)
    {
      targetPosition = defaultPosition;
      RaycastHit hit;
      Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
      direction.Normalize();

      // sphere that comes around the camera
      // if it intersects with an colliders (game environment) in the game it returns true (collision detected)
      if (Physics.SphereCast
        (cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition)
        , ignoreLayers))
      {
        float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
        targetPosition = -(dis - cameraCollisionOffset);
      }

      if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
      {
        targetPosition = -minimumCollisionOffset;
      }

      cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
      cameraTransform.localPosition = cameraTransformPosition;
    }
  }
}