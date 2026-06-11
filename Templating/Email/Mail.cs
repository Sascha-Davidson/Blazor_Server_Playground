using Playground.Templating.Email.Models;
using RazorLight;

namespace Playground.Templating.Email
{
    public class Mail
    {
        private readonly RazorLightEngine _engine;

        public Mail()
        {
            var projectRoot = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Templating")
            );

            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(projectRoot)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderWelcomeEmailAsync(WelcomeEmailModel model)
        {
            return await _engine.CompileRenderAsync(
                "Email/Template/WelcomeEmail.cshtml",
                model
            );
        }
    }
}