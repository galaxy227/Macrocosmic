using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float x;
    public float y;
    public float z;

    void Start()
    {
        OnStart();
    }

    private void UpdatePosition()
    {
        Vector3 followPosition = new Vector3(x, y, z);
        gameObject.transform.position = PlayerCamera.Instance.transform.position + followPosition;
    }

    // Utility
    private void OnStart()
    {
        PlayerCamera.AfterCamera.AddListener(UpdatePosition);
    }
}
