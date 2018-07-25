using System;
using System.Collections.Generic;

namespace Tables
{
    public class Logins
    {
        public int UserID;

        public int User_id {
            get => UserID;
            set => UserID = value;
        }

        public string Hash { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public byte Rules { get; set; }
        public long Phone { get; set; }

        public override string ToString() {
            return Login;
        }
    }

    public class Groups
    {
        public int GroupID;

        public int Group_id {
            get => GroupID;
            set => GroupID = value;
        }

        public string Name { get; set; }

        public List<UsersGroup> UsersGroups { get; set; }
    }

    public class UsersGroup
    {
        public int GroupID;

        public int UserID;

        public int User_id {
            get => UserID;
            set => UserID = value;
        }

        public int Group_id {
            get => GroupID;
            set => GroupID = value;
        }
    }

    public class Alert
    {
        public long AlertID;
        public int CreaterID;

        public long Alert_id {
            get => AlertID;
            set => AlertID = value;
        }

        public short Type { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }

        public int Creater_id {
            get => CreaterID;
            set => CreaterID = value;
        }

        public DateTime Date_create { get; set; }

        public bool IsValide() {
            return !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Contents) && Creater_id > 0;
        }
    }

    public class Dispatch
    {
        public long AlertID;
        public int SenderID;
        public string SendIndex;

        public string Send_index {
            get => SendIndex;
            set => SendIndex = value;
        }

        public long Alert_id {
            get => AlertID;
            set => AlertID = value;
        }

        public int Sender_id {
            get => SenderID;
            set => SenderID = value;
        }

        public DateTime Date_send { get; set; }
    }

    public class RecUser
    {
        public string SendIndex;
        public int UserID;

        public string Send_index {
            get => SendIndex;
            set => SendIndex = value;
        }

        public int User_id {
            get => UserID;
            set => UserID = value;
        }

        public short status { get; set; }
        public DateTime Date_change { get; set; }
    }

    public class RecGroup
    {
        public int GroupID;
        public string SendIndex;

        public string Send_index {
            get => SendIndex;
            set => SendIndex = value;
        }

        public int Group_id {
            get => GroupID;
            set => GroupID = value;
        }
    }

    public class UserAlertState
    {
        public string SendIndex;
        public int UserID;

        public string Send_index {
            get => SendIndex;
            set => SendIndex = value;
        }

        public int User_id {
            get => UserID;
            set => UserID = value;
        }

        public short state { get; set; }
    }

    public class Policy
    {
        public int PolicyID;
        public bool SendRights;
        public bool ViewRights;

        public int Policy_id {
            get => PolicyID;
            set => PolicyID = value;
        }

        public string Name { get; set; }

        public byte Rules { get; set; }

        public bool View_rights {
            get => ViewRights;
            set => ViewRights = value;
        }

        public bool Send_rights {
            get => SendRights;
            set => SendRights = value;
        }
    }

    public class SettingPolicy
    {
        public int PolicyID;
        public byte TypeControl;

        public int Policy_id {
            get => PolicyID;
            set => PolicyID = value;
        }

        public byte Type_control {
            get => TypeControl;
            set => TypeControl = value;
        }

        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Send { get; set; }
        public bool View { get; set; }
        public bool Stop { get; set; }
    }

    public class PolicyRecipientUser
    {
        public int PolicyID;
        public int UserID;

        public int Policy_id {
            get => PolicyID;
            set => PolicyID = value;
        }

        public int User_id {
            get => UserID;
            set => UserID = value;
        }
    }

    public class PolicyRecipientGroup
    {
        public int GroupID;
        public int PolicyID;

        public int Policy_id {
            get => PolicyID;
            set => PolicyID = value;
        }

        public int Group_id {
            get => GroupID;
            set => GroupID = value;
        }
    }

    public class PolicyRecipientPC
    {
        public int PCID;
        public int PolicyID;

        public int Policy_id {
            get => PolicyID;
            set => PolicyID = value;
        }

        public int PC_id {
            get => PCID;
            set => PCID = value;
        }
    }

    public class PolicyRecipientAD
    {
        public int ADID;
        public int PolicyID;

        public int Policy_id {
            get => PolicyID;
            set => PolicyID = value;
        }

        public int AD_id {
            get => ADID;
            set => ADID = value;
        }
    }

    //public class TPolicyRecipient
    //{
    //    protected int id;
    //    public int Policy_id {
    //        get;
    //        set;
    //    }
    //}

    //public class UserPolicy : TPolicyRecipient
    //{
    //    public int User_id {
    //        get => id;
    //        set => id = value;
    //    }
    //}
}