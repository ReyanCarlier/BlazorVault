namespace BlazorVault.Data.Models
{
    /*
     * Reesentation of a Password in the database.
     *      Id - The unique identifier for the password.
     *      UserId - The unique identifier of the user who own the password.
     *      Alias - The alias of the password.
     *      Domain - The domain of the password (e.g. google.com).
     *      Login - The login used to connect to the domain.
     *      Value - The value of the password, encrypted.
     *      CategoryName - The name of the category the password belongs to.
     *      SubcategoryName - The name of the subcategory the password belongs to.
     *      Shared - Whether the password is shared or not. (Unused)
     *      GroupId - The unique identifier of the group the password belongs to.
     *      Notes - Various informations about the password, written by the user.
     *      CreatedAt - The date when the password was created.
     *      UpdatedAt - The date when the password was last updated.
     */
    public class Password
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Alias { get; set; } = "";
        public string Domain { get; set; } = "";
        public string Login { get; set; } = "";
        public string Value { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string SubcategoryName { get; set; } = "";
        public bool Shared { get; set; } = false;
        public int GroupId { get; set; } = 0;
        public string Notes { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
