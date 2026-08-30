using UnityEngine;
using System.Collections;

public class ForceMicPermission : MonoBehaviour
{
    IEnumerator Start()
    {
        // Force Unity to ask macOS for permission directly
        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("Forcing macOS Microphone Prompt...");
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
    }
}