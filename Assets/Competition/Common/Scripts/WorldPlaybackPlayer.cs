using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using SIGVerse.Common;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SIGVerse.Competition
{
	[RequireComponent(typeof (WorldPlaybackCommon))]
	public class WorldPlaybackPlayer : MonoBehaviour
	{
		public enum Step
		{
			Waiting,
			Initializing,
			Playing,
		}

		public bool enablePlayerCamera = false;

		[HeaderAttribute("Avatar")]
		public GameObject leftHandPrefab;
		public GameObject rightHandPrefab;

		protected bool isPlay = true;

		protected Step step = Step.Waiting;
		protected bool isInitialized = false;

		protected bool canInstantiateRootObjects = false;
		protected HashSet<string> rootObjectNames;

		protected string errorMsg = string.Empty;

		protected float elapsedTime = 0.0f;
		protected float deltaTime   = 0.0f;

		protected float startTime = 0.0f;
		protected float endTime = 0.0f;

		protected float playingSpeed = 1.0f;
		protected bool  isRepeating = false;

		protected PlaybackTransformEventController   transformController;   // Transform
		protected PlaybackVideoPlayerEventController videoPlayerController; // Video Player

		protected string filePath;


		protected virtual void Awake()
		{
			if(!this.isPlay)
			{
				this.enabled = false;
			}
		}

		// Use this for initialization
		protected virtual void Start()
		{
			WorldPlaybackCommon common = this.GetComponent<WorldPlaybackCommon>();

			this.filePath = common.GetFilePath();

			this.transformController   = new PlaybackTransformEventController  (common);  // Transform
			this.videoPlayerController = new PlaybackVideoPlayerEventController(common);  // Video Player
		}

		// Update is called once per frame
		protected virtual void Update()
		{
			if (this.step == Step.Playing)
			{
				this.Update(Time.deltaTime * this.playingSpeed);
			}
			else if (this.step == Step.Waiting && this.isInitialized)
			{
				this.Update(this.deltaTime);

				this.deltaTime = 0.0f;
			}
		}


		private void Update(float deltaTime)
		{
			this.deltaTime = deltaTime;

			this.elapsedTime += this.deltaTime;

			this.UpdateData();
		}


		public bool Initialize(string filePath)
		{
			if(this.step == Step.Waiting)
			{
				this.step = Step.Initializing;

				this.filePath = filePath;

				this.errorMsg = string.Empty;

				this.canInstantiateRootObjects = false;
				this.rootObjectNames = new HashSet<string>();

				//this.StartInitializing();
				StartCoroutine(this.StartInitializingCoroutine());

				//Thread threadReadData = new Thread(new ThreadStart(this.ReadDataFromFile));
				//threadReadData.Start();

				return true;
			}

			return false;
		}

		public bool Initialize()
		{
			return this.Initialize(this.filePath);
		}


		public bool Play(float startTime)
		{
			if (this.step == Step.Waiting && this.isInitialized)
			{
				this.StartPlaying(startTime);
				return true;
			}

			return false;
		}

		public bool Play()
		{
			return this.Play(0.0f);
		}


		public bool Stop()
		{
			if (this.step == Step.Playing)
			{
				this.StopPlaying();
				return true;
			}

			return false;
		}


		protected virtual void StartInitializing()
		{
			//this.transformController.StartInitializingEvents(); // Transform
			this.videoPlayerController.StartInitializingEvents(); // Video Player

			//this.StartInitializingEvents();

			//WorldPlaybackCommon common = this.GetComponent<WorldPlaybackCommon>();
			//this.transformController = new PlaybackTransformEventController(common);  // Transform
			this.transformController = new PlaybackTransformEventController(this.filePath);  // Transform
			this.transformController.StartInitializingEvents();
			//this.stringController = new PlaybackStringEventController(this.stringDataDestinations);  // String Data
			//this.stringController.StartInitializingEvents();
		}
		protected virtual IEnumerator StartInitializingCoroutine()
		{
			Thread threadGetRootObjects = new Thread(new ParameterizedThreadStart(this.PrepareRootObjectNames));
			threadGetRootObjects.Start(this.filePath);

			while (!this.canInstantiateRootObjects)
			{
				yield return null;
			}

			this.InstantiateAdditionalRootObjects();

			this.DisableComponent();

			this.StartInitializing();

			Thread threadReadData = new Thread(new ThreadStart(this.ReadDataFromFile));
			threadReadData.Start();
		}

		protected virtual void PrepareRootObjectNames(object filePathObj)
		{
			try
			{
				string filePath = (string)filePathObj;

				if (!File.Exists(filePath))
				{
					throw new Exception("Playback file NOT found. Path=" + filePath);
				}

				// File open
				StreamReader streamReader = new StreamReader(filePath);

				while (streamReader.Peek() >= 0)
				{
					string lineStr = streamReader.ReadLine();

					string[] columnArray = lineStr.Split(new char[] { '\t' }, 2);

					if (columnArray.Length < 2) { continue; }

					string headerStr = columnArray[0];
					string dataStr = columnArray[1];

					string[] headerArray = headerStr.Split(',');

					// Transform data
					if (headerArray[1] == WorldPlaybackCommon.DataType1Transform)
					{
						// Definition
						if (headerArray[2] == WorldPlaybackCommon.DataType2TransformDef)
						{
							string[] dataArray = dataStr.Split('\t');

							foreach (string transformPath in dataArray)
							{
								this.rootObjectNames.Add(transformPath.Split(new char[] { '/' }, 2)[0]);
							}
						}
					}
				}

				streamReader.Close();

				this.canInstantiateRootObjects = true;
			}
			catch (Exception ex)
			{
				SIGVerseLogger.Error(ex.Message);
				SIGVerseLogger.Error(ex.StackTrace);
			}
		}

		//protected virtual void StartInitializingEvents()
		//{
		//	this.transformController = new PlaybackTransformEventController(this.filePath);  // Transform
		//	this.transformController.StartInitializingEvents();

		//	//this.stringController = new PlaybackStringEventController(this.stringDataDestinations);  // String Data
		//	//this.stringController.StartInitializingEvents();
		//}

		protected virtual void InstantiateAdditionalRootObjects()
		{
			List<string> existingObjects = new List<string>();

			foreach (GameObject rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
			{
				if (!rootObj.activeInHierarchy) { continue; }

				existingObjects.Add(rootObj.name);
			}

			foreach (string rootObjectName in this.rootObjectNames)
			{
				if (!existingObjects.Contains(rootObjectName))
				{
					string resourceName = rootObjectName.Split('#')[1];

					SIGVerseLogger.Info("Add object. name=" + resourceName);

					UnityEngine.Object resourceObj = Resources.Load(resourceName);

					if (resourceObj == null)
					{
						SIGVerseLogger.Error("Couldn't find a object. name=" + rootObjectName);
						continue;
					}
					else
					{
						GameObject instance = MonoBehaviour.Instantiate((GameObject)resourceObj);
						instance.name = rootObjectName;

						if(instance.tag == "Avatar")
						{
							if(instance.transform.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/EthanLeftHand(Clone)") == null)
							{
								Debug.LogWarning("Not found: OVRCameraRig/TrackingSpace/LeftHandAnchor/EthanLeftHand(Clone)");
								Instantiate(this.leftHandPrefab, instance.transform.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor")).name = "EthanLeftHand(Clone)";
							}
							if (instance.transform.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/EthanRightHand(Clone)") == null)
							{
								Debug.LogWarning("Not found: OVRCameraRig/TrackingSpace/RightHandAnchor/EthanRightHand(Clone)");
								Instantiate(this.rightHandPrefab, instance.transform.Find("OVRCameraRig/TrackingSpace/RightHandAnchor")).name = "EthanRightHand(Clone)";
							}
							if (instance.transform.Find("LeftEyeAnchor") == null)
							{
								Debug.LogWarning("Not found: LeftEyeAnchor");
								new GameObject("LeftEyeAnchor").transform.parent = instance.transform;
								//Instantiate(new GameObject("LeftEyeAnchor"), instance.transform);
							}
							if (instance.transform.Find("RightEyeAnchor") == null)
							{
								Debug.LogWarning("Not found: RightEyeAnchor");
								new GameObject("RightEyeAnchor").transform.parent = instance.transform;
								//Instantiate(new GameObject("RightEyeAnchor"), instance.transform);
							}
							if (instance.transform.Find("TrackerAnchor") == null)
							{
								Debug.LogWarning("Not found: TrackerAnchor");
								new GameObject("TrackerAnchor").transform.parent = instance.transform;
								//Instantiate(new GameObject("TrackerAnchor"), instance.transform);
							}
						}

						List<Component> allComponents = instance.GetComponentsInChildren<Component>().ToList();

						allComponents.ForEach(component => this.DisablePlayerComponent(component));
					}
				}
			}
		}

		protected virtual void DisableComponent()
		{
			List<Component> allComponents = GameObject.FindObjectsOfType<Component>().ToList(); // High Load

			allComponents.ForEach(component => this.DisableComponent(component));
		}

		protected virtual void DisableComponent(Component component)
		{
			Type type = component.GetType();

			if (type.IsSubclassOf(typeof(Collider)))
			{
				((Collider)component).enabled = false;
			}
			else if (type == typeof(Rigidbody))
			{
				((Rigidbody)component).isKinematic = true;
				((Rigidbody)component).velocity = Vector3.zero;
				((Rigidbody)component).angularVelocity = Vector3.zero;
			}
		}

		protected virtual void DisablePlayerComponent(Component component)
		{
			Type type = component.GetType();

			if (type.IsSubclassOf(typeof(Behaviour)))
			{
				if (type == typeof(Camera) && this.enablePlayerCamera)
				{
					((Camera)component).stereoTargetEye = StereoTargetEyeMask.None;
				}
				else
				{
					((Behaviour)component).enabled = false;
				}
			}
		}

		protected virtual void ReadDataFromFile()
		{
			try
			{
				if (!File.Exists(this.filePath))
				{
					throw new Exception("Playback file NOT found. Path=" + this.filePath);
				}

				// File open
				StreamReader streamReader = new StreamReader(this.filePath);

				while (streamReader.Peek() >= 0)
				{
					string lineStr = streamReader.ReadLine();

					string[] columnArray = lineStr.Split(new char[]{'\t'}, 2);

					if (columnArray.Length < 2) { continue; }

					string headerStr = columnArray[0];
					string dataStr   = columnArray[1];

					string[] headerArray = headerStr.Split(',');

					this.ReadData(headerArray, dataStr);
				}

				streamReader.Close();

				SIGVerseLogger.Info("Playback player : File reading finished.");

				this.endTime = this.GetTotalTime();

				SIGVerseLogger.Info("Playback player : Total time=" + this.endTime);

				this.isInitialized = true;

				this.step = Step.Waiting;
			}
			catch (Exception ex)
			{
				SIGVerseLogger.Error(ex.Message);
				SIGVerseLogger.Error(ex.StackTrace);

				this.errorMsg = "Cannot read the file !";
				this.step = Step.Waiting;
			}
		}


		protected virtual void ReadData(string[] headerArray, string dataStr)
		{
			this.transformController  .ReadEvents(headerArray, dataStr); // Transform
			this.videoPlayerController.ReadEvents(headerArray, dataStr); // Video Player
		}


		protected virtual void StartPlaying(float startTime)
		{
			SIGVerseLogger.Info("( Start the world playback playing from "+startTime+"[s] )");

			this.step = Step.Playing;

			this.UpdateIndexAndElapsedTime(startTime);
		}


		protected virtual void UpdateIndexAndElapsedTime(float elapsedTime)
		{
			this.elapsedTime = elapsedTime;

			this.deltaTime = 0.0f;

			this.transformController  .UpdateIndex(elapsedTime); // Transform
			this.videoPlayerController.UpdateIndex(elapsedTime); // Video Player
		}


		protected virtual void StopPlaying()
		{
			SIGVerseLogger.Info("( Stop the world playback playing )");

			this.step = Step.Waiting;
		}


		protected virtual void UpdateData()
		{
			if (this.elapsedTime > this.endTime)
			{
				if(this.isRepeating)
				{
					// Wait 10 seconds until the next start
					if(this.elapsedTime > this.endTime + 10.0f)
					{
						this.UpdateDataByLatest(this.startTime);
					}
				}
				else
				{
					this.Stop();
				}
				return;
			}

			this.transformController  .ExecutePassedLatestEvents(this.elapsedTime, this.deltaTime); // Transform
			this.videoPlayerController.ExecutePassedLatestEvents(this.elapsedTime, this.deltaTime); // Video Player
		}


		protected virtual void UpdateDataByLatest(float elapsedTime)
		{
			this.UpdateIndexAndElapsedTime(elapsedTime);

			this.transformController  .ExecuteLatestEvents(); // Transforms
			this.videoPlayerController.ExecuteLatestEvents(); // Video Players
		}

		protected virtual float GetTotalTime()
		{
			return Mathf.Max(this.transformController.GetTotalTime(), this.videoPlayerController.GetTotalTime());
		}

		protected float GetMax(float x, float y)
		{
			return 1.0f;
		}


		public Step GetStep()
		{
			return this.step;
		}

		public float GetPlayingSpeed()
		{
			return this.playingSpeed;
		}
	}
}

