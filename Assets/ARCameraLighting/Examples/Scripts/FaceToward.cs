using UnityEngine;

public static class FaceToward
 {

	public static void SetFacing( Transform t, Vector3 lookAt) 
	{
        var faceTarget = lookAt;
		faceTarget.y = t.position.y;
        t.LookAt(faceTarget);
    }
}
