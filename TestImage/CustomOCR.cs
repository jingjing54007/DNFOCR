using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestImage
{
    class CustomOCR
    {

        OCRContext Context = new OCRContext();

        static Dictionary<string, string> DbDictionary;


        static CustomOCR()
        {
            using (var ctx = new OCRContext())
            {
                DbDictionary = ctx.Values.ToDictionary(v => v.HashText, v => v.Value);
            }
        }

        private string targetLocation;

        public CustomOCR(string targetLocation)
        {
            this.targetLocation = targetLocation;
        }

        internal string Run()
        {
            var fileName = @"C:\Program Files (x86)\Tesseract-OCR\tesseract.exe";

            var img = Image.FromFile(targetLocation);
            var bmp = new Bitmap(img);

            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < bmp.Height; j++)
            {
                var val = 0x0;
                for (int i = 0; i < bmp.Width; i++)
                {
                    var isBlack = SplitFile.Calculate(bmp.GetPixel(i, j));
                    if (isBlack)
                    {
                        val |= (isBlack ? 1 : 0);
                        val = val << 1;
                    }
                }
                sb.Append(val.ToString("x4"));
            }

            var findValue = FindHashText(sb.ToString());

            if (findValue == null)
            {
                var p = Process.Start(new ProcessStartInfo(fileName, string.Format("{0} stdout -l chi_sim -psm 10", targetLocation))
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8
                });

                var value = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit();
                var newValue = new DicValue { HashText = sb.ToString(), Location = targetLocation, Value = value };
                Insert(newValue);
                DbDictionary[newValue.HashText] = newValue.Value;
                Console.WriteLine(newValue.Value);
                return value;
            }
            Console.WriteLine(findValue);
            return findValue;
        }

        public string FindHashText(string hashText)
        {
            double min = double.MaxValue;
            string key = "";

            foreach (var item in DbDictionary)
            {
                if (hashText == item.Key)
                {
                    return item.Value;
                }
                var compareRes = Compare(hashText, item.Key);
                if (compareRes < min)
                {
                    min = compareRes;
                    key = item.Key;
                }
            }
            if (min < 0.01)
            {
                return DbDictionary[key];
            }
            return null;
        }

        public double Compare(string src, string dest)
        {
            if (src.Length != dest.Length)
            {
                return int.MaxValue;
            }

            int ret = 0;
            int all = 0;

            for (int i = 0; i < src.Length; i += 4)
            {
                var srcTmp = src.Substring(i, 4);
                var destTmp = dest.Substring(i, 4);

                var srcInt = int.Parse(srcTmp, System.Globalization.NumberStyles.AllowHexSpecifier);
                var destInt = int.Parse(destTmp, System.Globalization.NumberStyles.AllowHexSpecifier);

                var result = srcInt ^ destInt;

                while (result != 0)
                {
                    all++;
                    if ((result & 0x1) == 0x1)
                    {
                        ret++;
                    }
                    result = result >> 1;
                }
            }
            return ((double)ret) / (src.Length * 4);
        }



        static object obj = new object();
        internal void Insert(DicValue value)
        {
            try
            {
                var findValue = Context.Values.Find(value.HashText);

                if (findValue == null)
                {
                    Context.Values.Add(value);
                    Context.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
