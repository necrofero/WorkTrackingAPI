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

        public static List<WorkRecord> GetRecords(string start, string end, string employee)
        {
            var result = new List<WorkRecord>();
            var start_splitted = start.Split('_');
            var end_splitted = end.Split('_');
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Gte("date_time", new DateTime(int.Parse(start_splitted[2]), int.Parse(start_splitted[1]), int.Parse(start_splitted[0]), 0, 0, 0)) &
                         filterBuilder.Lte("date_time", new DateTime(int.Parse(end_splitted[2]), int.Parse(end_splitted[1]), int.Parse(end_splitted[0]), 23, 59, 59));
            if (employee != "_")
            {
                filter = filter & filterBuilder.Eq("employee_id", employee);
            }
            var sortBuilder = Builders<BsonDocument>.Sort;
            var sort = sortBuilder.Ascending("date_time").Descending("employee_name");

            List<BsonDocument> documents = MongoHelper.GetDatabase().GetCollection<BsonDocument>("work_records").Find(filter).Sort(sort).ToList();
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

        public static WorkRecord InsertRecord(WorkRecord record)
        {
            BsonDocument newRecord = new BsonDocument();
            newRecord.Add("employee_id", record.EmployeeId);
            newRecord.Add("employee_name", record.EmployeeName);
            newRecord.Add("date_time", record.DateTime);
            newRecord.Add("type", record.Type);
            newRecord.Add("week", record.Week);
            newRecord.Add("Synced", record.Synced);
            MongoHelper.GetDatabase().GetCollection<BsonDocument>("work_records").InsertOne(newRecord);
            record.Id = newRecord.GetValue("_id").ToString();
            return record;
        }

        public static void DeleteRecord(string recordId)
        {
            var filter = new BsonDocument {
                {"_id", ObjectId.Parse(recordId)}
            };
            MongoHelper.GetDatabase().GetCollection<BsonDocument>("work_records").DeleteOne(filter);
        }

    }

}