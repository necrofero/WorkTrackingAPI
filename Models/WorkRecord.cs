using System;

namespace WorkTrackingAPI.Models
{

    public class WorkRecord
    {
        public string Id { get; set; } = "";  
        public string EmployeeId { get; set; } = "";  
        public string EmployeeName { get; set; } = "";  
        public DateTime DateTime { get; set; }
        public string Type { get; set; } = "";
        public int Week { get; set; } = 0;
        public bool Synced { get; set; } = false;
    }

}