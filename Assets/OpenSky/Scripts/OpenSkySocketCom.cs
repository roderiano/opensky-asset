using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
public class OpenSkySocketCom : MonoBehaviour
{
    private Thread _connectionThread;
    private volatile bool _cancelFlag = false;
    void Start() {
        _connectionThread = new Thread(RefreshServerGameData);
        _connectionThread.Start();
    }

    void RefreshServerGameData() {
        UdpClient client = new UdpClient(5000);
        try {
            client.Connect("127.0.0.1", 8888);

            while(!_cancelFlag) {
                string watchersComponentsData = OpenSkyDataHandler.Data.GetJsonWatchersComponentsData();
                byte[] sendBytes = Encoding.ASCII.GetBytes(watchersComponentsData);
                client.Send(sendBytes, sendBytes.Length);

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5000);
                byte[] receiveBytes = client.Receive(ref remoteEndPoint);
                string receivedString = Encoding.ASCII.GetString(receiveBytes);

                string[] pieces = receivedString.Split(new[] { ';' }, 2);
                if(Int32.Parse(pieces[0]) == pieces[1].Length)
                    OpenSkyDataHandler.Data.SetJsonWatchersComponentsData(pieces[1]);
                
                print("Message received from the server \n " + receivedString);
            }
        }
        catch(Exception e) {
            print(e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        _cancelFlag = true;
    }

    void OnGUI()
    {
        string watchersComponentsData = OpenSkyDataHandler.Data.GetJsonWatchersComponentsData();
        GUI.Box(new Rect(0, 0, Screen.width / 4, Screen.height), "");
        GUI.Label(new Rect(0, 0, Screen.width / 4, Screen.height), watchersComponentsData);
    }
}