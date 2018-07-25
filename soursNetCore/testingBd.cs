using System.Collections.Generic;
using Tables;

namespace DeskAlerts
{
    internal class testingBd
    {
        public static void tableSetPolicy() {
            //Имя для политики нужно сделать уникальным
            var listPolicy = new List<Policy> {
                new Policy {Name = "SYSADMIN", Rules = 3, View_rights = true, Send_rights = true},
                new Policy {Name = "SHAHA", Rules = 2, Send_rights = true},
                new Policy {Name = "USER", Rules = 1, View_rights = true}
            };

            DBManager.policy.CreatePolicy(new Policy());
            var id = DBManager.policy.CreatePolicyOutId(listPolicy[0]);
            listPolicy.ForEach(el => {
                var idU = DBManager.policy.CreatePolicy(el);
            });
        }

        public static void tableGetPolicy() {
            var policy = DBManager.policy.GetPolicy(1);
            var listSetPolicys = DBManager.policy.GetSetPolicy(1);
        }


        public static void tableSettingPolicy() {
            var listSetPolicy = new List<SettingPolicy> {
                new SettingPolicy {Create = false, Delete = false, View = false},
                new SettingPolicy {
                    Create = false,
                    Edit = false,
                    Delete = false,
                    Send = false,
                    View = false,
                    Stop = false
                },
                new SettingPolicy {Create = true, Edit = true, Delete = true, Send = true, View = true, Stop = true}
            };
            var i = 1;
            listSetPolicy.ForEach(el => {
                el.Policy_id = 1;
                el.Type_control = (byte)i;
                DBManager.policy.SetSettingPolicy(el);
                i += 3;
            });
        }

        public static void tablesPolicyRecipient() {
            var listIdGroup = new List<int> {1, 2, 3};
            var listIdUser = new List<int> {50, 55, 58, 65};
            DBManager.policy.SetPolicyRecipient(1, listIdGroup, (PolicyRepository.Recipient) 2);
            DBManager.policy.SetPolicyRecipient(1, listIdUser, (PolicyRepository.Recipient) 1);
        }


        public void tableLogin() { }

        public void tableGroup() { }


        public void tableAlert() { }


        public void tablerecepUser() { }


        public void tableDispatch() { }
    }
}