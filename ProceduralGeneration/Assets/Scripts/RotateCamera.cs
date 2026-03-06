using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotateRate;

    private void Update()
    {
        transform.Rotate(0, 1 * rotateRate * Time.deltaTime, 0);
    }
}
