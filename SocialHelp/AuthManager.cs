using SocialHelp.Models;

namespace SocialHelp
{
    public static class AuthManager
    {
        public static Employee CurrentUser { get; private set; }

        public static void SetCurrentUser(Employee employee)
        {
            CurrentUser = employee;
        }

        public static void ClearCurrentUser()
        {
            CurrentUser = null;
        }
    }
}