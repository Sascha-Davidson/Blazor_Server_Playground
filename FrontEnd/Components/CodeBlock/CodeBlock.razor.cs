using Playground.Lib.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;

namespace Playground.FrontEnd.Components.CodeBlock
{
    public partial class CodeBlock
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;

        /// The source code to be displayed inside the <code> block. 
        /// It will be HTML-encoded to ensure safe rendering.
        /// </summary>
        [Parameter]
        public string Code { get; set; }

        /// <summary>
        /// The programming language used for syntax highlighting.
        /// Examples: "csharp", "html", "javascript", "json", etc.
        /// Will be used to assign a CSS class like "language-csharp".
        /// </summary>
        [Parameter]
        public CodeLanguage Language { get; set; } = CodeLanguage.None;

        /// <summary>
        /// Number of spaces to represent a tab character in the code display.
        /// Affects how indentation is rendered inside the <pre> block.
        /// Default is 4.
        /// </summary>
        [Parameter]
        public int TabSize { get; set; } = 4;

        protected string EncodedCode => WebUtility.HtmlEncode(Code ?? "");
        protected string CodeClass => Language == CodeLanguage.None ? null : $"language-{LanguageType.GetLanguageClass(Language)}";

        private ElementReference CodeElement;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Language != CodeLanguage.None)
            {
                await JS.InvokeVoidAsync("Prism.highlightElement", CodeElement);
            }
        }
    }
}
