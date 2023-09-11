using System.Text;

namespace AssParser.Lib
{
    public class UUEncode
    {
        private static readonly char[] EncLUT =
        {
            '!', '"', '#', '$', '%', '&', '\'', '(',
            ')', '*', '+', ',', '-', '.', '/', '0',
            '1', '2', '3', '4', '5', '6', '7', '8',
            '9', ':', ';', '<', '=', '>', '?', '@',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', '[', '\\', ']', '^', '_', '`',
        };
        /// <summary>
        /// Use UUEncode to encode byte[] data. Despite being called uuencoding by ass_specs.doc, the format is actually somewhat different from real uuencoding.
        /// Please refer to https://github.com/Aegisub/Aegisub/blob/6f546951b4f004da16ce19ba638bf3eedefb9f31/libaegisub/ass/uuencode.cpp for more information.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="insertBr">Whether break the line after 80 characters.</param>
        /// <param name="crlf">The linebreak type of source string. True if is CRLF.</param>
        /// <returns>UUEncoded string.</returns>
        public static string Encode(byte[] data, bool insertBr = true, bool crlf = true)
        {
            var written = 0;
            var curr = 0;
            var resLength = data.Length / 3 * 4 + (data.Length % 3 == 0 ? 0 : data.Length % 3 + 1);
            if (insertBr)
            {
                resLength += (resLength / 80 - (resLength % 80 == 0 ? 1 : 0)) * (crlf ? 2 : 1);
            }
            var res = new char[resLength];
            var dst = new char[4];
            var length = data.Length;
            for (var pos = 0; pos < length; pos += 3)
            {
                var numBytesRemain = Math.Min(length - pos, 3);

                dst[0] = EncLUT[(data[pos] >> 2) & 0x3f];
                switch (numBytesRemain)
                {
                    case 1:
                        dst[1] = EncLUT[(data[pos + 0] << 4) & 0x3f];
                        break;
                    case 2:
                        dst[1] = EncLUT[(data[pos + 0] << 4) & 0x3f | (data[pos + 1] >> 4) & 0x0f];
                        dst[2] = EncLUT[(data[pos + 1] << 2) & 0x3f];
                        break;
                    case 3:
                        dst[1] = EncLUT[(data[pos + 0] << 4) & 0x3f | (data[pos + 1] >> 4) & 0x0f];
                        dst[2] = EncLUT[(data[pos + 1] << 2) & 0x3f | (data[pos + 2] >> 6) & 0x03];
                        dst[3] = EncLUT[(data[pos + 2] << 0) & 0x3f];
                        break;
                }
                for (var i = 0; i < numBytesRemain + 1; i++)
                {
                    res[curr++] = dst[i];
                    written++;
                    if (insertBr && written == 80 && numBytesRemain == 3)
                    {
                        if (crlf) res[curr++] = '\r';
                        res[curr++] = '\n';
                        written = 0;
                    }
                }
            }
            return new string(res);
        }
        /// <summary>
        /// Use UUEncode to decode byte[] data. Despite being called uuencoding by ass_specs.doc, the format is actually somewhat different from real uuencoding.
        /// Please refer to https://github.com/Aegisub/Aegisub/blob/6f546951b4f004da16ce19ba638bf3eedefb9f31/libaegisub/ass/uuencode.cpp for more information.
        /// </summary>
        /// <param name="data">UUEncoded string.</param>
        /// <param name="crlf">The linebreak type of source string. True if is CRLF.</param>
        /// <returns>UUDecoded byte[].</returns>
        public static byte[] Decode(string data, out bool crlf)
        {
            crlf = false;
            var byteData = Encoding.ASCII.GetBytes(data);
            var length = byteData.Length;
            var curr = 0;
            var src = new byte[4];
            var res = new byte[length * 3 / 4];
            for (var pos = 0; pos + 1 < length;)
            {
                var numBytesRemain = Math.Min(length - pos, 4);
                var bytes = 0;
                for (var i = 0; i < numBytesRemain; ++pos)
                {
                    var c = byteData[pos];
                    if (c != '\n' && c != '\r')
                    {
                        src[i++] = (byte)(c - 33);
                        bytes++;
                    }
                    else
                    {
                        if (!crlf && c == '\r') crlf = true;
                    }
                }
                if (bytes > 1)
                    res[curr++] = (byte)((src[0] << 2) & 0xff | (src[1] >> 4) & 0x03);
                if (bytes > 2)
                    res[curr++] = (byte)((src[1] << 4) & 0xff | (src[2] >> 2) & 0x0f);
                if (bytes > 3)
                    res[curr++] = (byte)((src[2] << 6) & 0xff | (src[3] >> 0) & 0x3f);
            }
            Array.Resize(ref res, curr);
            return res;
        }
    }
}
