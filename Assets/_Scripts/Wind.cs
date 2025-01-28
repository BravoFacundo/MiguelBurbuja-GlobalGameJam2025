using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector2 windDirection;
    public float windStrength = 10f;
void Update()
    {
        float zRotation = transform.rotation.eulerAngles.z;
        if (zRotation == 0)
        {
            windDirection = new Vector2(0, windStrength);

        }
        else if (zRotation == 90)
        {
            windDirection = new Vector2(windStrength, 0);

        }
        else if (zRotation == 180)
        {
            windDirection = new Vector2(0,-windStrength);

        }
        else if (zRotation == 270)
        {
            windDirection = new Vector2(-windStrength, 0);

        }
        else
        {
            windDirection = Vector2.zero; // Default case if the rotation is not exactly 0, 90, 180, or 270
        }
    }
}
