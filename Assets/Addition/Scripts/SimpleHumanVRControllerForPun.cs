using System;
using System.Collections.Generic;
using UnityEngine;
using SIGVerse.Human.VR;
using SIGVerse.Competition.HumanNavigation;

#if SIGVERSE_PUN
using Photon.Pun;
#endif


namespace SIGVerse.PunTest
{
	public class SimpleHumanVRControllerForPun : SimpleHumanVRController
	{
#if SIGVERSE_PUN
		private PhotonView photonView;

		protected override void Start()
		{
			base.Start();

			this.photonView = this.transform.root.GetComponent<PhotonView>();
		}

		protected override void Update()
		{
			if (photonView.IsMine)
//			if(!HumanNaviConfig.Instance.configInfo.photonServerMachine)
			{
				base.Update();
			}
		}
#endif
	}
}

