using System;
using Origam.Security.Common;

namespace Origam.ServerCore
{
    public class User: IOrigamUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public Guid ProviderUserKey { get; set; }
        public string TransactionId { get; set; }
        public string SecurityStamp { get; set; }
        public bool Is2FAEnforced { get; set; }
        
        public User(string userName)
        {
            UserName = userName;
            IsApproved = true;
        }
    }
}