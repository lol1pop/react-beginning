using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Tables;

namespace DeskAlerts
{
    internal class LoginsRepository : DBManager
    {
        public enum UserRules
        {
            USER = 0,
            PUBLISHER = 1,
            ADMIN = 2
        }

        [Obsolete("ОБЕРНУТЬ В ОФСЕТТ")] //!!!
        public Logins GetOneLogins() {
            try {
                Connection.Open();
                var output = Connection.QuerySingle<Logins>(@"Select TOP 1 * from users");
                Console.WriteLine(
                    $"Succsseful LoginsRepository::GetOneLogins::QuerySingle<Logins>::Select TOP 1 * from users");
                Connection.Close();
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($"Invalide LoginsRepository::GetOneLogins::Query"); //log
                return new Logins();
            }
        }

        public Logins CheckOneLogins(string login, string hash) {
            try {
                Connection.Open();
                var output =
                    Connection.Query<Logins>(
                        $"Select * from users where login = @Login and hash = '{hash}'", new { Login = login }).SingleOrDefault();
                Connection.Close();
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($"Invalide parametrs");
                return null;
            }
        }

        public List<Logins> GetAllLogins() {
            try {
                Connection.Open();
                var output = Connection.Query<Logins>(getLimitedQuery(@"Select * from users")).ToList();
                Console.WriteLine($"Succsseful LoginsRepository::GetOneLogins::GetAllLogins::Select * from users");
                Connection.Close();
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($"Invalide LoginsRepository::GetAllLogins::QuerySingle"); //log
                return new List<Logins>();
            }
        }

        public int GetUserId(string login, string hash) {
            try {
                Connection.Open();
                var query = $"Select user_id from users WHERE login = @Login and hash = '{hash}'";
                var output = Connection.ExecuteScalar<int>(query, new { Login = login });
                Console.WriteLine($"Succsseful LoginsRepository::GetUserId::Query:{query}"); //log
                Connection.Close();
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($"Invalide LoginsRepository::GetUserId::QuerySingle"); //log
                return new int();
            }
        }

        public int GetUserIDtoLogin(string login) {
            try {
                Connection.Open();
                var output = Connection.Query<int>($"Select user_id from users WHERE login = @Login", new { Login = login })
                    .SingleOrDefault();
                Console.WriteLine(
                    $"Succsseful LoginsRepository::GetUserIDtoLogin::QuerySingle<Logins>::Select TOP 1 * from users"); //log
                Connection.Close();
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($"Invalide LoginsRepository::GetUserIDtoLogin::QuerySingle"); //log
                return new int();
            }
        }

        public List<Logins> GetUserPacket(int count, int offset = 0) {
            try {
                var query = getLimitedQuery("SELECT * FROM users", "user_id", offset, count);
                Connection.Open();
                var output = Connection.Query<Logins>(query).ToList();
                Console.WriteLine($"Succsseful LoginsRepository::GetUserPacket::Query:{query}"); //log
                Connection.Close();
                return output;
            }
            catch (Exception ex) {
                Connection.Close();
                Console.WriteLine($"Invalide LoginsRepository::GetUserPacket::Query"); //log
                return new List<Logins>();
            }
        }

        public void AddUserProcedure(string login, string hash, string email = null, long? phone = null,
            UserRules rules = UserRules.USER) {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(hash))
                throw new Exception($"Invalid parametr '{login}', '{hash}'"); //Log("Invalid parametr '{login}', '{hash}')
            var query = "dbo.login_insert @login, @hash, @email, @rules, @phone";
            //List<Logins> user = new List<Logins>();
            //user.Add(new Logins { Login = login, Hash = hash});
            Connection.Open();
            var output = Connection.Query("dbo.login_insert @login, @hash, @email, @rules, @phone",
                new {login, hash, email, phone, Rules = (byte) rules});
            Connection.Close();
            if (output.Count() == 1)
                Console.WriteLine($" fail such a name already exists: {login}"); //log
            Console.WriteLine($"LoginRepository::AddUser::Query: {query}"); //log
        }
    }
}