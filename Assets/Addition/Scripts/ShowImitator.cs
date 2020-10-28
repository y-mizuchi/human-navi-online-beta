using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIGVerse.Competition.HumanNavigation
{
	public class ShowImitator : MonoBehaviour
	{
		void Awake()
		{
			Camera camera = this.GetComponent<Camera>();
			if (HumanNaviConfig.Instance.configInfo.showImitator)
			{
				camera.cullingMask |= (1 << 16);
			}
			else
			{
				camera.cullingMask &= ~(1 << 16);
			}
		}
	}
}
