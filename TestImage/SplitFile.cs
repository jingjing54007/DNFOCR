using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestImage
{
    class SplitFile
    {
        public static string OutDir = "d:\\Out";

        private string file;

        public SplitFile(string file)
        {
            this.file = file;
        }

        public int Page { set; get; }
        internal void SplitY()
        {
            Image fromImage = Image.FromFile(file);
            var bmp = new Bitmap(fromImage);

            var result = new List<Tuple<int, int>>();

            int startPos = 0;

            bool preHasContent = false;

            for (int j = 0; j < bmp.Height; j++)
            {
                bool? isBlack = null;
                bool hasContent = false;
                for (int i = 0; i < bmp.Width; i++)
                {
                    if (i == 0)
                    {
                        isBlack = Calculate(bmp.GetPixel(i, j));
                    }
                    else
                    {
                        var currentPixel = Calculate(bmp.GetPixel(i, j));
                        if (isBlack == currentPixel)
                        {
                            continue;
                        }
                        else
                        {
                            hasContent = true;
                            break;
                        }
                    }
                }
                if (hasContent && !preHasContent)
                {
                    startPos = j - 1;
                }
                if (!hasContent && preHasContent)
                {
                    result.Add(new Tuple<int, int>(startPos, j + 1));
                }
                preHasContent = hasContent;
            }

            int index = 1;

            var ctx = new DNFContext();
            foreach (var res in result)
            {
                var fileInfo = new FileInfo(file);
                var parentFolderName = fileInfo.Directory.FullName;
                var targetLocation = Path.Combine("d:\\Out\\Y", fileInfo.Name.Replace(fileInfo.Extension, "_" + index + "_" + fileInfo.Extension));
                if (!File.Exists(targetLocation))
                {
                    CaptureImageFromBmp(fromImage, targetLocation, fromImage.Width, res.Item2 - res.Item1, 0, res.Item1);
                }


                var name = SplitX(targetLocation);

                ctx.DNFItems.Add(new DNFItem { ItemId = Guid.NewGuid(), Name = name.Trim(), PageIndex = string.Format("SS_{0}_{1}", Page, index) });
                ctx.SaveChanges();

                index++;
            }
        }

        public string SplitX(string currentFile)
        {
            Image fromImage = Image.FromFile(currentFile);
            var bmp = new Bitmap(fromImage);
            List<Tuple<int, int>> allPos = new List<Tuple<int, int>>();

            int startPos = 0;

            bool preHasContent = false;
            for (int i = 0; i < bmp.Width; i++)
            {
                bool? isBlack = null;
                bool hasContent = false;
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (j == 0)
                    {
                        isBlack = Calculate(bmp.GetPixel(i, j));
                    }
                    else
                    {
                        var currentPixel = Calculate(bmp.GetPixel(i, j));
                        if (isBlack == currentPixel)
                        {
                            continue;
                        }
                        else
                        {
                            hasContent = true;
                            break;
                        }
                    }
                }
                if (hasContent && !preHasContent)
                {
                    startPos = i - 1;
                }
                if (!hasContent && preHasContent)
                {
                    if (i - startPos < 11)
                    {
                        continue;
                    }
                    else
                    {
                        allPos.Add(new Tuple<int, int>(startPos, i + 1));
                    }
                }
                preHasContent = hasContent;
            }

            int index = 1;
            StringBuilder sb = new StringBuilder();
            foreach (var res in allPos)
            {
                var fileInfo = new FileInfo(currentFile);
                var parentFolderName = fileInfo.Directory.FullName;
                var targetLocation = Path.Combine("d:\\Out\\X", fileInfo.Name.Replace(fileInfo.Extension, "_" + index + "_" + fileInfo.Extension));

                if (!File.Exists(targetLocation))
                {
                    CaptureImageFromBmp(fromImage, targetLocation, res.Item2 - res.Item1, fromImage.Height, res.Item1, 0);
                }

                sb.Append(RunOCR(targetLocation));

                index++;
            }
            sb.AppendLine();
            Console.WriteLine(sb.ToString());

            return sb.ToString();
        }

        private string RunOCR(string targetLocation)
        {
            var ocr = new CustomOCR(targetLocation);
            return ocr.Run();
        }

        public static bool Calculate(Color color)
        {
            return (color.R + color.G + color.B) / 3 + 1 >= 128;
        }


        ///   <summary> 
        ///   从图片中截取部分生成新图 
        ///   </summary> 
        ///   <param   name= "sFromFilePath "> 原始图片 </param> 
        ///   <param   name= "saveFilePath "> 生成新图 </param> 
        ///   <param   name= "width "> 截取图片宽度 </param> 
        ///   <param   name= "height "> 截取图片高度 </param> 
        ///   <param   name= "spaceX "> 截图图片X坐标 </param> 
        ///   <param   name= "spaceY "> 截取图片Y坐标 </param> 
        public static void CaptureImageFromBmp(Image fromImage, string saveFilePath, int width, int height, int spaceX, int spaceY)
        {
            int x = 0;   //截取X坐标 
            int y = 0;   //截取Y坐标 
            //原图宽与生成图片宽   之差     
            //当小于0(即原图宽小于要生成的图)时，新图宽度为较小者   即原图宽度   X坐标则为0   
            //当大于0(即原图宽大于要生成的图)时，新图宽度为设置值   即width         X坐标则为   sX与spaceX之间较小者 
            //Y方向同理 
            int sX = fromImage.Width - width;
            int sY = fromImage.Height - height;
            if (sX > 0)
            {
                x = sX > spaceX ? spaceX : sX;
            }
            else
            {
                width = fromImage.Width;
            }
            if (sY > 0)
            {
                y = sY > spaceY ? spaceY : sY;
            }
            else
            {
                height = fromImage.Height;
            }

            //创建新图位图 
            Bitmap bitmap = new Bitmap(width, height);
            //创建作图区域 
            Graphics graphic = Graphics.FromImage(bitmap);
            //截取原图相应区域写入作图区 
            graphic.DrawImage(fromImage, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            //从作图区生成新图 
            Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
            //保存图象 
            saveImage.Save(saveFilePath, ImageFormat.Jpeg);
            //释放资源 
            saveImage.Dispose();
            bitmap.Dispose();
            graphic.Dispose();
        }
    }
}
