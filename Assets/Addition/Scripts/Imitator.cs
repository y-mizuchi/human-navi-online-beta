using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIGVerse.Common;
using NewtonVR;
using SIGVerse.Competition.HumanNavigation;
using static Photon.Pun.PhotonAnimatorView;

#if SIGVERSE_PUN
using Photon.Pun;
#endif

namespace SIGVerse.PunTest
{
#if SIGVERSE_PUN && SIGVERSE_OCULUS

	public class Imitator : MonoBehaviour
	{
		public Transform[] srcTransforms;
		public Transform[] destTransforms;

		public Vector3 addPos;

		public Animator animatorSrc;
		public Animator animatorDest;

		private bool isServer = false;

		void Start()
		{
			this.isServer = HumanNaviConfig.Instance.configInfo.photonServerMachine;
		}


		void Update()
		{
			for (int i=0; i<srcTransforms.Length; i++)
			{
				this.destTransforms[i].position = this.srcTransforms[i].position + this.addPos;
				this.destTransforms[i].rotation = this.srcTransforms[i].rotation;
			}

			if (this.isServer) 
			{ 
				this.animatorDest.SetFloat("Forward", this.animatorSrc.GetFloat("Forward"));
				this.animatorDest.SetFloat("Turn",    this.animatorSrc.GetFloat("Turn"));
			}
			else
			{
			}
		}
	}
#endif
}
