using System;
using System.Collections.Generic;
using System.Text;

namespace Playground.Lib.Extensions
{
    public static class StringExtensions
    {
        extension(string value)
        {
            public bool IsNullOrEmpty()
            {
                return string.IsNullOrWhiteSpace(value);
            }
        }
    }
}
