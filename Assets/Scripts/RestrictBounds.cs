using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// move object down in Z value, where distance moved is equivalent to half of its height

public class RestrictBounds : MonoBehaviour
{
    private void Start()
    {
        AdjustPosition();
    }

    private void AdjustPosition()
    {
        float difference = gameObject.transform.localPosition.z - gameObject.GetComponent<MeshRenderer>().bounds.max.z;

        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, difference);
    }
}
