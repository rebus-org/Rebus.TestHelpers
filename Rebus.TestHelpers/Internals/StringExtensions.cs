using System;
using System.Linq;

namespace Rebus.TestHelpers.Internals;

static class StringExtensions
{
    public static string Indented(this string str, int indent)
    {
        var indentedLines = str
            .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
            .Select(line => string.Concat(new string(' ', indent), line));

        return string.Join(Environment.NewLine, indentedLines);
    }
}