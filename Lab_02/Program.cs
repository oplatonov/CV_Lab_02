using System;
using OpenCvSharp;

namespace Lab_02
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Otsu - 1\n" + "Bradley - 2");
                string methodNumber = Console.ReadLine();
                string openCvOrCustom = default;
                bool isOpenCv = default;
                if (methodNumber == "1")
                {
                    Console.WriteLine("OpenCv - 1\n" + "Custom - 2");
                    openCvOrCustom = Console.ReadLine();
                    isOpenCv = Convert.ToInt32(openCvOrCustom) == 1 ? true : false;
                }

                switch (methodNumber)
                {
                    case "1":
                        GetOtsu(isOpenCv);
                        break;
                    case "2":
                        GetBradley();
                        break;
                    default:
                        throw new ArgumentException("Введены некорректные данные");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        static void GetOtsu(bool isOpenCv)
        {
            Mat image = Cv2.ImRead("pic.jpg");

            Mat gray = new(image.Rows, image.Cols, MatType.CV_8UC1);
            BinarizationExtention.ShowImage(image, "color");

            Cv2.CvtColor(image, gray, ColorConversionCodes.RGB2GRAY);
            BinarizationExtention.ShowImage(gray, "gray");

            if (isOpenCv)
            {
                Mat binary = gray.Threshold(0, 255, ThresholdTypes.Otsu);
                BinarizationExtention.ShowImage(binary, "binary");
            }
            else
            {
                BinarizationExtention.Put(gray, BinarizationExtention.Otsu(gray));
                BinarizationExtention.ShowImage(gray, "Otsu selfmade");
            }
        }

        static void GetBradley()
        {
            Mat image = Cv2.ImRead("pic.jpg");
            Mat gray = new(image.Rows, image.Cols, MatType.CV_8UC1);

            BinarizationExtention.ShowImage(image, "color");

            Cv2.CvtColor(image, gray, ColorConversionCodes.RGB2GRAY);

            BinarizationExtention.ShowImage(gray, "gray");

            BinarizationExtention.Bradley(gray);
        }
    }
}
