using System.Collections.Generic;
using UnityEngine;

public class SphereLogger : MonoBehaviour
{
    public static SphereLogger Instance; // Singleton

    private List<LoggedSphereData> loggedSpheres = new List<LoggedSphereData>();

    [System.Serializable]
    public class LoggedSphereData
    {
        public string name;
        public Vector3 position;
        public float scale;

        public LoggedSphereData(string name, Vector3 position, float scale)
        {
            this.name = name;
            this.position = position;
            this.scale = scale;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LogSphere(GameObject sphere)
    {
        string name = sphere.name;
        Vector3 position = sphere.transform.position;
        float scale = sphere.transform.localScale.x;

        LoggedSphereData data = new LoggedSphereData(name, position, scale);
        loggedSpheres.Add(data);

        // Log to terminal (logcat)
        Debug.Log($"[SPHERE LOG] Name: {name}, Position: {position}, Scale: {scale:F2}");
    }

    // Optional: call this if you want to export or view later
    public List<LoggedSphereData> GetLoggedSpheres()
    {
        return loggedSpheres;
    }
}
