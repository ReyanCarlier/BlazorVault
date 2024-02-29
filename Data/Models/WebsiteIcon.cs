namespace BlazorVault.Data.Models
{
    /*
     * Representation of a WebsiteIcon in the database.
     *      Id - The unique identifier for the icon.
     *      Domain - The domain of the website. (e.g. google.com)
     *      Icon - The icon of the website, in bytes. (Used to store the icon in the database)
     */
    public class WebsiteIcon
    {
        public int Id { get; set; }
        public string Domain { get; set; } = "";
        public byte[] Icon { get; set; } = [];
    }
}
