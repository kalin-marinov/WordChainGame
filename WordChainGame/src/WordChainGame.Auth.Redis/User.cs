using System;

namespace WordChainGame.Auth
{
    public class User
    {
        public string UserName { get; set; }

        public string NormalizedName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public bool IsAdmin { get; set; }


        public User()
        {
        }
        

        public User(string userName, string fullName, string email)
            : this()
        {
            this.UserName = userName;
            this.FullName = fullName;
            this.Email = email;
            this.SecurityStamp = Guid.NewGuid().ToString();
        }
    }

}
