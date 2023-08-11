namespace AssParser.Lib
{
    public class AssSubtitleModel
    {
        public ScriptInfo ScriptInfo { get; set; }
        public Styles styles { get; set; }
        public Events events { get; set; } 
    }
    public class ScriptInfo
    {
        public string[] Coments;
        public string Title;
        public string ScriptType;
        public int WrapStyle;
        public string ScaledBorderAndShadow;
        public string YCbCrMatrix;
        public int PlayResX;
        public int PlayResY;
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
    }

    public enum EventType
    {
        Comment,
        Dialogue
    }

    public class FontDetail
    {
        public string FontName;
        public string UsedChar;
    }
}
