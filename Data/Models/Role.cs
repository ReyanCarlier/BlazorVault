namespace BlazorVault.Data.Models
{
    public class Role
    {
        /*
         * Representation of a Role in the database.
         *      Id - The unique identifier for the role.
         *      Name - The name of the role.
         */
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
