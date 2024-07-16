using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class TextDebug : MonoBehaviour
{
    public TMP_Text textDebugInstance; // This will be set in the Unity Editor

    public static TMP_Text textDebug;
    private static Queue<string> messages = new Queue<string>();
    private static int maxMessages = 2;

    void Awake()
    {
        // Assign the static variable from the instance variable
        if (textDebugInstance != null)
        {
            textDebug = textDebugInstance;
        }
    }

    public static void SetTextDebug(string message)
    {
        if (textDebug != null)
        {
            // Add the new message to the queue
            messages.Enqueue(message);

            // Remove the oldest message if the queue exceeds the maximum count
            if (messages.Count > maxMessages)
            {
                messages.Dequeue();
            }

            // Update the text component to display the last 5 messages
            textDebug.text = string.Join("\n", messages);
        }
    }
}
