using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestImage
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = ListFile.List("d:\\装备名");
            //var files = ListFile.List(@"D:\Out\Y");

            //foreach (var file in files)
            //{
            //    var split = new SplitFile(file);
            //    split.SplitX(file);
            //}

            int i = 1;
            foreach (var file in files)
            {
                var split = new SplitFile(file);
                split.Page = i++;
                split.SplitY();
            }

        }

        public static List<int> CutX(Bitmap bmp)
        {
            List<int> Ys = new List<int>();

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
                    Ys.Add(j);
                }
                preHasContent = hasContent;
            }

            return Ys;
        }

        public static List<int> CutY(Bitmap bmp)
        {
            List<int> Xs = new List<int>();

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
                    Xs.Add(i);
                }
                preHasContent = hasContent;
            }

            return Xs;
        }

        public static bool Calculate(Color color)
        {
            return (color.R + color.G + color.B) / 3 + 1 >= 128;
        }

        #region   截取图象

        ///   <summary> 
        ///   从图片中截取部分生成新图 
        ///   </summary> 
        ///   <param   name= "sFromFilePath "> 原始图片 </param> 
        ///   <param   name= "saveFilePath "> 生成新图 </param> 
        ///   <param   name= "width "> 截取图片宽度 </param> 
        ///   <param   name= "height "> 截取图片高度 </param> 
        ///   <param   name= "spaceX "> 截图图片X坐标 </param> 
        ///   <param   name= "spaceY "> 截取图片Y坐标 </param> 
        public static void CaptureImage(string sFromFilePath, string saveFilePath, int width, int height, int spaceX, int spaceY)
        {
            //载入底图 
            Image fromImage = Image.FromFile(sFromFilePath);
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
        #endregion


        ///   <summary> 
        ///   从图片中截取部分生成新图 
        ///   </summary> 
        ///   <param   name= "sFromFilePath "> 原始图片 </param> 
        ///   <param   name= "saveFilePath "> 生成新图 </param> 
        ///   <param   name= "width "> 截取图片宽度 </param> 
        ///   <param   name= "height "> 截取图片高度 </param> 
        ///   <param   name= "spaceX "> 截图图片X坐标 </param> 
        ///   <param   name= "spaceY "> 截取图片Y坐标 </param> 
        public static void CaptureImageFromBmp(string sFromFilePath, string saveFilePath, int width, int height, int spaceX, int spaceY)
        {
            //载入底图 
            Image fromImage = Image.FromFile(sFromFilePath);
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




        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

    }
}

