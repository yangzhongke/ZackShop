using UsersDomain.Shared;
using UsersDomain.Shared.Entities;

namespace AuthOpenAPIs.IntegrationTests.Helpers
{
    class DataBaseHelper(UsersDbContext usersDbContext)
    {
        public void ClearUsers()
        {
            usersDbContext.Users.RemoveRange(usersDbContext.Users);
            usersDbContext.SaveChanges();
        }

        public void AddUser(User user)
        {
            usersDbContext.Users.Add(user);
            usersDbContext.SaveChanges();
        }

        public User[] GetAllUsers()
        {
            return usersDbContext.Users.ToArray();
        }

        public User? GetUserByEmail(string email)
        {
            return usersDbContext.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
