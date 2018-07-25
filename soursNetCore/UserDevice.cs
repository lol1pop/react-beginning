using System;
using System.Text;
using DeskAlerts.Incoming;
using DeskAlerts.Outcoming;
using fastJSON;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DeskAlerts
{
    public enum eClientType
    {
        DESK_WIN = 1,
        DESK_MAC,
        MOBILE_IOS,
        MOBILE_ANDROID
    }

    public class WebSocketPacket
    {
        public packetTypeIncoming packet_type { get; set; }
        public string data { get; set; }
    }

    public class UserDevice : WebSocketBehavior
    {
        public delegate void ClientConnect(UserDevice sender);

        public delegate void ClientDisconnect(UserDevice sender);

        public delegate void IncomingCallback(UserDevice sender, WebSocketPacket data);

        public eClientType ClientType;

        public UserDevice() {
            IsAuth = false;
        }

        public int UserID { get; private set; }
        public bool IsAuth { get; private set; }
        public event IncomingCallback onIncoming;
        public event ClientConnect onClientConnect;
        public event ClientDisconnect onClientDisconnected;

        private void OnAuthentication(string data) {
            if (Context.IsSecureConnection == false) {
                //decode data
            }

            try {
                var authData = JSON.ToObject<stAuthData>(data);
                var UserID = DBManager.login.GetUserId(authData.Login, authData.Hash);
                if (UserID == 0) {
                    SendData(JSON.ToJSON(new stAuthRespose {
                        status = AuthStatus.FAIL,
                        response = "Invalid auth data!"
                    }));
                    Disconnect(CloseStatusCode.InvalidData, "Invalid auth data!");
                }
                else {
                    this.UserID = UserID;
                    IsAuth = true;
                    ClientType = authData.client_type;
                    SendData(JSON.ToJSON(new stAuthRespose {status = AuthStatus.OK, response = ""}));
                    onClientConnect?.Invoke(this);
                }
            }
            catch (Exception ex) {
                /*Logger : $"Exception from {ex.Source}: {ex.Message}\nIncoming data:{data}\nStack trace:\n{ex.StackTrace}" */
            }
        }

        protected override void OnMessage(MessageEventArgs e) {
            var data = e.IsText ? e.Data : Encoding.UTF8.GetString(e.RawData);
            try {
                var packet = JSON.ToObject<stIncomingPacket>(data);
                if (packetTypeIncoming.auth == packet.packet_type && IsAuth == false)
                    OnAuthentication(data);
                else if (IsAuth)
                    onIncoming?.Invoke(this, new WebSocketPacket {packet_type = packet.packet_type, data = data});
                /*System.Threading.ThreadPool.QueueUserWorkItem(
                    state => onIncoming?.Invoke(this, new WebSocketPacket() { packet_type = packet.packet_type, data = data });
                    );*/
            }
            catch (Exception ex) {
                /*Logger : $"Exception from {ex.Source}: {ex.Message}\nIncoming data:{data}\nStack trace:\n{ex.StackTrace}" */
            }
        }

        protected override void OnOpen() {
            /*RSA generate keys*/
            SendData(JSON.ToJSON(new stAuthCreate {key = ""}));
        }

        protected override void OnClose(CloseEventArgs e) {
            onClientDisconnected?.Invoke(this);
        }

        public void Disconnect(CloseStatusCode code = CloseStatusCode.Normal, string reason = null) {
            //Context.WebSocket.CloseAsync(code, reason ?? string.Empty);
            Context.WebSocket.Close(code, reason ?? string.Empty);
        }

        public void SendData(string data) {
            try {
                SendAsync(data, completed => {
                    /* if completed == false =>  Logger : failed send data*/
                });
            }
            catch (Exception ex) {
                Send(data);
            }
        }

        public void SendData(string data, string id) {
            try {
                Sessions.SendToAsync(data, id, completed => {
                    /* if completed == false =>  Logger : failed send data*/
                });
            }
            catch (Exception ex) {
                /*Logger : $"Send to client exception ({id}): {ex.Message}"*/
            }
        }
    }
}