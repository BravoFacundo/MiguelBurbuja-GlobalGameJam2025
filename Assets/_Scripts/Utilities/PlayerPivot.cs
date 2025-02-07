using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPivot : MonoBehaviour
{
    private void Awake() => GetComponentInChildren<SpriteRenderer>().enabled = false;
}
