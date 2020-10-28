using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIGVerse.Common;
using NewtonVR;
using SIGVerse.Competition.HumanNavigation;
using static Photon.Pun.PhotonAnimatorView;
using Photon.Realtime;
using System.Linq;
using SIGVerse.Competition;
using UnityEngine.EventSystems;

#if SIGVERSE_PUN
using Photon.Pun;
#endif

namespace SIGVerse.PunTest
{
#if SIGVERSE_PUN && SIGVERSE_OCULUS

	public class ImitatorTransform : CommonInitializer
	{
		public Transform[] srcTransforms;
		public Transform[] destTransforms;

		public Transform[] srcTransformsRot;
		public Transform[] destTransformsRot;

		public Vector3 offset;
//		public Vector3 addPos;

		private bool isServer = false;
		private bool isImitatorTarget = false;

		private Transform srcAvatar;
		private bool initilized = false;

		private DateTime prevDateTime;
		private GameObject playbackManager;

		private readonly string imitatorTargetPrefix = "HumanX";

		private void Awake()
		{
			this.isImitatorTarget = HumanNaviConfig.Instance.configInfo.showImitator;
		}

		void Start()
		{
			this.isServer = HumanNaviConfig.Instance.configInfo.photonServerMachine;

			PhotonView photonView = this.GetComponent<PhotonView>();
			if (HumanNaviConfig.Instance.configInfo.playbackType != WorldPlaybackCommon.PlaybackTypePlay)
			{
				StartCoroutine(this.SetAvatarName(photonView));

				this.playbackManager = GameObject.Find("PlaybackManager"); // TODO: this process should be re-organized
			}

			//Player[] players = PhotonNetwork.PlayerListOthers;
			//int numberOfAvatars = players.Count(p => p.NickName.StartsWith("Human"));
			////Debug.LogWarning("numberOfAvatars: " + numberOfAvatars);
			//if (numberOfAvatars > 0)
			//{
			//	this.SetSourceTransform();
			//}
			//else
			//{
			//	Debug.LogWarning("Imitator: source not found.");
			//}

			this.prevDateTime = DateTime.Now;
		}


		void Update()
		{
			if (!this.isServer) { return; }

			if (!this.SourceTransformInitialized()) { return; }

			this.transform.position = this.srcAvatar.position + this.offset;

			for (int i = 0; i < srcTransforms.Length; i++)
			{
				this.destTransforms[i].localPosition = this.srcTransforms[i].localPosition;
				this.destTransforms[i].localRotation = this.srcTransforms[i].localRotation;
			}

			for (int i = 0; i < srcTransformsRot.Length; i++)
			{
				this.destTransformsRot[i].localRotation = this.srcTransformsRot[i].localRotation;
			}

			DateTime currDateTime = DateTime.Now;
			TimeSpan ts = currDateTime - this.prevDateTime;
			this.RecordEventLog("DeltaTime" + "\t" + (float)ts.TotalMilliseconds);
			this.prevDateTime = currDateTime;
		}

		private bool SourceTransformInitialized()
		{
			if (this.initilized)
			{
				return true;
			}

			this.initilized = SetSourceTransform();

			return this.initilized;
		}

		private bool SetSourceTransform()
		{
			if (!this.isServer) { return false; }

			//Player[] players = PhotonNetwork.PlayerListOthers;
			//int numberOfAvatars = players.Count(p => p.NickName.StartsWith("Human"));

			if (PhotonNetwork.PlayerListOthers.Count(p => p.NickName.StartsWith(this.imitatorTargetPrefix)) == 0)
			{
				Debug.LogWarning("Imitator: source not found.");
				return false;
			}

			//this.srcAvatar = GameObject.FindGameObjectWithTag("Avatar").transform; // TODO: target should be selected in some way (e.g., a config value)
			GameObject[] avatars =  GameObject.FindGameObjectsWithTag("Avatar");
			foreach(GameObject avatar in avatars)
			{
				if (avatar.name.StartsWith(this.imitatorTargetPrefix))
				{
					this.srcAvatar = avatar.transform;
					break;
				}
			}

			if (this.srcAvatar == null)
			{
				Debug.LogWarning("Imitator: source not found.");
				return false;
			}

			if (this.srcAvatar.GetComponent<HumanAvatarInitializer>().IsInitialized())
			{
				List<Transform> transforms = new List<Transform>();
				foreach (Transform dsrTransform in destTransforms)
				{
					transforms.Add(SIGVerseUtils.FindTransformFromChild(this.srcAvatar.transform, dsrTransform.name));
				}
				srcTransforms = transforms.ToArray();

				List<Transform> transformsRot = new List<Transform>();
				foreach (Transform dsrTransformRot in destTransformsRot)
				{
					transformsRot.Add(SIGVerseUtils.FindTransformFromChild(this.srcAvatar.transform, dsrTransformRot.name));
				}
				srcTransformsRot = transformsRot.ToArray();

				return true;
			}

			return false;
		}

		//public void ReleaseTransform()
		//{
		//	 //TODO: Target transforms of imitator avatar should be released when the target avatar left the room
		//}

		private void RecordEventLog(string log)
		{
			// For recording
			ExecuteEvents.Execute<IRecordEventHandler>
			(
				target: this.playbackManager,
				eventData: null,
				functor: (reciever, eventData) => reciever.OnRecordEvent(log)
			);
		}
	}
#endif
}
