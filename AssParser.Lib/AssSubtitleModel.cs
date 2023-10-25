namespace AssParser.Lib
{
    public class AssSubtitleModel
    {
        public ScriptInfo ScriptInfo { get; set; } = new();
        public Styles Styles { get; set; } = new();
        public Events Events { get; set; } = new();
        public List<string> Ord { get; set; } = new();
        public Dictionary<string, string> UnknownSections { get; set; } = new();
        public override string ToString()
        {
            using MemoryStream stream = new();
            using StreamWriter writer = new(stream, leaveOpen: true);
            AssParser.WriteToStreamAsync(this, writer).Wait();
            stream.Position = 0;
            using StreamReader reader = new(stream, leaveOpen: true);
            return reader.ReadToEnd();
        }
    }

    public class ScriptInfo
    {
        public ScriptInfo()
        {
            SciptInfoItems = new();
        }
        public Dictionary<string, string?> SciptInfoItems;
        public string? Title
        {
            set
            {
                SciptInfoItems["Title"] = value;
            }
            get
            {
                if (SciptInfoItems.TryGetValue("Title", out var item))
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }
        public string? ScriptType
        {
            set
            {
                SciptInfoItems["ScriptType"] = value;
            }
            get
            {
                if (SciptInfoItems.TryGetValue("ScriptType", out var item))
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }
        public int? WrapStyle
        {
            set
            {
                SciptInfoItems["WrapStyle"] = value.ToString();
            }
            get
            {
                if (SciptInfoItems.TryGetValue("WrapStyle", out var item))
                {
                    return Convert.ToInt32(item);
                }
                else
                {
                    return null;
                }
            }
        }
        public string? ScaledBorderAndShadow
        {
            set
            {
                SciptInfoItems["ScaledBorderAndShadow"] = value;
            }
            get
            {
                if (SciptInfoItems.TryGetValue("ScaledBorderAndShadow", out var item))
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }
        public string? YCbCrMatrix
        {
            set
            {
                SciptInfoItems["YCbCrMatrix"] = value;
            }
            get
            {
                if (SciptInfoItems.TryGetValue("YCbCrMatrix", out var item))
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }
        public int? PlayResX
        {
            set
            {
                SciptInfoItems["PlayResX"] = value.ToString();
            }
            get
            {
                if (SciptInfoItems.TryGetValue("PlayResX", out var item))
                {
                    return Convert.ToInt32(item);
                }
                else
                {
                    return null;
                }
            }
        }
        public int? PlayResY
        {
            set
            {
                SciptInfoItems["PlayResY"] = value.ToString();
            }
            get
            {
                if (SciptInfoItems.TryGetValue("PlayResY", out var item))
                {
                    return Convert.ToInt32(item);
                }
                else
                {
                    return null;
                }
            }
        }
    }
    public class Styles
    {
        public string[] Format;
        public List<Style> styles;
    }
    public struct Style
    {
        public string Name;
        public string Fontname;
        public string Fontsize;
        public string PrimaryColour;
        public string SecondaryColour;
        public string OutlineColour;
        public string BackColour;
        public string Bold;
        public string Italic;
        public string Underline;
        public string StrikeOut;
        public string ScaleX;
        public string ScaleY;
        public string Spacing;
        public string Angle;
        public string BorderStyle;
        public string Outline;
        public string Shadow;
        public string Alignment;
        public string MarginL;
        public string MarginR;
        public string MarginV;
        public string Encoding;
        public int LineNumber;
    }
    public class Events
    {
        public string[] Format;
        public List<Event> events;
    }
    public class Event
    {
        public EventType Type;
        public string Layer;
        public string Start;
        public string End;
        public string Style;
        public string Name;
        public string MarginL;
        public string MarginR;
        public string MarginV;
        public string Effect;
        public string Text;
        public int LineNumber;
    }

    public enum EventType
    {
        Comment,
        Dialogue
    }

    public class FontDetail : IEquatable<FontDetail?>
    {
        public string FontName = "";
        public string UsedChar = "";
        public int Bold;
        public bool IsItalic;

        public override bool Equals(object? obj)
        {
            return Equals(obj as FontDetail);
        }

        public bool Equals(FontDetail? other)
        {
            return other is not null &&
                   FontName == other.FontName &&
                   Bold == other.Bold &&
                   IsItalic == other.IsItalic;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FontName, Bold, IsItalic);
        }

        public static bool operator ==(FontDetail? left, FontDetail? right)
        {
            return EqualityComparer<FontDetail>.Default.Equals(left, right);
        }

        public static bool operator !=(FontDetail? left, FontDetail? right)
        {
            return !(left == right);
        }
    }
}
