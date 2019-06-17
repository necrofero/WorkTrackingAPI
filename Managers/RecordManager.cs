using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using WorkTrackingAPI.Helpers;
using WorkTrackingAPI.Models;

namespace WorkTrackingAPI.Managers
{

    public static class RecordManager
    {

        public static List<WorkRecord> GetRecords(string start, string end)
        {
            var result = new List<WorkRecord>();
            var filter = new BsonDocument {
            };
            List<BsonDocument> documents = MongoHelper.GetDatabase().GetCollection<BsonDocument>("work_records").Find(filter).ToList();
            foreach (var document in documents)
            {
                var record = new WorkRecord();
                record.Id = document.GetValue("_id", "").ToString();
                record.EmployeeId = document.GetValue("employee_id", "").ToString();
                record.EmployeeName = document.GetValue("employee_name", "").ToString();
                record.DateTime = document.GetValue("date_time").ToUniversalTime();
                record.Type = document.GetValue("type", "").ToString();
                record.Week = document.GetValue("week", 0).ToInt32();
                record.Synced = document.GetValue("synced", false).ToBoolean();
                result.Add(record);
            }
            return result;
        }

    }

}