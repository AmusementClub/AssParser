using System.Text;

namespace AssParser.Lib
{
    public class UUEncode
    {
        public static string Encode(byte[] data, bool insertBr = true, bool crlf = true)
        {
            var written = 0;
            var curr = 0;
            //TODO Calculate exact res length to remove .TrimEnd('\0')
            var res = new char[(data.Length + 2) / 3 * 4 + (data.Length + 2) / 3 * 4 / 80 * 2];
            var dst = new byte[4];
            var length = data.Length;
            for (var pos = 0; pos < length; pos += 3)
            {
                var numBytesRemain = Math.Min(length - pos, 3);
                
                dst[0] = (byte)(data[pos] >> 2);
                dst[1] = 0;
                switch (numBytesRemain)
                {
                    case 2:
                        dst[1] = (byte)(((data[pos] & 0x3) << 4) | ((data[pos + 1] & 0xF0) >> 4));
                        dst[2] = 0;
                        break;
                    case 3:
                        dst[1] = (byte)(((data[pos] & 0x3) << 4) | ((data[pos + 1] & 0xF0) >> 4));
                        dst[2] = (byte)(((data[pos + 1] & 0xF) << 2) | ((data[pos + 2] & 0xC0) >> 6));
                        dst[3] = (byte)(data[pos + 2] & 0x3F);
                        break;
                }
                for (var i = 0; i < numBytesRemain + 1; i++)
                {
                    res[curr++] = (char)(dst[i] + 33);
                    written++;
                    if (insertBr && written == 80 && numBytesRemain == 3)
                    {
                        if (crlf) res[curr++] = '\r';
                        res[curr++] = '\n';
                        written = 0;
                    }
                }
            }
            return new string(res).TrimEnd('\0');
        }

        public static byte[] Decode(string data)
        {
            var byteData = Encoding.ASCII.GetBytes(data);
            var curr = 0;
            var src = new byte[4];
            var res = new byte[byteData.Length / 4 * 3];
            var length = byteData.Length;
            for (var pos = 0; pos + 1 < length;)
            {
                var numBytesRemain = Math.Min(length - pos, 4);
                var bytes = 0;
                for (var i = 0; i < numBytesRemain; ++pos)
                {
                    var c = byteData[pos];
                    if (c is not ((byte)'\n' or (byte)'\r'))
                    {
                        src[i++] = (byte)(c - 33);
                        bytes++;
                    }
                }
                if (bytes > 1)
                    res[curr++] = (byte)((src[0] << 2) | (src[1] >> 4));
                if (bytes > 2)
                    res[curr++] = (byte)(((src[1] & 0xF) << 4) | (src[2] >> 2));
                if (bytes > 3)
                    res[curr++] = (byte)(((src[2] & 0x3) << 6) | (src[3]));
            }
            Array.Resize(ref res, curr);
            return res;
        }
    }
}
