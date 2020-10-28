using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIGVerse.Common;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Linq;
using Unity.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if SIGVERSE_PUN
using Photon.Pun;
using Photon.Realtime;
using SIGVerse.Competition.HumanNavigation;
//using UnityEngine.XR.Management;
#endif

namespace SIGVerse.PunTest
{
#if SIGVERSE_PUN

	public class PunLauncher : MonoBehaviourPunCallbacks
	{
		public const string AvatarNameKey = "AvatarName";

//		public const string GameVersion = "0.1";
		public const string HumanNamePrefix = "Human";
		public const string RobotNamePrefix = "HSR";
		private const string SubViewControllerStr = "SubviewController";

		//[HeaderAttribute("Spawn Info")]
		//public int humanMaxNumber = 1;
		//public Vector3[] humanPositions = new Vector3[] { };
		//public Vector3[] humanEulerAngles = new Vector3[] {};

		//public int robotMaxNumber = 1;
		//public Vector3[] robotPositions = new Vector3[] {};
		//public Vector3[] robotEulerAngles = new Vector3[] {};


		[HeaderAttribute("Objects")]
		public GameObject human;
		public GameObject robot;
		public GameObject imitator;

		public PunRpcManager rpcManager;

		public GameObject[] rootsOfSyncTarget;

//		[HeaderAttribute("Scripts")]
//		public PunRpcManager chatManager;

		//[HeaderAttribute("UI")]
		//public Button humanLoginButton;
		//public Button robotLoginButton;
		//public GameObject mainPanel;
		//public GameObject noticePanel;
		//public Text roomNameText;
		//public Text roomNameDefaultText;
		//public Text errorMessageText;

//		[HideInInspector]
		public List<GameObject> roomObjects;

		// -----------------------
//		private string roomName;

//		private bool isHuman;

		private List<GameObject> graspables;

		void Awake()
		{
			if (HumanNaviConfig.Instance.configInfo.photonServerMachine)
			{
				XRSettings.enabled = false;
			}
			else
			{
				XRSettings.enabled = true;
			}

			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = HumanNaviConfig.Instance.configInfo.photonAppIdRuntime;
			PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = HumanNaviConfig.Instance.configInfo.photonUseNameServer;
			PhotonNetwork.PhotonServerSettings.AppSettings.Server = HumanNaviConfig.Instance.configInfo.photonServerIP;
			PhotonNetwork.PhotonServerSettings.AppSettings.Port = HumanNaviConfig.Instance.configInfo.photonPort;
		}

