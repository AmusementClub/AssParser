using System.Text;

namespace AssParser.Lib
{
    public class AssParser
    {
        static async Task<(string header, string body)> ParseLine(StreamReader streamReader)
        {
            var line = await streamReader.ReadLineAsync();
            if (line == null)
            {
                return ("", "");
            }
            var data = line.Split(':');
            string header = data[0].Trim();
            if (data.Length > 1)
            {
                string body = string.Join(':', data[1..]).Trim();
                return (header, body);
            }
            else
            {
                return (header, "");
            }
        }

        public static async Task<AssSubtitleModel> ParseAssFile(string assFile)
        {
            using StreamReader assfile = new(File.Open(assFile, FileMode.Open));
            AssSubtitleModel assSubtitleModel = new();
            string header;
            string body;
            while (assfile.Peek() > -1)
            {
                var tag = await assfile.ReadLineAsync();
                if (tag == null || tag.Length == 0)
                {
                    continue;
                }
                if (tag is not ['[', .., ']'])
                {
                    throw new($"{tag} is not a valid section name");
                }
                assSubtitleModel.Ord.Add(tag);
                switch (tag)
                {
                    case "[Script Info]":
                        while (assfile.Peek() is not '[' and > -1)
                        {
                            if (assfile.Peek() is '\r' or '\n')
                            {
                                assfile.Read();
                                continue;
                            }
                            if (assfile.Peek() == ';')
                            {
                                assfile.Read();
                                assSubtitleModel.ScriptInfo.SciptInfoItems.Add(";", await assfile.ReadLineAsync());
                            }
                            (header, body) = await ParseLine(assfile);
                            assSubtitleModel.ScriptInfo.SciptInfoItems.Add(header, body);
                        }
                        break;
                    case "[V4+ Styles]":
                        //[V4+ Styles]
                        //Read format line
                        (header, body) = await ParseLine(assfile);
                        if (header != "Format")
                        {
                            throw new("No format line");
                        }
                        assSubtitleModel.Styles.Format = body.Split(',');
                        for (int i = 0; i < assSubtitleModel.Styles.Format.Length; i++)
                        {
                            assSubtitleModel.Styles.Format[i] = assSubtitleModel.Styles.Format[i].Trim();
                        }
                        assSubtitleModel.Styles.styles = new();
                        //Read sytle lines
                        while (assfile.Peek() is not '[' and > -1)
                        {
                            if (assfile.Peek() is '\r' or '\n')
                            {
                                assfile.Read();
                                continue;
                            }
                            (header, body) = await ParseLine(assfile);
                            if (header != "Style")
                            {
                                throw new("Wrong Style Line");
                            }
                            var data = body.Split(",");
                            Style style = new();
                            for (int i = 0; i < assSubtitleModel.Styles.Format.Length; i++)
                            {
                                switch (assSubtitleModel.Styles.Format[i])
                                {
                                    case "Name":
                                        style.Name = data[i];
                                        break;
                                    case "Fontname":
                                        style.Fontname = data[i];
                                        break;
                                    case "Fontsize":
                                        style.Fontsize = data[i];
                                        break;
                                    case "PrimaryColour":
                                        style.PrimaryColour = data[i];
                                        break;
                                    case "SecondaryColour":
                                        style.SecondaryColour = data[i];
                                        break;
                                    case "OutlineColour":
                                        style.OutlineColour = data[i];
                                        break;
                                    case "BackColour":
                                        style.BackColour = data[i];
                                        break;
                                    case "Bold":
                                        style.Bold = data[i];
                                        break;
                                    case "Italic":
                                        style.Italic = data[i];
                                        break;
                                    case "Underline":
                                        style.Underline = data[i];
                                        break;
                                    case "StrikeOut":
                                        style.StrikeOut = data[i];
                                        break;
                                    case "ScaleX":
                                        style.ScaleX = data[i];
                                        break;
                                    case "ScaleY":
                                        style.ScaleY = data[i];
                                        break;
                                    case "Spacing":
                                        style.Spacing = data[i];
                                        break;
                                    case "Angle":
                                        style.Angle = data[i];
                                        break;
                                    case "BorderStyle":
                                        style.BorderStyle = data[i];
                                        break;
                                    case "Outline":
                                        style.Outline = data[i];
                                        break;
                                    case "Shadow":
                                        style.Shadow = data[i];
                                        break;
                                    case "Alignment":
                                        style.Alignment = data[i];
                                        break;
                                    case "MarginL":
                                        style.MarginL = data[i];
                                        break;
                                    case "MarginR":
                                        style.MarginR = data[i];
                                        break;
                                    case "MarginV":
                                        style.MarginV = data[i];
                                        break;
                                    case "Encoding":
                                        style.Encoding = data[i];
                                        break;
                                    default:
                                        throw new Exception("Invalid style");
                                }

                            }
                            assSubtitleModel.Styles.styles.Add(style);
                        }
                        break;
                    case "[Events]":
                        //Read Events
                        //Read format line
                        (header, body) = await ParseLine(assfile);
                        if (header != "Format")
                        {
                            throw new("No format line");
                        }
                        assSubtitleModel.Events.Format = body.Split(',');
                        assSubtitleModel.Events.events = new();
                        for (int i = 0; i < assSubtitleModel.Events.Format.Length; i++)
                        {

                            assSubtitleModel.Events.Format[i] = assSubtitleModel.Events.Format[i].Trim();
                        }

                        while (assfile.Peek() is not '[' and > -1)
                        {
                            if (assfile.Peek() is '\r' or '\n')
                            {
                                assfile.Read();
                                continue;
                            }
                            Event events = new();
                            (header, body) = await ParseLine(assfile);
                            if (header == "Comment")
                            {
                                events.Type = EventType.Comment;
                            }
                            else if (header == "Dialogue")
                            {
                                events.Type = EventType.Dialogue;
                            }
                            else
                            {
                                throw new Exception("Invalid event");
                            }
                            var data = body.Split(",");
                            for (int i = 0; i < assSubtitleModel.Events.Format.Length; i++)
                            {
                                switch (assSubtitleModel.Events.Format[i])
                                {
                                    case "Layer":
                                        events.Layer = data[i];
                                        break;
                                    case "Start":
                                        events.Start = data[i];
                                        break;
                                    case "End":
                                        events.End = data[i];
                                        break;
                                    case "Style":
                                        events.Style = data[i];
                                        break;
                                    case "Name":
                                        events.Name = data[i];
                                        break;
                                    case "MarginL":
                                        events.MarginL = data[i];
                                        break;
                                    case "MarginR":
                                        events.MarginR = data[i];
                                        break;
                                    case "MarginV":
                                        events.MarginV = data[i];
                                        break;
                                    case "Effect":
                                        events.Effect = data[i];
                                        break;
                                    case "Text":
                                        events.Text = string.Join(',', data[i..]);
                                        break;
                                    default:
                                        throw new Exception("Invalid event");
                                }
                            }
                            assSubtitleModel.Events.events.Add(events);
                        }
                        break;
                    default:
                        StringBuilder BodyBuffer = new();
                        while (assfile.Peek() is not '[' and > -1)
                        {
                            BodyBuffer.AppendLine(await assfile.ReadLineAsync());
                        }
                        assSubtitleModel.UnknownSections.Add(tag, BodyBuffer.ToString());
                        break;
                }
            }
            return assSubtitleModel;
        }
    }
}
