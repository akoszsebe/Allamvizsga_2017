namespace Allamvizsga2017.Models
{
    class CredentialsProvider
    {
        public static string Login(string email, string password)
        {
           return RestClient.Login(new LoginUser(email,password));
        }

        public void Register(string email, string password1, string password2)
        {
            
        }

        public void ResetPassword(string email)
        {
            
        }

    }

    class LoginUser
    {
        public string Email { get; }
        public string Password { get; }

        public string Phonenumber { get; }

        public LoginUser(string email,string password)
        {
            Email = email;
            Password = password;
        }

        public LoginUser(string email, string password,string phonenumber)
        {
            Email = email;
            Password = password;
            Phonenumber = phonenumber;
        }
    }
}