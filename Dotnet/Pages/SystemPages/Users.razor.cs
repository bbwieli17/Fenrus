using Fenrus.Components;
using Fenrus.Components.SideEditors;
using Fenrus.Models;
using Fenrus.Services;

namespace Fenrus.Pages;

/// <summary>
/// Users page
/// </summary>
public partial class Users: CommonPage<User>
{
    /// <summary>
    /// Gets or sets the items 
    /// </summary>
    public List<User> Items { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the table instance
    /// </summary>
    private FenrusTable<User> Table { get; set; }

    /// <summary>
    /// Gets the page title
    /// </summary>
    private string Title => "Users";

    /// <summary>
    /// Gets the page description
    /// </summary>
    private string Description =>
        "This page lets you manage users for Fenrus.";
    
    /// <summary>
    /// Called after the user has been fetched
    /// </summary>
    protected override async Task PostGotUser()
    {
        Items = new UserService().GetAll();
    }

    /// <summary>
    /// Gets or sets if registrations are allowed
    /// </summary>
    private bool AllowRegistrations
    {
        get => SystemSettings.AllowRegister;
        set
        {
            if (value == SystemSettings.AllowRegister)
                return;
            SystemSettings.AllowRegister = value;
            SystemSettings.Save();
            ToastService.ShowSuccess("Updated Successfully");
        }
    }

    /// <summary>
    /// Checks if the item is the same as the operating user
    /// </summary>
    /// <param name="item">the item to check</param>
    /// <returns>if the item is the same as the operating user</returns>
    private bool IsSelf(Models.User item)
        => item.Uid == Settings.Uid;

    /// <summary>
    /// Updates a users admin status
    /// </summary>
    /// <param name="user">the user being updated</param>
    /// <param name="isAdmin">whether or not they are now an admin</param>
    private void AdminUpdated(User user, bool isAdmin)
    {
        if (IsSelf(user))
            return;
        if (user.IsAdmin != isAdmin)
        {
            user.IsAdmin = isAdmin;
            new UserService().Update(user);
        }

    }
}