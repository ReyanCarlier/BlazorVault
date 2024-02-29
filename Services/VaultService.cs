using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BlazorVault;
using BlazorVault.Data;
using BlazorVault.Data.Models;
using BlazorVault.Utils;

/*
 *  VaultService class defines the methods used to interact with the database.
 */
internal class VaultService(VaultDbContext dbContext)
{
    private readonly VaultDbContext _dbContext = dbContext;

    public async Task<User?> GetUserAsync(string email)
    {
        if (email == null)
        {
            return null;
        }
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Mail == email);
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

    public async Task<bool> AddUserAsync(User user)
    {
        var existingUser = await GetUserAsync(user.Id);
        if (existingUser != null)
        {
            return false;
        }

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUserAsync(User user, string? newPassword = null)
    {
        var existingUser = await GetUserAsync(user.Id);
        if (existingUser == null)
        {
            return false;
        }

        if (newPassword != null)
        {
            if (!await UpdateMasterPasswordAsync(existingUser, newPassword))
            {
                return false;
            }
        }
        _dbContext.Users.Update(existingUser);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var existingUser = await GetUserAsync(userId);
        if (existingUser == null)
        {
            return false;
        }

        _dbContext.Users.Remove(existingUser);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(User user)
    {
        var existingUser = await GetUserAsync(user.Id);
        if (existingUser == null)
        {
            return false;
        }

        _dbContext.Users.Remove(existingUser);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<List<Password>> GetPasswordsByUserIdAsync(int userId)
    {
        return await _dbContext.Passwords.Where(p => p.UserId == userId && p.GroupId == 0).ToListAsync();
    }

    public async Task<Password?> GetPasswordAsync(int passwordId)
    {
        return await _dbContext.Passwords.FindAsync(passwordId);
    }

    public async Task<bool> AddPasswordAsync(Password password, User user)
    {
        var existingPassword = await GetPasswordAsync(password.Id);
        if (existingPassword != null)
        {
            return false;
        }
        if (password.Domain.IsNullOrEmpty())
            password.Domain = "";
        else
        {
            if (password.Domain.StartsWith("http://"))
            {
                password.Domain = password.Domain[7..];
            }
            else if (password.Domain.StartsWith("https://"))
            {
                password.Domain = password.Domain[8..];
            }

            var slashIndex = password.Domain.IndexOf('/');
            if (slashIndex != -1)
            {
                password.Domain = password.Domain[..slashIndex];
            }
        }
        

        if (password.GroupId == 0)
            password.Value = Crypto.Encrypt(password.Value, Crypto.Decrypt(user.MasterPassword, Program.AdminPassword));
        else
        {
            var group = await GetGroupAsync(password.GroupId) ?? throw new Exception("Group not found");
            if (group.CypheredPassword == null)
                throw new Exception("Group password not found");
            else if (!group.UsersMail.Contains(user.Mail))
                throw new Exception("User is not in group");
            else
                password.Value = Crypto.Encrypt(password.Value, Crypto.Decrypt(group.CypheredPassword, Program.AdminPassword));
        }
        await _dbContext.Passwords.AddAsync(password);
        await _dbContext.SaveChangesAsync();

        if (await AddWebsiteIcon(password.Domain))
        {
            Console.WriteLine($"Added website icon for {password.Domain}");
            return true; // TODO : Add a better handler of this function.
        }
        return true;
    }

    public async Task<bool> UpdatePasswordAsync(Password password)
    {
        var existingPassword = await GetPasswordAsync(password.Id);
        if (existingPassword == null)
        {
            return false;
        }

        _dbContext.Passwords.Update(password);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePasswordAsync(int passwordId)
    {
        var existingPassword = await GetPasswordAsync(passwordId);
        if (existingPassword == null)
        {
            return false;
        }

        _dbContext.Passwords.Remove(existingPassword);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Password>> GetPasswordsAsync()
    {
        return await _dbContext.Passwords.ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _dbContext.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(int categoryId)
    {
        return await _dbContext.Categories.FindAsync(categoryId);
    }

    public async Task<bool> AddCategoryAsync(Category category)
    {
        var existingCategory = await GetCategoryAsync(category.Id);
        if (existingCategory != null)
        {
            return false;
        }

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        var existingCategory = await GetCategoryAsync(category.Id);
        if (existingCategory == null)
        {
            return false;
        }

        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        var existingCategory = await GetCategoryAsync(categoryId);
        if (existingCategory == null)
        {
            return false;
        }

        _dbContext.Categories.Remove(existingCategory);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        return await _dbContext.Roles.ToListAsync();
    }

    public async Task<Role?> GetRoleAsync(int roleId)
    {
        return await _dbContext.Roles.FindAsync(roleId);
    }

    public async Task<bool> AddRoleAsync(Role role)
    {
        var existingRole = await GetRoleAsync(role.Id);
        if (existingRole != null)
        {
            return false;
        }

        await _dbContext.Roles.AddAsync(role);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRoleAsync(Role role)
    {
        var existingRole = await GetRoleAsync(role.Id);
        if (existingRole == null)
        {
            return false;
        }

        _dbContext.Roles.Update(role);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRoleAsync(int roleId)
    {
        var existingRole = await GetRoleAsync(roleId);
        if (existingRole == null)
        {
            return false;
        }

        _dbContext.Roles.Remove(existingRole);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<string> DecryptUserPassword(int userId, string password, string masterPassword)
    {
        try
        {
            var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
            string cypheredMasterPassword = Crypto.Decrypt(user.MasterPassword, Program.AdminPassword);
            string decypheredMasterPassword = Crypto.Decrypt(cypheredMasterPassword, masterPassword);
            
            string decryptedPassword = Crypto.Decrypt(password, decypheredMasterPassword);
            return decryptedPassword;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> ExistsWebsiteIcon(string domain)
    {
        var data = await _dbContext.WebsiteIcons.FirstOrDefaultAsync(w => w.Domain == domain);
        return data != null && data.Icon != null;
    }

    public async Task<byte[]> GetWebsiteIcon(string domain)
    {
        byte[] bytes = [];
        var websiteIcon = await _dbContext.WebsiteIcons.FirstOrDefaultAsync(w => w.Domain == domain);
        return websiteIcon?.Icon ?? bytes;
    }

    public async Task<byte[]> GetWebsiteIconByPasswordId(int passwordId)
    {
        byte[] bytes = [];
        var password = await _dbContext.Passwords.FirstOrDefaultAsync(p => p.Id == passwordId);
        if (password == null)
        {
            return bytes;
        }

        var websiteIcon = await _dbContext.WebsiteIcons.FirstOrDefaultAsync(w => w.Domain == password.Domain);
        return websiteIcon?.Icon ?? bytes;
    }

    public async Task<bool> AddWebsiteIcon(string domain)
    {
        if (await ExistsWebsiteIcon(domain))
            return true;

        if (domain.StartsWith("http://"))
        {
            domain = domain[7..];
        }
        else if (domain.StartsWith("https://"))
        {
            domain = domain[8..];
        }
        var slashIndex = domain.IndexOf('/');
        if (slashIndex != -1)
        {
            domain = domain[..slashIndex];
        }

        var existingWebsiteIcon = await _dbContext.WebsiteIcons.FirstOrDefaultAsync(w => w.Domain == domain);
        if (existingWebsiteIcon != null)
        {
            return false;
        }

        var httpClient = new HttpClient();
        var faviconDownloader = new FaviconDownloader(httpClient);
        

        try
        {
            byte[] faviconBytes = await faviconDownloader.DownloadFaviconAsync(domain);
            var websiteIcon = new WebsiteIcon
            {
                Domain = domain,
                Icon = faviconBytes
            };

            _dbContext.WebsiteIcons.Add(websiteIcon);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }

    public async Task<bool> DeleteWebsiteIcon(string domain)
    {
        var existingWebsiteIcon = await _dbContext.WebsiteIcons.FirstOrDefaultAsync(w => w.Domain == domain);
        if (existingWebsiteIcon == null)
        {
            return false;
        }

        _dbContext.WebsiteIcons.Remove(existingWebsiteIcon);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateMasterPasswordAsync(int userId, string masterPassword)
    {
        try
        {
            var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
            return await UpdateMasterPasswordAsync(user, masterPassword);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        
    }

    public async Task<bool> UpdateMasterPasswordAsync(User user, string newMasterPassword)
    {
        try
        {
            string decypheredMasterPassword = Crypto.Decrypt(user.MasterPassword, Program.AdminPassword);

            foreach (var password in await GetPasswordsByUserIdAsync(user.Id))
            {
                if (password.Value == null)
                {
                    continue;
                }
                password.Value = Crypto.Decrypt(password.Value, decypheredMasterPassword);
                password.Value = Crypto.Encrypt(password.Value, newMasterPassword);
                await UpdatePasswordAsync(password);
            }

            user.MasterPassword = Crypto.Encrypt(newMasterPassword, Program.AdminPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<RoleGrade> GetUserRoleAsync(User user)
    {
        var role = await GetRoleAsync((int) user.RoleId);
        return role?.Name switch
        {
            "Utilisateur" => RoleGrade.Utilisateur,
            "Support" => RoleGrade.Support,
            "Administrateur" => RoleGrade.Administrateur,
            "Editeur" => RoleGrade.Editeur,
            _ => RoleGrade.Utilisateur
        };
    }

    public async Task<RoleGrade> GetUserRoleAsync(string email)
    {
        var user = await GetUserAsync(email);
        return user == null ? throw new Exception("User not found") : await GetUserRoleAsync(user);
    }

    public async Task<RoleGrade> GetUserRoleAsync(int userId)
    {
        var user = await GetUserAsync(userId);
        return user == null ? throw new Exception("User not found") : await GetUserRoleAsync(user);
    }

    public async Task<bool> UpdateUserRoleAsync(int userId, RoleGrade role)
    {
        try
        {
            var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
            return await UpdateUserRoleAsync(user, role);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateUserRoleAsync(User user, RoleGrade role)
    {
        try
        {
            var roleEnum = await GetRoleAsync((int) role) ?? throw new Exception("Role not found");
            user.RoleId = role;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateTwoFactorAuthenticationAsync(int userId, bool twoFactorEnabled)
    {
        try
        {
            var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
            return await UpdateTwoFactorAuthenticationAsync(user, twoFactorEnabled);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateTwoFactorAuthenticationAsync(User user, bool twoFactorEnabled)
    {
        try
        {
            user.TwoFactorEnabled = twoFactorEnabled;
            if (!twoFactorEnabled) { user.TwoFactorSecret = ""; }
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateTwoFactorSecretAsync(int userId, string twoFactorSecret)
    {
        try
        {
            var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
            return await UpdateTwoFactorSecretAsync(user, twoFactorSecret);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateTwoFactorSecretAsync(User user, string twoFactorSecret)
    {
        try
        {
            user.TwoFactorSecret = twoFactorSecret;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> AddUserToGroup(User user, Group group)
    {
        try
        {
            group.UsersMail += "," + user.Mail;
            _dbContext.Groups.Update(group);
            await _dbContext.SaveChangesAsync();

            user.GroupsIds += "," + group.Id;
            _dbContext.Users.Update(user);
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /**
     * When a Group is created, the cypheredPassword passed in parameters in the group password cyphered with
     * the password chosen by the group owner. This cyphered password is then used to cypher all the passwords
     * within the group. When the group password is changed, all the passwords within the group must be decrypted
     * and re-cyphered with the new group password.
     */
    public async Task<bool> CreateGroupAsync(User user, string groupName, string cypheredPassword)
    {
        try
        {
            Group group = new()
            {
                Name = groupName,
                OwnerId = user.Id,
                CypheredPassword = Crypto.Encrypt(cypheredPassword, Program.AdminPassword)
            };
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();
            await AddUserToGroupAsync(user, group);
            _dbContext.Users.Update(user);
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> AddGroupAsync(Group group)
    {
        try
        {
            group.CreationDate = DateTime.Now;
            group.LastUpdate = DateTime.Now;
            group.CypheredPassword = Crypto.Encrypt(group.CypheredPassword, Program.AdminPassword);
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();
            await AddUserToGroupAsync(group.OwnerId, group);
            var user = await GetUserAsync(group.OwnerId);
            if (user == null)
            {
                return false;
            }
            user.GroupsIds += "," + group.Id;
            _dbContext.Users.Update(user);
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<List<Password>> GetPasswordsByGroupIdAsync(int groupId)
    {
        if (groupId == 0) // GroupId 0 is used for passwords that are not in a group, so we return an empty list
            return [];
        return await _dbContext.Passwords.Where(p => p.GroupId == groupId).ToListAsync();
    }

    public async Task<bool> DeleteGroupAsync(int groupId)
    {
        try
        {
            var group = await GetGroupAsync(groupId) ?? throw new Exception("Group not found");
            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();
            foreach (var password in await GetPasswordsByGroupIdAsync(groupId))
            {
                await DeletePasswordAsync(password.Id);
            }

            foreach (var user in await GetUsersAsync())
            {
                if (user.GroupsIds.Contains(groupId.ToString()))
                {
                    user.GroupsIds = user.GroupsIds.Replace(groupId.ToString(), "");
                    user.GroupsIds = user.GroupsIds.Replace(",,", ",");
                    _dbContext.Users.Update(user);
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Group?> GetGroupAsync(int groupId)
    {
        return await _dbContext.Groups.FindAsync(groupId);
    }

    public async Task<List<Group>> GetGroupsAsync()
    {
        return await _dbContext.Groups.ToListAsync();
    }

    public async Task<List<Group>> GetGroupsOwnedByUserAsync(User user)
    {
        return await _dbContext.Groups.Where(g => g.OwnerId == user.Id).ToListAsync();
    }

    public async Task<List<Group>> GetGroupsOwnedByUserAsync(string email)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await GetGroupsOwnedByUserAsync(user);
    }

    public async Task<List<Group>> GetGroupsOwnedByUserAsync(int userId)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await GetGroupsOwnedByUserAsync(user);
    }

    public async Task<List<Group>> GetGroupsWhereUserIsMemberAsync(User user)
    {
        List<Group> groups = await _dbContext.Groups.ToListAsync();
        List<Group> groupsWhereUserIsMember = [];
        foreach (var group in groups)
        {
            if (group.UsersMail.Contains(user.Mail))
            {
                groupsWhereUserIsMember.Add(group);
            }
        }
        return groupsWhereUserIsMember;
    }

    public async Task<List<Group>> GetGroupsWhereUserIsMemberAsync(string email)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await GetGroupsWhereUserIsMemberAsync(user);
    }

    public async Task<List<Group>> GetGroupsWhereUserIsMemberAsync(int userId)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await GetGroupsWhereUserIsMemberAsync(user);
    }

    public async Task<bool> AddUserToGroupAsync(User user, Group group)
    {
        try
        {
            var existingGroup = await GetGroupAsync(group.Id) ?? throw new Exception("Group not found");

            List<Group> groups = await GetGroupsWhereUserIsMemberAsync(user);
            if (groups.Any(g => g.Id == group.Id))
            {
                throw new Exception("User is already in group");
            }
            var userMails = string.IsNullOrEmpty(existingGroup.UsersMail) ? [] : existingGroup.UsersMail.Split(',').ToList();
            userMails.Add(user.Mail);
            existingGroup.UsersMail = string.Join(",", userMails.Distinct());
            _dbContext.Groups.Update(existingGroup);
            user.GroupsIds = user.GroupsIds.Trim(',');
            var groupIds = string.IsNullOrEmpty(user.GroupsIds) ? [] : user.GroupsIds.Split(',').Select(int.Parse).ToList();
            groupIds.Add(group.Id);
            user.GroupsIds = string.Join(",", groupIds.Distinct());
            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            // Log the exception details
            Console.WriteLine($"Error in AddUserToGroupAsync: {e.Message}");
            // Consider what action to take in case of an error
            return false;
        }
    }


    public async Task<bool> AddUserToGroupAsync(string email, Group group)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await AddUserToGroupAsync(user, group);
    }

    public async Task<bool> AddUserToGroupAsync(int userId, Group group)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await AddUserToGroupAsync(user, group);
    }

    public async Task<bool> RemoveUserFromGroupAsync(User user, Group group)
    {
        try
        {
            var existingGroup = await GetGroupAsync(group.Id) ?? throw new Exception("Group not found");

            var userMails = existingGroup.UsersMail.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            userMails.Remove(user.Mail);
            existingGroup.UsersMail = string.Join(",", userMails);

            _dbContext.Groups.Update(existingGroup);

            var groupIds = user.GroupsIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(id => int.TryParse(id, out int result) ? result : (int?)null)
                                         .Where(id => id.HasValue && id.Value != group.Id)
                                         .Select(id => id.Value.ToString());

            user.GroupsIds = string.Join(",", groupIds);
            await Task.Yield();
            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            // Log the exception details
            Console.WriteLine($"Error in RemoveUserFromGroupAsync: {e.Message}");
            // Consider what action to take in case of an error
            return false;
        }
    }

    public async Task<bool> RemoveUserFromGroupAsync(string email, Group group)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await RemoveUserFromGroupAsync(user, group);
    }

    public async Task<bool> RemoveUserFromGroupAsync(int userId, Group group)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await RemoveUserFromGroupAsync(user, group);
    }

    public async Task<bool> IsUserInGroupAsync(User user, Group group)
    {
        var existingGroup = await GetGroupAsync(group.Id) ?? throw new Exception("Group not found");
        return existingGroup.UsersMail.Contains(user.Mail);
    }

    public async Task<bool> IsUserInGroupAsync(string email, Group group)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await IsUserInGroupAsync(user, group);
    }

    public async Task<bool> IsUserInGroupAsync(int userId, Group group)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await IsUserInGroupAsync(user, group);
    }

    public async Task<bool> IsUserOwnerOfGroupAsync(User user, Group group)
    {
        var existingGroup = await GetGroupAsync(group.Id) ?? throw new Exception("Group not found");
        return existingGroup.OwnerId == user.Id;
    }

    public async Task<bool> IsUserOwnerOfGroupAsync(string email, Group group)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await IsUserOwnerOfGroupAsync(user, group);
    }

    public async Task<bool> IsUserOwnerOfGroupAsync(int userId, Group group)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await IsUserOwnerOfGroupAsync(user, group);
    }

    public async Task<bool> IsUserInGroupAsync(User user, int groupId)
    {
        var existingGroup = await GetGroupAsync(groupId) ?? throw new Exception("Group not found");
        return existingGroup.UsersMail.Contains(user.Mail);
    }

    public async Task<bool> IsUserInGroupAsync(string email, int groupId)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await IsUserInGroupAsync(user, groupId);
    }

    public async Task<bool> IsUserInGroupAsync(int userId, int groupId)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await IsUserInGroupAsync(user, groupId);
    }

    public async Task<bool> IsUserOwnerOfGroupAsync(User user, int groupId)
    {
        var existingGroup = await GetGroupAsync(groupId) ?? throw new Exception("Group not found");
        return existingGroup.OwnerId == user.Id;
    }

    public async Task<bool> IsUserOwnerOfGroupAsync(string email, int groupId)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await IsUserOwnerOfGroupAsync(user, groupId);
    }

    public async Task<bool> IsUserOwnerOfGroupAsync(int userId, int groupId)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await IsUserOwnerOfGroupAsync(user, groupId);
    }

    public async Task<bool> EditGroupPasswordAsync(User user, Group group, string uncypheredOldPassword, string uncypheredNewPassword)
    {
        try
        {
            if (!await IsUserOwnerOfGroupAsync(user, group))
            {
                throw new Exception("User is not owner of group");
            }
            var existingGroup = await GetGroupAsync(group.Id) ?? throw new Exception("Group not found");

            foreach (var password in await GetPasswordsByGroupIdAsync(group.Id))
            {
                if (password.Value == null)
                {
                    continue;
                }
                password.Value = Crypto.Decrypt(password.Value, uncypheredOldPassword);
                password.Value = Crypto.Encrypt(password.Value, uncypheredNewPassword);
                await UpdatePasswordAsync(password);
            }
            group.CypheredPassword = Crypto.Encrypt(uncypheredNewPassword, Program.AdminPassword);
            _dbContext.Groups.Update(existingGroup);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> EditGroupPasswordAsync(string email, Group group, string uncypheredOldPassword, string uncypheredNewPassword)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await EditGroupPasswordAsync(user, group, uncypheredOldPassword, uncypheredNewPassword);
    }

    public async Task<bool> EditGroupPasswordAsync(int userId, Group group, string uncypheredOldPassword, string uncypheredNewPassword)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await EditGroupPasswordAsync(user, group, uncypheredOldPassword, uncypheredNewPassword);
    }

    public async Task<bool> EditGroupPasswordAsync(User user, int groupId, string uncypheredOldPassword, string uncypheredNewPassword)
    {
        try
        {
            var group = await GetGroupAsync(groupId) ?? throw new Exception("Group not found");
            return await EditGroupPasswordAsync(user, group, uncypheredOldPassword, uncypheredNewPassword);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    public async Task<bool> EditGroupNameAsync(User user, Group group, string groupName)
    {
        try
        {
            if (!await IsUserOwnerOfGroupAsync(user, group))
            {
                throw new Exception("User is not owner of group");
            }
            var existingGroup = await GetGroupAsync(group.Id) ?? throw new Exception("Group not found");
            existingGroup.Name = groupName;
            _dbContext.Groups.Update(existingGroup);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> EditGroupNameAsync(string email, Group group, string groupName)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await EditGroupNameAsync(user, group, groupName);
    }

    public async Task<bool> EditGroupNameAsync(int userId, Group group, string groupName)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await EditGroupNameAsync(user, group, groupName);
    }

    public async Task<bool> EditGroupNameAsync(User user, int groupId, string groupName)
    {
        try
        {
            var group = await GetGroupAsync(groupId) ?? throw new Exception("Group not found");
            return await EditGroupNameAsync(user, group, groupName);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> EditGroupNameAsync(string email, int groupId, string groupName)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await EditGroupNameAsync(user, groupId, groupName);
    }

    public async Task<bool> EditGroupNameAsync(int userId, int groupId, string groupName)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await EditGroupNameAsync(user, groupId, groupName);
    }

    public async Task<bool> EditGroupPasswordAsync(string email, int groupId, string oldCypheredPassword, string newCypheredPassword)
    {
        var user = await GetUserAsync(email) ?? throw new Exception("User not found");
        return await EditGroupPasswordAsync(user, groupId, oldCypheredPassword, newCypheredPassword);
    }

    public async Task<bool> EditGroupPasswordAsync(int userId, int groupId, string oldCypheredPassword, string newCypheredPassword)
    {
        var user = await GetUserAsync(userId) ?? throw new Exception("User not found");
        return await EditGroupPasswordAsync(user, groupId, oldCypheredPassword, newCypheredPassword);
    }

    public async Task<List<User>> GetUsersInGroupAsync(Group group)
    {
        List<string> userMails = [.. group.UsersMail.Split(',')];
        List<User> usersInGroup = [];

        foreach (var userMail in userMails)
        {
            var user = await GetUserAsync(userMail);
            if (user != null)
            {
                usersInGroup.Add(user);
            }
        }
        return usersInGroup;
    }

    public async Task<List<User>> GetUsersInGroupAsync(int groupId)
    {
        var group = await GetGroupAsync(groupId) ?? throw new Exception("Group not found");
        return await GetUsersInGroupAsync(group);
    }
}