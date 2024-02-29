namespace BlazorVault.Data.Models
{
    /**
     * Representation of a Category in the database.
     *
     *  Id - The unique identifier for the category.
     *  Name - The name of the category.
     *  OwnerId - The unique identifier of the user who created the category.
     *
     */
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int OwnerId { get; set;}
    }
}
