﻿using System.Text;

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

        public async Task<AssSubtitleModel> ParseAssFile2(string assFile)
        {
            using StreamReader assfile = new(File.Open(assFile, FileMode.Open));
            AssSubtitleModel assSubtitleModel = new();
            assSubtitleModel.ScriptInfo = new ScriptInfo();
            if (await assfile.ReadLineAsync() != "[Script Info]")
            {
                throw new Exception("Script Info");
            }
            StringBuilder HeaderBuffer = new();
            StringBuilder BodyBuffer = new();
            string header;
            string body;
            List<string> commentsBuffer = new();
            while (assfile.Peek() != '[')
            {
                if (assfile.Peek() == '\r' || assfile.Peek() == '\n')
                {
                    assfile.Read();
                    continue;
                }
                if (assfile.Peek() == ';')
                {
                    assfile.Read();
                    commentsBuffer.Add(await assfile.ReadLineAsync() ?? "");
                }
                (header, body) = await ParseLine(assfile);
                int value;
                switch (header)
                {
                    case "Title":
                        assSubtitleModel.ScriptInfo.Title = body;
                        break;
                    case "ScriptType":
                        assSubtitleModel.ScriptInfo.ScriptType = body;
                        break;
                    case "WrapStyle":
                        if (int.TryParse(body, out value))
                        {
                            assSubtitleModel.ScriptInfo.WrapStyle = int.Parse(body);
                        }
                        else
                        {
                            throw new Exception("Wrong WrapStyle");
                        }
                        break;
                    case "ScaledBorderAndShadow":
                        assSubtitleModel.ScriptInfo.ScaledBorderAndShadow = body;
                        break;
                    case "YCbCr Matrix":
                        assSubtitleModel.ScriptInfo.YCbCrMatrix = body;
                        break;
                    case "PlayResX":
                        if (int.TryParse(body, out value))
                        {
                            assSubtitleModel.ScriptInfo.PlayResX = int.Parse(body);
                        }
                        else
                        {
                            throw new Exception("Wrong WrapStyle");
                        }
                        break;
                    case "PlayResY":
                        if (int.TryParse(body, out value))
                        {
                            assSubtitleModel.ScriptInfo.PlayResY = int.Parse(body);
                        }
                        else
                        {
                            throw new Exception("Wrong WrapStyle");
                        }
                        break;
                    default: break;
                }
                HeaderBuffer.Clear();
                BodyBuffer.Clear();
            }
            assSubtitleModel.ScriptInfo.Coments = commentsBuffer.ToArray();

            //[V4+ Styles]
            if (await assfile.ReadLineAsync() != "[V4+ Styles]")
            {
                throw new Exception("[V4+ Styles]");
            }

            //Read format line
            (header, body) = await ParseLine(assfile);
            if (header != "Format")
            {
                throw new("No format line");
            }
            assSubtitleModel.styles = new();
            assSubtitleModel.styles.Format = body.Split(',');
            for (int i = 0; i < assSubtitleModel.styles.Format.Length; i++)
            {
                assSubtitleModel.styles.Format[i] = assSubtitleModel.styles.Format[i].Trim();
            }
            assSubtitleModel.styles.styles = new();
            //Read sytle lines
            while (assfile.Peek() != '[')
            {
                if (assfile.Peek() == '\r' || assfile.Peek() == '\n')
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
                for (int i = 0; i < assSubtitleModel.styles.Format.Length; i++)
                {
                    switch (assSubtitleModel.styles.Format[i])
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
                            style.Fontname = data[i];
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
                assSubtitleModel.styles.styles.Add(style);
            }
            //Read Events
            if (await assfile.ReadLineAsync() != "[Events]")
            {
                throw new Exception("[Events]");
            }

            //Read format line
            (header, body) = await ParseLine(assfile);
            if (header != "Format")
            {
                throw new("No format line");
            }
            assSubtitleModel.events = new();
            assSubtitleModel.events.Format = body.Split(',');
            assSubtitleModel.events.events = new();
            for (int i = 0; i < assSubtitleModel.events.Format.Length; i++)
            {

                assSubtitleModel.events.Format[i] = assSubtitleModel.events.Format[i].Trim();
            }

            while (assfile.Peek() > -1)
            {
                if (assfile.Peek() == '\r' || assfile.Peek() == '\n')
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
                for (int i = 0; i < assSubtitleModel.events.Format.Length; i++)
                {
                    switch (assSubtitleModel.events.Format[i])
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
                assSubtitleModel.events.events.Add(events);
            }
            return assSubtitleModel;
        }
    }
}