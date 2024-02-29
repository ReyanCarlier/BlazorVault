namespace BlazorVault
{
    // This class is used to store session data.
    public class SessionService
    {
        public bool TwoFactorAuthentified { get; set; } = false;
    }
}
