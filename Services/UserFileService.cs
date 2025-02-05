using Fenrus.Models;
using Fenrus.Services.FileStorages;

namespace Fenrus.Services;

/// <summary>
/// Service for user files
/// </summary>
public class UserFilesService
{
    private bool UseFileSystem = true;

    private IFileStorage? _Storage;
    private IFileStorage Storage
    {
        get
        {
            _Storage ??= UseFileSystem ? new FileSystemFileStorage() : new LiteDbFileStorage();
            return _Storage;
        }
    }

    /// <summary>
    /// Gets all the UID for files for a user
    /// </summary>
    /// <param name="userUid">the UID of the user</param>
    /// <param name="path">the users folder path to get</param>
    /// <returns>all the files UIDs for the user</returns>
    public List<UserFile> GetAll(Guid userUid, string path)
        => Storage.GetAll(userUid, path);

    /// <summary>
    /// Gets the file data by its path
    /// </summary>
    /// <param name="userUid">the UID of the user</param>
    /// <param name="fullPath">the full path of the file</param>
    /// <returns>the file</returns>
    public (Stream Data, string Filename, string MimeType)? GetFileData(Guid userUid, string fullPath)
        => Storage.GetFileData(userUid, fullPath);
    
    /// <summary>
    /// Adds a file item to the database for the user
    /// </summary>
    /// <param name="userUid">the UID of the user</param>
    /// <param name="path">the path to upload this file to</param>
    /// <param name="filename">the short filename of the file being added</param>
    /// <param name="formFile">the form file being added</param>
    /// <returns>the id of the newly created file</returns>
    public Task<UserFile?> Add(Guid userUid, string path, string filename, IFormFile formFile)
        => Storage.Add(userUid, path, filename, formFile);

    /// <summary>
    /// Deletes a file
    /// </summary>
    /// <param name="userUid">the UID of the user</param>
    /// <param name="fullPath">the full path of the file</param>
    /// <returns>true if the files no longer exists afterwards</returns>
    public bool Delete(Guid userUid, string fullPath)
        => Storage.Delete(userUid, fullPath);

    /// <summary>
    /// Creates a folder
    /// </summary>
    /// <param name="userUid">the UID of the user</param>
    /// <param name="path">the full path of the file</param>
    public void CreateFolder(Guid userUid, string path)
        => Storage.CreateFolder(userUid, path);
}