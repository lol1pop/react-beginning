using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Tables;

namespace DeskAlerts
{
    internal class PolicyRepository : DBManager
    {
        public enum ContolType
        {
            DesktopAlerts = 1,
            Campaigns,
            EmailAlerts,
            SmsAlerts,
            Surveys,
            Users,
            Groups,
            MessageTemplates,
            Reports,
            IpGroups,
            Rss,
            Screensavers,
            Wallpapers,
            EmergencyAlerts,
            ColorCodes
        }

        public enum Recipient
        {
            policy_recipient_user = 1,
            policy_recipient_group,
            policy_recipient_pc,
            policy_recipient_ad
        }

        public enum Rules
        {
            User = 1,
            Publisher,
            Sysadmin,
            Admin
        }

        public enum Rights
        {
            Create,
            Edit,
            Delete,
            Send,
            View,
            Stop
        }

        public bool CheckRight(int userId, ContolType type, Rights right) {
            try {
                var query = $@"Select [{right}] from setting_policy
                                join users on user_id = {userId} and users.policy_id = setting_policy.policy_id
                               where type_control = {(byte)type}";
                Connection.Open();
                var outpute = Connection.ExecuteScalar<bool>(query);
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::CheckRight::Query: {query}"); //log
                return outpute;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::CreatePolicy::Query:"); //log
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public int CreatePolicy(Policy policy) {
            try {
                var query = $@"INSERT INTO dbo.policy (name , rules, view_rights, send_rights) 
                                        VALUES(@Name, {policy.Rules}, {(policy.View_rights ? 1 : 0)}, {(policy.Send_rights ? 1 : 0)});
                               SELECT CAST(SCOPE_IDENTITY() as int)";
                Connection.Open();
                var outpute = Connection.Query<int>(query, new {Name = policy.Name}).SingleOrDefault();
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::CreatePolicy::Query: {query}"); //log
                return outpute;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::CreatePolicy::Query:"); //log
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public int CreatePolicyOutId(Policy policy) {
            try {
                var query = $@"DECLARE @id bigint; 
                                INSERT INTO dbo.policy (name, rules, view_rights, send_rights) 
                                     VALUES(@Name, {policy.Rules}, {(policy.View_rights ? 1 : 0)},{(policy.Send_rights ? 1 : 0)}) 
                                SET @id = @@IDENTITY;
	                                 SELECT @id;";
                Connection.Open();
                var outpute = Connection.Query<int>(query, new { Name = policy.Name}).SingleOrDefault();
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::CreatePolicy::Query: {query}"); //log
                return outpute;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::CreatePolicy::Query:"); //log
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        private void InsertSettingPolicy(SettingPolicy setPolicy) {//TODO: Если вызов контролируется и реализация не изменится, то можно убрать подключение и оставить только запрос
            try {
                var query = $@"INSERT INTO [dbo].[setting_policy] ([policy_id]
                                                      ,[type_control]   
                                                      ,[create]
                                                      ,[edit]
                                                      ,[delete]
                                                      ,[send]
                                                      ,[view]
                                                      ,[stop]) 
                                      VALUES({setPolicy.Policy_id}, {setPolicy.Type_control}
                                              , {(setPolicy.Create ? 1 : 0)}, {(setPolicy.Edit ? 1 : 0)}, {(setPolicy.Delete ? 1 : 0)} 
                                              , {(setPolicy.Send ? 1 : 0)}, {(setPolicy.View ? 1 : 0)}, {(setPolicy.Stop ? 1 : 0)})";
                Connection.Open();
                Connection.Query(query);
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::InsertSettingPolicy::Query: {query}"); //log
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::InsertSettingPolicy::Query:"); //log
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateSettingPolicy(SettingPolicy setPolicy) {//TODO: Если вызов контролируется и реализация не изменится, то можно убрать подключение и оставить только запрос
            try {
                var query = $@"UPDATE [dbo].[setting_policy]
                               SET [create] = {(setPolicy.Create ? 1 : 0)},
                                   [edit] = {(setPolicy.Edit ? 1 : 0)},
                                   [delete] = {(setPolicy.Delete ? 1 : 0)},
                                   [send] = {(setPolicy.Send ? 1 : 0)},
                                   [view] = {(setPolicy.View ? 1 : 0)},
                                   [stop] = {(setPolicy.Stop ? 1 : 0)}
                               WHERE [policy_id] = {setPolicy.Policy_id} AND [type_control] = {setPolicy.Type_control}";
                Connection.Open();
                Connection.Query(query);
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::UpdateSettingPolicy::Query: {query}"); //log
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::UpdateSettingPolicy::Query:"); //log
                Console.WriteLine(ex.Message);
            }
        }

        public void SetSettingPolicy(SettingPolicy setPolicy) {
            try {
                var query = $@"Select COUNT(*) from [dbo].[setting_policy]
                                where [policy_id] = {setPolicy.Policy_id} AND [type_control] = {setPolicy.Type_control}";
                Connection.Open();
                var exist = Connection.ExecuteScalar<int>(query) != 0;
                Connection.Close();//TODO: Если вызываемые методы будут использоваться только тут, то можно закрыть подключение после их выполнения, если убрать его там
                Console.WriteLine($" Succsesful PolicyRepository::SetSettingPolicy::Query: {query}"); //log
                if (exist)
                    UpdateSettingPolicy(setPolicy);
                else
                    InsertSettingPolicy(setPolicy);
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::SetSettingPolicy::Query:"); //log
                Console.WriteLine(ex.Message);
            }
        }

        public void DeletetSettingPolicy(int policyId, ContolType type)
        {
            try {
                var query = $@"Delete from [dbo].[setting_policy]
                                where [policy_id] = {policyId} AND [type_control] = {(byte)type}";
                Connection.Open();
                Connection.Query(query);
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::DeletePolicy::Query: {query}"); //log
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::DeletePolicy::Query:"); //log
                Console.WriteLine(ex.Message);
            }
        }

        public void SetPolicyRecipient(int policyId, List<int> idList, Recipient type) {
            try {
                Connection.Open();
                foreach (var id in idList) {
                    var query = $@"INSERT INTO [dbo].[{type}] ([policy_id],[group_id])
                                         VALUES({policyId}, {id})";
                    Connection.Query(query);
                    Console.WriteLine($" Succsesful PolicyRepository::SetSettingPolicy::Query: {query}"); //log
                }
                Connection.Close();
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::SetSettingPolicy::Query:"); //log
                Console.WriteLine(ex.Message);
            }
        }

        public Policy GetPolicy(int policyId) {
            try {
                var query = $@"SELECT * FROM dbo.policy where policy_id = {policyId}";
                Connection.Open();
                var output = Connection.Query<Policy>(query).SingleOrDefault();
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::GetPolicy::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::GetPolicy::Query:"); //log
                Console.WriteLine(ex.Message);
                return new Policy();
            }
        }

        public int GetPolicyIdByName(string name) {
            try {
                var query = $@"SELECT policy_id FROM [dbo].[policy] where name = @Name";
                Connection.Open();
                var output = Connection.Query(query, new { Name = name }).SingleOrDefault();
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::GetPolicyIdToName::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::GetPolicyIdToName::Query:"); //log
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public List<SettingPolicy> GetSetPolicy(int policyId) {
            try {
                var query = $@"SELECT * FROM [dbo].[setting_policy] where policy_id = {policyId}";
                Connection.Open();
                var output = Connection.Query<SettingPolicy>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful PolicyRepository::GetSetPolicy::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::GetSetPolicy::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<SettingPolicy>();
            }
        }

        public List<int> GetPolicyRecipientUser(int policyId) {
            try {
                var query = $@"SELECT * [dbo].[policy_recipient_user] WHERE policy_id = {policyId}";
                Connection.Open();
                var outpute = Connection.Query<int>(query).ToList();
                Console.WriteLine($" Succsesful PolicyRepository::GetPolicyRecipientUser::Query: {query}"); //log

                Connection.Close();
                return outpute;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide PolicyRepository::SetSettingPolicy::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<int>();
            }
        }
    }
}