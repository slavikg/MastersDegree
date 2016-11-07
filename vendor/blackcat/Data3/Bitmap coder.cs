using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using Sapienware.Types;
using System.Windows.Forms;
using System.IO;
// 23.09.2009
using System.Collections;

namespace Bitmap_Coder
{
    public partial class MainForm : Form
    {
        private int kmax;

        // 16.11.2009
        private int kp;

        private int rows;
        private int col;
        // 23.09.2009
        private const double coef = 2.5;
        private DoubleVector xc;
        private List<DoubleVector> zList;
        private List<DoubleVector> zzList;

        // 26.11.2009
        private List<DoubleVector> XBList;
        //private List<DoubleVector> x2;
        private List<int> bk;

        // 23.09.2009
        private int[] nbits;
        BitArray ba;
        // 19.05.2010
        private int[] code;
        private int nbytes;
        private int ndwords;
        private int bits;

        private bool cmode;

        Bitmap myBitmap;
        Bitmap myBitmap2;
        Bitmap myBitmap3;
        Bitmap myBitmap4;
        Bitmap myBitmap5;
        Bitmap myBitmap6;

        // 26.01.2010
        private int blocks;

        // 31.01.2010
        private int height, width;
        double APSNR;


        // 08.03.2010
        private List<double> DList;

        // 25.04.2010
        private int[] nbits2;
        double PSNRmin;
        double PSNRl;
        double PSNRr;
        double PSNRm;
        private int kpl, kpr, kpm;

        // 07.05.2010
        private int bitsl, bitsr, bitsm;

        double SSIM;
        byte zeros;


        public MainForm()
        {
            InitializeComponent();
        }

        private bool Part1(Image image, int cols, int cols1)
        {
            col = cols*cols;
            double sum;
            // 29.09.2009
            /*
            while (text.Length % col != 0)
            {
                text += " ";
            }
            */

            // 02.11.2009
            if (cmode == false)
            rows = image.Height*image.Width / col;
            else
            rows = image.Height * image.Width / col * 3;

            //2
            //Encoding unicode = Encoding.Unicode;
            
            //byte[] unicodeBytes = unicode.GetBytes(text);
            //byte[] asciiBytes = Encoding.Convert(unicode, Encoding.GetEncoding(1251), unicodeBytes);

            DoubleMatrix x = new DoubleMatrix(rows, col);

            // 02.11.2009
            if (cmode == false)
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < col; j++)
            {
                //x[i, j] = (double)(byte)myBitmap.GetPixel( ((i * cols) / image.Width) * cols + j / cols, (i % col) * cols + j % cols).ToArgb() % 0x000000ff;

                // 28.10.2009
                x[i, j] = (double)((byte)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols).ToArgb() & 0x000000ff);


                /*if ((j / cols) % 2 == 0)
                    x[i, j] = myBitmap.GetPixel(i * cols + j % cols, j % cols);
                else
                    x[i, j] = myBitmap.GetPixel(i * cols + j % cols, cols - j % cols);
                 */
            }
            else
            // 02.11.2009
            for (int i = 0; i < rows/3; i++)
                for (int j = 0; j < col; j++)
            {
                //x[i, j] = (double)(byte)myBitmap.GetPixel( ((i * cols) / image.Width) * cols + j / cols, (i % col) * cols + j % cols).ToArgb() % 0x000000ff;

                x[i*3, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows/3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows/3 * col)) / cols)) * cols + j % cols).R);
                x[i*3+1, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows/3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows/3 * col)) / cols)) * cols + j % cols).G);
                x[i*3+2, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows/3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows/3 * col)) / cols)) * cols + j % cols).B);


                /*if ((j / cols) % 2 == 0)
                    x[i, j] = myBitmap.GetPixel(i * cols + j % cols, j % cols);
                else
                    x[i, j] = myBitmap.GetPixel(i * cols + j % cols, cols - j % cols);
                 */
            }
           
            //3
            DoubleVector xt = new DoubleVector(col);
            for (int i = 0; i < col; i++)
            {
                sum = 0;
                for (int j = 0; j < rows; j++)
                {
                    sum += x[j, i];
                }
                xt[i] = sum / rows;
            }

            //4
            xc = new DoubleVector(col);
            int index;
            int rmin = 0;
            double summin = 0;
            for (int r = 0; r < rows; r++)
            {
                sum = 0;
                for (index = 0; index < col; index++)
                {
                    sum += (xt[index] - x[r, index]) * (xt[index] - x[r, index]);
                }
                if ((r == 0) || (sum < summin))
                {
                    summin = sum;
                    rmin = r;
                }
            }
            for (index = 0; index < col; index++)
            {
                xc[index] = x[rmin, index];
            }

            //5
            DoubleMatrix x1 = new DoubleMatrix(x);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    x1[r, c] -= xc[c];
                }
            }

            int k = 0;
            zList = new List<DoubleVector>();
            zzList = new List<DoubleVector>();

            // 26.11.2009

            //x2 = new List<DoubleVector>();

            // 15.04.2010
            bk = new List<int>();


            XBList = new List<DoubleVector>();

            // 08.03.2010
            DList = new List<double>();


            DoubleMatrix x10 = new DoubleMatrix(x1);
            while (zero(x1, 0.0000000001) == false)
            {
                // 6
                // 15.04.2010
                bk.Add(b(x1));
                DoubleVector xB = new DoubleVector(x1.Row(b(x1)));
                // New part. Calculate Vi
                double sum1;
                double sum2;
                DoubleVector Vi = new DoubleVector(rows, 0.0);
                for (int rr=0; rr<cols1; rr++)
                {

                for (int r = 0; r < rows; r++)
                {
                    sum1 = 0;
                    sum2 = 0;
                    for (int c = 0; c < col; c++)
                    {
                        sum1 += x1[r, c] * xB[c];
                        sum2 += xB[c] * xB[c];
                    }
                    Vi[r] = sum1 / sum2;
                }
                //change xB
                for (int c = 0; c < col; c++)
                {
                    sum1 = 0;
                    sum2 = 0;
                    for (int r = 0; r < rows; r++)
                    {
                        sum1 += x1[r, c] * Vi[r];
                        sum2 += Vi[r] * Vi[r];
                    }
                    xB[c] = sum1 / sum2;
                }
            }
                //12
                //x2.Add(new DoubleVector(new DoubleVector(x10.Row(bk[bk.Count - 1]))));
                //7
                double Dk = D(xB);
                //8
                DoubleVector XB = new DoubleVector(xB);
                XB.Scale(1 / Dk);
                XBList.Add(new DoubleVector(XB));
                
                // 08.03.2010
                DList.Add(Dk);

                //9
                zList.Add(new DoubleVector(rows));
                zzList.Add(new DoubleVector(rows));
                for (int r = 0; r < rows; r++)
                {
                    sum = 0;
                    for (int c = 0; c < col; c++)
                    {
                        sum += x1[r, c] * xB[c];
                    }
                    zList[zList.Count - 1][r] = sum / Dk;
                    zzList[zList.Count - 1][r] = zList[zList.Count - 1][r] * 2;
                    zzList[zList.Count - 1][r] = Math.Round(zzList[zList.Count - 1][r]);
                }
                //10
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < col; c++)
                    {
                        x1[r, c] = x1[r, c] - XB[c] * zList[zList.Count - 1][r];
                    }
                }
                k++;
            }


