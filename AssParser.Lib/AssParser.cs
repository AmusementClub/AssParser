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
        /// <summary>
        /// Parse a single ass file asynchronously.
        /// </summary>
        /// <param name="assStream">A StreamReader of your ass file.</param>
        /// <returns></returns>
        /// <exception cref="Exception">If there is any unvalid part.</exception>
        public static async Task<AssSubtitleModel> ParseAssFile(StreamReader assStream)
        {
            AssSubtitleModel assSubtitleModel = new();
            string header;
            string body;
            while (assStream.Peek() > -1)
            {
                var tag = await assStream.ReadLineAsync();
                if (tag == null || tag.Length == 0)
                {
                    continue;
                }
                if (tag is not ['[', .., ']'])
                {
                    throw new AssParserException($"{tag} is not a valid section name", assStream, AssParserErrorType.InvalidSection);
                }
                assSubtitleModel.Ord.Add(tag);
                switch (tag)
                {
                    case "[Script Info]":
                        int commentCount = 0;
                        while (assStream.Peek() is not '[' and > -1)
                        {
                            if (assStream.Peek() is '\r' or '\n')
                            {
                                assStream.Read();
                                continue;
                            }
                            if (assStream.Peek() == ';')
                            {
                                assSubtitleModel.ScriptInfo.SciptInfoItems.Add($";{commentCount++}", await assStream.ReadLineAsync());
                                continue;
                            }
                            (header, body) = await ParseLine(assStream);
                            assSubtitleModel.ScriptInfo.SciptInfoItems.Add(header, body);
                        }
                        break;
                    case "[V4+ Styles]":
                        //[V4+ Styles]
                        //Read format line
                        (header, body) = await ParseLine(assStream);
                        if (header != "Format")
                        {
                            throw new AssParserException($"No format line", assStream, AssParserErrorType.MissingFormatLine);
                        }
                        assSubtitleModel.Styles.Format = body.Split(',');
                        for (int i = 0; i < assSubtitleModel.Styles.Format.Length; i++)
                        {
                            assSubtitleModel.Styles.Format[i] = assSubtitleModel.Styles.Format[i].Trim();
                        }
                        assSubtitleModel.Styles.styles = new();
                        //Read sytle lines
                        while (assStream.Peek() is not '[' and > -1)
                        {
                            if (assStream.Peek() is '\r' or '\n')
                            {
                                assStream.Read();
                                continue;
                            }
                            (header, body) = await ParseLine(assStream);
                            if (header != "Style")
                            {
                                throw new AssParserException($"Wrong Style Line", assStream, AssParserErrorType.InvalidStyleLine);
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
                                        throw new AssParserException($"Invalid style", assStream, AssParserErrorType.InvalidStyle);
                                }

                            }
                            assSubtitleModel.Styles.styles.Add(style);
                        }
                        break;
                    case "[Events]":
                        //Read Events
                        //Read format line
                        (header, body) = await ParseLine(assStream);
                        if (header != "Format")
                        {
                            throw new AssParserException($"No format line", assStream, AssParserErrorType.MissingFormatLine);
                        }
                        assSubtitleModel.Events.Format = body.Split(',');
                        assSubtitleModel.Events.events = new();
                        for (int i = 0; i < assSubtitleModel.Events.Format.Length; i++)
                        {

                            assSubtitleModel.Events.Format[i] = assSubtitleModel.Events.Format[i].Trim();
                        }

                        while (assStream.Peek() is not '[' and > -1)
                        {
                            if (assStream.Peek() is '\r' or '\n')
                            {
                                assStream.Read();
                                continue;
                            }
                            Event events = new();
                            (header, body) = await ParseLine(assStream);
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
                                throw new AssParserException($"Invalid event", assStream, AssParserErrorType.InvalidEvent);
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
                                        throw new AssParserException($"Invalid event", assStream, AssParserErrorType.InvalidEvent);
                                }
                            }
                            assSubtitleModel.Events.events.Add(events);
                        }
                        break;
                    default:
                        StringBuilder BodyBuffer = new();
                        while (assStream.Peek() is not '\r' and not '\n' and > -1)
                        {
                            BodyBuffer.AppendLine(await assStream.ReadLineAsync());
                        }
                        assSubtitleModel.UnknownSections.Add(tag, BodyBuffer.ToString());
                        break;
                }
            }
            return assSubtitleModel;
        }

        /// <summary>
        /// Parse a single ass file asynchronously.
        /// </summary>
        /// <param name="assFile">Path to your ass file.</param>
        /// <returns>Parsed AssSubtitleModel object.</returns>
        /// <exception cref="Exception">If there is any unvalid part.</exception>
        public static async Task<AssSubtitleModel> ParseAssFile(string assFile)
        {
            using StreamReader assStream = new(File.Open(assFile, FileMode.Open));
            return await ParseAssFile(assStream);
        }
        /// <summary>
        /// Build the ass file and write it into StreamWriter.
        /// </summary>
        /// <param name="assSubtitleModel">Ass model.</param>
        /// <param name="stream">Destination stream writer.</param>
        /// <returns>A Task.</returns>
        /// <exception cref="Exception">If there is any invalid element in the ass model.</exception>
        public static async Task WriteToStreamAsync(AssSubtitleModel assSubtitleModel, StreamWriter stream)
        {
            await stream.WriteLineAsync("[Script Info]");
            foreach (var item in assSubtitleModel.ScriptInfo.SciptInfoItems)
            {
                if (item.Key.StartsWith(";"))
                {
                    await stream.WriteLineAsync($"{item.Value}");
                }
                else
                {
                    await stream.WriteLineAsync($"{item.Key}: {item.Value}");
                }
            }
            await stream.WriteLineAsync();
            if (assSubtitleModel.UnknownSections.ContainsKey("[Aegisub Project Garbage]"))
            {
                await stream.WriteLineAsync("[Aegisub Project Garbage]");
                await stream.WriteLineAsync(assSubtitleModel.UnknownSections["[Aegisub Project Garbage]"]);
            }
            await stream.WriteLineAsync("[V4+ Styles]");
            await stream.WriteLineAsync($"Format: {string.Join(", ", assSubtitleModel.Styles.Format)}");
            foreach (var style in assSubtitleModel.Styles.styles)
            {
                await stream.WriteAsync("Style: ");
                for (int i = 0; i < assSubtitleModel.Styles.Format.Length; i++)
                {
                    switch (assSubtitleModel.Styles.Format[i])
                    {
                        case "Name":
                            await stream.WriteAsync(style.Name);
                            break;
                        case "Fontname":
                            await stream.WriteAsync(style.Fontname);
                            break;
                        case "Fontsize":
                            await stream.WriteAsync(style.Fontsize);
                            break;
                        case "PrimaryColour":
                            await stream.WriteAsync(style.PrimaryColour);
                            break;
                        case "SecondaryColour":
                            await stream.WriteAsync(style.SecondaryColour);
                            break;
                        case "OutlineColour":
                            await stream.WriteAsync(style.OutlineColour);
                            break;
                        case "BackColour":
                            await stream.WriteAsync(style.BackColour);
                            break;
                        case "Bold":
                            await stream.WriteAsync(style.Bold);
                            break;
                        case "Italic":
                            await stream.WriteAsync(style.Italic);
                            break;
                        case "Underline":
                            await stream.WriteAsync(style.Underline);
                            break;
                        case "StrikeOut":
                            await stream.WriteAsync(style.StrikeOut);
                            break;
                        case "ScaleX":
                            await stream.WriteAsync(style.ScaleX);
                            break;
                        case "ScaleY":
                            await stream.WriteAsync(style.ScaleY);
                            break;
                        case "Spacing":
                            await stream.WriteAsync(style.Spacing);
                            break;
                        case "Angle":
                            await stream.WriteAsync(style.Angle);
                            break;
                        case "BorderStyle":
                            await stream.WriteAsync(style.BorderStyle);
                            break;
                        case "Outline":
                            await stream.WriteAsync(style.Outline);
                            break;
                        case "Shadow":
                            await stream.WriteAsync(style.Shadow);
                            break;
                        case "Alignment":
                            await stream.WriteAsync(style.Alignment);
                            break;
                        case "MarginL":
                            await stream.WriteAsync(style.MarginL);
                            break;
                        case "MarginR":
                            await stream.WriteAsync(style.MarginR);
                            break;
                        case "MarginV":
                            await stream.WriteAsync(style.MarginV);
                            break;
                        case "Encoding":
                            await stream.WriteAsync(style.Encoding);
                            break;
                        default:
                            throw new Exception($"Invalid style {assSubtitleModel.Styles.Format[i]} in [V4+ Styles]");
                    }
                    if (i != assSubtitleModel.Styles.Format.Length - 1)
                    {
                        await stream.WriteAsync(',');
                    }
                }
                await stream.WriteAsync(Environment.NewLine);
            }
            await stream.WriteLineAsync();
            if (assSubtitleModel.UnknownSections.ContainsKey("[Fonts]"))
            {
                await stream.WriteLineAsync("[Fonts]");
                await stream.WriteLineAsync(assSubtitleModel.UnknownSections["[Fonts]"]);
            }
            if (assSubtitleModel.UnknownSections.ContainsKey("[Graphics]"))
            {
                await stream.WriteLineAsync("[Graphics]");
                await stream.WriteLineAsync(assSubtitleModel.UnknownSections["[Graphics]"]);
            }
            await stream.WriteLineAsync("[Events]");
            await stream.WriteAsync($"Format: {string.Join(", ", assSubtitleModel.Events.Format)}");
            foreach (var item in assSubtitleModel.Events.events)
            {
                await stream.WriteAsync(Environment.NewLine);
                await stream.WriteAsync($"{(item.Type == EventType.Dialogue ? "Dialogue" : "Comment")}: ");
                for (int i = 0; i < assSubtitleModel.Events.Format.Length; i++)
                {
                    switch (assSubtitleModel.Events.Format[i])
                    {
                        case "Layer":
                            await stream.WriteAsync(item.Layer);
                            break;
                        case "Start":
                            await stream.WriteAsync(item.Start);
                            break;
                        case "End":
                            await stream.WriteAsync(item.End);
                            break;
                        case "Style":
                            await stream.WriteAsync(item.Style);
                            break;
                        case "Name":
                            await stream.WriteAsync(item.Name);
                            break;
                        case "MarginL":
                            await stream.WriteAsync(item.MarginL);
                            break;
                        case "MarginR":
                            await stream.WriteAsync(item.MarginR);
                            break;
                        case "MarginV":
                            await stream.WriteAsync(item.MarginV);
                            break;
                        case "Effect":
                            await stream.WriteAsync(item.Effect);
                            break;
                        case "Text":
                            await stream.WriteAsync(item.Text);
                            break;
                        default:
                            throw new ($"Invalid style {assSubtitleModel.Events.Format[i]}");
                    }
                    if (i != assSubtitleModel.Events.Format.Length - 1)
                    {
                        await stream.WriteAsync(',');
                    }
                }
            }
            if (assSubtitleModel.UnknownSections.ContainsKey("[Aegisub Extradata]"))
            {
                await stream.WriteLineAsync();
                await stream.WriteLineAsync();
                await stream.WriteLineAsync("[Aegisub Extradata]");
                await stream.WriteAsync(assSubtitleModel.UnknownSections["[Aegisub Extradata]"]);
            }
            await stream.FlushAsync();
        }
    }
}
