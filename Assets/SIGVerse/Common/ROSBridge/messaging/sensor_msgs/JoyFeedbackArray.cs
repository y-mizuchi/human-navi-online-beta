// Generated by gencs from sensor_msgs/JoyFeedbackArray.msg
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.RosBridge;
using UnityEngine;

using SIGVerse.RosBridge.sensor_msgs;

namespace SIGVerse.RosBridge 
{
	namespace sensor_msgs 
	{
		[System.Serializable]
		public class JoyFeedbackArray : RosMessage
		{
			public System.Collections.Generic.List<sensor_msgs.JoyFeedback>  array;


			public JoyFeedbackArray()
			{
				this.array = new System.Collections.Generic.List<sensor_msgs.JoyFeedback>();
			}

			public JoyFeedbackArray(System.Collections.Generic.List<sensor_msgs.JoyFeedback>  array)
			{
				this.array = array;
			}

			new public static string GetMessageType()
			{
				return "sensor_msgs/JoyFeedbackArray";
			}

			new public static string GetMD5Hash()
			{
				return "cde5730a895b1fc4dee6f91b754b213d";
			}
		} // class JoyFeedbackArray
	} // namespace sensor_msgs
} // namespace SIGVerse.ROSBridge