/*            {
                // 6
                bk.Add(b(x1));
                DoubleVector xB = new DoubleVector(x1.Row(bk[bk.Count - 1]));
                //12
                x2.Add(new DoubleVector(new DoubleVector(x10.Row(bk[bk.Count - 1]))));
                //7
                double Dk = D(xB);
                //8
                DoubleVector XB = new DoubleVector(xB);
                XB.Scale(1 / Dk);
                //9
                zList.Add(new DoubleVector(rows));
                zzList.Add(new DoubleVector(rows));
                for (int r = 0; r < rows; r++)
                {
                    sum = 0;
                    for (int c = 0; c < col; c++)
                    {
                        sum += x1[r, c] * xB[c];
                    }
                    zList[zList.Count - 1][r] = sum / Dk;
                    zzList[zList.Count - 1][r] = zList[zList.Count - 1][r] * coef;
                    zzList[zList.Count - 1][r] = Math.Round(zzList[zList.Count - 1][r]);
                }
                //10
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < col; c++)
                    {
                        x1[r, c] = x1[r, c] - XB[c] * zList[zList.Count - 1][r];
                    }
                }
                k++;
            }
 */
            kmax = k - 1;
            return true;
        }

        private string Part2()
        {

            int cols = (int)Math.Sqrt(col);

            // 16.11.2009 kmax->kp
      
            DoubleMatrix XB = new DoubleMatrix(kp + 1, col);

            // 28.10.2009
            DoubleMatrix xb2 = new DoubleMatrix(kp + 1, col);
            //List<DoubleVector> xb2 = new List<DoubleVector>();

            // 26.11.2009
            DoubleMatrix x1 = new DoubleMatrix(rows, col, 0);


            // 07.05.2010
            uint mask = 0xffffffff;
            for (byte m=0; m<bitsm; m++)
                mask^=(uint)(1<<m);

            double Z;
            for (int k = kp; k >= 0; k--)
            {
                for (int i = 0; i < rows; i++)
                {
                    // 07.05.2010

                    // 24.05.2010
                    if (zzList[k][i]>0)
                    Z=(double)((uint)zzList[k][i] & mask);
                    else
                    Z = (double)-(~(((uint)zzList[k][i]) & mask)+1);

                    for (int j = 0; j < col; j++)
                    {
                        x1[i, j] += XBList[k][j] * (Z / 2);
                    }
                }
            }
            DoubleMatrix x = new DoubleMatrix(rows, col, 0);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    x[i, j] = x1[i, j] + xc[j];
                    x[i, j] = Math.Round(x[i, j]);
                }
            }

            /*for (int k = 0; k <= kp; k++)
                for (int j = 0; j < col; j++)
                    xb2[k,j] = x2[k][j];
            
            for (int k = 0; k <= kp+1; k++)
            {
                double sum = 0;
                
              
                for (int j = 0; j < col; j++)
                {
                    sum += x2[k][j] * x2[k][j];
                }
                for (int j = 0; j < col; j++)
                {
                    XB[k, j] = x2[k][j] / Math.Sqrt(sum);
                }
                if (k == kp)
                {
                    break;
                }
                for (int p = k + 1; p <= kp; p++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        x2[p][j] -= XB[k, j] * (zzList[k][bk[p]] / coef);
                    }
                }
            }

            DoubleMatrix x1 = new DoubleMatrix(rows, col, 0);
            for (int k = kp; k >= 0; k--)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        x1[i, j] += XB[k, j] * (zzList[k][i] / coef);
                    }
                }
            }
            DoubleMatrix x = new DoubleMatrix(rows, col, 0);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    x[i, j] = x1[i, j] + xc[j];
                    x[i, j] = Math.Round(x[i, j]);
                }
            }
            */

            // 16.11.2009
            double MSE = 0;
            double PSNR = 0;

            DoubleMatrix xo = new DoubleMatrix(rows, col);

            // 27.05.2010
            double mxR = 0, mxG = 0, mxB = 0;
            double myR = 0, myG = 0, myB = 0;
            double s2xR = 0, s2xG = 0, s2xB = 0;
            double s2yR = 0, s2yG = 0, s2yB = 0;
            double sxyR = 0, sxyG = 0, sxyB = 0;
            double c1 = 0, c2 = 0;
            uint L = 255;
            double k1 = 0.01, k2 = 0.03;
            double SSIMR = 0, SSIMG = 0, SSIMB = 0;


            if (cmode == false)
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < col; j++)
                    {
                        if (x[i, j] < 0) x[i, j] = 0;
                        if (x[i, j] > 255) x[i, j] = 255;
                        // 16.11.2009
                        xo[i, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols).ToArgb() & 0x000000ff);

                        //myBitmap2.SetPixel((i / col) * cols + j / cols, (i % col) * cols + j % cols,Color.FromArgb(Convert.ToByte(x[i, j]),Convert.ToByte(x[i, j]),Convert.ToByte(x[i, j])));
                        myBitmap2.SetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols, Color.FromArgb(Convert.ToByte(x[i, j]), Convert.ToByte(x[i, j]), Convert.ToByte(x[i, j])));
                        myBitmap3.SetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols, Color.FromArgb
                        (Convert.ToByte(255 - Math.Abs(xo[i, j] - x[i, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i, j] - x[i, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i, j] - x[i, j]))));
                        MSE = MSE += Math.Pow((Math.Abs(xo[i, j] - x[i, j])), 2);

                        // 27.05.2010
                        mxR += xo[i, j]; mxG += xo[i, j]; mxB += xo[i, j];
                        myR += x[i, j]; myG += x[i, j]; myB += x[i, j];

                    }
                MSE = MSE / (rows * col);

                // 27.05.2010
                mxR = mxR / (rows * col); mxG = mxG / (rows * col); mxB = mxB / (rows * col);
                myR = myR / (rows * col); myG = myG / (rows * col); myB = myB / (rows * col);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < col; j++)
                    {
                        s2xR += Math.Pow(xo[i, j] - mxR, 2); s2xG += Math.Pow(xo[i, j] - mxG, 2); s2xB += Math.Pow(xo[i, j] - mxB, 2);
                        s2yR += Math.Pow(x[i, j] - myR, 2); s2yG += Math.Pow(x[i, j] - myG, 2); s2yB += Math.Pow(x[i, j] - myB, 2);
                        sxyR += (xo[i, j] - mxR) * (x[i, j] - myR); sxyG += (xo[i, j] - mxG) * (x[i, j] - myG); sxyB += (xo[i, j] - mxB) * (x[i, j] - myB);
                    }
                s2xR = s2xR / (rows * col - 1); s2xG = s2xG / (rows * col - 1); s2xB = s2xB / (rows * col - 1);
                s2yR = s2yR / (rows * col - 1); s2yG = s2yG / (rows * col - 1); s2yB = s2yB / (rows * col - 1);
                sxyR = sxyR / (rows * col - 1); sxyG = sxyG / (rows * col - 1); sxyB = sxyB / (rows * col - 1);
                c1 = Math.Pow(k1 * L, 2); c2 = Math.Pow(k2 * L, 2);
                SSIMR = (2 * mxR * myR + c1) * (2 * sxyR + c2) / ((mxR * mxR + myR * myR + c1) * (s2xR + s2yR + c2));
                SSIMG = (2 * mxG * myG + c1) * (2 * sxyG + c2) / ((mxG * mxG + myG * myG + c1) * (s2xG + s2yG + c2));
                SSIMB = (2 * mxB * myB + c1) * (2 * sxyB + c2) / ((mxB * mxB + myB * myB + c1) * (s2xB + s2yB + c2));
                SSIM = (SSIMR + SSIMG + SSIMB) / 3;

                PSNR = 10 * Math.Log10(255 * 255 / MSE);
            }
            else
            // 02.11.2009
            {
                for (int i = 0; i < rows / 3; i++)
                    for (int j = 0; j < col; j++)
                    {
                        if (x[i * 3, j] < 0) x[i * 3, j] = 0;
                        if (x[i * 3, j] > 255) x[i * 3, j] = 255;
                        if (x[i * 3 + 1, j] < 0) x[i * 3 + 1, j] = 0;
                        if (x[i * 3 + 1, j] > 255) x[i * 3 + 1, j] = 255;
                        if (x[i * 3 + 2, j] < 0) x[i * 3 + 2, j] = 0;
                        if (x[i * 3 + 2, j] > 255) x[i * 3 + 2, j] = 255;

                        // 16.11.2009
                        xo[i * 3, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).R);
                        xo[i * 3 + 1, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).G);
                        xo[i * 3 + 2, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).B);

                        //myBitmap2.SetPixel((i / col) * cols + j / cols, (i % col) * cols + j % cols,Color.FromArgb(Convert.ToByte(x[i, j]),Convert.ToByte(x[i, j]),Convert.ToByte(x[i, j])));
                        myBitmap2.SetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols, Color.FromArgb(Convert.ToByte(x[i * 3, j]), Convert.ToByte(x[i * 3 + 1, j]), Convert.ToByte(x[i * 3 + 2, j])));
                        myBitmap3.SetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols, Color.FromArgb
                        (Convert.ToByte(255 - Math.Abs(xo[i * 3, j] - x[i * 3, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i * 3 + 1, j] - x[i * 3 + 1, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i * 3 + 2, j] - x[i * 3 + 2, j]))));
                        MSE = MSE += Math.Pow((Math.Abs(xo[i * 3, j] - x[i * 3, j])), 2);
                        MSE = MSE += Math.Pow((Math.Abs(xo[i * 3 + 1, j] - x[i * 3 + 1, j])), 2);
                        MSE = MSE += Math.Pow((Math.Abs(xo[i * 3 + 2, j] - x[i * 3 + 2, j])), 2);

                        // 27.05.2010
                        mxR += xo[i * 3, j]; mxG += xo[i * 3 + 1, j]; mxB += xo[i * 3 + 2, j];
                        myR += x[i * 3, j]; myG += x[i * 3 + 1, j]; myB += x[i * 3 + 2, j];


                    }

                MSE = MSE / (rows * col);
                PSNR = 10 * Math.Log10(255 * 255 / MSE);

                // 27.05.2010
                mxR = mxR / (rows * col / 3); mxG = mxG / (rows * col / 3); mxB = mxB / (rows * col / 3);
                myR = myR / (rows * col / 3); myG = myG / (rows * col / 3); myB = myB / (rows * col / 3);
                for (int i = 0; i < rows / 3; i++)
                    for (int j = 0; j < col; j++)
                    {
                        s2xR += Math.Pow(xo[i * 3, j] - mxR, 2); s2xG += Math.Pow(xo[i * 3 + 1, j] - mxG, 2); s2xB += Math.Pow(xo[i * 3 + 2, j] - mxB, 2);
                        s2yR += Math.Pow(x[i * 3, j] - myR, 2); s2yG += Math.Pow(x[i * 3 + 1, j] - myG, 2); s2yB += Math.Pow(x[i * 3 + 2, j] - myB, 2);
                        sxyR += (xo[i * 3, j] - mxR) * (x[i * 3, j] - myR); sxyG += (xo[i * 3 + 1, j] - mxG) * (x[i * 3 + 1, j] - myG); sxyB += (xo[i * 3 + 2, j] - mxB) * (x[i * 3 + 2, j] - myB);
                    }
                s2xR = s2xR / (rows * col / 3 - 1); s2xG = s2xG / (rows * col / 3 - 1); s2xB = s2xB / (rows * col / 3 - 1);
                s2yR = s2yR / (rows * col / 3 - 1); s2yG = s2yG / (rows * col / 3 - 1); s2yB = s2yB / (rows * col / 3 - 1);
                sxyR = sxyR / (rows * col / 3 - 1); sxyG = sxyG / (rows * col / 3 - 1); sxyB = sxyB / (rows * col / 3 - 1);
                c1 = Math.Pow(k1 * L, 2); c2 = Math.Pow(k2 * L, 2);
                SSIMR = (2 * mxR * myR + c1) * (2 * sxyR + c2) / ((mxR * mxR + myR * myR + c1) * (s2xR + s2yR + c2));
                SSIMG = (2 * mxG * myG + c1) * (2 * sxyG + c2) / ((mxG * mxG + myG * myG + c1) * (s2xG + s2yG + c2));
                SSIMB = (2 * mxB * myB + c1) * (2 * sxyB + c2) / ((mxB * mxB + myB * myB + c1) * (s2xB + s2yB + c2));
                SSIM = (SSIMR + SSIMG + SSIMB) / 3;

            }

            /*byte[] asciiBytes = new byte[x.Rows * x.Columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    asciiBytes[i * x.Columns + j] = (byte)(x[i, j]);
                }
            }

            Encoding unicode = Encoding.Unicode;
            byte[] unicodeBytes = Encoding.Convert(Encoding.GetEncoding(1251), unicode, asciiBytes);
            string str = unicode.GetString(unicodeBytes);
            return str;
            */
            /*for (int k = 0; k <= kp; k++)
                for (int j = 0; j < col; j++)
                    x2[k][j] = xb2[k,j];
             */


            APSNR += PSNR;
            return Convert.ToString(PSNR);
        }

        private int b(DoubleMatrix x)
        {
            double sum, maxsum = 0;
            int rez = 0;
            for (int r = 0; r < x.Rows; r++)
            {
                sum = 0;
                for (int c = 0; c < x.Columns; c++)
                {
                    sum += x[r, c] * x[r, c];
                }
                if ((r == 0) || (sum > maxsum))
                {
                    rez = r;
                    maxsum = sum;
                }
            }
            return rez;
        }

        private int b(List<DoubleVector> x, int first, int last)
        {
            double sum, maxsum = 0;
            int rez = 0;
            for (int r = first; r <= last; r++)
            {
                sum = 0;
                for (int c = 0; c < x[r].Length; c++)
                {
                    sum += x[r][c] * x[r][c];
                }
                if ((r == 0) || (sum > maxsum))
                {
                    rez = r;
                    maxsum = sum;
                }
            }
            return rez;
        }

        private bool zero(DoubleMatrix x, double e)
        {
            for (int r = 0; r < x.Rows; r++)
            {
                for (int c = 0; c < x.Columns; c++)
                {
                    if (Math.Abs(x[r, c]) > e)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private double D(DoubleVector x)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * x[i];
            }
            return Math.Sqrt(sum);
        }

        // Start
        private void button1_Click(object sender, EventArgs e)
        {
            // 02.11.2009
            cmode = radioButton1.Checked;

            //textBox_Zvit.Clear();

            Part1(pictureBox1.Image, (int)numericUpDown1.Value, (int)numericUpDown3.Value);

            // 23.09.2009
            string Zvit = "";

            
            //textBox_Zvit.Text += "Bitmap code =";

            //textBox_Zvit.Text += "\r\n";

            // 07.05.2010
            Zvit += "kmax= " + kmax + "\r\n";
            Zvit += "rows = " + rows + "\r\n";
            Zvit += "cols = " + col + "\r\n\r\n";

            //textBox_Zvit.Text += "xc = \r\n";
            /*
             Zvit += "xc=";
            for (int i = 0; i < col; i++)
            {
                Zvit += "" + xc[i].ToString() + ";";
            }
            Zvit += "\r\nz=";
            for (int i = 0; i <= kmax; i++)
            {
                Zvit += "";
                for (int j = 0; j < rows; j++)
                {
                    //Zvit += zzList[i][j].ToString() + ";";
                }
            }

            Zvit += "\r\nx2=";
            for (int i = 0; i <= kmax; i++)
            {
                Zvit += "";
                for (int j = 0; j < col; j++)
                {
                    Zvit += x2[i][j].ToString() + ";";
                }
            }

            Zvit += "\r\nbk =";
            for (int i = 0; i <= kmax; i++)
            {
                Zvit += "" + bk[i].ToString() + ";";
            }
            */
            textBox_Zvit.Text = Zvit;
                        
            // 07.10.2009
            // ...
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox4.Image = System.Drawing.Image.FromFile(openFileDialog.FileName);
                myBitmap4 = new Bitmap(openFileDialog.FileName);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(saveFileDialog.FileName));
                bw.Write(kp);
                //bw.Write(rows);
                //bw.Write(col);
                for (int i = 0; i < col; i++)
                {
                    bw.Write(xc[i]);
                }
                for (int i = 0; i <= kp; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        bw.Write(zzList[i][j]);
                    }
                }

                // 26.11.2009 !!!
                // 15.04.2010

                for (int i = 0; i <= kp; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        bw.Write(XBList[i][j]);
                    }
                }
                for (int i = 0; i <= kp; i++)
                {
                    bw.Write(bk[i]);
                }
                 
                
                bw.Close();
            }
        }

        private void textBox_in_TextChanged(object sender, EventArgs e)
        {
            //label1.Text = textBox_in.Text.Length.ToString();
        }

        // 23.09.2009

        private void Step2()
        {

            int k;

            // 16.11.2009 kmax->kp

            nbits = new int[kp + 1];
            nbits2 = new int[kp + 1];

            // 19.05.2010

            bits = 0;

            // 21.09.2009 - two bits per number for "zero sign" and sign

            for (k = 0; k <= kp; k++)
            {
                // 21.09.2009
                // 29.09.2009

                // 26.11.2009 !!!
                // 15.04.2010

                // 19.05.2010
                // 24.05.2010
                // 17.06.2010
                if (Math.Abs(zzList[k][bk[k]]) != 0)
                {
                    nbits[k] = (int)(Math.Truncate(Math.Log(Math.Abs(zzList[k][bk[k]]), 2) + 1) + 1);
                    nbits2[k] = nbits[k] - bitsm;
                }
                else
                {
                    nbits[k] = 1;
                    nbits2[k] = nbits[k]; // ??? 24.05.2010
                }
                bits += (nbits2[k] + 1) * rows;
            }
            ndwords = (int)Math.Truncate((double)(bits / 32)) + 1;
        }

        static void DisplayBits(BitArray bits)
        {
            foreach (bool bit in bits)
            {
                Console.Write(bit ? 1 : 0);
            }
        }

        private void Step3()
        {

            int r, k, z, z1, z2;

            int resCount, bitPos, mask;
            // 16.11.2009 kmax->kp
            // 19.05.2010

            resCount = 0;
            bitPos = 0;

            code = new int[ndwords + 1];
            for (k = 0; k <= kp; k++)
            {
                // 24.05.2010

                mask = (1 << nbits2[k]) - 1;
                for (r = 0; r < rows; r++)
                {
                    if (bitPos > 31)
                    {
                        bitPos ^= 32;
                        resCount++;
                        if (bitPos > 0)
                            if (r > 0)
                            {
                                z1 = (int)zzList[k][r - 1] >> bitsm;
                                code[resCount] ^= ((z1 & mask) >> (nbits2[k] - bitPos));
                            }
                            else
                            {
                                z2 = (int)zzList[k - 1][rows - 1] >> bitsm;
                                code[resCount] ^= ((z2 & mask) >> (nbits2[k] - bitPos));
                            }
                    }
                    // !!!!!!!!!!!!!
                    // 17.06.2010
                        z = (int)zzList[k][r] >> bitsm;
                        if (z == 0)
                            bitPos++;
                        else
                        {
                            code[resCount] ^= (1 << bitPos);
                            bitPos++;
                            if (bitPos < 32)
                                code[resCount] ^= (z & mask) << bitPos;
                            bitPos += nbits2[k];
                        }
                    // !!!!!!!!!!!!!
                }
            }
            mask = (1 << nbits2[kp]) - 1;
            if (bitPos > 31)
            {
                bitPos ^= 32;
                resCount++;
                z1 = (int)zzList[kp][rows - 1] >> bitsm;
                code[resCount] ^= (z1 & mask) >> (nbits2[kp] - bitPos);
            }

            // 18.06.2010
            ndwords = resCount;
        }

        
        private void Step4(string FileName)
        {

            int i, j, k;
            // 16.11.2009 kmax->kp
            // 19.05.2010

            // 23.09.2009
            // FileInfo f = new FileInfo(FileName);
            BinaryWriter FOut = new BinaryWriter(File.Create(FileName));

            // 17.06.2010
            FOut.Write((byte)0);
            FOut.Write((byte)(kp + 1));
            FOut.Write((byte)(bitsm));
            for (k = 0; k <= kp; k++)
                FOut.Write((byte)nbits[k]);
            FOut.Write(rows);
            FOut.Write(ndwords);
            for (i = 0; i <= ndwords; i++)
                FOut.Write((int)code[i]);

            // 16.11.2009
            FOut.Write((byte)(col));
            for (i = 0; i < col; i++)
            {
                FOut.Write((short)xc[i]);
            }

            // 26.11.2009 !!!
            // 15.04.2010

            for (i = 0; i <= kp; i++)
            {
                for (j = 0; j < col; j++)
                {
                    FOut.Write(/*(short)*/XBList[i][j]);
                }
            }
            for (i = 0; i <= kp; i++)
            {
                FOut.Write(bk[i]);
            }


            FOut.Close();
        }

        private void Step5(string FileName)
        {
            int i, j, k;

            // 23.09.2009
            // FileInfo f = new FileInfo(FileName);
            BinaryReader FIn = new BinaryReader(File.OpenRead(FileName));

            // 16.11.2009 kmax->kp

            // 17.06.2010
            zeros = FIn.ReadByte();
            kp = FIn.ReadByte() - 1;
            bitsm = FIn.ReadByte();
            nbits = new int[kp + 1];

            for (k = 0; k <= kp; k++)
                nbits[k] = FIn.ReadByte();

            // 24.05.2010
            nbits2 = new int[kp + 1];

            for (k = 0; k <= kp; k++)
                nbits2[k] = nbits[k] - bitsm;

            
            
            rows = FIn.ReadInt32();
            // 18.06.2010
            ndwords = FIn.ReadInt32();
            bits = 0;

            // 18.06.2010
            //for (k = 0; k <= kp; k++)
                //bits += (nbits2[k]+1) * rows;
            //ndwords = (int)Math.Truncate((double)(bits / 32)) + 1;


            // 21.09.2009
            code = new int[ndwords + 1];


            for (i = 0; i <= ndwords; i++)
                code[i] = FIn.ReadInt32();

            // 16.11.2009
            col = FIn.ReadByte();
            xc = new DoubleVector(col);
            for (i = 0; i < col; i++)
            {
                xc[i] = FIn.ReadInt16();
            }

            // 26.11.2009 !!!
            // 15.04.2010

            XBList = new List<DoubleVector>();

            for (i = 0; i <= kp; i++)
            {
                XBList.Add(new DoubleVector(new DoubleVector(col)));
                for (j = 0; j < col; j++)
                {
                    XBList[i][j] = FIn.ReadDouble();//ReadInt16();
                }
            }
            bk = new List<int>();
            for (i = 0; i <= kp; i++)
            {
                bk.Add(FIn.ReadInt32());
            }



            FIn.Close();
        }

        private void Step6()
        {
            // 17.06.2010
        }

        private void Step7()
        {
// !!!!!!!!!!!!!!!!!!!!1
            int r, k;

            // 16.11.2009 kmax->kp
            // 19.05.2010

            int resCount, bitPos;
            zzList = new List<DoubleVector>();

            resCount = 0;
            bitPos = 0;
            for (k = 0; k <= kp; k++)
            {
                zzList.Add(new DoubleVector(rows));
                for (r = 0; r < rows; r++)
                {
                    if (bitPos > 31)
                    {
                        bitPos ^= 32;
                        resCount++;
                        if (bitPos > 0)
                            // 24.05.2010
                            if (r > 0)
                                zzList[k][r - 1] = (int)zzList[k][r - 1] ^ (code[resCount] & ((1u << bitPos) - 1u)) << (nbits2[k] - bitPos);
                            else
                                zzList[k - 1][rows - 1] = (int)zzList[k - 1][rows - 1] ^ (code[resCount] & ((1u << bitPos) - 1u)) << (nbits2[k - 1] - bitPos);
                    }

                    // !!!!!!!!!!!!!!!!
                    // 17.06.2010
                    if ((code[resCount] & (1u << bitPos)) == 0)
                        {
                            zzList[k][r] = 0;
                            bitPos++;
                        }
                        else
                        {
                            bitPos++;
                            if (bitPos < 32)
                            {
                                if (bitPos + nbits2[k] > 31)
                                    zzList[k][r] = (code[resCount] & (0u - (1u << bitPos))) >> bitPos;
                                else
                                    zzList[k][r] = (code[resCount] & ((1u << (bitPos + nbits2[k])) - (1 << bitPos))) >> bitPos;
                            }
                            bitPos += nbits2[k];
                        }
                    // !!!!!!!!!!!!!!!
                }
            }
            for (k = 0; k <= kp; k++)
                for (r = 0; r < rows; r++)
                {
                    if (zzList[k][r] >= (1 << (nbits2[k] - 1)))
                        zzList[k][r] -= (1 << nbits2[k]);

                    // 24.05.2010
                    zzList[k][r] = (int)zzList[k][r] << bitsm;
                }
// !!!!!!!!!!!!!!!!!!!

        }

        private void Step8()
        {
            // 16.11.2009 kmax->kp

            string Zvit = "";

            /*textBox_Zvit.Text += "\r\nz=";
            for (int i = 0; i <= kp; i++)
            {
                textBox_Zvit.Text += "";
                for (int j = 0; j < rows; j++)
                {
                    Zvit += zzList[i][j].ToString() + ";";
                }
            }
            textBox_Zvit.Text = Zvit;*/
        }

        private void Step12()
        {
            int k;

            // 16.11.2009 kmax->kp
            
            nbits = new int[kp + 1];
            nbits2 = new int[kp + 1];

            // 19.05.2010

            bits = 0;

            for (k = 0; k <= kp; k++)
            {
                // 21.09.2009
                // 29.09.2009

                // 26.11.2009 !!!
                // 15.04.2010

                // 19.05.2010
                // 24.05.2010
                if (Math.Abs(zzList[k][bk[k]]) != 0)
                {
                    nbits[k] = (int)(Math.Truncate(Math.Log(Math.Abs(zzList[k][bk[k]]), 2) + 1) + 1);
                    nbits2[k] = nbits[k] - bitsm;
                }
                else
                {
                    nbits[k] = 1;
                    nbits2[k] = nbits[k]; // ??? 24.05.2010
                }
            bits += nbits2[k] * rows;
            }
            ndwords = (int)Math.Truncate((double)(bits / 32)) + 1;
    }

        private void Step13()
        {
            int r, k, z, z1, z2;
            
            int resCount, bitPos, mask;
            // 16.11.2009 kmax->kp
            // 19.05.2010

            resCount = 0;
            bitPos = 0;
            
            code = new int[ndwords+1];
            for (k = 0; k <= kp; k++)
            {
                // 24.05.2010

                mask = (1 << nbits2[k]) - 1;
                for (r = 0; r < rows; r++)
                {
                    if (bitPos > 31)
                    {
                        bitPos ^= 32;
                        resCount++;
                        if (bitPos > 0)
                            if (r > 0)
                            {
                                z1 = (int)zzList[k][r - 1] >> bitsm;
                                code[resCount] ^= ((z1 & mask) >> (nbits2[k] - bitPos));
                            }
                            else
                            {
                                z2 = (int)zzList[k - 1][rows - 1] >> bitsm;
                                code[resCount] ^= ((z2 & mask) >> (nbits2[k] - bitPos));
                            }
                    }
                    z = (int)zzList[k][r] >> bitsm;
                    code[resCount] ^= (z & mask) << bitPos;
                    bitPos += nbits2[k];
                }
            }
            mask = (1 << nbits2[kp]) - 1;
            if(bitPos > 31)
            {
		        bitPos ^= 32;
		        resCount++;
                z1 = (int)zzList[kp][rows - 1] >> bitsm;
                code[resCount] ^= (z1 & mask) >> (nbits2[kp] - bitPos);
            }
        }

        private void Step14(string FileName)
        {
            int i, j, k;
            // 16.11.2009 kmax->kp
            // 19.05.2010

            // 23.09.2009
            // FileInfo f = new FileInfo(FileName);
            BinaryWriter FOut = new BinaryWriter(File.Create(FileName));

            // 17.06.2010
            FOut.Write((byte)1);
            FOut.Write((byte)(kp + 1));
            FOut.Write((byte)(bitsm));
            for (k = 0; k <= kp; k++)
                FOut.Write((byte)nbits[k]);
            FOut.Write(rows);
            for (i = 0; i <= ndwords; i++)
                FOut.Write((int)code[i]);

            // 16.11.2009
            FOut.Write((byte)(col));
            for (i = 0; i < col; i++)
            {
                FOut.Write((short)xc[i]);
            }

            // 26.11.2009 !!!
            // 15.04.2010

            for (i = 0; i <= kp; i++)
            {
                for (j = 0; j < col; j++)
                {
                    FOut.Write(/*(short)*/XBList[i][j]);
                }
            }
            for (i = 0; i <= kp; i++)
            {
                FOut.Write(bk[i]);
            }
            
             
            FOut.Close();
        }

        private void Step15(string FileName)
        {
            int i, j, k;

            // 23.09.2009
            // FileInfo f = new FileInfo(FileName);
            BinaryReader FIn = new BinaryReader(File.OpenRead(FileName));

            // 16.11.2009 kmax->kp

            // 17.06.2010
            zeros = FIn.ReadByte();
            kp = FIn.ReadByte() - 1;
            bitsm = FIn.ReadByte();
            nbits = new int[kp + 1];

            for (k = 0; k <= kp; k++)
                nbits[k] = FIn.ReadByte();

            // 24.05.2010
            nbits2 = new int[kp + 1];

            for (k = 0; k <= kp; k++)
                nbits2[k] = nbits[k] - bitsm;

            rows = FIn.ReadInt32();
            bits = 0;

            for (k = 0; k <= kp; k++)
                bits += nbits2[k] * rows;
            ndwords = (int)Math.Truncate((double)(bits / 32)) + 1;

            // 21.09.2009
            code = new int[ndwords+1];


            for (i = 0; i <= ndwords; i++)
                code[i] = FIn.ReadInt32();

            // 16.11.2009
            col = FIn.ReadByte();
            xc = new DoubleVector(col);
            for (i = 0; i < col; i++)
            {
                xc[i] = FIn.ReadInt16();
            }

            // 26.11.2009 !!!
            // 15.04.2010

            XBList = new List<DoubleVector>();
        
            for (i = 0; i <= kp; i++)
            {
                XBList.Add(new DoubleVector(new DoubleVector(col)));
                for (j = 0; j < col; j++)
                {
                    XBList[i][j] = FIn.ReadDouble();//ReadInt16();
                }
            }
            bk = new List<int>();
            for (i = 0; i <= kp; i++)
            {
                bk.Add(FIn.ReadInt32());
            }
            
            

            FIn.Close();
        }


        private void Step16()
        {
            // 19.05.2010
        }

        private void Step17()
        {
            int r, k;

            // 16.11.2009 kmax->kp
            // 19.05.2010

            int resCount, bitPos;
            zzList = new List<DoubleVector>();
            
            resCount = 0;
            bitPos = 0;
            for (k = 0; k <= kp; k++)
            {
                zzList.Add(new DoubleVector(rows));
                for (r = 0; r < rows; r++)
                {
                if (bitPos > 31)
                {
                    bitPos ^= 32;
                    resCount++;
                    if (bitPos > 0)
                        // 24.05.2010
                      if (r>0)
                        zzList[k][r - 1] = (int)zzList[k][r - 1] ^ (code[resCount] & ((1u << bitPos) - 1u)) << (nbits2[k] - bitPos);
                      else
                        zzList[k-1][rows-1] = (int)zzList[k-1][rows - 1] ^ (code[resCount] & ((1u << bitPos) - 1u)) << (nbits2[k-1] - bitPos);
                }
                if (bitPos + nbits2[k] > 31)
                    zzList[k][r] = (code[resCount] & (0u - (1u << bitPos))) >> bitPos;
                else

                    zzList[k][r] = (code[resCount] & ((1u << (bitPos + nbits2[k])) - (1 << bitPos))) >> bitPos;
                bitPos += nbits2[k];
                }
            }
            for (k = 0; k <= kp; k++)
                for (r = 0; r < rows; r++)
                {
                    if (zzList[k][r] >= (1 << (nbits2[k] - 1)))
                        zzList[k][r] -= (1 << nbits2[k]);

                    // 24.05.2010
                    zzList[k][r] = (int)zzList[k][r] << bitsm;
                }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // 16.11.2009
                kp = (int)numericUpDown2.Value;
                bitsm = (int)numericUpDown6.Value;

                if (checkBox1.Checked == true)
                {
                    // BP 2 - with "zero exception"
                Step2();
                Step3();
                Step4(saveFileDialog1.FileName);
                }
                else
                {
                    // BP 1 - without "zero exception"
                Step12();
                Step13();
                Step14(saveFileDialog1.FileName);

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                    // BP 2 - with "zero exception"                
                if (checkBox1.Checked == true)
                {
                Step5(openFileDialog1.FileName);
                Step6();
                Step7();
                Step8();
                }
                    // BP 1 - without "zero exception"
                else
                {
                Step15(openFileDialog1.FileName);
                Step16();
                Step17();
                Step8();
                }
                numericUpDown2.Value = kp;
                numericUpDown6.Value = bitsm;

            }
        }

        // 07.10.2009

        private void button4_Click(object sender, EventArgs e)
        {
            
            //textBox_Zvit.Text = "";

            // 15.02.2010
            int BlockNo;
            BlockNo = (int)numericUpDown5.Value;

            // 16.11.2009
            

            cmode = radioButton1.Checked;

            buttonSave.Enabled = false;
            kp = (int)numericUpDown2.Value;

            // 07.05.2010
            bitsm = (int)numericUpDown6.Value;
            // 28.10.2009
            myBitmap2 = new Bitmap(pictureBox1.Image.Width,pictureBox1.Image.Height);
            // 02.11.2009
            myBitmap3 = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
            textBox_Zvit.Text = textBox_Zvit.Text + Part2()+" ("+Convert.ToString(BlockNo)+")\r\n";
            // 28.10.2009
            pictureBox2.Image = (Image)myBitmap2;
            // 02.11.2009
            pictureBox3.Image = (Image)myBitmap3;

            buttonSave.Enabled = true;

            // 27.05.2010
            textBox_Zvit.Text = textBox_Zvit.Text + "SSIM: " + Convert.ToString(SSIM)+"\r\n";
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        // 26.01.2010
        private void button5_Click(object sender, EventArgs e)
        {

            blocks = (int)numericUpDown4.Value;
            height = pictureBox4.Image.Height/blocks;
            width = pictureBox4.Image.Width/blocks;
            bool margins = checkBox2.Checked;

            myBitmap = new Bitmap(height, width);

            myBitmap4 = new Bitmap(openFileDialog.FileName);


            int i, j;
            // 15.02.2010

            cmode = radioButton1.Checked;

            if (margins == true && cmode == true)
            {
                for (i = 1; i < blocks; i++)
                    for (j = 0; j < pictureBox4.Image.Width; j++)
                        myBitmap4.SetPixel(j, (i * height) + (i * height)%2, Color.Blue);

                for (i = 0; i < pictureBox4.Image.Height; i++)
                    for (j = 1; j < blocks; j++)
                        myBitmap4.SetPixel(j * width + (j * width) % 2, i, Color.Blue);
            }

            pictureBox4.Image = (Image)myBitmap4;
        }

        // 26.01.2010
        private void button6_Click(object sender, EventArgs e)
        {
            int BlockNo;
            BlockNo = (int)numericUpDown5.Value;
            int i0, j0;

            i0 = (int)((BlockNo - 1) / blocks) * height;
            j0 = (int)((BlockNo - 1) % blocks) * width;


            for (int i = i0; i < i0+height; i++)
                for (int j = j0; j < j0+width; j++)

                    myBitmap.SetPixel(j - j0, i - i0, myBitmap4.GetPixel(j, i));
            pictureBox1.Image = (Image)myBitmap;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 15.02.2010
            //pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            myBitmap5 = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height);
            myBitmap6 = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height);
            int BlockNo;
            int i0, j0;

            APSNR = 0;
            textBox_Zvit.Clear();
            for (BlockNo = 1; BlockNo <= blocks * blocks; BlockNo++)
            {
                numericUpDown5.Value = BlockNo;
                button6.PerformClick();
                button1.PerformClick();
                button4.PerformClick();

                i0 = (int)((BlockNo - 1) / blocks) * height;
                j0 = (int)((BlockNo - 1) % blocks) * width;


                for (int i = i0; i < i0 + height; i++)
                    for (int j = j0; j < j0 + width; j++)
                    {
                        myBitmap5.SetPixel(j, i, myBitmap2.GetPixel(j - j0, i - i0));
                        myBitmap6.SetPixel(j, i, myBitmap3.GetPixel(j - j0, i - i0));
                    }
            }
            // 15.02.2010
            
            pictureBox5.Image = (Image)myBitmap5;
            pictureBox6.Image = (Image)myBitmap6;

            APSNR /= (blocks * blocks);
            textBox_Zvit.Text = textBox_Zvit.Text + "APSNR="+Convert.ToString(APSNR) + "\r\n";
        }


        // 08.03.2010
        private bool Part11(Image image, int cols, int cols1)
        {
            col = cols * cols;
            double sum;

            if (cmode == false)
                rows = image.Height * image.Width / col;
            else
                rows = image.Height * image.Width / col * 3;

            //2

            DoubleMatrix x = new DoubleMatrix(rows, col);

            if (cmode == false)
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < col; j++)
                    {
                        x[i, j] = (double)((byte)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols).ToArgb() & 0x000000ff);
                    }
            else
                for (int i = 0; i < rows / 3; i++)
                    for (int j = 0; j < col; j++)
                    {
                        x[i * 3, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).R);
                        x[i * 3 + 1, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).G);
                        x[i * 3 + 2, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).B);
                    }

            DoubleMatrix x1 = new DoubleMatrix(x);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    x1[r, c] -= xc[c];
                }
            }

            zList = new List<DoubleVector>();
            zzList = new List<DoubleVector>();

            // 08.03.2010
            for (int k1=0; k1<=kmax; k1++)
            {
                DoubleVector xB = new DoubleVector(XBList[k1]);
                DoubleVector XB = new DoubleVector(xB);
                xB.Scale(DList[k1]);
                zList.Add(new DoubleVector(rows));
                zzList.Add(new DoubleVector(rows));

                for (int r = 0; r < rows; r++)
                {
                    sum = 0;
                    for (int c = 0; c < col; c++)
                    {
                        sum += x1[r, c] * xB[c];
                    }
                    zList[zList.Count - 1][r] = sum / DList[k1];
                    zzList[zList.Count - 1][r] = zList[zList.Count - 1][r] * 2;
                    zzList[zList.Count - 1][r] = Math.Round(zzList[zList.Count - 1][r]);
                }

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < col; c++)
                    {
                        x1[r, c] = x1[r, c] - XB[c] * zList[zList.Count - 1][r];
                    }
                }
            }
            return true;
        }



        // 08.03.2010
        // Start 2
        private void button8_Click(object sender, EventArgs e)
        {
            cmode = radioButton1.Checked;
            Part11(pictureBox1.Image, (int)numericUpDown1.Value, (int)numericUpDown3.Value);
        }

        // 25.04.2010
        private void button9_Click(object sender, EventArgs e)
        {
            int i, total_bits, compr_bits;
            double Gain;

            PSNRmin = Convert.ToDouble(textBox1.Text);
            kpl = 0;
            kpr = kmax;//(int)numericUpDown2.Value;
            while (kpl <= kpr)
            {
                
                kpm = (kpl + kpr) / 2;
                //numericUpDown2.Value=kpl;
                //button4.PerformClick();
                //PSNRl = APSNR;

                //numericUpDown2.Value = kpr;
                //button4.PerformClick();
                //PSNRr = APSNR;

                APSNR = 0;
                numericUpDown2.Value = kpm;
                button4.PerformClick();
                PSNRm = APSNR;

                if (PSNRm > PSNRmin)
                {   
                    kpr = kpm - 1;
                }
                else
                if (PSNRm < PSNRmin)
                {
                    kpl = kpm + 1;
                }
            }
            if (PSNRm < PSNRmin)
            {
                kpm = kpm + 1;
                APSNR = 0;
                numericUpDown2.Value = kpm;
                button4.PerformClick();
                PSNRm = APSNR;
            }

            kp = kmax;
            if (checkBox1.Checked == true)
                Step2();
            else
                Step12();

            total_bits=0;
            // 17.05.2010
            for (i = 0; i <= kp; i++)
                total_bits += 8;

            compr_bits=0;
            for (i = 0; i <= kpm; i++)
                compr_bits += nbits[i];

            Gain = (double)total_bits / (double)compr_bits;
            label7.Text = "Compression: " + Convert.ToString(Gain)+" : 1";

        }

        // 07.05.2010
        private void button10_Click(object sender, EventArgs e)
        {
            int i, total_bits, compr_bits;
            double Gain;

            PSNRmin = Convert.ToDouble(textBox1.Text);

            // 24.05.2010
            bitsm = (int)numericUpDown6.Value;
            kp = (int)numericUpDown2.Value;

            if (checkBox1.Checked == true)
                Step2();
            else
                Step12();

            bitsl = 0;
            bitsr = nbits[0];//(int)numericUpDown2.Value;
            while (bitsl <= bitsr)
            {

                bitsm = (bitsl + bitsr) / 2;
                //numericUpDown2.Value=kpl;
                //button4.PerformClick();
                //PSNRl = APSNR;

                //numericUpDown2.Value = kpr;
                //button4.PerformClick();
                //PSNRr = APSNR;

                APSNR = 0;
                numericUpDown6.Value = bitsm;
                button4.PerformClick();
                PSNRm = APSNR;

                if (PSNRm < PSNRmin)
                {
                    bitsr = bitsm - 1;
                }
                else
                    if (PSNRm > PSNRmin)
                    {
                        bitsl = bitsm + 1;
                    }
            }
            if (PSNRm < PSNRmin)
            {
                bitsm = bitsm - 1;
                APSNR = 0;
                numericUpDown6.Value = bitsm;
                button4.PerformClick();
                PSNRm = APSNR;
            }

            total_bits = 0;
            // 17.05.2010
            for (i = 0; i <= kmax; i++)
                total_bits += 8;

            compr_bits = 0;
            for (i = 0; i <= kp; i++)
            {
                compr_bits += nbits[i];
                compr_bits -= bitsm;
            }

            Gain = (double)total_bits / (double)compr_bits;
            label7.Text = "Compression: " + Convert.ToString(Gain) + " : 1";
            //label8.Text = "Bits: -" + Convert.ToString(bitsm);

        }

        // 17.06.2010
        private void button11_Click(object sender, EventArgs e)
        {
        

            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                myBitmap2.Save(saveFileDialog2.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}
