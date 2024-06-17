using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

public class EEGReceiver : MonoBehaviour
{
    TcpClient client; // TCP client for receiving data from EEG device/server.
    Thread receiveThread; // Thread for handling data reception to avoid blocking the main thread.
    string receivedData = ""; // Store the last chunk of received data as a string.

    // Server IP address and port number.
    string host = "127.0.0.1";
    int port = 8000;

    private bool keepReceiving = false; // Flag to control the reception thread.

    public ConcurrentQueue<float> dataQueue = new ConcurrentQueue<float>(); // Thread-safe queue for storing received EEG data.

    private double settingband; // Variable to store processed EEG data result.

    EEGDataProcessor processor = new EEGDataProcessor(); // Instance of EEG data processor for analyzing received data.

    void Awake()
    {
        // Check if an EEGReceiver object already exists in the scene to prevent duplicates.
        if (FindObjectsOfType<EEGReceiver>().Length > 1)
        {
            // If exists, destroy this newly created game object.
            Destroy(gameObject);
        }
        else
        {
            // If not exists, keep this game object across scenes.
            DontDestroyOnLoad(gameObject);
        }
    }

    public async void StartReceiving()
    {
        // Start receiving data if not already doing so.
        if (!keepReceiving) 
        {
            keepReceiving = true;
            await ConnectToServerAsync(); // Establish connection to the server asynchronously.
        }
    }

    public void StopReceiving()
    {
        // Stop receiving data and close the connection.
        CloseConnection();
        // Process received EEG data using the EEGDataProcessor instance.
        settingband = processor.ProcessData(dataQueue);
        Debug.Log("settingband " + settingband); // Log the result for debugging purposes.
    }

    async Task ConnectToServerAsync()
    {
        try
        {
            // Attempt to connect to the EEG data server.
            client = new TcpClient();
            await client.ConnectAsync(host, port);
            Debug.Log("Connected to server");

            // If connection is established and we're set to receive data, start the receive thread.
            if (client.Connected && keepReceiving)
            {
                receiveThread = new Thread(new ThreadStart(ReceiveData));
                receiveThread.IsBackground = true; // Run in background so it doesn't prevent app from exiting.
                receiveThread.Start();
            }
            else
            {
                Debug.LogWarning("Connection was successful but receiving was stopped before thread could start.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("On client connect exception: " + e.Message); // Log any exceptions during connection.
        }
    }

    void ReceiveData()
    {
        try
        {
            Byte[] bytes = new Byte[1024]; // Buffer for receiving data.
            while (keepReceiving && client != null && client.Connected) // Keep receiving data as long as conditions are met.
            {
                using (NetworkStream stream = client.GetStream())
                {
                    int length;
                    // Read data from stream until there's none left.
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incomingData = new byte[length]; // Temporary store for the received data.
                        Array.Copy(bytes, 0, incomingData, 0, length); // Copy received bytes into incomingData.
                        string serverMessage = Encoding.ASCII.GetString(incomingData); // Convert bytes to string.
                        
                        receivedData = serverMessage; // Update last received data.
                        
                        // Try to parse received data as float and enqueue it.
                        if (float.TryParse(receivedData, out float avgValue))
                        {
                            dataQueue.Enqueue(avgValue);
                            EventManager.BroadcastJumpRequest(avgValue); // Broadcast event for jump request based on received value.
                        }
                        else
                        {
                            Debug.LogError("Failed to parse received data as float."); // Log error if parsing fails.
                        }
                    }
                }
                Thread.Sleep(1); // Sleep to prevent tight loop consuming too much CPU.
            }
        }
        catch (SocketException socketException)
        {
            Debug.LogError("Socket exception: " + socketException); // Log socket exceptions.
        }
        finally
        {
            // Ensure resources are cleaned up.
            if (client != null)
            {
                client.Close();
                client = null; 
            }
        }
    }

    private void CloseConnection()
    {
        // Signal the receiving thread to stop and wait for it to finish.
        keepReceiving = false;
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(); // Wait for the thread to terminate.
        }
        // Close the client connection if it's not null.
        if (client != null)
        {
            client.Close();
            client = null;
        }
        Debug.Log("Connection closed."); // Log the closure of the connection.
    }

    void OnApplicationQuit()
    {
        // Ensure the connection is closed when the application quits.
        CloseConnection();
    }
}
