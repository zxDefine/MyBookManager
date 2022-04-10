using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MyBookManager
{
    public static class CommonFunction
    {
        public static async Task<string> ResizeImageAndChangeToBase64(StorageFile imagefile, int reqWidth, int reqHeight)
        {
            //open file as stream
            using (IRandomAccessStream fileStream = await imagefile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                //如果Image大小比变化后的Size小的话
                if (decoder.PixelWidth <= reqWidth && decoder.PixelHeight <= reqHeight) return "";

                var resizedStream = new InMemoryRandomAccessStream();

                BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                double widthRatio = (double)reqWidth / decoder.PixelWidth;
                double heightRatio = (double)reqHeight / decoder.PixelHeight;

                double scaleRatio = Math.Min(widthRatio, heightRatio);

                if (reqWidth == 0)
                    scaleRatio = heightRatio;

                if (reqHeight == 0)
                    scaleRatio = widthRatio;

                uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
                uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

                encoder.BitmapTransform.ScaledHeight = aspectHeight;
                encoder.BitmapTransform.ScaledWidth = aspectWidth;

                await encoder.FlushAsync();
                resizedStream.Seek(0);
                var outBuffer = new byte[resizedStream.Size];
                await resizedStream.ReadAsync(outBuffer.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);

                return Convert.ToBase64String(outBuffer);

                //输出到文件,这里并不需要输出,所以注释掉,但为了借鉴,保留代码
                //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                //StorageFile sampleFile = await storageFolder.CreateFileAsync("testZhou.jpg", CreationCollisionOption.ReplaceExisting);
                //await FileIO.WriteBytesAsync(sampleFile, outBuffer);
            }
        }
    }
}
