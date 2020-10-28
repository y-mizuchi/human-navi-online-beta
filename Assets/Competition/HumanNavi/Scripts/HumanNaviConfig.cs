using UnityEngine;
using System;
using System.IO;
using System.Text;
using SIGVerse.Common;
using System.Collections.Generic;
using System.Linq;

namespace SIGVerse.Competition.HumanNavigation
{
	[System.Serializable]
	public class TaskInfo
	{
		public string environment;
		public string target;
		public string destination;
	}

	[System.Serializable]
	public class HumanNaviConfigInfo
	{
		public string teamName;
		public int sessionTimeLimit;
		public int  maxNumberOfTrials;
		public bool recoverUsingScoreFile;
		public List<TaskInfo> taskInfoList;
		public int playbackType;
		public int playbackTrialNum;
		public int executionMode;
		public string language;
		public string guidanceMessageForPractice;
		public bool photonServerMachine;

		// Photon
		public string photonAppIdRuntime;
		public bool   photonUseNameServer;
		public string photonServerIP;
		public int photonPort;

		// Imitator
		public bool showImitator;
		public bool isImitatorTarget;
	}

	public enum ExecutionMode
	{
		Competition = 1,
		Practice,
	}

	public class Language
	{
		public const string English = "409";
		public const string Japanese = "411";
	}

	public class HumanNaviConfig : Singleton<HumanNaviConfig>
	{
		public const string FolderPath = "/../SIGVerseConfig/HumanNavi/";
		public const string ConfigFileName = "HumanNaviConfig.json";
		public const string ScoreFileName = "HumanNaviScore.txt";

		private string configFilePath;
		private string scoreFilePath;

		protected HumanNaviConfig() { } // guarantee this will be always a singleton only - can't use the constructor!

		public HumanNaviConfigInfo configInfo;

		public int numberOfTrials;

		public List<int> scores;

		public string ttsLanguageId;

		void Awake()
		{
			this.configFilePath = Application.dataPath + HumanNaviConfig.FolderPath + HumanNaviConfig.ConfigFileName;

			this.configInfo = new HumanNaviConfigInfo();

			if (File.Exists(configFilePath))
			{
				// File open
				StreamReader streamReader = new StreamReader(configFilePath, Encoding.UTF8);

				this.configInfo = JsonUtility.FromJson<HumanNaviConfigInfo>(streamReader.ReadToEnd());

				streamReader.Close();

			}
			else
			{
				SIGVerseLogger.Warn("HumanNavi config file does not exists.");

				this.configInfo.teamName = "XXXX";
				this.configInfo.sessionTimeLimit = 300;
				this.configInfo.maxNumberOfTrials = 1;
				this.configInfo.recoverUsingScoreFile = false;
				this.configInfo.executionMode = (int)ExecutionMode.Competition;
				this.configInfo.playbackType = WorldPlaybackCommon.PlaybackTypeNone;
				this.configInfo.language = "English";
				List<TaskInfo> taskInfoList = new List<TaskInfo>();
				taskInfoList.Add(new TaskInfo() { environment = "Default_Environment", target = "petbottle_500ml_empty_01", destination = "trashbox_01" });
				this.configInfo.taskInfoList = taskInfoList;
				this.configInfo.guidanceMessageForPractice = "";
				this.configInfo.photonServerMachine = false;

				// Photon
				this.configInfo.photonAppIdRuntime = "";
				this.configInfo.photonUseNameServer = false;
				this.configInfo.photonServerIP = "";
				this.configInfo.photonPort = 5055;

				// Copied Avatar
				this.configInfo.showImitator = true;
				this.configInfo.isImitatorTarget = false;

				this.SaveConfig();
			}


			this.scoreFilePath = Application.dataPath + HumanNaviConfig.FolderPath + HumanNaviConfig.ScoreFileName;

			this.scores = new List<int>();

			if (this.configInfo.recoverUsingScoreFile)
			{
				// File open
				StreamReader streamReader = new StreamReader(scoreFilePath, Encoding.UTF8);

				string line;

				while ((line = streamReader.ReadLine()) != null)
				{
					string scoreStr = line.Trim();

					if (scoreStr == string.Empty) { continue; }

					this.scores.Add(Int32.Parse(scoreStr));
				}

				streamReader.Close();

				this.numberOfTrials = this.scores.Count;

				if(this.configInfo.playbackType != WorldPlaybackCommon.PlaybackTypePlay)
				{
					if (this.numberOfTrials >= this.configInfo.maxNumberOfTrials)
					{
						SIGVerseLogger.Error("this.numberOfTrials >= this.configFileInfo.maxNumberOfTrials");
						Application.Quit();
					}
				}
			}
			else
			{
				this.numberOfTrials = 0;
			}

			if(this.configInfo.playbackType == WorldPlaybackCommon.PlaybackTypePlay)
			{
				this.numberOfTrials = this.configInfo.playbackTrialNum;
			}

			switch (this.configInfo.language)
			{
				case "English":  { this.ttsLanguageId = Language.English;  break; }
				case "Japanese": { this.ttsLanguageId = Language.Japanese; break; }
				default:         { this.ttsLanguageId = Language.English; break; }
			}
		}

		public void SaveConfig()
		{
			StreamWriter streamWriter = new StreamWriter(configFilePath, false, Encoding.UTF8);

			SIGVerseLogger.Info("Save HumanNavi config : " + JsonUtility.ToJson(HumanNaviConfig.Instance.configInfo));

			streamWriter.WriteLine(JsonUtility.ToJson(HumanNaviConfig.Instance.configInfo, true));

			streamWriter.Flush();
			streamWriter.Close();
		}

		public void InclementNumberOfTrials(int playbackType = 0)
		{
			if (playbackType != WorldPlaybackCommon.PlaybackTypePlay)
			{
				this.numberOfTrials++;
			}
		}

		public void RandomNumberOfTrials()
		{
			this.numberOfTrials = UnityEngine.Random.Range(1, this.configInfo.maxNumberOfTrials + 1);
		}

		public void AddScore(int score)
		{
			this.scores.Add(score);
		}

		public int GetTotalScore()
		{
			return this.scores.Where(score => score > 0).Sum();
		}

		public void RecordScoreInFile()
		{
			string filePath = Application.dataPath + HumanNaviConfig.FolderPath + HumanNaviConfig.ScoreFileName;

			bool append = true;

			if (this.numberOfTrials == 1) { append = false; }

			StreamWriter streamWriter = new StreamWriter(filePath, append, Encoding.UTF8);

			SIGVerseLogger.Info("Record the socre in a file. path=" + filePath);

			streamWriter.WriteLine(this.scores[this.scores.Count - 1]);

			streamWriter.Flush();
			streamWriter.Close();
		}

	}
}

