using System;
using System.Collections.Generic;
using System.Linq;
using DeskAlerts.Incoming;
using fastJSON;

namespace DeskAlerts
{
    public class User
    {
        public readonly int ID;
        private List<object> Groups;

        public User(UserDevice Device) {
            Groups = new List<object>();
            Devices = new List<UserDevice>();
            ID = Device.UserID;
            AddDevice(Device);
            //Groups = DBManager.Groups.GetByUserID(ID);
        }

        public List<UserDevice> Devices { get; }

        private void OnMessageHandle(UserDevice sender, WebSocketPacket data) {
            if (data.packet_type == packetTypeIncoming.state) {
                var stateData = JSON.ToObject<stSyncState>(data.data);
                DBManager.state.UpdateStatusContent(ID, stateData.ID, (short) stateData.State);
            }
        }

        public void AddDevice(UserDevice Device) {
            if (Device.IsAuth)
                if (Devices.Exists(x => x.ID == Device.ID) == false) {
                    Device.onIncoming += OnMessageHandle;
                    Devices.Add(Device);
                }
        }

        public void RemoveDevice(UserDevice Device) {
            if (Device.IsAuth)
                Devices.Remove(Device);
        }
    }

    public class UserManager
    {
        private readonly List<User> UsersOnline;
        //private List<object> Groups;

        public UserManager() {
            UsersOnline = new List<User>();
            //Groups = new List<object>();
        }

        public void UserDeviceDisconnected(UserDevice Device) {
            try {
                var conn = UsersOnline.Single(usr => usr.ID == Device.UserID);
                conn.RemoveDevice(Device);
                if (conn.Devices.Count == 0)
                    UsersOnline.Remove(conn);
            }
            catch (Exception ex) {
                ;
            }
        }

        public void UserDeviceConnected(UserDevice Device) {
            try {
                var conn = UsersOnline.Single(usr => usr.ID == Device.UserID);
                conn.AddDevice(Device);
            }
            catch (InvalidOperationException ex) {
                UsersOnline.Add(new User(Device));
                //TODO: get user groups
            }

            Program.content.sendContent(DBManager.state.GetUnsentAlertsT(Device.UserID), Device.UserID);
        }

        public List<UserDevice> GetSendDevices(HashSet<int> users) {
            var resultData = new List<UserDevice>();
            foreach (var id in users)
                try {
                    var user = UsersOnline.Single(x => x.ID == id);
                    resultData.AddRange(user.Devices);
                }
                catch (Exception) {
                    ; //TODO
                }

            return resultData;
        }

        public List<UserDevice> GetSendDevices(List<int> users, List<int> groups = null) {
            HashSet<int> resultData;
            if (users != null)
                resultData = new HashSet<int>(users);
            else
                resultData = new HashSet<int>();
            /*
            if (groups != null)
            foreach(var group in groups)
            {
                var gr = groups.Single(x => x.ID == group);
                resultData.AddRange(gr.UserList);
            }
            */
            return GetSendDevices(resultData);
        }
    }
}