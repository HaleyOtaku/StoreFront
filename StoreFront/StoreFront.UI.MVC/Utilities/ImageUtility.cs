﻿//IMAGE UPLOAD - STEP 08
//  - Create the Utilities folder in the UI Project
//  - Right click the folder > Add > Class > name it ImageUtility.cs
//  - Copy & paste the code from ImageUtility
//  - Add the using statements below

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace GadgetStore.UI.MVC.Utilities
{
    public class ImageUtility
    {
        public static void ResizeImage(string savePath, string fileName, Image image, int maxImgSize, int maxThumbSize)
        {
            //Get new proportional image dimensions based off current image size and maxImgSize
            int[] newImageSizes = GetNewSize(image.Width, image.Height, maxImgSize);
            //Resize the image to new dimensions returned from above
            Bitmap newImage = DoResizeImage(newImageSizes[0], newImageSizes[1], image);
            //save new image to path w/ filename
            newImage.Save(savePath + fileName);//calculate proportional size for thumbnail based on maxThumbSize            
            int[] newThumbSizes = GetNewSize(newImage.Width, newImage.Height, maxThumbSize);
            //Create thumbnail image
            Bitmap newThumb = DoResizeImage(newThumbSizes[0], newThumbSizes[1], image);
            //Save it with t_ prefix
            newThumb.Save(savePath + "t_" + fileName);
            //Clean up service
            newImage.Dispose(); newThumb.Dispose(); image.Dispose();
        }

        public static int[] GetNewSize(int imgWidth, int imgHeight, int maxImgSize)
        {
            // Calculate which dimension is being changed the most and use that as the aspect ratio for both sides
            float ratioX = (float)maxImgSize / (float)imgWidth;
            float ratioY = (float)maxImgSize / (float)imgHeight;
            float ratio = Math.Min(ratioX, ratioY);
            // Calculate the new width and height based on aspect ratio
            int[] newImgSizes = new int[2];
            newImgSizes[0] = (int)(imgWidth * ratio);
            newImgSizes[1] = (int)(imgHeight * ratio);
            // Return the new proportional image sizes
            return newImgSizes;
        }

        /// <summary>
        /// Perform image resize.
        /// </summary>
        /// <param name="imgWidth">Desired width</param>
        /// <param name="imgHeight">Desired height</param>
        /// <param name="image">Image to be resized</param>
        /// <returns></returns>
        public static Bitmap DoResizeImage(int imgWidth, int imgHeight, Image image)
        {
            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);
            // If the image format supports transparency apply transparency
            newImage.MakeTransparent();// Set image to screen resolution of 72 dpi (the resolution of monitors)
            newImage.SetResolution(72, 72);
            // Do the resize
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, imgWidth, imgHeight);
            }
            // Return the resized image
            return newImage;
        }

        public static void Delete(string path, string fileName)
        {

            FileInfo baseFile = new FileInfo(path + fileName);
            FileInfo thumbImg = new FileInfo(path + "t_" + fileName);

            //Check if designated file exists and, if so, delete it
            if (baseFile.Exists)
            {
                baseFile.Delete();
            }

            //Check if a thumbnail version exists for the file and, if so, delete it
            if (thumbImg.Exists)
            {
                thumbImg.Delete();
            }
        }
    }
}
