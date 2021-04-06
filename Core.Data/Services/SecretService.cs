using Core.Common.Services;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Data.Services
{
    /// <summary>使用自定义的base64，使其无法暴力破解</summary>
    public sealed class SecretService : ISecretService
    {
        private static readonly byte[] KEY = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        //private static readonly byte[] IV = new byte[] { 65, 20, 91, 123, 102, 126, 105, 28, 15, 13, 51, 32, 53, 45, 97, 40 };
        private static readonly string HEX = "mngwsutcevorzayx";

        /// <summary>解密</summary>
        public string Decrypt(string encrypt, string iv)
        {
            byte[] byteArray = ChaoticBase64.FromBase64String(encrypt);
            byte[] IV = Encoding.ASCII.GetBytes(iv);

            using var ms = new MemoryStream();
            using var des = new DESCryptoServiceProvider();
            using var crypto = des.CreateDecryptor(KEY, IV);
            using var cs = new CryptoStream(ms, crypto, CryptoStreamMode.Write);

            cs.Write(byteArray, 0, byteArray.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        /// <summary>加密</summary>
        public string Encrypt(string plain, string iv)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(plain);
            byte[] IV = Encoding.ASCII.GetBytes(iv);

            using var ms = new MemoryStream();
            using var des = new DESCryptoServiceProvider();
            using var crypto = des.CreateEncryptor(KEY, IV);
            using var cs = new CryptoStream(ms, crypto, CryptoStreamMode.Write);

            cs.Write(byteArray, 0, byteArray.Length);
            cs.FlushFinalBlock();

            return ChaoticBase64.ToBase64String(ms.ToArray());
        }

        /// <summary>Hash</summary>
        public string Hash(string plain)
        {
            using MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = ChaoticBase64.FromBase64String(plain);
            byte[] bytes = md5.ComputeHash(fromData);

            return ChaoticHex(bytes);
        }

        /// <summary>自定义打乱的hex</summary>
        private static string ChaoticHex(byte[] bytes)
        {
            int index = 0;
            int count = bytes.Length;
            char[] chars = new char[count + count];
            for (int i = 0; i < count; i++)
            {
                chars[index++] = HEX[(bytes[i] & 0xF0) >> 4];
                chars[index++] = HEX[bytes[i] & 0x0F];
            }

            return new string(chars);
        }

        /// <summary>自定义打乱的base64</summary>
        private static class ChaoticBase64
        {
            /// <summary>使用符合变量命名的字符</summary>
            private static readonly string base64Table = "Xk2VwC5jQt3d6Ia4U8xP1sNJL$EfA9B0FZhKiybgOlSDYcemoznHTWMuv_p7rqGR";
            /// <summary>字符映射表</summary>
            private static readonly int[] base64Index = new int[]
            {
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,25,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,31,20,
                02,10,15,06,12,59,17,29,-1,-1,
                -1,-1,-1,-1,-1,28,30,05,43,26,
                32,62,51,13,23,35,24,54,22,40,
                19,08,63,42,52,16,03,53,00,44,
                33,-1,-1,-1,-1,57,-1,14,38,45,
                11,46,27,39,34,36,07,01,41,47,
                50,48,58,61,60,21,09,55,56,04,
                18,37,49
            };

            public static byte[] FromBase64String(string inData)
            {
                int inDataLength = inData.Length;
                int lengthmod4 = inDataLength % 4;
                int calcLength = (inDataLength - lengthmod4);
                byte[] outData = new byte[inDataLength / 4 * 3 + 3];
                int j = 0;
                int i;
                int num1, num2, num3, num4;
                for (i = 0; i < calcLength; i += 4, j += 3)
                {
                    num1 = base64Index[inData[i]];
                    num2 = base64Index[inData[i + 1]];
                    num3 = base64Index[inData[i + 2]];
                    num4 = base64Index[inData[i + 3]];
                    outData[j] = (byte)((num1 << 2) | (num2 >> 4));
                    outData[j + 1] = (byte)(((num2 << 4) & 0xf0) | (num3 >> 2));
                    outData[j + 2] = (byte)(((num3 << 6) & 0xc0) | (num4 & 0x3f));
                }
                i = calcLength;
                switch (lengthmod4)
                {
                    case 3:
                        num1 = base64Index[inData[i]];
                        num2 = base64Index[inData[i + 1]];
                        num3 = base64Index[inData[i + 2]];
                        outData[j] = (byte)((num1 << 2) | (num2 >> 4));
                        outData[j + 1] = (byte)(((num2 << 4) & 0xf0) | (num3 >> 2));
                        j += 2;
                        break;
                    case 2:
                        num1 = base64Index[inData[i]];
                        num2 = base64Index[inData[i + 1]];
                        outData[j] = (byte)((num1 << 2) | (num2 >> 4));
                        j += 1;
                        break;
                }
                Array.Resize(ref outData, j);
                return outData;
            }

            public static string ToBase64String(byte[] inData)
            {
                int inDataLength = inData.Length;
                int outDataLength = inDataLength / 3 * 4 + 4;
                char[] outData = new char[outDataLength];
                int lengthmod3 = inDataLength % 3;
                int calcLength = (inDataLength - lengthmod3);
                int j = 0;
                int i;
                for (i = 0; i < calcLength; i += 3, j += 4)
                {
                    outData[j] = base64Table[inData[i] >> 2];
                    outData[j + 1] = base64Table[((inData[i] & 0x03) << 4) | (inData[i + 1] >> 4)];
                    outData[j + 2] = base64Table[((inData[i + 1] & 0x0f) << 2) | (inData[i + 2] >> 6)];
                    outData[j + 3] = base64Table[(inData[i + 2] & 0x3f)];
                }
                i = calcLength;
                switch (lengthmod3)
                {
                    case 2:
                        outData[j] = base64Table[inData[i] >> 2];
                        outData[j + 1] = base64Table[((inData[i] & 0x03) << 4) | (inData[i + 1] >> 4)];
                        outData[j + 2] = base64Table[(inData[i + 1] & 0x0f) << 2];
                        j += 3;
                        break;
                    case 1:
                        outData[j] = base64Table[inData[i] >> 2];
                        outData[j + 1] = base64Table[(inData[i] & 0x03) << 4];
                        j += 2;
                        break;
                }
                return new string(outData, 0, j);
            }
        }
    }
}