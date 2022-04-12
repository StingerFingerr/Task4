using System;

namespace Task4.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegisterDate { get; set; }  
        public DateTime LastLoginDate { get; set; }
        public bool isBlocked { get; set; }
    }
}
