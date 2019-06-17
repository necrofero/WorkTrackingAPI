using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using WorkTrackingAPI.Helpers;
using WorkTrackingAPI.Models;

namespace WorkTrackingAPI.Managers
{

    public static class UserManager
    {

        public static User Authenticate(UserLogin user)
        {
            var result = new User();
            var filter = new BsonDocument {
                {"login", user.UserName},
                {"password", user.Password}
            };
            List<BsonDocument> documents = MongoHelper.GetDatabase().GetCollection<BsonDocument>("users").Find(filter).ToList();
            if (documents.Count > 0)
            {
                result.Id = documents[0].GetValue("_id", "").ToString();
                result.Name = documents[0].GetValue("name", "").ToString();
                result.Surname = documents[0].GetValue("surname", "").ToString();
                result.IsAdmin = documents[0].GetValue("admin", false).ToBoolean();
            }
            return result;
        }

    }

}