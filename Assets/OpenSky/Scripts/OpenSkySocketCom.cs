using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class OpenSkySocketCom
{
    private enum SocketCommand {SendData, GetData};
    public bool isConnected {
        get { return (_connectionThread != null && !_connectionThread.IsAlive); }
    }
    
    private Thread _connectionThread;
    private volatile bool _cancelFlag;

    private static OpenSkySocketCom _instance;

    public static OpenSkySocketCom Socket {
        get {
            if(_instance == null) 
                _instance = new OpenSkySocketCom();

            return _instance;
        }
        set {}
    }

    private OpenSkySocketCom() {}

    public void StartCommunication() {
        _cancelFlag = false;
        _connectionThread = new Thread(_RefreshServerGameData);
        _connectionThread.Start();
    }

    private void _RefreshServerGameData() {
        try {
            UdpClient client = new UdpClient(8133);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 8133);
            client.Connect("127.0.0.1", 8888);
            
            SocketCommand _command = SocketCommand.GetData;

            while(!_cancelFlag) {

                if(_command == SocketCommand.GetData) 
                {
                    string message = "0;get;";
                    byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                    client.Send(sendBytes, sendBytes.Length);
                    
                    byte[] receiveBytes = client.Receive(ref remoteEndPoint);
                    string receivedString = Encoding.ASCII.GetString(receiveBytes);

                    string[] pieces = receivedString.Split(new[] { ';' }, 2);
                    if(Int32.Parse(pieces[0]) == pieces[1].Length)
                    {
                        DataPackage<ClientData> dataPackage = JsonUtility.FromJson<DataPackage<ClientData>>(pieces[1]);
                        OpenSkyDataHandler.Data.SetAnotherClientsData(dataPackage);

                        UnityThreadDispatcher.wkr.AddJob(() => {
                            OpenSkyDataHandler.Data.UpdateAnotherClientsByData();
                        });
                        while (UnityThreadDispatcher.jobs.Count > 0) 
                            Thread.Sleep(10);

                        _command = SocketCommand.SendData;
                    }
                    else
                    {
                        OpenSkyLogger.Info(string.Format("Package ignored by data lost. Excepceted {0} bytes but received {1}", Int32.Parse(pieces[0]), pieces[1].Length));
                        _command = SocketCommand.GetData;
                    }
                }
                else if(_command == SocketCommand.SendData) 
                {
                    UnityThreadDispatcher.wkr.AddJob(() => {
                        OpenSkyDataHandler.Data.SetOwnClientData();
                    }); 
                    while (UnityThreadDispatcher.jobs.Count > 0) 
                        Thread.Sleep(10);

                    DataPackage<ClientData> dataPackage = new DataPackage<ClientData>();
                    ClientData[] clientData = { OpenSkyDataHandler.Data.GetOwnClientData() };

                    dataPackage.clientsData = clientData;
                    string message = UnityEngine.JsonUtility.ToJson(dataPackage);
                    message = string.Format("{0};set;{1}", message.Length, message);

                    byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                    client.Send(sendBytes, sendBytes.Length);
                    
                    byte[] receiveBytes = client.Receive(ref remoteEndPoint);
                    string receivedString = Encoding.ASCII.GetString(receiveBytes);

                    _command = SocketCommand.GetData;
                }
            }
        }
        catch(Exception exc) {
            OpenSkyClient.Client.Disconnect(exc.Message);
        }
    }

    public void StopCommunication()
    {
        _cancelFlag = true;
    }
}