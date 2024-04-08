using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Core
{
	public class NetworkExample : MonoBehaviour
	{
		[SerializeField] private string url;
		private void Start()
		{
			StartRun(url, Success, Error);
		}

		private void StartRun(string url, Action<string> callback, Action<string> error) => StartCoroutine(Run(url, callback, error));

		private IEnumerator Run(string url, Action<string> callback, Action<string> error = null) 
		{
			UnityWebRequest www = UnityWebRequest.Get(url);

			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				error?.Invoke(www.error);
			}
			else
			{
				callback?.Invoke(www.downloadHandler.text);
			}
			
			www.Dispose();
		}

		private void Error(string e) => Debug.LogError(e);
		private void Success(string result) => Debug.Log(result);
	}
}