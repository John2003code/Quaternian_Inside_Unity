using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTester : MonoBehaviour
{
   
    public float moveSpeed = 4f;
    public LayerMask groundLayer;

 
    void Update()
    {
        Vector3 groundNormal = Vector3.up;
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 2f, groundLayer))
        {
            groundNormal = hit.normal;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(moveX, 0, moveZ);

        if (moveDir.magnitude > 0.1f)
        {
            // Use your CQuoternian LookRotation!
            // We use the inputDir to decide 'Forward' so the Cube faces where it walks
            CQuoternian myTargetRot = CQuoternian.LookRotation(moveDir, groundNormal);

            // Smoothly rotate using Unity's Slerp (to avoid the instant-snap jitter)
            transform.rotation = Quaternion.Slerp(transform.rotation, myTargetRot.ToUnity(), 0.3f);

            // Use Unity's simple Move command
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
        }
    }
}