		void Start()
		{
			PhotonNetwork.AutomaticallySyncScene = true;

			//this.Connect();

			// Check for duplication
			List<string> duplicateNames = roomObjects.GroupBy(obj => obj.name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

			if (duplicateNames.Count > 0)
			{
//				throw new Exception("There are multiple objects with the same name. e.g. " + duplicateNames[0]);
			}

			// Manage the synchronized room objects using singleton
			RoomObjectManager.Instance.roomObjects = roomObjects;
		}

		//void Update()
		//{
		//}

		public void Connect()
		{
//			this.isHuman = isHuman;

//			this.roomName = (roomNameText.text != string.Empty) ? roomNameText.text : roomNameDefaultText.text;

			PhotonNetwork.GameVersion = PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion;

//			Debug.LogError("PhotonNetwork.GameVersion="+PhotonNetwork.GameVersion);

			if (!PhotonNetwork.ConnectUsingSettings())
			{
				SIGVerseLogger.Error("Failed to connect Photon Server.");
			}
		}

		//public void GetOwnership()
		//{
		//	PhotonView photonView = this.GetComponent<PhotonView>();

		//	photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
		//}

		public override void OnConnectedToMaster()
		{
//			Debug.Log("OnConnectedToMaster RoomName=" + this.roomName);

//			PhotonNetwork.JoinOrCreateRoom(this.roomName, new RoomOptions(), TypedLobby.Default); 
			PhotonNetwork.JoinOrCreateRoom("HumanNaviRoom" + HumanNaviConfig.Instance.numberOfTrials, new RoomOptions(), TypedLobby.Default);  // for experiment on 2020-10-09
//			Debug.LogWarning("RoomName="+"HumanNaviRoom" + HumanNaviConfig.Instance.numberOfTrials);
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
//			Debug.Log("OnDisconnected");

//			this.chatManager.ClearChatUserList();
		}

		public override void OnJoinedRoom()
		{
			//if (this.ShouldDisconnect(out int numberOfLogins))
			//{
			//	this.StartCoroutine(this.Disconnect());
			//	return;
			//}


			if (!HumanNaviConfig.Instance.configInfo.photonServerMachine)
			{
				Application.runInBackground = true;

				//Player[] players = PhotonNetwork.PlayerListOthers;
				//int numberOfAvatars = players.Count(p => p.NickName.StartsWith(HumanNamePrefix));


				//PhotonView photonView = this.human.GetComponent<PhotonView>();
				//photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

				//GameObject player = PhotonNetwork.Instantiate(this.human.name, new Vector3(numberOfAvatars, 0, 1.2f), Quaternion.identity);
				float xPos = (float)((PhotonNetwork.LocalPlayer.ActorNumber - 1) / 3);
				float yPos = (float)((int)xPos % 3);
				GameObject player = PhotonNetwork.Instantiate(this.human.name, new Vector3(xPos, 0, 1.2f), Quaternion.identity);
				//PhotonView[] photonViews = this.human.GetComponentsInChildren<PhotonView>();
				PhotonView[] photonViews = player.GetComponentsInChildren<PhotonView>();

				//				Debug.LogError("photonView.OwnershipTransfer 1");
				foreach (PhotonView photonView in photonViews)
				{
//					Debug.LogError("photonView.name =" + photonView.name);
//					Debug.LogError("photonView.OwnershipTransfer 2");
					//if (photonView.OwnershipTransfer != OwnershipOption.Takeover)
					//{
					//	//						Debug.LogError("photonView.OwnershipTransfer 3");
					//	//						Debug.LogError("photonView.OwnershipTransfer="+photonView.OwnershipTransfer+", name="+photonView.name);
					//	//						Debug.LogError("photonView.OwnershipTransfer 4");
					//}

//					Debug.LogError("photonView.OwnershipTransfer 5");
//					if (photonView.name == "Avatar_SimpleIK")
					{
						photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
					}
				}

				//				Debug.LogError("photonView.OwnershipTransfer 6");
				//PhotonNetwork.NickName = HumanNamePrefix + String.Format("{0:D2}", PhotonNetwork.LocalPlayer.ActorNumber);
				//PhotonNetwork.NickName = HumanNamePrefix + String.Format("{0:D2}", numberOfAvatars);
				string ImitationTargetPrefix = string.Empty;
				if (HumanNaviConfig.Instance.configInfo.isImitatorTarget)
				{
					if (PhotonNetwork.PlayerListOthers.Count(p => p.NickName.StartsWith(HumanNamePrefix + "X")) == 0)
					{
						ImitationTargetPrefix = "X";
					}
					else
					{ 
						Debug.LogWarning("Imitator target already exists");
					}
				}
				PhotonNetwork.NickName = HumanNamePrefix + ImitationTargetPrefix + String.Format("{0:D2}", PhotonNetwork.LocalPlayer.ActorNumber);

				ExitGames.Client.Photon.Hashtable customPropertie = new ExitGames.Client.Photon.Hashtable();
				customPropertie.Add(AvatarNameKey, PhotonNetwork.NickName + "#" + this.human.name);
				PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertie);

//				Debug.LogError("photonView.OwnershipTransfer 7");


//				Debug.LogError("photonView.OwnershipTransfer 8");

				//foreach (GameObject rootOfSyncTarget in this.rootsOfSyncTarget)
				//{
				//	PhotonView[] graspablesPhotonView = rootOfSyncTarget.GetComponentsInChildren<PhotonView>(false);

				//	foreach(PhotonView graspablePhotonView in graspablesPhotonView)
				//	{
				//		graspablePhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
				//	}
				//}
//				Debug.LogError("photonView.OwnershipTransfer 9");
			}
			else
			{
				//PhotonNetwork.NickName = "HSR";

				//Player[] players = PhotonNetwork.PlayerListOthers;
				//int numberOfRobots = players.Count(p => p.NickName.StartsWith(RobotNamePrefix));


				//PhotonView photonView = this.robot.GetComponent<PhotonView>();
				//photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

				PhotonView[] photonViews = this.robot.GetComponentsInChildren<PhotonView>();

				foreach (PhotonView photonView in photonViews)
				{
//					if (photonView.name == "HSR-B")
					{
						photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
					}
				}

				// Add imitator
				if (HumanNaviConfig.Instance.configInfo.showImitator)
				{
					Player[] players = PhotonNetwork.PlayerListOthers;
					int numberOfAvatars = players.Count(p => p.NickName.StartsWith(HumanNamePrefix + "X"));
					if (numberOfAvatars > 0)
					{
						GameObject player = PhotonNetwork.Instantiate(this.imitator.name, new Vector3(-1, 0, 1.2f), Quaternion.identity);

						photonViews = player.GetComponentsInChildren<PhotonView>();
						foreach (PhotonView photonView in photonViews)
						{
							photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
						}
					}
				}

				//PhotonNetwork.NickName = RobotNamePrefix + String.Format("{0:D2}", numberOfRobots);
				PhotonNetwork.NickName = RobotNamePrefix + String.Format("{0:D2}", PhotonNetwork.LocalPlayer.ActorNumber);
				ExitGames.Client.Photon.Hashtable customPropertie = new ExitGames.Client.Photon.Hashtable();
				//customPropertie.Add(AvatarNameKey, PhotonNetwork.NickName + "#" + this.robot.name);
				customPropertie.Add(AvatarNameKey, PhotonNetwork.NickName + "#" + this.imitator.name);
				PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertie);

//				XRSettings.enabled = false;
//				XRDisplaySubsystem subSystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
//				subSystem.Stop();

//				GameObject player = PhotonNetwork.Instantiate(this.robotSource.name, this.robotPositions[numberOfLogins], Quaternion.Euler(this.robotEulerAngles[numberOfLogins]));

				foreach (GameObject roomObject in this.roomObjects)
				{
					Rigidbody rigidbodyGraspable = roomObject.GetComponent<Rigidbody>();
					rigidbodyGraspable.useGravity = false;
					rigidbodyGraspable.collisionDetectionMode = CollisionDetectionMode.Discrete;
					rigidbodyGraspable.isKinematic = true;
					rigidbodyGraspable.constraints = RigidbodyConstraints.FreezeAll;
				}
			}

