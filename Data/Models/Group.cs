namespace BlazorVault.Data.Models
{
    /*
     * Representation of a Group in the database.
     *  Id - The unique identifier for the group.
     *  Name - The name of the group, displayed in the UI.
     *  OwnerId - The unique identifier of the user who created the group.
     *  UsersMail - A comma separated list of emails of the users in the group.
     *  CypheredPassword - The cyphered password of the group.
     *  CreationDate - The date when the group was created.
     *  LastUpdate - The date when the group was last updated, either by adding/removing a user or changing the password.
     */
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int OwnerId { get; set; } = 0;
        // UsersMail is a comma separated list of emails
        public string UsersMail { get; set; } = "";
        public string CypheredPassword { get; set; } = "";
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }
}
