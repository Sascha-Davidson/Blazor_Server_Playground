using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Playground.Lib.Extensions
{
    public static class HtmlExtensions
    {

        public static MarkupString Raw(this string content)
        {
            return (MarkupString)content;
        }
    }
}