			this.graspables = GameObject.FindGameObjectsWithTag("Graspables").ToList();

			//			this.mainPanel.SetActive(false);
		}

		public void TransferOwnershipOfGraspables(NVRHandData hand)
		{
			if (hand.HoldButtonDownForModerator && hand.IsInteracting)
			{
				if (hand.CurrentlyInteractingTag == "Graspables")
				{
					GameObject graspedObj = this.graspables.SingleOrDefault(obj => obj.name == hand.CurrentlyInteractingName);

					if(graspedObj.name == hand.CurrentlyInteractingName)
					{
						graspedObj.GetComponent<Rigidbody>().useGravity = true;
						graspedObj.GetComponent<Rigidbody>().isKinematic = false;

						graspedObj.GetComponentInChildren<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
						//PhotonView[] photonViews = graspedObj.GetComponentsInChildren<PhotonView>();
						//foreach (PhotonView photonView in photonViews)
						//{
						//	photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
						//}

						this.rpcManager.ForwardObjectGrasp(graspedObj.name);
					}
				}
			}
		}

		public void DesableRigidbody(string objectName)
		{
			GameObject graspedObj = this.graspables.SingleOrDefault(obj => obj.name == objectName);

			if (graspedObj.name == objectName)
			{
				if (!graspedObj.GetComponentInChildren<PhotonView>().IsMine)
				{
					graspedObj.GetComponent<Rigidbody>().useGravity = false;
					graspedObj.GetComponent<Rigidbody>().isKinematic = true;
				}
			}
		}

