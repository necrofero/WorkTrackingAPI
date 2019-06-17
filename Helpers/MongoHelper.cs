using System;
using MongoDB.Driver;

namespace WorkTrackingAPI.Helpers
{

    public static class MongoHelper
    {

        private static string CONNECTION = "mongodb+srv://sivsa:sivsa@mia-practicas-dgrvh.mongodb.net/test?retryWrites=true&w=majority";
        private static string DATABASE = "WorkTracking";

        public static IMongoDatabase GetDatabase()
        {
            var client = new MongoClient(CONNECTION);
            return client.GetDatabase(DATABASE);
        }

    }

}