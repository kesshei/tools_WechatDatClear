using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WechatClear.Common
{
    public class WeChatDecode
    {
        static Dictionary<byte[], string> map = new Dictionary<byte[], string>() {
            { new byte[]{0xFF,0xD8 },"jpg" },
            { new byte[]{0x89,0x50 },"png" },
            { new byte[]{0x47,0x49 },"gif" }
        };
        public static byte[] Decode(byte[] data, int offset = 0)
        {
            byte[] fileBytesArr = data;
            byte key = 0;
            foreach (var item in map)
            {
                var needHead = item.Key;
                if ((fileBytesArr[0] ^ needHead[0]) == (fileBytesArr[1] ^ needHead[1]))
                {
                    key = (byte)(fileBytesArr[0] ^ needHead[0]);
                    break;
                }
            }
            if (key > 0)
            {
                for (int i = 0; i < fileBytesArr.Length; i++)
                {
                    fileBytesArr[i] ^= key;
                }
            }
            return fileBytesArr;
        }
    }
}
