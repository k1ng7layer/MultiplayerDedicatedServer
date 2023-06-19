namespace PBMultiplayerServer.Authentication
{
    public readonly struct LoginResult
    {
        public readonly ELoginResult Result;
        public readonly string Message;

        public LoginResult(ELoginResult result, string message)
        {
            Result = result;
            Message = message;
        }
    }
}