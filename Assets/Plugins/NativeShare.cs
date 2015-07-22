﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/*
 * If you find this useful, please support me by buying one of my games.
 * www.chrismaire.com
 * 
 * If you don't...submit a fix :)
 * https://github.com/ChrisMaire/unity-native-sharing
 * 
 */

public class NativeShare : MonoBehaviour {
	public string ScreenshotName = "screenshot.png";

    public void ShareScreenshotWithText(string text)
    {
        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        Application.CaptureScreenshot(ScreenshotName);

        Share(text,screenShotPath,"");
    }

    //Note: Android share only uses the text and screenshot path.
	public void Share(string shareText, string imagePath, string url, string subject = "")
	{
#if UNITY_ANDROID
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotPath);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
		
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareString);
		
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		
		AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, shareString);
		currentActivity.Call("startActivity", jChooser);
#elif UNITY_IOS
		CallSocialShareAdvanced(shareText, subject, url, screenShotPath);
#else
		Debug.Log("No sharing set up for this platform.");
#endif
	}

#if UNITY_IOS
	public struct ConfigStruct
	{
		public string title;
		public string message;
	}

	[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);
	
	public struct SocialSharingStruct
	{
		public string text;
		public string url;
		public string image;
		public string subject;
	}
	
	[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);
	
	public static void CallSocialShare(string title, string message)
	{
		ConfigStruct conf = new ConfigStruct();
		conf.title  = title;
		conf.message = message;
		showAlertMessage(ref conf);
	}

	public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
	{
		SocialSharingStruct conf = new SocialSharingStruct();
		conf.text = defaultTxt; 
		conf.url = url;
		conf.image = img;
		conf.subject = subject;
		
		showSocialSharing(ref conf);
	}
#endif
}
