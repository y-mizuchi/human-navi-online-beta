using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIGVerse.Common;
using NewtonVR;
using SIGVerse.Competition.HumanNavigation;
using static Photon.Pun.PhotonAnimatorView;
using SIGVerse.Human.IK;
using SIGVerse.Competition;
using System.Linq;

#if SIGVERSE_PUN
using Photon.Pun;
#endif

namespace SIGVerse.PunTest
{
#if SIGVERSE_PUN && SIGVERSE_OCULUS

	public class HumanAvatarInitializer : CommonInitializer
	{
		public NVRPlayer nvrPlayer;

		public GameObject ovrCameraRig;

		public GameObject ethan;

		public GameObject centerEyeAnchor;

		private bool initialized = false;

		private void Awake()
		{
			//	Debug.Log("HumanAvatarInitializer: Awake");
			//if (!this.GetComponent<PhotonView>().IsMine)
			//{
			//	this.gameObject.SetActive(false);
			//	Destroy(this.nvrPlayer);
			//	Destroy(this.ovrCameraRig.GetComponent<OVRCameraRig>());
			//	Destroy(this.ovrCameraRig.GetComponent<OVRManager>());

			//}

			if (this.GetComponent<PhotonView>().IsMine)
			{
				InitializeNVRInteractables();
			}
			
		}

		public static void InitializeNVRInteractables()
		{
			NVRInteractableItem[] nvrInteractableItems = SIGVerseUtils.FindObjectsOfInterface<NVRInteractableItem>();

			foreach(NVRInteractableItem nvrInteractableItem in nvrInteractableItems)
			{
				nvrInteractableItem.enabled = true;
			}
		}


		void Start()
		{
			PhotonView photonView = this.GetComponent<PhotonView>();

			if (HumanNaviConfig.Instance.configInfo.playbackType != WorldPlaybackCommon.PlaybackTypePlay)
			{
				StartCoroutine(this.SetAvatarName(photonView));
			}

			if (photonView.IsMine)
			{
				if (!HumanNaviConfig.Instance.configInfo.photonServerMachine)
				{
					if (HumanNaviConfig.Instance.configInfo.playbackType == WorldPlaybackCommon.PlaybackTypePlay)
					{
						this.ethan.GetComponent<Rigidbody>().useGravity = false;
						return;
					}

					//				this.GetComponent<HumanAvatarChat>().enabled = true;

					this.nvrPlayer.enabled = true;

					this.ovrCameraRig.GetComponent<OVRCameraRig>().enabled = true;
					this.ovrCameraRig.GetComponent<OVRManager>().enabled = true;
					this.ovrCameraRig.GetComponent<SIGVerse.Human.IK.AnchorPostureCalculator>().enabled = true;

					NVRHand[] nvrHands = this.ovrCameraRig.GetComponentsInChildren<NVRHand>();
					foreach (NVRHand nvrHand in nvrHands)
					{
						nvrHand.enabled = true;
					}

					HumanNaviModerator moderator = GameObject.FindGameObjectWithTag("Moderator").GetComponent<HumanNaviModerator>();
					NVRHandData[] nvrHandDataArr = this.ovrCameraRig.GetComponentsInChildren<NVRHandData>();
					foreach (NVRHandData nvrHandData in nvrHandDataArr)
					{
						//Debug.Log("HandData: "+ nvrHandData.IsLeft + "," + nvrHandData.IsRight);
						if (nvrHandData.name.StartsWith("Left")) { moderator.leftHandData = nvrHandData; } // TODO: should be used IsLeft/IsRight
						else if (nvrHandData.name.StartsWith("Right")) { moderator.rightHandData = nvrHandData; } // TODO: should be used IsLeft/IsRight
					}

					this.centerEyeAnchor.GetComponent<Camera>().enabled = true;
					//this.centerEyeAnchor.GetComponent<AudioListener>().enabled = true;
					this.ethan.GetComponent<Animator>().enabled = true;
					this.ethan.GetComponent<SimpleHumanVRControllerForPun>().enabled = true;
					this.ethan.GetComponent<SimpleIK>().enabled = true;
					this.ethan.GetComponent<CapsuleCollider>().enabled = true;

					GameObject.Find("SubViewCameraForAvatar").GetComponent<SyncTransform>().srcObject = this.centerEyeAnchor;
					//Common.HumanNavigation.SubviewOptionControllerForAvatar subViewController = GameObject.Find("SubViewController").GetComponent<Common.HumanNavigation.SubviewOptionControllerForAvatar>();
					//subViewController.subviewCameraForSimpleIK = this.centerEyeAnchor.GetComponent<Camera>();
					//subViewController.ResetCamera(this.centerEyeAnchor.GetComponent<Camera>());
					//CleanupAvatarVRHandControllerForRift[] cleanupAvatarVRHandControllerForRiftList = this.ethan.GetComponents<CleanupAvatarVRHandControllerForRift>();

					//foreach(CleanupAvatarVRHandControllerForRift cleanupAvatarVRHandControllerForRift in cleanupAvatarVRHandControllerForRiftList)
					//{
					//	cleanupAvatarVRHandControllerForRift.enabled = true;
					//}

					//				PunLauncher.EnableSubview(this.gameObject);
				}
			}
			else
			{
				this.gameObject.SetActive(true);

				//this.ethan.GetComponent<OVRCameraRig>().enabled = false;

				this.ethan.GetComponent<Rigidbody>().useGravity = false;
				this.ethan.GetComponent<Rigidbody>().isKinematic = true;

				NVRHandData[] nvrHandDataArr = this.ovrCameraRig.GetComponentsInChildren<NVRHandData>();
				foreach (NVRHandData nvrHandData in nvrHandDataArr)
				{
					nvrHandData.enabled = false;
				}

				//this.GetComponent<OVRCameraRig>().enabled = false;
				this.centerEyeAnchor.GetComponent<Camera>().enabled = false;

				//Destroy(this.nvrPlayer);
				//Destroy(this.ovrCameraRig.GetComponent<OVRCameraRig>());
				//Destroy(this.ovrCameraRig.GetComponent<OVRManager>());

				//this.nvrPlayer.enabled = false;

				//this.ovrCameraRig.GetComponent<OVRCameraRig>().enabled = false;
				//this.ovrCameraRig.GetComponent<OVRManager>().enabled = false;
				//this.ovrCameraRig.GetComponent<SIGVerse.Human.IK.AnchorPostureCalculator>().enabled = false;

				//NVRHand[] nvrHands = this.ovrCameraRig.GetComponentsInChildren<NVRHand>();
				//foreach (NVRHand nvrHand in nvrHands)
				//{
				//	nvrHand.enabled = false;
				//}

				//this.centerEyeAnchor.GetComponent<Camera>().enabled = false;
				//this.ethan.GetComponent<Animator>().enabled = false;
				//this.ethan.GetComponent<SimpleHumanVRControllerForPun>().enabled = false;
				//this.ethan.GetComponent<SimpleIK>().enabled = false;
				//this.ethan.GetComponent<CapsuleCollider>().enabled = false;
			}

			this.initialized = true;
		}

		//private float timeElapsed;

		//private void Update()
		//{
		//	//	timeElapsed += Time.deltaTime;

		//	//	if (timeElapsed >= 1.0f)
		//	//	{
		//	//		SIGVerseLogger.Error("Ethan Update x=" + this.ethan.transform.position.x+", z=" + this.ethan.transform.position.z);
		//	//		timeElapsed = 0.0f;
		//	//	}
		//}

		public bool IsInitialized()
		{
			return this.initialized;
		}
	}
#endif
}
