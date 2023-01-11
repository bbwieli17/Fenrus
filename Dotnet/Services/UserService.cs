namespace Fenrus.Services;

/// <summary>
/// Service for users
/// </summary>
public class UserService
{
    /// <summary>
    /// Validates a user and returns the user if valid
    /// </summary>
    /// <param name="username">the username</param>
    /// <param name="password">the password</param>
    /// <returns>the user if validates, otherwise null</returns>
    public Models.User? Validate(string username, string password)
    {
        var user = DbHelper.GetByName<Models.User>(username);
        bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (valid == false)
            return null;
        return user;
    }

    /// <summary>
    /// Registers a user
    /// </summary>
    /// <param name="username">the username</param>
    /// <param name="password">the password</param>
    /// <param name="isAdmin">if this user is an admin or not</param>
    /// <returns>the newly registered user</returns>
    public Models.User Register(string username, string password, bool isAdmin = false)
    {
        var user = DbHelper.GetByName<Models.User>(username);
        if (user != null)
        {
            // already exists, just update them
            user.Password = BCrypt.Net.BCrypt.HashPassword(password);;
            user.IsAdmin = isAdmin;
            DbHelper.Update(user);
            return user;
        }

        user = new();
        user.Name = username;
        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
        user.Uid = Guid.NewGuid();
        user.IsAdmin = isAdmin;
        DbHelper.Insert(user);
        return user;
    }

    /// <summary>
    /// Gets all users in the system
    /// </summary>
    /// <returns>all users in the system</returns>
    public List<Models.User> GetAll()
        => DbHelper.GetAll<Models.User>();

    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="user">the user to update</param>
    public void Update(Models.User user)
        => DbHelper.Update(user);
}