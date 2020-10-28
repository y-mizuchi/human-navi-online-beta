using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
	public GameObject srcObject;

    // Update is called once per frame
    void Update()
    {
		this.transform.position = srcObject.transform.position;
		this.transform.rotation = srcObject.transform.rotation;
	}
}
