namespace VaporStore.DataProcessor
{
	using System;
	using Data;
    using VaporStore.Data.Models;
    using System.Linq;

    public static class Bonus
	{
		public static string UpdateEmail(VaporStoreDbContext context, string username, string newEmail)
		{
            User user = context.Users.FirstOrDefault(x => x.Username == username);
            if(user == null)
            {
                return $"User {username} not found";
            }

            User userWithNewEmail = context.Users.FirstOrDefault(x => x.Email == newEmail);
            if(userWithNewEmail != null)
            {
                return $"Email {newEmail} is already taken";
            }

            user.Email = newEmail;
            context.SaveChanges();
            return $"Changed {username}'s email successfully";
		}
	}
}
