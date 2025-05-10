using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class GameLogger : MonoBehaviour
{
    private static string logDirectory = "Logs";
    private static string logFilePath = Path.Combine(logDirectory, "game_log.csv");

    void Start()
    {
        // Ensure the Logs directory exists
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // Create CSV header if the file does not exist
        if (!File.Exists(logFilePath))
        {
            File.AppendAllText(logFilePath, "Time,XP,Weapons\n");
        }
    }

    // Logs game data to a CSV file.
    public static void LogGameData(float timeAlive, int completedCheckpoints)
    {
        string logEntry = $"{timeAlive:F2},{completedCheckpoints}\n";
        
        File.AppendAllText(logFilePath, logEntry);
        
        Debug.Log("Game log updated: " + logEntry);
    }
}
