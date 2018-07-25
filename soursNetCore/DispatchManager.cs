using System;
using System.Collections.Generic;
using System.Threading;
using WebSocketSharp.Server;

namespace DeskAlerts
{
    public class DispathArgs
    {
        public string Data { get; set; }
        public List<UserDevice> Devices { get; set; }
    }

    public class DispatchManager : IDisposable
    {
        private readonly DeviceManager DeviceManager;
        private readonly WebSocketSessionManager sessions;
        private readonly WebSocketServer webSocket;

        public DispatchManager(DeviceManager DeviceManager) {
            webSocket = new WebSocketServer(81);
            webSocket.AddWebSocketService("/", DeviceManager.OnDeviceConnect);
            webSocket.Start();
            sessions = webSocket.WebSocketServices["/"].Sessions;
            this.DeviceManager = DeviceManager;
        }

        public void Dispose() {
            if (webSocket.IsListening)
                webSocket.Stop();
        }

        internal void send(DispathArgs data) {
            /*
             * Тут должен быть планировщик нагрузки и т.д.
             */
            if (data.Devices == null) {
                //broadcast
                sessions.BroadcastAsync(data.Data, () => {
                    /* action */
                }); //Эта хуйня отправит и тем кто ещё не прошёл аутентификацию
                foreach (var device in DeviceManager.DeviceList)
                    if (device.IsAuth)
                        device.SendData(data.Data);
            }
            else {
                foreach (var device in data.Devices) device?.SendData(data.Data);
            }
        }

        private void sendAsync(DispathArgs data) {
            /*
             * Тут должен быть планировщик нагрузки и т.д.
             */

            //Заюзать Task?
            ThreadPool.QueueUserWorkItem(
                state => send(data)
            );
        }

        public void Send(DispathArgs data) {
            if (data == null || data.Data == null)
                throw new ArgumentNullException("error"); //TODO:
            if (webSocket.IsListening != true)
                throw new Exception("Server is not running"); //TODO:
            sendAsync(data);
        }
    }
}