using System;

namespace WorkTrackingAPI.Models
{

    public class SyncedData
    {
        public string Id { get; set; } = "";  
        public int Year { get; set; } = 0;  
        public int Week { get; set; } = 0;
        public string Employee { get; set; } = "";
        public string Tx { get; set; } = "";
    }

}