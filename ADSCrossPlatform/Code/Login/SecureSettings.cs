namespace ADSCrossPlatform.Code.Models
{
    public class SecureSettings
    {
        private string TokenKey = "token_x2";
        private string UsernameKey = "username_x2";

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.Default.SetAsync(TokenKey, token);
        }

        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.Default.GetAsync(TokenKey) ?? string.Empty;
        }

        public async Task SaveUsernameAsync(string username)
        {
            await SecureStorage.Default.SetAsync(UsernameKey, username);
        }

        public async Task<string> GetUsernameAsync()
        {
            return await SecureStorage.Default.GetAsync(UsernameKey) ?? string.Empty;
        }
    }
}

