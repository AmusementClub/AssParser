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
            Dictionary<string, Style> styles = new();
            foreach (var style in assSubtitle.styles.styles)
            {
                styles.TryAdd(style.Name, style);
            }
            Parallel.ForEach(assSubtitle.events.events, item =>
            {
                var spLeft = item.Text.Split('{').ToList();
                Regex NonWord = NonWordFactory();
                var currentStyle = styles[item.Style];
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
                        var isBold = currentStyle.Bold != "0";
                        var isItalic = currentStyle.Italic != "0";
                        result.TryAdd(currentStyle.Fontname + (isBold ? "_Bold" : "") + (isItalic ? "_Italic" : ""), new());
                        words.Add(new()
                        {
                            FontName = currentStyle.Fontname,
                            UsedChar = word,
                            IsBold = isBold,
                            IsItalic = isItalic
                        });
                    }
                }
                if (spLeft == null)
                {
                    return;
                }
                foreach (var s in spLeft)
                {
                    var spRight = s.Split('}');
                    if (spRight.Length > 0)
                    {
                        var tags = spRight[0].Split("\\");
                        foreach (var t in tags)
                        {
                            if (t.Length == 0)
                            {
                                continue;
                            }
                            switch (t[0])
                            {
                                case 'f':
                                    if (t.Length > 2 && t[1] == 'n')
                                    {
                                        currentStyle.Fontname = t[2..];
                                    }
                                    break;
                                case 'b':
                                    if (t.Length > 1)
                                    {
                                        currentStyle.Bold = t[1..];
                                    }
                                    break;
                                case 'i':
                                    if (t.Length == 2)
                                    {
                                        currentStyle.Italic = t[1..];
                                    }
                                    break;
                                case 'r':
                                    if (t.Length == 1)
                                    {
                                        currentStyle = styles[item.Style];
                                    }
                                    else if (t.Length > 1)
                                    {
                                        if (!styles.ContainsKey(t[1..]))
                                        {
                                            throw new Exception($"Style {t} not found");
                                        }
                                        currentStyle = styles[t[1..]];
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (spRight.Length > 1)
                        {
                            var word = NonWord.Replace(spRight[1], "");
                            if (word.Length > 0)
                            {
                                var isBold = currentStyle.Bold != "0";
                                var isItalic = currentStyle.Italic != "0";
                                result.TryAdd(currentStyle.Fontname + (isBold ? "_Bold" : "") + (isItalic ? "_Italic" : ""), new());
                                words.Add(new()
                                {
                                    FontName = currentStyle.Fontname,
                                    UsedChar = word,
                                    IsBold = isBold,
                                    IsItalic = isItalic
                                });
                            }
                        }
                    }
                }
            });
            foreach (var w in words)
            {
                foreach (var s in w.UsedChar)
                {
                    result[w.FontName + (w.IsBold ? "_Bold" : "") + (w.IsItalic ? "_Italic" : "")].Add(s);
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
        [GeneratedRegex("\\s|(\\\\n)|(\\\\N)|(\\\\h)")]
        private static partial Regex NonWordFactory();
    }
}
