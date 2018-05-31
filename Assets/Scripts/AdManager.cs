using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour {

	private static int count = 0;
	void Start()
	{
		Advertisement.Initialize ("2586857");


	}

	public static void ShowAd()
	{
		ShowOptions so = new ShowOptions ();
		so.resultCallback = HandleShowResult;
		if (count < 4) {
			count++;
			Advertisement.Show ("video", so);
		} else {
			Advertisement.Show ("rewardedVideo", so);
			count = 0;
		}

	}
	private static void  HandleShowResult(ShowResult obj)
	{
		switch (obj) {
		case ShowResult.Finished:
			Debug.Log ("finish");
			break;
		case ShowResult.Skipped:
			Debug.Log ("skipeed");
			break;
		case ShowResult.Failed:
			Debug.Log ("failed");
			break;

		}
	}

}
