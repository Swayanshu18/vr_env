using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // <-- This is needed for Toggle

public class SessionHandler : MonoBehaviour
{
    public TMP_InputField sessionIdInput;
    public string nextSceneNameToggleOn = "SceneToggleOn"; // When toggle is ON
    public string nextSceneNameToggleOff = "SceneToggleOff"; // When toggle is OFF
    public Toggle toggle; // Reference to the UI toggle

    public void OnContinueButtonClicked()
    {
        string sessionId = sessionIdInput.text;

        if (!string.IsNullOrEmpty(sessionId))
        {
            PersistentUserData.Instance.sessionID = sessionId;

            string path = Path.Combine(Application.persistentDataPath, "UserSessionData.csv");

            try
            {
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = new StreamWriter(path, append: false))
                    {
                        sw.WriteLine("Name,SessionID");
                    }
                }

                using (StreamWriter sw = new StreamWriter(path, append: true))
                {
                    string entry = $"{PersistentUserData.Instance.userName},{sessionId}";
                    sw.WriteLine(entry);
                }

                Debug.Log("Saved to CSV at: " + path);

                // Load scene based on toggle state
                if (toggle.isOn)
                {
                    SceneManager.LoadScene(nextSceneNameToggleOn);
                }
                else
                {
                    SceneManager.LoadScene(nextSceneNameToggleOff);
                }
            }
            catch (IOException ioEx)
            {
                Debug.LogError("File access error: " + ioEx.Message);
            }
        }
        else
        {
            Debug.LogWarning("Session ID is empty!");
        }
    }
}
