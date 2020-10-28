using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using NewtonVR;
using SIGVerse.Competition.HumanNavigation;

namespace SIGVerse.PunTest
{
    public class NVRHandData : MonoBehaviour//, IPunObservable
    {
//        [HideInInspector] public bool isEnable = false;

//        public string name;//
        [HideInInspector] private bool HoldButtonDown;  //
        [HideInInspector] private bool HoldButtonUp;//
        [HideInInspector] public bool HoldButtonPressed;//
        //public float HoldButtonAxis;

        //public bool UseButtonDown;
        //public bool UseButtonUp;
        //public bool UseButtonPressed;
        //public float UseButtonAxis;

        [HideInInspector] public bool IsRight;//
        [HideInInspector] public bool IsLeft;//

        [HideInInspector] public string CurrentlyInteractingName; //
        [HideInInspector] public string CurrentlyInteractingTag; //

        //private int EstimationSampleIndex;
        //private Vector3[] LastPositions;
        //private Quaternion[] LastRotations;
        //private float[] LastDeltas;
        //private int EstimationSamples;

        //public bool IsHovering;
        [HideInInspector] public bool IsInteracting;//
                                                    //public bool HasCustomMode;
                                                    //public bool IsCurrentlyTracked;
                                                    //public Vector3 CurrentForward;
                                                    //public Vector3 CurrentPosition;

        [HideInInspector] public bool HoldButtonDownForModerator;  //
        [HideInInspector] public bool HoldButtonUpForModerator;//

        private NVRHand nvrHand;

//        private PunRpcManager2 punRpcManager;
        private PhotonView photonView;

//        private NVRHandData preNvrHandData;

        private bool preHoldButtonDown;
		private bool preHoldButtonUp;
		private bool preHoldButtonPressed;
		private bool preIsRight;
		private bool preIsLeft;
		private string preCurrentlyInteractingName;
		private string preCurrentlyInteractingTag;
		private bool preIsInteracting;

        //public static object Deserialize(byte[] data)
        //{
        //    var result = new NVRHandData();
        //    result.Name = data[0].ToString();
        //    return result;
        //}

        //public static byte[] Serialize(object customType)
        //{
        //    var c = (NVRHandData)customType;
        //    return new byte[] { c.Id };
        //}

        public void Start()
        {
            this.photonView = this.GetComponent<PhotonView>();

            this.nvrHand = this.GetComponent<NVRHand>();

//            this.punRpcManager = this.GetComponent<PunRpcManager2>();

//            this.preNvrHandData = new NVRHandData();
        }

        public void Update()
        {
            if (!HumanNaviConfig.Instance.configInfo.photonServerMachine)
            {
//                Debug.LogError("OnPhotonSerializeViewOnPhotonSerializeView");
				this.HoldButtonDown           = nvrHand.HoldButtonDown;
				this.HoldButtonUp             = nvrHand.HoldButtonUp;
				this.HoldButtonPressed        = nvrHand.HoldButtonPressed;
				this.IsRight                  = nvrHand.IsRight;
				this.IsLeft                   = nvrHand.IsLeft;
				this.CurrentlyInteractingName = nvrHand.CurrentlyInteracting==null? "" : nvrHand.CurrentlyInteracting.name;
				this.CurrentlyInteractingTag  = nvrHand.CurrentlyInteracting==null? "" : nvrHand.CurrentlyInteracting.tag;
				this.IsInteracting            = nvrHand.IsInteracting;

                if(PhotonNetwork.IsConnectedAndReady)
                {
                    if(this.IsChanged())
                    {
                        //this.photonView.RPC("ForwardNVRHandDataRPC", RpcTarget.Others, 
                        //    this.HoldButtonDown, this.HoldButtonUp, this.HoldButtonPressed, this.IsRight, this.IsLeft, 
                        //    this.CurrentlyInteractingName, this.CurrentlyInteractingTag, this.IsInteracting
                        //);
//                        this.punRpcManager.ForwardNVRHandData(this);
                        this.ForwardNVRHandData(this);
                    }
                }

                this.UpdatePreNVRHandData();
            }
        }

        private bool IsChanged()
        {
            if (this.HoldButtonDown   !=this.preHoldButtonDown   ) { return true; }
            if (this.HoldButtonUp     !=this.preHoldButtonUp     ) { return true; }
            if (this.HoldButtonPressed!=this.preHoldButtonPressed) { return true; }

            if (this.IsRight!=this.preIsRight) { return true; }
            if (this.IsLeft !=this.preIsLeft ) { return true; }

            if (this.CurrentlyInteractingName.Equals(this.preCurrentlyInteractingName)) { return true; }
            if (this.CurrentlyInteractingTag .Equals(this.preCurrentlyInteractingTag )) { return true; }

            if (this.IsInteracting!=this.preIsInteracting) { return true; }

            return false;
        }

