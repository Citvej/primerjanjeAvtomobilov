using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace primerjanjeAvtomobilov.Models {
    public class RazpoznavaAvtomobila {
        int[] uniformPatterns = new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 12, 14, 15, 16, 24, 28, 30, 31, 32,
            48, 56, 60, 62, 63, 64, 96, 112, 120, 124, 126, 127, 128, 129, 131, 135, 143, 159, 191, 192,
            193, 195, 199, 207, 223, 224, 225, 227, 231, 239, 240, 241, 243, 247, 248, 249, 251, 252,
            253, 254, 255 };
        #region LBP

        int boolToInt(bool[] arr) {
            double num = 0;
            for (int i = 0; i < 8; i++) { // on which end is the most significant bit?
                num += Convert.ToInt32(arr[i]) * Math.Pow(2, 7 - i);
            }
            /*for (int i = 0; i < 8; i++) { 
				num += Convert.ToInt32(arr[i]) * Math.Pow(2, i);
			}*/
            return Convert.ToInt32(num);
        }

        bool[] resetBin(bool[] bin) {
            for (int i = 0; i < 8; i++) {
                bin[i] = false;
            }
            return bin;
        }

        int isUniform(int pattern) {
            for (int i = 0; i < 58; i++) {
                if (pattern == uniformPatterns[i]) {
                    return i;
                }
            }
            return -1;
        }

        int[,] generateCoordinates(int x, int y) {
            int[,] coordinates = new int[,] {
				/* x--
				   ---
				   --- */
				{ x-1, y-1 }, 

				/* -x-
				   ---
				   --- */
				{ x-1, y }, 

				/* --x
				   ---
				   --- */
				{ x-1, y+1 }, 

				/* ---
				   --x
				   --- */
				{ x, y+1 },

				/* ---
				   ---
				   --x */
				{ x+1, y+1 },

				/* ---
				   ---
				   -x- */
				{ x+1, y },

				/* ---
				   ---
				   x-- */
				{ x+1, y-1 },

				/* ---
				   x--
				   --- */
				{ x, y-1 }
            };

            return coordinates;
        }

        List<int> LBP(Image<Gray, byte> img, int cellsize) { // regionSize - npr 16
            bool[] bin = new bool[8];
            for (int i = 0; i < 8; i++) {
                bin[i] = false;
            }

            List<int> histogram = new List<int>();

            int rows = img.Rows;
            int cols = img.Cols;

            int lenRows = rows - cellsize - 1;
            int lenCols = cols - cellsize - 1;

            // resize image
            // Image<Bgr, byte> cpimg = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);
            int tmpsize;
            if (img.Width - 2 % cellsize != 0) {
                tmpsize = img.Width + cellsize - (img.Width % cellsize); // +2 or not +2?
                img.Resize(tmpsize, img.Height, Emgu.CV.CvEnum.Inter.Linear);
            }
            if (img.Height - 2 % cellsize != 0) {
                tmpsize = img.Height + cellsize - (img.Height % cellsize);
                img.Resize(img.Width, tmpsize, Emgu.CV.CvEnum.Inter.Linear);
            }

            for (int i = 1; i < lenRows; i += cellsize) {
                for (int j = 1; j < lenCols; j += cellsize) {
                    histogram = new List<int>(new int[256]);
                    int innerLenX = i + cellsize;
                    int innerLenY = j + cellsize;
                    for (int k = i; k < innerLenX; k++) { // rows
                        for (int l = j; l < innerLenY; l++) { // cols
                            double centreGray = img[k, l].Intensity;

                            // clockwise
                            int[,] coordinates = generateCoordinates(k, l);
                            for (int m = 0; m < 8; m++) {
                                int x = coordinates[m, 0];
                                int y = coordinates[m, 1];
                                if (img[x, y].Intensity == centreGray)
                                    bin[m] = true;
                            }

                            // bin to int
                            int num = boolToInt(bin);
                            histogram[num]++;

                            // reset bin
                            bin = resetBin(bin);
                        }
                    }
                }
            }
            return histogram;
        }

        List<int> LBPu(Image<Gray, byte> img, int cellsize) {
            bool[] bin = new bool[8];
            for (int i = 0; i < 8; i++) {
                bin[i] = false;
            }

            List<int> histogram = new List<int>();

            int rows = img.Rows;
            int cols = img.Cols;

            int lenRows = rows - cellsize - 1;
            int lenCols = cols - cellsize - 1;

            // resize image
            // Image<Bgr, byte> cpimg = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);
            int tmpsize;
            if (img.Width - 2 % cellsize != 0) {
                tmpsize = img.Width + cellsize - (img.Width % cellsize); // +2 or not +2?
                img.Resize(tmpsize, img.Height, Emgu.CV.CvEnum.Inter.Linear);
            }
            if (img.Height - 2 % cellsize != 0) {
                tmpsize = img.Height + cellsize - (img.Height % cellsize);
                img.Resize(img.Width, tmpsize, Emgu.CV.CvEnum.Inter.Linear);
            }

            for (int i = 1; i < lenRows; i += cellsize) {
                for (int j = 1; j < lenCols; j += cellsize) {
                    histogram = new List<int>(new int[59]);
                    int innerLenX = i + cellsize;
                    int innerLenY = j + cellsize;
                    for (int k = i; k < innerLenX; k++) { // rows
                        for (int l = j; l < innerLenY; l++) { // cols
                            double centreGray = img[k, l].Intensity;

                            // clockwise
                            int[,] coordinates = generateCoordinates(k, l);
                            for (int m = 0; m < 8; m++) {
                                int x = coordinates[m, 0];
                                int y = coordinates[m, 1];
                                if (img[x, y].Intensity == centreGray)
                                    bin[m] = true;
                            }

                            // bin to int
                            int num = boolToInt(bin);
                            if (isUniform(num) != -1) {
                                histogram[isUniform(num)]++;
                            }

                            // reset bin
                            bin = resetBin(bin);
                        }
                    }
                }
            }
            return histogram;
        }

        List<int> LBPd(Image<Gray, byte> img, int cellsize, int distance) {
            bool[] bin = new bool[8];
            for (int i = 0; i < 8; i++) {
                bin[i] = false;
            }

            List<int> histogram = new List<int>();

            int rows = img.Rows;
            int cols = img.Cols;

            int lenRows = rows - cellsize - 1;
            int lenCols = cols - cellsize - 1;

            // resize image
            // Image<Bgr, byte> cpimg = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);
            int tmpsize;
            if (img.Width - 2 % cellsize != 0) {
                tmpsize = img.Width + cellsize - (img.Width % cellsize);
                img.Resize(tmpsize, img.Height, Emgu.CV.CvEnum.Inter.Linear);
            }
            if (img.Height - 2 % cellsize != 0) {
                tmpsize = img.Height + cellsize - (img.Height % cellsize);
                img.Resize(img.Width, tmpsize, Emgu.CV.CvEnum.Inter.Linear);
            }

            for (int i = 1; i < lenRows; i += cellsize) {
                for (int j = 1; j < lenCols; j += cellsize) {
                    histogram = new List<int>(new int[256]);
                    int innerLenX = i + cellsize;
                    int innerLenY = j + cellsize;
                    for (int k = i; k < innerLenX; k++) { // rows
                        for (int l = j; l < innerLenY; l++) { // cols

                            // clockwise
                            int[,] coordinates = generateCoordinates(k, l);
                            int next = 0;
                            for(int m = 0; m < 8; m++) {
                                next = m + distance % distance;
                                int x = coordinates[m, 0];
                                int y = coordinates[m, 1];
                                int xNext = coordinates[next, 0];
                                int yNext = coordinates[next, 1];
                                if (img[x, y].Intensity == img[xNext, yNext].Intensity)
                                    bin[m] = true;
                            }

                            // bin to int
                            int num = boolToInt(bin);
                            histogram[num]++;

                            // reset bin
                            bin = resetBin(bin);
                        }
                    }
                }
            }
            return histogram;
        }
        #endregion


        #region HOG
        List<double> gradientVector(double[,] power, double[,] orientation, int startX, int startY, int blockSize) {
            IDictionary<int, double> gradient = new Dictionary<int, double>() {
                { 0, 0 }, { 20, 0 }, { 40, 0 }, { 60, 0 }, { 80, 0 }, { 100, 0 },
                { 120, 0 }, { 140, 0 }, { 160, 0 }, { 180, 0 }
            };
            double gP;
            double gO;

            int lenX = startX + blockSize;
            int lenY = startY + blockSize;
            for (int i = startX; i < lenX; i++) {
                for (int j = startY; j < lenY; j++) {
                    gP = power[i, j];
                    gO = orientation[i, j];

                    for (int k = 0; k < 180; k += 20) {
                        int nextbin = k + 20;
                        // gO equals bin
                        if (gO == k) { // what to do in case of 0? fix this
                            gradient[k] += gP;
                            break;
                        }
                        // gO does not equal bin
                        else if (k < gO && nextbin > gO) {
                            gradient[k] += (gO - k) / nextbin * gP; // 117, 6: bin 100 = 3/20*6 
                            gradient[nextbin] += (nextbin - gO) / nextbin * gP; // 117, 6: bin 120 = 17/20*6
                            break;
                        }
                    }
                }
            }
            // convert dict to list?
            List<double> values = new List<double>();
            foreach (KeyValuePair<int, double> bin in gradient) {
                values.Add(bin.Value);
            }

            return values;
        }

        // for a region
        List<double> region(double[,] power, double[,] orientation, int startX, int startY, int blockSize, int regionSize) {
            List<double> bins = new List<double>();
            List<double> tmp;
            int len = blockSize * regionSize;
            for (int i = startX; i < len; i += blockSize) {
                for (int j = startY; j < len; j += blockSize) {
                    tmp = gradientVector(power, orientation, i, j, blockSize);
                    bins = bins.Concat(tmp).ToList();
                }
            }

            // normalize each element in list
            // "vse elemente kvadriramo, jih med sabo seštejemo in vrednost korenimo"
            double sum = 0;
            for (int i = 0; i < bins.Count; i++) {
                sum += Math.Pow(bins[i], 2);
            }
            double L2 = Math.Sqrt(sum);

            // "vsak element delimo z L2"
            if(L2 > 0) {
                for (int i = 0; i < bins.Count; i++) {
                    bins[i] = bins[i] / L2;
                }
            }
            
            return bins;
        }

        // main
        List<double> HOG(Image<Bgr, byte> img, int cellsize, int regions) {
            // resize 135x150
            // cellsize 15
            // blocksize 2
            // bins 9 > [0|20|40|60|80|100|120|140|160]
            Image<Gray, byte> grayImg = img.Convert<Gray, byte>();

            // resize image
            // Image<Bgr, byte> cpimg = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);
            int tmpsize;
            if (grayImg.Width % cellsize != 0) {
                tmpsize = grayImg.Width + cellsize - (grayImg.Width % cellsize);
                grayImg.Resize(tmpsize, grayImg.Height, Emgu.CV.CvEnum.Inter.Linear);
            }
            if (grayImg.Height % cellsize != 0) {
                tmpsize = grayImg.Height + cellsize - (grayImg.Height % cellsize);
                grayImg.Resize(grayImg.Width, tmpsize, Emgu.CV.CvEnum.Inter.Linear);
            }

            // preprocessing
            // sobel
            Image<Gray, float> sobelX = grayImg.Sobel(1, 0, 1);
            Image<Gray, float> sobelY = grayImg.Sobel(0, 1, 1);
            int rows = sobelX.Rows;
            int columns = sobelX.Cols;
            //Image<Gray, float> power = new Image<Gray, float>(sobelX.Width, sobelX.Height);
            //Image<Gray, float> orientation = new Image<Gray, float>(sobelX.Width, sobelX.Height);
            double[,] power = new double[rows, columns];
            double[,] orientation = new double[rows, columns];

            // power/orientation vectors
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    double x = sobelX[i, j].Intensity;
                    double y = sobelY[i, j].Intensity;

                    // power vector
                    power[i, j] = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)); // values range up to 1000 - why?

                    // orientation vector 
                    orientation[i, j] = Math.Abs(Math.Atan2(y, x) * (180 / Math.PI));
                }
            }

            // debug
            //debugBox1.Text = printMatrix(power);
            //debugBox2.Text = printMatrix(orientation);

            // loop through regions
            List<double> histogram = new List<double>();
            List<double> tmp = new List<double>();
            int step = cellsize * regions / 2;
            for (int i = 0; i < grayImg.Width; i += step) {
                for (int j = 0; j < grayImg.Height; j += step) { // would it go out of bounds?
                    tmp = region(power, orientation, i, j, cellsize, regions);
                    histogram = histogram.Concat(tmp).ToList();
                }
            }

            return histogram;
        }
        #endregion

        #region primerjava

        double diff(List<double> hist1, List<double> hist2) { // hist1 in hist2 OBVEZNO enake velikosti!!!!!
            int len = hist1.Count;
            double[] diff = new double[len];

            for(int i = 0; i < len; i++) {
                diff[i] = ((float)Math.Abs(hist1[i] - hist2[i])) / len;
            }

            return diff.Sum() / len * 100;
        }

        // overload za LBP
        double diff(List<int> hist1, List<int> hist2) { 
            int len = hist1.Count;
            double[] diff = new double[len];

            for (int i = 0; i < len; i++) {
                diff[i] = ((float)Math.Abs(hist1[i] - hist2[i])) / len;
            }

            return diff.Sum() / len * 100;
        }

        double compare(string filename1, string filename2) {
            Bitmap bmp1 = new Bitmap(filename1);
            Bitmap bmp2 = new Bitmap(filename2);

            if (bmp1.Width != bmp2.Width && bmp1.Height != bmp2.Height) {
                int minwidth = (bmp1.Width > bmp2.Width) ? bmp2.Width : bmp1.Width;
                int minheight = (bmp1.Height > bmp2.Height) ? bmp2.Height : bmp1.Height;

                Rectangle r = new Rectangle(0, 0, minwidth, minheight);
                bmp1 = bmp1.Clone(r, bmp1.PixelFormat);
                bmp2 = bmp2.Clone(r, bmp2.PixelFormat);
            }

            Image<Bgr, byte> image1 = new Image<Bgr, byte>(bmp1);
            Image<Bgr, byte> image2 = new Image<Bgr, byte>(bmp2);

            List<double> HOG1 = HOG(image1, 8, 2);
            List<double> HOG2 = HOG(image2, 8, 2);
            double diffHOG = diff(HOG1, HOG2);

            List<int> LBP1 = LBP(image1.Convert<Gray, byte>(), 10);
            List<int> LBP2 = LBP(image2.Convert<Gray, byte>(), 10);
            double diffLBP = diff(LBP1, LBP2);

            List<int> LBPu1 = LBPu(image1.Convert<Gray, byte>(), 10);
            List<int> LBPu2 = LBPu(image2.Convert<Gray, byte>(), 10);
            double diffLBPu = diff(LBPu1, LBPu2);

            List<int> LBPd1 = LBPd(image1.Convert<Gray, byte>(), 10, 2);
            List<int> LBPd2 = LBPd(image2.Convert<Gray, byte>(), 10, 2);
            double diffLBPd = diff(LBPd1, LBPd2);

            return (diffHOG + diffLBP + diffLBPu + diffLBPd)/4;
        }
        #endregion
    }
}
