using System;

namespace WorkTrackingAPI.Models
{

    public class User
    {
        public string Id { get; set; } = "";  
        public string Name { get; set; } = "";  
        public string Surname { get; set; } = "";  
        public bool IsAdmin { get; set; } = false;
    }

    public class UserLogin
    {
        public string UserName { get; set; } = "";  
        public string Password { get; set; } = "";  
    }

}