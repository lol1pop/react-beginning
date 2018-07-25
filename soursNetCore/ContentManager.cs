using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DeskAlerts.Outcoming;
using fastJSON;
using Tables;

namespace DeskAlerts
{
    public enum ePriority
    {
        critical = 1,
        rush,
        def
    }

    public enum eDistributionType
    {
        broadcast,
        ip_range,
        normal
    }

    public enum eEndDevices
    {
        all,
        desktop,
        mobile
    }

    public class IReceivers
    {
        public eDistributionType Distribution { get; set; }
        public eEndDevices Device { get; set; }
    }

    public class BroadcastReceivers : IReceivers { }

    public class IPRangeReceivers : IReceivers
    {
        public IPAddressRange Range { get; set; }
    }

    public class NormalReceivers : IReceivers
    {
        public List<int> UserList { get; set; }
        public List<int> GroupList { get; set; }
    }

    public class ContentNode
    {
        public string ID { get; set; }
        public long AlertID { get; set; }
        public ePriority Priority { get; set; }
        public IReceivers Receivers { get; set; }
    }

    public class AlertCache
    {
        public string title { get; set; }
        public string content { get; set; }
    }


    public class ContentManager
    {
        private readonly List<ContentNode> contentQueue;
        private readonly DeviceManager deviceManager;
        private readonly DispatchManager dispatchManager;

        private Alert TMP;
        private readonly UserManager userManager;

        public ContentManager(UserManager UserManager, DeviceManager DeviceManager, DispatchManager DispatchManager) {
            contentQueue = new List<ContentNode>();
            userManager = UserManager;
            deviceManager = DeviceManager;
            dispatchManager = DispatchManager;
        }

        public async void ContentManagerAsync() {
            await shipment();
        }

        public async Task shipment() {
            var send = contentQueue.FirstOrDefault();
            contentQueue.RemoveAt(0);
            dispatchManager.Send(new DispathArgs {
                Data = serializationData(send),
                Devices = DeviceList(send.Receivers)
            });
        }

        public List<UserDevice> DeviceList(IReceivers Receivers) {
            if (Receivers.Distribution == eDistributionType.normal) {
                var NReceivers = Receivers as NormalReceivers;
                return userManager.GetSendDevices(NReceivers.UserList, NReceivers.GroupList);
            }

            if (Receivers.Distribution == eDistributionType.ip_range) {
                var IPReceivers = Receivers as IPRangeReceivers;
                return deviceManager.GetDeviceByRange(IPReceivers.Range);
            }

            return null;
        }

        public string serializationData(ContentNode Content) {
            var alertData = TMP; //СacheManager.GetAlertToId(Content.AlertID);
            var data = JSON.ToJSON(new stAlertOutcome {
                id = Content.ID,
                //priority = Content.Priority,
                type = alertType.native, //TODO:
                title = alertData.Title, //.title,
                content = alertData.Contents
            });
            return data;
        }

        public void sendContent(List<Dispatch> notReceived, int userID) {
            foreach (var alert in notReceived) {
                var send = new ContentNode {
                    ID = alert.SendIndex,
                    AlertID = alert.AlertID,
                    Priority = ePriority.def, //TODO: get from alert type
                    Receivers = new NormalReceivers {
                        Distribution = eDistributionType.normal,
                        Device = eEndDevices.all, //TODO: get from DB
                        GroupList = null,
                        UserList = new List<int> {userID}
                    }
                };

                TMP = DBManager.alert.GetAlert(alert.AlertID);
                contentQueue.Add(send);
                shipment();
            }
        }

        //Если повторение старого, то надо сделать поиск ИДа алерта
        public void sendContent(Alert alert, List<int> Users, List<int> Groups, eEndDevices devices = eEndDevices.all,
            ePriority priority = ePriority.def) {
            var alertID = DBManager.alert.AddAlertProcedure(alert);
            var nowDate = DateTime.Now;
            var SendIndex =
                $"{nowDate.Date.Year}{nowDate.Date.Month}{nowDate.Date.Day}{nowDate.TimeOfDay.Hours}{nowDate.TimeOfDay.Minutes}{nowDate.TimeOfDay.Seconds}{nowDate.TimeOfDay.Milliseconds}{alertID}{alert.CreaterID}";
            DBManager.alert.SetSendIndex(alertID, SendIndex, alert.CreaterID); /// добавть Sender_id

            var send = new ContentNode {
                ID = SendIndex,
                AlertID = alertID,
                Priority = priority,
                Receivers = new NormalReceivers {
                    Distribution = eDistributionType.normal,
                    Device = devices,
                    GroupList = Groups,
                    UserList = Users
                }
            };

            DBManager.alert.DispatchAlertUser(SendIndex, Users);
            //DBManager.alert.DispatchAlertUser(SendIndex, Groups);

            if (priority != ePriority.def) {
                var index = contentQueue.FindIndex(el => el.Priority > send.Priority);
                contentQueue.Insert(index, send);
            }
            else {
                contentQueue.Add(send);
            }

            TMP = alert;
            shipment();
            //СacheManager.PutAlert(alertID, alert);
        }

        public void sendContent(Alert alert, IPAddress lower, IPAddress upper, eEndDevices devices = eEndDevices.all,
            ePriority priority = ePriority.def) {
            var alertID = DBManager.alert.AddAlertProcedure(alert);
            var nowDate = DateTime.Now;
            var SendIndex =
                $"{nowDate.Date.Year}{nowDate.Date.Month}{nowDate.Date.Day}{nowDate.TimeOfDay.Hours}{nowDate.TimeOfDay.Minutes}{nowDate.TimeOfDay.Seconds}{nowDate.TimeOfDay.Milliseconds}{alertID}{alert.CreaterID}";
            DBManager.alert.SetSendIndex(alertID, SendIndex, alert.CreaterID);

            var send = new ContentNode {
                ID = SendIndex,
                AlertID = alertID,
                Priority = priority,
                Receivers = new IPRangeReceivers {
                    Distribution = eDistributionType.ip_range,
                    Device = devices,
                    Range = new IPAddressRange(lower, upper)
                }
            };

            //DBManager.alert.DispatchAlertUser(SendIndex, Users);

            if (priority != ePriority.def) {
                var index = contentQueue.FindIndex(el => el.Priority > send.Priority);
                contentQueue.Insert(index, send);
            }
            else {
                contentQueue.Add(send);
            }

            // СacheManager.PutAlert(alertID, alert);
        }

        public void sendContent(Alert alert, eEndDevices devices = eEndDevices.all,
            ePriority priority = ePriority.def) {
            var alertID = DBManager.alert.AddAlertProcedure(alert);
            var nowDate = DateTime.Now;
            var SendIndex =
                $"{nowDate.Date.Year}{nowDate.Date.Month}{nowDate.Date.Day}{nowDate.TimeOfDay.Hours}{nowDate.TimeOfDay.Minutes}{nowDate.TimeOfDay.Seconds}{nowDate.TimeOfDay.Milliseconds}{alertID}{alert.CreaterID}";
            DBManager.alert.SetSendIndex(alertID, SendIndex, alert.CreaterID);

            var send = new ContentNode {
                ID = SendIndex,
                AlertID = alertID,
                Priority = priority,
                Receivers = new BroadcastReceivers {
                    Distribution = eDistributionType.broadcast,
                    Device = devices
                }
            };

            if (priority != ePriority.def) {
                var index = contentQueue.FindIndex(el => el.Priority > send.Priority);
                contentQueue.Insert(index, send);
            }
            else {
                contentQueue.Add(send);
            }

            //СacheManager.PutAlert(alertID, alert);
        }
    }
}