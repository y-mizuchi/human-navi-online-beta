using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using SIGVerse.Common;
using SIGVerse.Competition.HumanNavigation;

#if SIGVERSE_PUN
using Photon.Pun;
#endif

namespace SIGVerse.PunTest
{

#if SIGVERSE_PUN

	public class PunRpcManager : MonoBehaviour
	{
		//-----------------------------

		public GameObject moderator;

		private PhotonView photonView;

		public SAPIVoiceSynthesisExternal tts;

//		public NVRHandData nvrHandData;

		public PunLauncher punLauncher;

		void Awake()
		{
		}

		void Start()
		{
			this.photonView = this.GetComponent<PhotonView>();
		}

		public void ForwardSubRosMessage(RosBridge.human_navigation.HumanNaviMsg humanNaviMsg)
		{
			this.photonView.RPC("ForwardSubRosMessageRPC", RpcTarget.All, humanNaviMsg.message, humanNaviMsg.detail);
		}

		[PunRPC]
		private void ForwardSubRosMessageRPC(string message, string detail)
		{
			Debug.LogWarning("ForwardSubRosMessageRPC msg="+message);

			// Forward the message 
//			foreach (GameObject destination in this.destinations)
			{
				ExecuteEvents.Execute<IReceiveHumanNaviMsgHandler>
				(
//					target: destination,
					target: this.moderator,
					eventData: null,
					functor: (reciever, eventData) => reciever.OnReceiveRosMessage(new RosBridge.human_navigation.HumanNaviMsg(message, detail))
				);
			}
		}


		public void ForwardSubRosGuidanceMessage(RosBridge.human_navigation.HumanNaviGuidanceMsg guidaneMsg)
		{
			this.photonView.RPC("ForwardSubRosGuidanceMessageRPC", RpcTarget.All, guidaneMsg.message, guidaneMsg.display_type, guidaneMsg.source_language, guidaneMsg.target_language);
		}

		[PunRPC]
		private void ForwardSubRosGuidanceMessageRPC(string message, string display_type, string source_language, string target_language)
		{
			Debug.LogWarning("ForwardSubRosGuidanceMessageRPC msg="+message);

			this.tts.OnReceiveROSHumanNaviGuidanceMessage(new RosBridge.human_navigation.HumanNaviGuidanceMsg(message, display_type, source_language, target_language));
		}

		public void ForwardObjectGrasp(string objectName)
		{
			this.photonView.RPC("ForwardObjectGraspRPC", RpcTarget.All, objectName);
		}
		[PunRPC]
		private void ForwardObjectGraspRPC(string objectName)
		{
			this.punLauncher.DesableRigidbody(objectName);
		}

		public void ForwardTargetObjectGrasp(string graspedObjectName)
		{
			this.photonView.RPC("ForwardTargetObjectGraspRPC", RpcTarget.All, graspedObjectName);
		}
		[PunRPC]
		private void ForwardTargetObjectGraspRPC(string graspedObjectName)
		{
			this.moderator.GetComponent<HumanNaviModerator>().TargetObjectGrasp(graspedObjectName);
		}

		public void ForwardWringObjectGrasp(string graspedObjectName)
		{
			this.photonView.RPC("ForwardWringObjectGraspRPC", RpcTarget.All, graspedObjectName);
		}
		[PunRPC]
		private void ForwardWringObjectGraspRPC(string graspedObjectName)
		{
			this.moderator.GetComponent<HumanNaviModerator>().WrongObjectGrasp(graspedObjectName);
		}

		//public void ForwardNVRHandData(NVRHandData nvrHandData)
		//{
		//	this.photonView.RPC("ForwardNVRHandDataRPC", RpcTarget.Others,
		//		nvrHandData.HoldButtonDownForModerator, nvrHandData.HoldButtonUpForModerator, nvrHandData.HoldButtonPressed, nvrHandData.IsRight, nvrHandData.IsLeft,
		//		nvrHandData.CurrentlyInteractingName, nvrHandData.CurrentlyInteractingTag, nvrHandData.IsInteracting
		//	);
		//}

		//[PunRPC]
		//private void ForwardNVRHandDataRPC(
		//	bool holdButtonDown, bool holdButtonUp, bool holdButtonPressed, bool isRight, bool isLeft,
		//	string currentlyInteractingName, string currentlyInteractingTag, bool isInteracting
		//)
		//{
		//	Debug.LogWarning("ForwardNVRHandDataRPC");

		//	//this.nvrHandData.HoldButtonDown = holdButtonDown;
		//	//this.nvrHandData.HoldButtonUp = holdButtonUp;
		//	//this.nvrHandData.HoldButtonPressed = holdButtonPressed;
		//	//this.nvrHandData.IsRight = isRight;
		//	//this.nvrHandData.IsLeft = isLeft;
		//	//this.nvrHandData.CurrentlyInteractingName = currentlyInteractingName;
		//	//this.nvrHandData.CurrentlyInteractingTag = currentlyInteractingTag;
		//	//this.nvrHandData.IsInteracting = isInteracting;
		//	this.moderator.GetComponent<HumanNaviModerator>().SetNVRHandData(
		//		holdButtonDown, holdButtonUp, holdButtonPressed, isRight, isLeft, currentlyInteractingName, currentlyInteractingTag, isInteracting
		//		);
		//}
	}
#endif
}

