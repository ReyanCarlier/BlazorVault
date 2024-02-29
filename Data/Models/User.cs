namespace BlazorVault.Data.Models
{
    public enum RoleGrade
    {
        Utilisateur = 0,
        Support = 1,
        Administrateur = 2,
        Editeur = 3
    }

    /*
     * Representation of a User in the database.
     *      Id - The unique identifier for the user.
     *      GroupsIds - A comma-separated list of group ids.
     *      Mail - The email of the user. (e.g. firstname.lastname@domain.com)
     *      MasterPassword - The master password of the user, encrypted. Used to encrypt/decrypt the passwords.
     *      RoleId - The role of the user.
     *      TwoFactorEnabled - Whether the two factor authentication is enabled or not. (Unused)
     *      TwoFactorSecret - The secret used to generate the two factor authentication code. (Unused)
     *      CreatedAt - The date when the user was created.
     *      UpdatedAt - The date when the user was last updated.
     */
    public class User
    {
        public int Id { get; set; }
        // GroupsIds is a comma-separated list of group ids
        public string GroupsIds { get; set; } = "";
        public string Mail { get; set; } = "";
        public string MasterPassword { get; set; } = "";
        public RoleGrade RoleId { get; set; }
        public bool TwoFactorEnabled { get; set; } = false;
        public string TwoFactorSecret { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
