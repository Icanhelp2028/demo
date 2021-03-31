using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Core.Common.Generic
{
    public class Captcha
    {
        //private const string Letters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string Letters = "0123456789";

        public string GenerateImage(string captchaCode, int width, int height)
        {
            //验证码颜色集合
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

            //验证码字体集合
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial" };

            //定义图像的大小，生成图像的实例
            using var image = new Bitmap(width <= 0 ? captchaCode.Length * 25 : width, height);

            using var g = Graphics.FromImage(image);

            //背景设为白色
            g.Clear(Color.White);

            var random = new Random();

            for (var i = 0; i < 100; i++)
            {
                var x = random.Next(image.Width);
                var y = random.Next(image.Height);
                g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }

            //验证码绘制在g中
            for (var i = 0; i < captchaCode.Length; i++)
            {
                //随机颜色索引值
                var cindex = random.Next(c.Length);

                //随机字体索引值
                var findex = random.Next(fonts.Length);

                //字体
                using var f = new Font(fonts[findex], 15, FontStyle.Bold);

                //颜色  
                Brush b = new SolidBrush(c[cindex]);

                var ii = 4;
                if ((i + 1) % 2 == 0)
                    ii = 2;

                //绘制一个验证字符  
                g.DrawString(captchaCode.Substring(i, 1), f, b, 17 + (i * 17), ii);
            }

            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            return Convert.ToBase64String(ms.ToArray());
        }

        public string GenerateCode(int codeLength = 4)
        {
            var random = new Random();

            var captcheCode = string.Empty;

            for (int i = 0; i < codeLength; i++)
            {
                captcheCode += Letters[random.Next(Letters.Length)];
            }
            return captcheCode;
        }
    }
}