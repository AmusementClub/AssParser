using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssParser.Lib
{
    public class UUEncode
    {
        static public string Encode(byte[] data, bool insertBr = true)
        {
            int written = 0;
            int j = 0;
            //TODO Calculate exact res length to remove .TrimEnd('\0')
            char[] res = new char[(data.Length + 2) / 3 * 4 + (data.Length + 2) / 3 * 4 / 80 * 2];
            byte[] dst = new byte[4];
            for (int i = 0; i < data.Length; i += 3)
            {
                dst[0] = (byte)(data[i] >> 2);
                dst[1] = (byte)(((data[i] & 0x3) << 4) | ((data.ElementAtOrDefault(i + 1) & 0xF0) >> 4));
                dst[2] = (byte)(((data.ElementAtOrDefault(i + 1) & 0xF) << 2) | ((data.ElementAtOrDefault(i + 2) & 0xC0) >> 6));
                dst[3] = (byte)(data.ElementAtOrDefault(i + 2) & 0x3F);
                for (int k = 0; k < Math.Min(data.Length - i + 1, 4); k++)
                {
                    res[j++] = (char)(dst[k] + 33);
                    written++;
                    if (insertBr && written == 80 && i + 3 < data.Length)
                    {
                        res[j++] = '\r';
                        res[j++] = '\n';
                        written = 0;
                    }
                }
            }
            return new string(res).TrimEnd('\0');
        }
        static public byte[] Decode(string data)
        {
            List<byte> res = new(data.Length * 3 / 4);
            for (int i = 0; i + 1 < data.Length;)
            {
                int bytes = 0;
                byte[] src = new byte[4];
                for (int j = 0; j < 4 && i < data.Length; ++i)
                {
                    var c = data[i];
                    if (c is not ('\n' or '\r'))
                    {
                        src[j++] = (byte)(c - 33);
                        bytes++;
                    }
                }
                if (bytes > 1)
                    res.Add((byte)((src[0] << 2) | (src[1] >> 4)));
                if (bytes > 2)
                    res.Add((byte)(((src[1] & 0xF) << 4) | (src[2] >> 2)));
                if (bytes > 3)
                    res.Add((byte)(((src[2] & 0x3) << 6) | (src[3])));
            }
            return res.ToArray();
        }
    }
}
