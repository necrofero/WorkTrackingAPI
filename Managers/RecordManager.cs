using System;
using System.Numerics;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using WorkTrackingAPI.Helpers;
using WorkTrackingAPI.Models;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


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

        public static SyncedData GetSyncedData(int year, int week, string employee)
        {
            var result = new SyncedData();
            var filter = new BsonDocument {
                { "year", year },
                { "week", week },
                { "employee", employee }
            };
            List<BsonDocument> documents = MongoHelper.GetDatabase().GetCollection<BsonDocument>("synced_data").Find(filter).ToList();
            if (documents.Count > 0)
            {
                result.Id = documents[0].GetValue("_id", "").ToString();
                result.Year = documents[0].GetValue("year", 0).ToInt32();
                result.Week = documents[0].GetValue("week", 0).ToInt32();
                result.Employee = documents[0].GetValue("employee", "").ToString();
                result.Tx = documents[0].GetValue("tx", "").ToString();
            }
            return result;
        }

        public static async Task<SyncedData> SetSyncedDataAsync(int year, int week, string employee)
        {
            DateTime dayInWeek = new DateTime(year, 1, 1);
            dayInWeek = dayInWeek.AddDays((week - 1) * 7);
            while (dayInWeek.DayOfWeek != DayOfWeek.Monday)
            {
                dayInWeek = dayInWeek.AddDays(-1);
            }
            DateTime startDate = dayInWeek.Date;
            DateTime endDate = startDate.AddDays(6);
            var start = startDate.Day + "_" + startDate.Month + "_" + startDate.Year;
            var end = endDate.Day + "_" + endDate.Month + "_" + endDate.Year;

            var records = RecordManager.GetRecords(start, end, employee);

            var hash = "";
            var csv = "Fecha,Hora,Tipo,Empleado";
            foreach (var record in records)
            {
                var csv_line = new List<string>();
                record.DateTime = record.DateTime.AddHours(2);
                csv_line.Add(record.DateTime.Day + "/" + record.DateTime.Month + "/" + record.DateTime.Year);
                csv_line.Add(record.DateTime.Hour + ":" + record.DateTime.Minute);
                csv_line.Add(record.Type == "entrance" ? "Entrada" : "Salida");
                csv_line.Add(record.EmployeeName);
                csv = csv + "\n" + String.Join(',', csv_line);
            }
            using (var sha256 = SHA256.Create())
            {
                Byte[] hashedCsv = sha256.ComputeHash(Encoding.UTF8.GetBytes(csv));
                StringBuilder sb = new StringBuilder();
                foreach (Byte b in hashedCsv)
                {
                    sb.Append(b.ToString("x2"));
                }
                hash = sb.ToString();
            }

            var tx = "";
            var web3 = new Web3("https://ropsten.infura.io/v3/7b9974f32d58401f917c63242e1dcb48");
            var account = new Account("9817CF212721DDAB1087FF642C7D52DCC1DDC0CBE1ABB92A085C12EE92365673");
            try
            {
                //var balance = await web3.Eth.GetBalance.SendRequestAsync("0xF510450c7731B584E58635D361Fd33570dD92A98");
                //var etherAmount = Web3.Convert.FromWei(balance.Value);
                var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync("0xF510450c7731B584E58635D361Fd33570dD92A98");
                var encoded = Web3.OfflineTransactionSigner.SignTransaction("9817CF212721DDAB1087FF642C7D52DCC1DDC0CBE1ABB92A085C12EE92365673", "0xF510450c7731B584E58635D361Fd33570dD92A98", new HexBigInteger(0), txCount.Value, new BigInteger(2), new BigInteger(26000), "0x" + hash);
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encoded);
                tx = transactionHash;
            }
            catch (System.Exception e)
            {
                throw e;
            }


            var result = new SyncedData {
                Id = "",
                Year = year,
                Week = week,
                Employee = employee,
                Tx = tx
            };
            BsonDocument newData = new BsonDocument();
            newData.Add("year", result.Year);
            newData.Add("week", result.Week);
            newData.Add("employee", result.Employee);
            newData.Add("tx", result.Tx);
            MongoHelper.GetDatabase().GetCollection<BsonDocument>("synced_data").InsertOne(newData);
            result.Id = newData.GetValue("_id").ToString();
            return result;
        }

    }

}