using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
            ConcurrentDictionary<FontDetail, ConcurrentDictionary<char, byte>> result = new();
            BlockingCollection<FontDetail> words = new();
            Dictionary<string, Style> styles = new();
            foreach (var style in assSubtitle.Styles.styles)
            {
                styles.TryAdd(style.Name, style);
            }
            Parallel.ForEach(assSubtitle.Events.events, item =>
            {
                var spLeft = item.Text.Split('{').ToList();
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
                    var word = text.Replace("\\N", "").Replace("\\n", "").Replace("\\h", "\u00A0");
                    if (word.Length > 0)
                    {
                        var bold = Convert.ToInt32(currentStyle.Bold);
                        var isItalic = currentStyle.Italic != "0";
                        var detail = new FontDetail()
                        {
                            FontName = currentStyle.Fontname,
                            UsedChar = word,
                            Bold = bold,
                            IsItalic = isItalic
                        };
                        var charDir =  result.GetOrAdd(detail, new ConcurrentDictionary<char, byte>());
                        foreach (var c in word)
                        {
                            charDir.TryAdd(c, 0);
                        }
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
                                    if (t.Length == 2 || t.Length == 4)
                                    {
                                        if (int.TryParse(t[1..], out var weight))
                                        {
                                            currentStyle.Bold = weight.ToString();
                                        }
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
                            var word = spRight[1].Replace("\\N", "").Replace("\\n", "").Replace("\\h", "\u00A0");
                            if (word.Length > 0)
                            {
                                var bold = Convert.ToInt32(currentStyle.Bold);
                                var isItalic = currentStyle.Italic != "0";
                                var detail = new FontDetail()
                                {
                                    FontName = currentStyle.Fontname,
                                    UsedChar = word,
                                    Bold = bold,
                                    IsItalic = isItalic
                                };
                                var charDir = result.GetOrAdd(detail, new ConcurrentDictionary<char, byte>());
                                foreach (var c in word)
                                {
                                    charDir.TryAdd(c, 0);
                                }
                            }
                        }
                    }
                }
            });
            var fonts = new FontDetail[result.Count];
            int i = 0;
            foreach (var s in result)
            {
                var sb = new StringBuilder();
                foreach (var c in s.Value)
                {
                    sb.Append(c.Key);
                }
                s.Key.UsedChar = sb.ToString();
                fonts[i++] = s.Key;
            }
            return fonts;
        }
    }
}
