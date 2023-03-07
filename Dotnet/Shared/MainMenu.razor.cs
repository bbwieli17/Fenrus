using Fenrus.Models;
using Fenrus.Pages;
using Fenrus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Fenrus.Shared;

/// <summary>
/// The Main Menu 
/// </summary>
public partial class MainMenu 
{
    /// <summary>
    /// Gets or sets the navigation manager used for routing
    /// </summary>
    [Inject] private NavigationManager Router { get; set; }
    
    [CascadingParameter] private App App { get; set; }

    private List<MenuGroup> Menu = new();

    private string lblAbout, lblVersion;

    protected override async Task OnInitializedAsync()
    {
        lblAbout = App.Translater.Instant("Pages.About.Title");
        lblVersion = App.Translater.Instant("Labels.VersionNumber", new { version = Globals.Version });
        
        Router.LocationChanged += (obj, e) => this.StateHasChanged();
        Menu.Add(new MenuGroup()
        {
            Name = App.Translater.Instant("Labels.General"), 
            Items = new List<MenuItem>()
            {
                new () { Name = "Home", Link = "/", Icon = "fa-solid fa-house"}
            }
        });
        Menu.Add(new MenuGroup()
        {
            Name = App.Translater.Instant("Labels.Dashboard"),
            Items = new List<MenuItem>()
            {
                new () { Name = App.Translater.Instant("Pages.Dashboards.Title"), Link = "/settings/dashboards", Icon = "fa-solid fa-table-cells-large"},
                new () { Name = App.Translater.Instant("Pages.Groups.Title"), Link = "/settings/groups", Icon = "fa-solid fa-puzzle-piece"},
                new () { Name = App.Translater.Instant("Pages.SearchEngines.Title"), Link = "/settings/search-engines", Icon = "fa-solid fa-magnifying-glass"}
            }
        });

        if (App.IsAdmin == true)
        {
            Menu.Add(new MenuGroup()
            {
                Name = App.Translater.Instant("Labels.Administrator"),
                Items = new List<MenuItem>()
                {
                    new () { Name = App.Translater.Instant("Pages.Dashboard.Title-Guest"), Link = "/settings/system/guest-dashboard", Icon = "fa-solid fa-table-cells-large"},
                    new () { Name = App.Translater.Instant("Pages.Groups.Title-System"), Link = "/settings/system/groups", Icon = "fa-solid fa-puzzle-piece"},
                    new () { Name = App.Translater.Instant("Pages.SearchEngines.Title-System"), Link = "/settings/system/search-engines", Icon = "fa-solid fa-magnifying-glass"},
                    new () { Name = App.Translater.Instant("Pages.Users.Title"), Link = "/settings/system/users", Icon = "fa-solid fa-user-group"},
                    new () { Name = App.Translater.Instant("Pages.Docker.Title"), Link = "/settings/system/docker", Icon = "fa-brands fa-docker"},
                    new () { Name = App.Translater.Instant("Pages.SystemSettings.Title"), Link = "/settings/system/system-settings", Icon = "fa-solid fa-gear"},
                }
            });
        }

        // update the icon if the accent color changes
        App.AccentColorUpdated = EventCallback.Factory.Create(this, StateHasChanged);
    }

    private bool IsActive(MenuItem item)
        => IsActive(item.Link);

    private bool IsActive(string itemLink)
    {
        string url = Router.Uri;
        url = url.Substring(Router.BaseUri.Length - 1);
        return itemLink == url;
    }
}

/// <summary>
/// Menu Group
/// </summary>
class MenuGroup
{
    /// <summary>
    /// Gets the name of of the menu group
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the menu items
    /// </summary>
    public List<MenuItem> Items { get; init; } = new List<MenuItem>();
}

/// <summary>
/// A menu item
/// </summary>
public class MenuItem
{
    /// <summary>
    /// Gets the name
    /// </summary>
    public string Name { get; init; }
    /// <summary>
    /// Gets the link
    /// </summary>
    public string Link { get; init; }
    /// <summary>
    /// Gets the icon
    /// </summary>
    public string Icon { get; init; }
}