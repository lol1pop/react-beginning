using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Tables;

namespace DeskAlerts
{
    internal class Сondition : DBManager
    {
        public List<Dispatch> GetUnsentAlerts(int userID) // на удаление 
        {
            try {
                var query = $@"dbo.get_not_received_alerts @UserID";
                Connection.Open();
                var output = Connection.Query<Dispatch>(query, new {userID}).ToList();
                Connection.Close();
                Console.WriteLine(
                    $" Succsesful Сonditio::GetUnsentAlerts::Query:{query} -> user_id_conect:{userID}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetUnsentAlerts::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<Dispatch>();
            }
        }

        public void SetState(int userID, string sendIndex, short state) // на удаление 
        {
            var query = $@"INSERT dbo.user_alert_state values({userID},'{sendIndex}',{state})";
            Connection.Open();
            Connection.Query(query);
            Connection.Close();
            Console.WriteLine($" Succsesful Сonditio::SetState::Query: {query}"); //log
        }

        public void SetDispatch(long AlertID, string sendIndex, int senderID) {
            try {
                var query = $@"INSERT INTO dbo.dispatch (send_index ,alert_id, sender_id) 
                                        VALUES('{sendIndex}',{AlertID}, {senderID})";
                Connection.Open();
                Connection.Query(query);
                Connection.Close();
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::SetSendIndex::Query:"); //log
                Console.WriteLine(ex.Message);
            }
        }

        public void UpdateStatusContent(int userID, string sendIndex, short state) {
            try {
                var query =
                    $"update recipient_users set status = {state} where user_id = {userID} and send_index = '{sendIndex}'";
                Connection.Open();
                Connection.Query<Dispatch>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::UpdataStatusContent::Query: {query}"); //log
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::UpdataStatusContent::Query:"); //log
            }
        }

        //+-ДУБЛЬ С ТАБЛИЦЕ АЛЕРТОВ , ПОКАК ТАК НАДО
        public List<Dispatch> GetAlerDispatchStatus(int userID, short? state = null) {
            try {
                var query =
                    $"select * from dispatch d join recipient_users ru on d.send_index = ru.send_index where user_id = {userID}";
                if (state != null)
                    query += $" and status = {state}";
                Connection.Open();
                var output = Connection.Query<Dispatch>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAlertIDForUserStatus::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAlertIDForSender::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<Dispatch>();
            }
        }

        public List<Dispatch> GetUnsentAlertsT(int userID) {
            var outpute = GetAlerDispatchStatus(userID, 0);
            return outpute;
        }

        public List<Dispatch> GetGottenAlerts(int userID) {
            var outpute = GetAlerDispatchStatus(userID, 1);
            return outpute;
        }

        public List<Dispatch> GetReadAlerts(int userID) {
            var outpute = GetAlerDispatchStatus(userID, 2);
            return outpute;
        }
    }
}