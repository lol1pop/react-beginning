using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Tables;

namespace DeskAlerts
{
    internal class AlertRepository : DBManager
    {
        public enum AlertType
        {
            ALERT = 0,
            WALLPEPER = 1,
            CONFIG = 2,
            RUSH = 3,
            CRITICAL = 4
        }

        public long AddAlertProcedure(Alert alert) {
            if (!(alert != null && alert.IsValide())) {
                Console.WriteLine(
                    $"Invalid parametr '{alert.Type}','{alert.Title}', '{alert.Contents}', '{alert.Creater_id}'"); //Log 
                throw new NullReferenceException();
            }

            try {
                var query = "dbo.alert_insert @type, @title, @contents, @creater_id";
                Connection.Open();
                var output = Connection.QuerySingle<long>(query,
                    new {alert.Type, alert.Title, alert.Contents, alert.Creater_id});
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::AddAlertProcedure::Query: {query}"); //log
                Console.WriteLine($" output alertId: {output}");
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::AddAlertProcedure::Query:"); //log
                Console.WriteLine(ex.Message);
                return new long();
            }
        }

        public List<Alert> GetAllAlert() {
            try {
                var query = "SELECT * FROM dbo.alerts";
                Connection.Open();
                var output = Connection.Query<Alert>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAllAlert::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAllAlert::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<Alert>();
            }
        }

        public List<Alert> GetAllAlert(int limitLine, int? offsetLine = null) {
            try {
                var query = getLimitedQuery("SELECT * FROM dbo.alerts", "alert_id", offsetLine, limitLine);
                Connection.Open();
                var output = Connection.Query<Alert>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAllAlert::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAllAlert::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<Alert>();
            }
        }

        public List<Alert> GetAlertForCreater(int userId) {
            try {
                var query = $"SELECT * FROM dbo.alerts WHERE creater_id = @CreaterID";
                Connection.Open();
                var output = Connection.Query<Alert>(query, new { CreaterID = userId }).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAlertForCreater::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAlertForCreater::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<Alert>();
            }
        }

        public Alert GetAlert(long alertId) {
            try {
                var query = $"SELECT top 1 * FROM dbo.alerts WHERE alert_id = @AlertID";
                Connection.Open();
                var output = Connection.Query<Alert>(query, new { AlertID = alertId }).SingleOrDefault();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAlert::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAlert::Query:"); //log
                Console.WriteLine(ex.Message);
                return new Alert();
            }
        }

        public List<long> GetAlertIDForSender(int userId) {
            try {
                var query = $"SELECT alert_id FROM dbo.alerts WHERE creater_id = @CreaterID";
                Connection.Open();
                var output = Connection.Query<long>(query, new { CreaterID = userId }).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAlertIDForSender::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAlertIDForSender::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<long>();
            }
        }

        //???????? были изменения в таблице 
        public List<int> GetUserForSendIndex(string sendIndex) {
            try {
                var query = $"SELECT user_id FROM dbo.recipient_users WHERE send_index = {sendIndex}";
                Connection.Open();
                var output = Connection.Query<int>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetUserForAlert::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetUserForAlert::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<int>();
            }
        }

        //???????? были изменения в таблице 
        public List<int> GetGroupForSendIndex(string sendIndex) {
            try {
                var query = $"SELECT group_id FROM dbo.recipient_group WHERE send_index = {sendIndex}";
                Connection.Open();
                var output = Connection.Query<int>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAlertIDForSender::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAlertIDForSender::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<int>();
            }
        }

        //???????? были изменения в таблице 
        public List<long> GetAlertIDForUserStatus(int userID, short? state = null) {
            try {
                var query =
                    $"select alert_id from dispatch d join recipient_users ru on d.send_index = ru.send_index where user_id = {userID}";
                if (state != null)
                    query += $" and status = {state}";
                Connection.Open();
                var output = Connection.Query<long>(query).ToList();
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::GetAlertIDForUserStatus::Query: {query}"); //log
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::GetAlertIDForSender::Query:"); //log
                Console.WriteLine(ex.Message);
                return new List<long>();
            }
        }

        public void SetAlertUser(string sendIndex, int userId) {
            try {
                var query = $@"INSERT INTO dbo.recipient_users (send_index ,user_id) 
                                        VALUES('{sendIndex}',{userId})";
                Connection.Open();
                Connection.Query(query);
                Connection.Close();
                Console.WriteLine($" Succsesful AlertRepository::SetAlertUser::Query: {query}"); //log
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($" Invalide AlertRepository::SetAlertUser::Query:"); //log
                Console.WriteLine(ex.Message);
            }
        }

        public void DispatchAlertUser(string sendIndex, List<int> userIds) {
            foreach (var user in userIds)
                SetAlertUser(sendIndex, user);
        }

        [Obsolete("Потом переименовать(перенести)")]
        public void SetSendIndex(long alertId, string sendIndex, int senderId) {
            try {
                var query = $@"INSERT INTO dbo.dispatch (send_index ,alert_id, sender_id) 
                                        VALUES('{sendIndex}',{alertId}, {senderId})";
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
    }
}