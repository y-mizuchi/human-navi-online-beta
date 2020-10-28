using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIGVerse.Competition
{
	public class AvatarColliderFixer : MonoBehaviour
	{
		private Vector3    posOrg;
		private Quaternion rotOrg;

		void Awake()
		{
			this.posOrg = this.transform.localPosition;
			this.rotOrg = this.transform.localRotation;
		}

		void LateUpdate()
		{
			this.transform.localPosition = this.posOrg;
			this.transform.localRotation = this.rotOrg;
		}
	}
}
