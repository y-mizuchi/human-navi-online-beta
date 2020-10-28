using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIGVerse.Common;
using SIGVerse.Competition.HumanNavigation;

#if SIGVERSE_PUN
using Photon.Pun;
#endif

namespace SIGVerse.PunTest
{
	public class HsrInitializer : CommonInitializer
	{
#if SIGVERSE_PUN

		public GameObject rosBridgeScripts;
		public GameObject competitionScripts;
		public GameObject moderator;

		void Awake()
		{
//			if (photonView.IsMine)
			if (HumanNaviConfig.Instance.configInfo.photonServerMachine &&
				HumanNaviConfig.Instance.configInfo.playbackType != SIGVerse.Competition.WorldPlaybackCommon.PlaybackTypePlay)
			{
//				this.GetComponent<GraspingDetectorForPun>().enabled = true;

//				this.GetComponent<HsrChat>().enabled = true;

				this.rosBridgeScripts.SetActive(true);

				//				PunLauncher.EnableSubview(this.gameObject);

				this.competitionScripts.GetComponent<HumanNaviSubGuidanceMessage>().enabled = true;

				this.moderator.GetComponent<HumanNaviPubTaskInfo>().enabled = true;
				this.moderator.GetComponent<HumanNaviPubMessage>().enabled = true;
				this.moderator.GetComponent<HumanNaviSubMessage>().enabled = true;
				this.moderator.GetComponent<HumanNaviPubAvatarStatus>().enabled = true;
				this.moderator.GetComponent<HumanNaviPubObjectStatus>().enabled = true;
			}
		}
		
		void Start()
		{
			//PhotonView photonView = this.GetComponent<PhotonView>();

			//StartCoroutine(this.SetAvatarName(photonView));
		}
#endif
	}
}
