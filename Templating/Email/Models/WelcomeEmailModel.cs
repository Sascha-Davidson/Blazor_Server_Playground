namespace Playground.Templating.Email.Models
{
    public class WelcomeEmailModel
    {
        public string UserName { get; set; } = string.Empty;

        public EmailHeaderModel Header { get; set; } = new();
    }
}
