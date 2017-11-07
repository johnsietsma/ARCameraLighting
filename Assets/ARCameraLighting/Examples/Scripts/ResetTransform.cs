using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTransform : MonoBehaviour {

	public void ResetPosition()
	{
        transform.localPosition = Vector3.zero;
    }
}
