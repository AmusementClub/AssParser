using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssParser.Lib
{
    public static partial class AssParserExt
    {
        public static FontDetail[] UsedFonts(this AssSubtitleModel assSubtitle)
        {
            ConcurrentDictionary<string, HashSet<char>> result = new();
            BlockingCollection<FontDetail> words = new();
            Dictionary<string, string> styles = new();
            foreach (var style in assSubtitle.styles.styles)
            {
                styles.TryAdd(style.Name, style.Fontname);
            }
            Parallel.ForEach(assSubtitle.events.events, item =>
            {
                var curStyle = item.Style;
                var spLeft = item.Text.Split('{').ToList();
                Regex FsReg = FsRegFactory();
                Regex NonWord = NonWordFactory();
                if (!item.Text.StartsWith("{"))
                {
                    string text;
                    if (spLeft == null || spLeft.Count == 0)
                    {
                        text = item.Text;
                    }
                    else
                    {
                        text = spLeft[0];
                        spLeft.RemoveAt(0);
                    }
                    var word = NonWord.Replace(text, "");
                    if (word.Length > 0)
                    {
                        result.TryAdd(styles[item.Style], new());
                        words.Add(new() { FontName = styles[item.Style], UsedChar = word });
                    }
                }
                if (spLeft == null)
                {
                    return;
                }
                foreach (var s in spLeft)
                {
                    var spRight = s.Split('}');
                    string font;
                    if (spRight.Length > 0)
                    {
                        var match = FsReg.Matches(spRight[0]);
                        if (match.Count > 0)
                        {
                            var mat = match.Last();
                            font = mat.Groups[1].Value;
                        }
                        else
                        {
                            font = styles[item.Style];
                        }
                        if (spRight.Length > 1)
                        {
                            var word = NonWord.Replace(spRight[1], "");
                            if (word.Length > 0)
                            {
                                result.TryAdd(font, new());
                                words.Add(new() { FontName = font, UsedChar = word });
                            }
                        }
                    }

                }
            });
            foreach (var w in words)
            {
                foreach (var s in w.UsedChar)
                {
                    result[w.FontName].Add(s);
                }
            }
            var fonts = new FontDetail[result.Count];
            int i = 0;
            foreach (var s in result)
            {
                var sb = new StringBuilder();
                foreach (var c in s.Value)
                {
                    sb.Append(c);
                }
                fonts[i++] = new()
                {
                    FontName = s.Key,
                    UsedChar = sb.ToString()
                };
            }
            return fonts;
        }
        [GeneratedRegex("\\\\fn([^\\\\]+)", RegexOptions.Compiled)]
        private static partial Regex FsRegFactory();
        [GeneratedRegex("\\s|(\\\\n)|(\\\\N)|(\\\\h)")]
        private static partial Regex NonWordFactory();
    }
}
