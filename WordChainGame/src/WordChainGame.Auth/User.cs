using System;

namespace WordChainGame.Auth
{
    public class User
    {
        public string UserName { get; set; }

        public string NormalizedName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public bool IsAdmin { get; set; }


        public User()
        {
        }
        

        public User(string userName)
            : this()
        {
            this.UserName = userName;
            this.SecurityStamp = Guid.NewGuid().ToString();
        }
    }

}