        private void UpdatePreNVRHandData()
        {
			this.preHoldButtonDown           = this.HoldButtonDown;
			this.preHoldButtonUp             = this.HoldButtonUp;
			this.preHoldButtonPressed        = this.HoldButtonPressed;
			this.preIsRight                  = this.IsRight;
			this.preIsLeft                   = this.IsLeft;
			this.preCurrentlyInteractingName = this.CurrentlyInteractingName;
			this.preCurrentlyInteractingTag  = this.CurrentlyInteractingTag;
			this.preIsInteracting            = this.IsInteracting;
        }

        public void UpdateModeratorData()
        {
			this.HoldButtonDownForModerator = this.HoldButtonDown;
			this.HoldButtonUpForModerator   = this.HoldButtonUp;
        }


  //      [PunRPC]
		//private void ForwardNVRHandDataRPC(
  //          bool holdButtonDown, bool holdButtonUp, bool holdButtonPressed, bool isRight, bool isLeft, 
  //          string currentlyInteractingName, string currentlyInteractingTag, bool isInteracting
  //      )
		//{
		//	Debug.LogError("ForwardNVRHandDataRPC");

		//	this.HoldButtonDown           = holdButtonDown;
		//	this.HoldButtonUp             = holdButtonUp;
		//	this.HoldButtonPressed        = holdButtonPressed;
		//	this.IsRight                  = isRight;
		//	this.IsLeft                   = isLeft;
		//	this.CurrentlyInteractingName = currentlyInteractingName;
		//	this.CurrentlyInteractingTag  = currentlyInteractingTag;
		//	this.IsInteracting            = isInteracting;
  //      }



        
		public void ForwardNVRHandData(NVRHandData handData)
		{
//			this.photonView.RPC("ForwardSubRosGuidanceMessageRPC", RpcTarget.All, guidaneMsg.message, guidaneMsg.display_type, guidaneMsg.source_language, guidaneMsg.target_language);

            this.photonView.RPC("ForwardNVRHandDataRPC", RpcTarget.Others, 
                handData.HoldButtonDown, handData.HoldButtonUp, handData.HoldButtonPressed, handData.IsRight, handData.IsLeft, 
                handData.CurrentlyInteractingName, handData.CurrentlyInteractingTag, handData.IsInteracting
            );
        }

        [PunRPC]
		private void ForwardNVRHandDataRPC(
            bool holdButtonDown, bool holdButtonUp, bool holdButtonPressed, bool isRight, bool isLeft, 
            string currentlyInteractingName, string currentlyInteractingTag, bool isInteracting
        )
		{
//			Debug.LogError("ForwardNVRHandDataRPC");

			this.HoldButtonDown           = holdButtonDown;
			this.HoldButtonUp             = holdButtonUp;
			this.HoldButtonPressed        = holdButtonPressed;
			this.IsRight                  = isRight;
			this.IsLeft                   = isLeft;
			this.CurrentlyInteractingName = currentlyInteractingName;
			this.CurrentlyInteractingTag  = currentlyInteractingTag;
			this.IsInteracting            = isInteracting;

			//this.nvrHandData.HoldButtonDown           = holdButtonDown;
			//this.nvrHandData.HoldButtonUp             = holdButtonUp;
			//this.nvrHandData.HoldButtonPressed        = holdButtonPressed;
			//this.nvrHandData.IsRight                  = isRight;
			//this.nvrHandData.IsLeft                   = isLeft;
			//this.nvrHandData.CurrentlyInteractingName = currentlyInteractingName;
			//this.nvrHandData.CurrentlyInteractingTag  = currentlyInteractingTag;
			//this.nvrHandData.IsInteracting            = isInteracting;
        }



        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        //        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //		{
        //			if (stream.IsWriting)
        //			{
        ////                Debug.LogError("OnPhotonSerializeView");
        ////				stream.SendNext(true);
        ////				stream.SendNext(nvrHand.name);
        //				stream.SendNext(this.HoldButtonDown);
        //				stream.SendNext(this.HoldButtonUp);
        //				stream.SendNext(this.HoldButtonPressed);
        //				stream.SendNext(this.IsRight);
        //				stream.SendNext(this.IsLeft);
        //				stream.SendNext(this.CurrentlyInteractingName);
        //				stream.SendNext(this.CurrentlyInteractingTag);
        //				stream.SendNext(this.IsInteracting);
        //			}
        //			else
        //			{
        ////				if (!hand.enabled) { return; }

        ////				Debug.LogError("UpdateNVRHand");

        ////				this.isEnable          = (bool)  stream.ReceiveNext();
        ////				this.name              = (string)stream.ReceiveNext();
        //				this.HoldButtonDown    = (bool)  stream.ReceiveNext();
        //				this.HoldButtonUp      = (bool)  stream.ReceiveNext();
        //				this.HoldButtonPressed = (bool)  stream.ReceiveNext();
        //				this.IsRight           = (bool)  stream.ReceiveNext();
        //				this.IsLeft            = (bool)  stream.ReceiveNext();
        //				this.CurrentlyInteractingName = (string)stream.ReceiveNext();
        //				this.CurrentlyInteractingTag  = (string)stream.ReceiveNext();
        //				this.IsInteracting     = (bool)stream.ReceiveNext();
        //			}
        //		}
    }
}

