namespace DeskAlerts
{
    namespace Incoming
    {
        public enum packetTypeIncoming
        {
            auth,
            state
        }

        public class stIncomingPacket
        {
            public packetTypeIncoming packet_type { get; set; }
        };

        public class stAuthData
        {
            public string Login { get; set; }
            public string Hash { get; set; }
            public eClientType client_type { get; set; }
        }

        public class stSyncState
        {
            public string ID { get; set; }
            public eContentState State { get; set; }
        }
    }

    namespace Outcoming
    {
        public enum packetTypeOutcoming
        {
            auth,
            auth_result,
            alert
        }

        public class stOutcomingPacket
        {
            public packetTypeOutcoming packet_type { get; set; }
        };

        public class stAuthCreate : stOutcomingPacket
        {
            //public packetTypeOutcoming packet_type { get; set; }
            public string key { get; set; }
            public stAuthCreate()
            {
                packet_type = packetTypeOutcoming.auth;
            }
        }

        public class stAuthRespose : stOutcomingPacket
        {
            public AuthStatus status { get; set; }
            public string response { get; set; }

            public stAuthRespose()
            {
                packet_type = packetTypeOutcoming.auth_result;
            }
        }

        public class stAlertOutcome : stOutcomingPacket
        {
            public string id { get; set; }//List<string>
            public alertType type { get; set; }
            public byte priority { get; set; }
            public string title { get; set; }
            public string content { get; set; }

            public stAlertOutcome()
            {
                packet_type = packetTypeOutcoming.alert;
            }
        }
    }

    public enum alertType
    {
        native
    }

    public enum eContentState
    {
        received = 1,
        readed,
        deleted
    };

    public enum AuthStatus
    {
        OK,
        FAIL
    }
}
