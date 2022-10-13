using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                        OpenSkyDataHandler.Data.SetJsonWatchersComponentsData(pieces[1]);

                        UnityThreadDispatcher.wkr.AddJob(() => {
                            OpenSkyDataHandler.Data.RefreshAllWatchersComponents();
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
                    string message = OpenSkyDataHandler.Data.GetJsonWatchersComponentsData();

                    UnityThreadDispatcher.wkr.AddJob(() => {
                        OpenSkyDataHandler.Data.SetAllComponentsData();
                    }); 
                    while (UnityThreadDispatcher.jobs.Count > 0) 
                        Thread.Sleep(10);

                    message = OpenSkyDataHandler.Data.GetJsonWatchersComponentsData();
                    message = string.Format("{0};send;{1}", message.Length, message);

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