		//public override void OnPlayerEnteredRoom(Player newPlayer)
		//{
		//	//base.OnPlayerEnteredRoom(newPlayer);

		//	if (HumanNaviConfig.Instance.configInfo.photonServerMachine && this.imitator != null)
		//	{
		//		Player[] players = PhotonNetwork.PlayerListOthers;
		//		int numberOfAvatars = players.Count(p => p.NickName.StartsWith(HumanNamePrefix));

		//		if(numberOfAvatars > 0)
		//		{
		//			this.imitator.GetComponent<ImitatorTransform>().SetSourceTransform();
		//		}
		//	}
		//}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			// TODO: Target transforms of imitator avatar should be released when the target avatar left the room
		}

		//		private bool ShouldDisconnect(out int numberOfLogins)
		//		{
		//			Player[] players = PhotonNetwork.PlayerListOthers;

		//			numberOfLogins = !HumanNaviConfig.Instance.configInfo.photonServerMachine? players.Count(p => p.NickName.StartsWith(HumanNamePrefix)) : players.Count(p => p.NickName.StartsWith(RobotNamePrefix));

		////			if ((this.isHuman && numberOfLogins>=this.humanMaxNumber) || (!this.isHuman && numberOfLogins >= this.robotMaxNumber))
		////			{
		////				string errorMessage = "Over capacity - logs you out.";

		////				SIGVerseLogger.Warn(errorMessage);

		//////				this.errorMessageText.text = errorMessage;
		//////				this.errorMessageText.gameObject.SetActive(true);
		//////
		//////				this.humanLoginButton.interactable = false;
		//////				this.robotLoginButton.interactable = false;

		////				return true;
		////			}

		//			return false;
		//		}

		public IEnumerator Disconnect()
		{
			yield return new WaitForSeconds(1.0f);

			PhotonNetwork.LeaveRoom();
			PhotonNetwork.Disconnect();

//			Debug.LogError("PUN Disconnect");
//			this.errorMessageText.gameObject.SetActive(false);
//
//			this.humanLoginButton.interactable = true;
//			this.robotLoginButton.interactable = true;
		}


		public void SetRoomObjects(List<GameObject> roomObjects)
		{
			this.roomObjects = roomObjects;
		}

		public static void AddPhotonTransformView(PhotonView photonView, GameObject synchronizedTarget, bool syncPos = false, bool syncRot = true)
		{
			PhotonTransformView photonTransformView = synchronizedTarget.AddComponent<PhotonTransformView>();

			photonTransformView.m_SynchronizePosition = syncPos;
			photonTransformView.m_SynchronizeRotation = syncRot;
			photonTransformView.m_SynchronizeScale    = false;

			photonView.ObservedComponents.Add(photonTransformView);
		}

		public static void EnableSubview(GameObject operationTarget)
		{
			operationTarget.transform.root.Find(SubViewControllerStr).gameObject.SetActive(true);

			// Update the camera list before enable SubviewOptionController
			GameObject.FindObjectOfType<SubviewManager>().UpdateCameraList();

			SubviewOptionController[] subviewOptionControllers = operationTarget.GetComponentsInChildren<SubviewOptionController>();

			foreach (SubviewOptionController subviewOptionController in subviewOptionControllers)
			{
				subviewOptionController.enabled = true;
			}
		}

		//public override void OnLeftRoom()
		//{
		//	SceneManager.LoadScene(0);
		//}

		//public void LeaveRoom()
		//{
		//	PhotonNetwork.LeaveRoom();
		//}
	}

