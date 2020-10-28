using UnityEngine;
using System;
using System.Collections.Generic;
using SIGVerse.Common;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace SIGVerse.Competition
{
	public class PlaybackTransformEvent : PlaybackEventBase
	{
		public Transform TargetTransform { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
		public Vector3 Scale    { get; set; }

		public void Execute()
		{
			this.TargetTransform.position    = this.Position;
			this.TargetTransform.eulerAngles = this.Rotation;
			this.TargetTransform.localScale  = this.Scale;
		}
	}


	public class PlaybackTransformEventList : PlaybackEventListBase<PlaybackTransformEvent>
	{
		public PlaybackTransformEventList()
		{
			this.EventList = new List<PlaybackTransformEvent>();
		}
	}


	// ------------------------------------------------------------------

	public class PlaybackTransformEventController : PlaybackEventControllerBase<PlaybackTransformEventList, PlaybackTransformEvent>
	{
		private WorldPlaybackCommon common;

		private List<Transform> targetTransforms;

		private Dictionary<string, Transform> targetTransformPathMap;

		private List<Transform> transformOrder;

		public bool IsRigidbodiesDisable { get; set; }
		public bool IsCollidersDisable { get; set; }


		public PlaybackTransformEventController(WorldPlaybackCommon playbackCommon)
		{
			this.common = playbackCommon;

			// Transform
			this.targetTransforms = this.common.GetTargetTransforms();

			this.targetTransformPathMap = new Dictionary<string, Transform>();

			foreach (Transform targetTransform in this.targetTransforms)
			{
				this.targetTransformPathMap.Add(SIGVerseUtils.GetHierarchyPath(targetTransform), targetTransform);
			}

			this.IsRigidbodiesDisable = true;
			this.IsCollidersDisable = true;
		}
		public PlaybackTransformEventController(string filePath)
		{
			this.targetTransforms = new List<Transform>();

			foreach (GameObject rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
			{
				if (!rootObj.activeInHierarchy) { continue; }

				this.targetTransforms.AddRange(rootObj.GetComponentsInChildren<Transform>(true));
			}

			this.targetTransformPathMap = new Dictionary<string, Transform>();

			foreach (Transform targetTransform in this.targetTransforms)
			{
				if (this.targetTransformPathMap.ContainsKey(SIGVerseUtils.GetHierarchyPath(targetTransform)))
				{
					//Debug.Log("Already exist: " + SIGVerseUtils.GetHierarchyPath(targetTransform));
				}
				else
				{
					this.targetTransformPathMap.Add(SIGVerseUtils.GetHierarchyPath(targetTransform), targetTransform);
				}
			}

			this.IsRigidbodiesDisable = true;
			this.IsCollidersDisable = true;
		}

		public override void StartInitializingEvents()
		{
			this.eventLists = new List<PlaybackTransformEventList>();

			this.transformOrder = new List<Transform>();


			// Disable Rigidbodies and colliders
			foreach (Transform targetTransform in this.targetTransforms)
			{
				// Disable rigidbodies
				if (this.IsRigidbodiesDisable)
				{
					Rigidbody[] rigidbodies = targetTransform.GetComponentsInChildren<Rigidbody>(true);

					foreach (Rigidbody rigidbody in rigidbodies)
					{
						rigidbody.isKinematic = true;
						rigidbody.velocity = Vector3.zero;
						rigidbody.angularVelocity = Vector3.zero;
					}
				}

				// Disable colliders
				if (this.IsCollidersDisable)
				{
					Collider[] colliders = targetTransform.GetComponentsInChildren<Collider>(true);

					foreach (Collider collider in colliders)
					{
						collider.enabled = false;
					}
				}
			}
		}

		public override bool ReadEvents(string[] headerArray, string dataStr)
		{
			// Transform data
			if (headerArray[1] == WorldPlaybackCommon.DataType1Transform)
			{
				string[] dataArray = dataStr.Split('\t');

				// Definition
				if (headerArray[2] == WorldPlaybackCommon.DataType2TransformDef)
				{
					this.transformOrder.Clear();

					SIGVerseLogger.Info("Playback player : transform data num=" + dataArray.Length);

					foreach (string transformPath in dataArray)
					{
						if (!this.targetTransformPathMap.ContainsKey(transformPath))
						{
							SIGVerseLogger.Error("Couldn't find the object that path is " + transformPath);
						}

						this.transformOrder.Add(this.targetTransformPathMap[transformPath]);
					}
				}
				// Value
				else if (headerArray[2] == WorldPlaybackCommon.DataType2TransformVal)
				{
					if (this.transformOrder.Count == 0) { return false; }

					PlaybackTransformEventList playbackTransformEventList = new PlaybackTransformEventList();

					playbackTransformEventList.ElapsedTime = float.Parse(headerArray[0]);

					for (int i = 0; i < dataArray.Length; i++)
					{
						string[] transformValues = dataArray[i].Split(',');

						PlaybackTransformEvent transformEvent = new PlaybackTransformEvent();

						transformEvent.TargetTransform = this.transformOrder[i];

						transformEvent.Position = new Vector3(float.Parse(transformValues[0]), float.Parse(transformValues[1]), float.Parse(transformValues[2]));
						transformEvent.Rotation = new Vector3(float.Parse(transformValues[3]), float.Parse(transformValues[4]), float.Parse(transformValues[5]));

						if (transformValues.Length == 6)
						{
							transformEvent.Scale = Vector3.one;
						}
						else if (transformValues.Length == 9)
						{
							transformEvent.Scale = new Vector3(float.Parse(transformValues[6]), float.Parse(transformValues[7]), float.Parse(transformValues[8]));
						}

						playbackTransformEventList.EventList.Add(transformEvent);
					}
					
					this.eventLists.Add(playbackTransformEventList);
				}

				return true;
			}

			return false;
		}


		public List<Transform> GetTargetTransforms()
		{
			return this.targetTransforms;
		}


		public static string GetDefinitionLine(List<Transform> targetTransforms)
		{
			string definitionLine = "0.0," + WorldPlaybackCommon.DataType1Transform + "," + WorldPlaybackCommon.DataType2TransformDef; // Elapsed time is dummy.

			foreach (Transform targetTransform in targetTransforms)
			{
				// Make a header line
				definitionLine += "\t" + SIGVerseUtils.GetHierarchyPath(targetTransform);
			}

			return definitionLine;
		}

		public static string GetDataLine(string elapsedTime, List<Transform> targetTransforms)
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.Append(elapsedTime).Append(",").Append(WorldPlaybackCommon.DataType1Transform).Append(",").Append(WorldPlaybackCommon.DataType2TransformVal);

			foreach (Transform transform in targetTransforms)
			{
				stringBuilder.Append("\t")
					.Append(Math.Round(transform.position.x,    4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.position.y,    4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.position.z,    4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.eulerAngles.x, 4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.eulerAngles.y, 4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.eulerAngles.z, 4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.localScale.x,  4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.localScale.y,  4, MidpointRounding.AwayFromZero)).Append(",")
					.Append(Math.Round(transform.localScale.z,  4, MidpointRounding.AwayFromZero));
			}

			return stringBuilder.ToString();
		}
	}
}

