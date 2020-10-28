using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIGVerse.Common;

#if SIGVERSE_PUN
using Photon.Pun;
#endif

namespace SIGVerse.PunTest
{
	public class CommonInitializer : MonoBehaviour
	{
#if SIGVERSE_PUN

		protected IEnumerator SetAvatarName(PhotonView photonView)
		{
			object avatarNameObj;

////			if(photonVie)
//			{
//				Debug.LogError(photonView.Owner);
//			}
			while (!photonView.Owner.CustomProperties.TryGetValue(PunLauncher.AvatarNameKey, out avatarNameObj))
			{
				yield return null;
			}

			this.gameObject.name = (string)avatarNameObj;

			yield return null;
		}
#endif
	}
}