#endif

#if SIGVERSE_PUN && UNITY_EDITOR
	[CustomEditor(typeof(PunLauncher))]
	public class PunLauncherEditor : Editor
	{
		//void OnEnable()
		//{
		//}

		public override void OnInspectorGUI()
		{
			PunLauncher punLauncher = (PunLauncher)target;

			//if (punLauncher.humanMaxNumber != punLauncher.humanPositions.Length || punLauncher.humanMaxNumber != punLauncher.humanEulerAngles.Length)
			//{
			//	Undo.RecordObject(target, "Update Human Spawn Info");
			//	Array.Resize(ref punLauncher.humanPositions, punLauncher.humanMaxNumber);
			//	Array.Resize(ref punLauncher.humanEulerAngles, punLauncher.humanMaxNumber);
			//}

			//if (punLauncher.robotMaxNumber != punLauncher.robotPositions.Length || punLauncher.robotMaxNumber != punLauncher.robotEulerAngles.Length)
			//{
			//	Undo.RecordObject(target, "Update Robot Spawn Info");
			//	Array.Resize(ref punLauncher.robotPositions, punLauncher.robotMaxNumber);
			//	Array.Resize(ref punLauncher.robotEulerAngles, punLauncher.robotMaxNumber);
			//}

			base.OnInspectorGUI();

			GUILayout.Space(10);

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Update Photon View", GUILayout.Width(200), GUILayout.Height(40)))
				{
					Undo.RecordObject(target, "Update Photon View");

					// Remove photon scripts
					RemoveScripts<PhotonTransformView>();
					RemoveScripts<LocalTransformView>();
					RemoveScripts<PhotonRigidbodyView>();
					RemoveScripts<PhotonView>();
//					RemoveScripts<PunOwnerChangerForObject>();

					// Add photon scripts
					List<GameObject> roomObjects = new List<GameObject>();

					foreach (GameObject sourceOfSyncTarget in punLauncher.rootsOfSyncTarget)
					{
						Rigidbody[] syncTargetRigidbodies = sourceOfSyncTarget.GetComponentsInChildren<Rigidbody>();

						foreach (Rigidbody syncTargetRigidbody in syncTargetRigidbodies)
						{
							roomObjects.Add(syncTargetRigidbody.gameObject);
						}
					}

					punLauncher.SetRoomObjects(roomObjects);

					foreach (GameObject roomObject in roomObjects)
					{
						PhotonView photonView = Undo.AddComponent<PhotonView>(roomObject);
						photonView.OwnershipTransfer = OwnershipOption.Takeover;
						photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
						photonView.ObservedComponents = new List<Component>();

//						PhotonTransformView photonTransformView = Undo.AddComponent<PhotonTransformView>(roomObject);
						LocalTransformView localTransformView = Undo.AddComponent<LocalTransformView>(roomObject);
//						PhotonRigidbodyView photonRigidbodyView = Undo.AddComponent<PhotonRigidbodyView>(roomObject);

//						photonView.ObservedComponents.Add(photonTransformView);
						photonView.ObservedComponents.Add(localTransformView);
//						photonView.ObservedComponents.Add(photonRigidbodyView);

//						Undo.AddComponent<PunOwnerChangerForObject>(roomObject);
					}
				}

				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);
		}

		private void RemoveScripts<T>() where T : Component
		{
			PunLauncher punLauncher = (PunLauncher)target;

			foreach (GameObject sourceOfSyncTarget in punLauncher.rootsOfSyncTarget)
			{
				T[] photonScripts = sourceOfSyncTarget.GetComponentsInChildren<T>();

				foreach (T photonScript in photonScripts)
				{
					Undo.DestroyObjectImmediate(photonScript);
				}
			}
		}
	}
#endif
}
