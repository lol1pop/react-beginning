using System;
using System.Collections.Generic;
using Dapper;
using Tables;

namespace DeskAlerts
{
    internal class GroupRepository : DBManager
    {
        public Dictionary<int, Groups> GetAllGroups() {
            Connection.Open();
            var query =
                @"SELECT g.group_id , g.name, u.user_id  FROM groups AS g INNER JOIN users_groups AS u ON  u.group_id = g.group_id";
            Console.WriteLine($"LoginRepository::GetUserPacket::Query: {query}");
            var orderDictionary = new Dictionary<int, Groups>();
            Connection.Query<Groups, UsersGroup, Groups>(
                query,
                (group, userGroup) => {
                    Groups groipsEntry;

                    if (!orderDictionary.TryGetValue(group.GroupID, out groipsEntry)) {
                        groipsEntry = group;
                        groipsEntry.UsersGroups = new List<UsersGroup>();
                        orderDictionary.Add(groipsEntry.GroupID, groipsEntry);
                    }

                    groipsEntry.UsersGroups.Add(userGroup);
                    return groipsEntry;
                }, splitOn: "UserID");
            Connection.Close();

            return orderDictionary;
        }
    }
}