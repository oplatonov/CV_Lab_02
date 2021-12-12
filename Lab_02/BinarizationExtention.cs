using OpenCvSharp;

namespace Lab_02
{
    public static class BinarizationExtention
    {
        public static void Put(Mat gray, int T)
        {
            int height = gray.Rows;
            int width = gray.Cols;

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (gray.At<byte>(x, y) < T)
                        gray.At<byte>(x, y) = 0;
                    else
                        gray.At<byte>(x, y) = 255;
                }
            }
        }

        public static void Bradley(Mat gray)
        {
            int height = gray.Rows;
            int width = gray.Cols;

            //Ширина сектора, который будет ходить по интегральной матрице
            int s2 = width / 8;
            int s = s2 / 2;

            //Координаты сектора
            int x1, x2, y1, y2;

            //Порог
            const decimal t = 0.15m;

            uint[,] integralImage = new uint[height, width];

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (x == 0 & y == 0)
                        integralImage[x, y] = gray.At<byte>(x, y);
                    else if (x == 0)
                        integralImage[x, y] = gray.At<byte>(x, y) + integralImage[x, y - 1];
                    else if (y == 0)
                        integralImage[x, y] = gray.At<byte>(x, y) + integralImage[x - 1, y];
                    else
                        integralImage[x, y] = gray.At<byte>(x, y) + 
                                                integralImage[x - 1, y] + 
                                                integralImage[x, y - 1] - 
                                                integralImage[x - 1, y - 1];
                }
            }

            Mat image = new Mat(gray.Rows, gray.Cols, MatType.CV_8UC1);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    decimal temp = integralImage[i, j];
                    temp /= integralImage[height - 1, width - 1];
                    if (temp * 255 > 255)
                        image.At<byte>(i, j) = 255;
                    else if (temp * 255 < 0)
                        image.At<byte>(i, j) = 0;
                    else
                        image.At<byte>(i, j) = (byte)(temp * 255);
                }
            }

            // Бинаризация
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    x1 = x - s;
                    x2 = x + s;
                    y1 = y - s;
                    y2 = y + s;

                    if (x1 < 0)
                        x1 = 0;
                    if (x2 >= height)
                        x2 = height - 1;
                    if (y1 < 0)
                        y1 = 0;
                    if (y2 >= width)
                        y2 = width - 1;

                    int count = (x2 - x1) * (y2 - y1);

                    //Суммарная яркость
                    uint sum = integralImage[x2, y2] - integralImage[x2, y1] - integralImage[x1, y2] + integralImage[x1, y1];
                    if (gray.At<byte>(x, y) * count < sum * (1.0m - t))
                        image.At<byte>(x, y) = 0;
                    else
                        image.At<byte>(x, y) = 255;
                }
            }

            ShowImage(image, "Bradley");
        }

        public static int Otsu(Mat gray)
        {
            //Гистограмма
            int[] gist = new int[256];

            int height = gray.Rows;
            int weight = gray.Cols;

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < weight; y++)
                    gist[gray.At<byte>(x, y)]++;
            }

            //Максимальная дисперсия
            double max_S = -1;

            //Порог
            int T = 0;

            //Общее кол-во px на изображении
            int Sm = height * weight;

            double count1 = 0;
            double count2 = Sm;

            //Значение яркости
            int n = 256;

            int SmI1 = 0;
            int SmI2 = 0;

            for (int i = 0; i < gist.Length; i++)
                SmI2 += gist[i] * i;
            
            _ = SmI2 / count2;

            //Цикл вычисления порога
            //t - порог/номер ячейки масссива гистограммы
            for (int t = 0; t < n; t++)
            {
                int newbin = gist[t];
                count1 += newbin;
                count2 -= newbin;

                //Вероятность попадания
                double w1 = count1 / Sm;
                double w2 = 1 - w1;

                SmI1 += newbin * t;
                SmI2 -= newbin * t;

                //Мат. ожидание
                double mu1 = SmI1 / count1;
                double mu2 = SmI2 / count2;

                double d = mu1 - mu2;

                double sigma = w1 * w2 * d * d;

                if (sigma > max_S)
                {
                    T = t;
                    max_S = sigma;
                }
            }

            return T;
        }

        public static void ShowImage(Mat image, string message)
        {
            Cv2.ImShow(message, image);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}