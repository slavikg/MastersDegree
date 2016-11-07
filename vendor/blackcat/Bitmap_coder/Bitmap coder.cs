using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Sapienware.Types;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace Bitmap_Coder
{
    public partial class MainForm : Form
    {
        private int kmax;

        private int kp;
        private int dkp;

        private int rows;
        private int col;
        private int rows2;
        private int col2;

        private DoubleVector xc;
        private List<DoubleVector> zList;
        private List<DoubleVector> zzList;

        private List<DoubleVector> XBList;

        private bool cmode;

        Bitmap myBitmap;
        Bitmap myBitmap2;
        Bitmap myBitmap3;

        Bitmap myBitmap4;
        Bitmap myBitmap5;
        Bitmap myBitmap6;

        int len;

        private List<double> DList;

        double SSIM;

        public MainForm()
        {
            InitializeComponent();
        }

        private bool Part1(Image image, int cols, string text)
        {
            col = cols*cols;
            double sum;

            if (cmode == false)
            rows = image.Height*image.Width / col;
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

            for (int i = 0; i < rows/3; i++)
                for (int j = 0; j < col; j++)
            {
                x[i*3, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows/3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows/3 * col)) / cols)) * cols + j % cols).R);
                x[i*3+1, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows/3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows/3 * col)) / cols)) * cols + j % cols).G);
                x[i*3+2, j] = (double)((uint)myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows/3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows/3 * col)) / cols)) * cols + j % cols).B);
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

            XBList = new List<DoubleVector>();

            DList = new List<double>();

            len = text.Length;

            DoubleMatrix x10 = new DoubleMatrix(x1);
            while (zero(x1, 0.0000000001) == false)
            {
                // 6

                DoubleVector xB = new DoubleVector(x1.Row(b(x1)));

                //12

                //7
                double Dk = D(xB);
                //8
                DoubleVector XB = new DoubleVector(xB);
                XB.Scale(1 / Dk);
                XBList.Add(new DoubleVector(XB));
                
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
                    zzList[zList.Count - 1][r] = zList[zList.Count - 1][r] * 1;
                    zzList[zList.Count - 1][r] = Math.Round(zzList[zList.Count - 1][r]);

                    // !!!!
                    
                    if (r <  len && zList.Count - 1==dkp)
                    {
                        zzList[zList.Count - 1][r] = text[r];
                    }

                    // !!!!

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

            kmax = k - 1;
            return true;
        }

        private string Part2()
        {

            int cols = (int)Math.Sqrt(col);

            // kmax->kp
      
            DoubleMatrix XB = new DoubleMatrix(kp + 1, col);

            DoubleMatrix xb2 = new DoubleMatrix(kp + 1, col);

            DoubleMatrix x1 = new DoubleMatrix(rows, col, 0);

            System.Text.StringBuilder text = new System.Text.StringBuilder();
            
            for (int k = kp; k >= 0; k--)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        x1[i, j] += XBList[k][j] * (zzList[k][i] / 1);

                    }
                    // !!!!!!!!!!!!!
                    if (i < len && k == dkp && cmode == false)
                    {
                        text.Append(((char)((int)(zzList[k][i])))).ToString();
                    }
                    else
                        if (i < len && k == dkp && cmode == true)
                        {
                            text.Append(((char)((int)(zzList[k][i])))).ToString();
                        }
                    // !!!!!!!!!!!!!
                }
            }

            //textBox1.Text = text.ToString();
            //-----------------------------------------------------------------------


            col2 = myBitmap4.Width;
            if (cmode == false)
                rows2 = myBitmap4.Height;
            else
                rows2 = myBitmap4.Height*3;
            double MSE2 = 0;
            double PSNR2 = 0;
            int p = 0;
            int pixel=0;
            byte pixel0=0;
            byte pixel1=0;
            byte pixel2=0;

            DoubleMatrix xo2 = new DoubleMatrix(rows2, col2);
            DoubleMatrix x2 = new DoubleMatrix(rows2, col2);

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
                for (int i = 0; i < rows2; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        pixel = Convert.ToInt32(text[p]);
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;

                        xo2[i, j] = myBitmap4.GetPixel(i, j).ToArgb() & 0x000000ff;
                        myBitmap5.SetPixel(i, j, Color.FromArgb(Convert.ToByte(pixel), Convert.ToByte(pixel), Convert.ToByte(pixel)));
                        myBitmap6.SetPixel(i, j, Color.FromArgb(Convert.ToByte(255 - Math.Abs(xo2[i, j] - pixel)), Convert.ToByte(255 - Math.Abs(xo2[i, j] - pixel)), Convert.ToByte(255 - Math.Abs(xo2[i, j] - pixel))));
                        MSE2 += Math.Pow((Math.Abs(xo2[i, j] - pixel)), 2);
                        mxR += xo2[i, j]; mxG += xo2[i, j]; mxB += xo2[i, j];
                        myR += pixel; myG += pixel; myB += pixel;

                        p++;
                    }
                MSE2 = MSE2 / (rows2 * col2);
                PSNR2 = 10 * Math.Log10(255 * 255 / MSE2);
                mxR = mxR / (rows2 * col2); mxG = mxG / (rows2 * col2); mxB = mxB / (rows2 * col2);
                myR = myR / (rows2 * col2); myG = myG / (rows2 * col2); myB = myB / (rows2 * col2);

                p = 0;
                for (int i = 0; i < rows2; i++)
                    for (int j = 0; j < col2; j++)
                    {

                        pixel = Convert.ToInt32(text[p]);
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;

                        s2xR += Math.Pow(xo2[i, j] - mxR, 2); s2xG += Math.Pow(xo2[i, j] - mxG, 2); s2xB += Math.Pow(xo2[i, j] - mxB, 2);
                        s2yR += Math.Pow(pixel - myR, 2); s2yG += Math.Pow(pixel - myG, 2); s2yB += Math.Pow(pixel - myB, 2);
                        sxyR += (xo2[i, j] - mxR) * (pixel - myR); sxyG += (xo2[i, j] - mxG) * (pixel - myG); sxyB += (xo2[i, j] - mxB) * (pixel - myB);
                        p++;
                    }
                s2xR = s2xR / (rows2 * col2 - 1); s2xG = s2xG / (rows2 * col2 - 1); s2xB = s2xB / (rows2 * col2 - 1);
                s2yR = s2yR / (rows2 * col2 - 1); s2yG = s2yG / (rows2 * col2 - 1); s2yB = s2yB / (rows2 * col2 - 1);
                sxyR = sxyR / (rows2 * col2 - 1); sxyG = sxyG / (rows2 * col2 - 1); sxyB = sxyB / (rows2 * col2 - 1);
                c1 = Math.Pow(k1 * L, 2); c2 = Math.Pow(k2 * L, 2);
                SSIMR = (2 * mxR * myR + c1) * (2 * sxyR + c2) / ((mxR * mxR + myR * myR + c1) * (s2xR + s2yR + c2));
                SSIMG = (2 * mxG * myG + c1) * (2 * sxyG + c2) / ((mxG * mxG + myG * myG + c1) * (s2xG + s2yG + c2));
                SSIMB = (2 * mxB * myB + c1) * (2 * sxyB + c2) / ((mxB * mxB + myB * myB + c1) * (s2xB + s2yB + c2));
                SSIM = (SSIMR + SSIMG + SSIMB) / 3;


            }
            else
            {
                for (int i = 0; i < rows2 / 3; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        pixel0 = Convert.ToByte(text[p]);
                        p++;
                        pixel1 = Convert.ToByte(text[p]);
                        p++;
                        pixel2 = Convert.ToByte(text[p]);
                        p++;

                        xo2[i * 3, j] = myBitmap4.GetPixel(i, j).R;
                        xo2[i * 3 + 1, j] = myBitmap4.GetPixel(i, j).G;
                        xo2[i * 3 + 2, j] = myBitmap4.GetPixel(i, j).B;

                        myBitmap5.SetPixel(i, j, Color.FromArgb(pixel0, pixel1, pixel2));
                        myBitmap6.SetPixel(i, j, Color.FromArgb(Convert.ToByte(255 - Math.Abs(xo2[i * 3, j] - pixel0)), Convert.ToByte(255 - Math.Abs(xo2[i * 3 + 1, j] - pixel1)), Convert.ToByte(255 - Math.Abs(xo2[i * 3 + 2, j] - pixel2))));
                     

                        MSE2 = MSE2 += Math.Pow((Math.Abs(xo2[i * 3, j] - pixel0)), 2);
                        MSE2 = MSE2 += Math.Pow((Math.Abs(xo2[i * 3 + 1, j] - pixel1)), 2);
                        MSE2 = MSE2 += Math.Pow((Math.Abs(xo2[i * 3 + 2, j] - pixel2)), 2);

                        mxR += xo2[i * 3, j]; mxG += xo2[i * 3 + 1, j]; mxB += xo2[i * 3 + 2, j];
                        myR += pixel0; myG += pixel1; myB += pixel2;

                    }
                MSE2 = MSE2 / (rows2 * col2);
                PSNR2 = 10 * Math.Log10(255 * 255 / MSE2);

                mxR = mxR / (rows2 * col2 / 3); mxG = mxG / (rows2 * col2 / 3); mxB = mxB / (rows2 * col2 / 3);
                myR = myR / (rows2 * col2 / 3); myG = myG / (rows2 * col2 / 3); myB = myB / (rows2 * col2 / 3);
                p = 0;

                for (int i = 0; i < rows2 / 3; i++)
                    for (int j = 0; j < col2; j++)
                    {


                        pixel0 = Convert.ToByte(text[p]);
                        p++;
                        pixel1 = Convert.ToByte(text[p]);
                        p++;
                        pixel2 = Convert.ToByte(text[p]);
                        p++;
                        s2xR += Math.Pow(xo2[i * 3, j] - mxR, 2); s2xG += Math.Pow(xo2[i * 3 + 1, j] - mxG, 2); s2xB += Math.Pow(xo2[i * 3 + 2, j] - mxB, 2);
                        s2yR += Math.Pow(pixel0 - myR, 2); s2yG += Math.Pow(pixel1 - myG, 2); s2yB += Math.Pow(pixel2 - myB, 2);
                        sxyR += (xo2[i * 3, j] - mxR) * (pixel0 - myR); sxyG += (xo2[i * 3 + 1, j] - mxG) * (pixel1 - myG); sxyB += (xo2[i * 3 + 2, j] - mxB) * (pixel2 - myB);


                    }
                s2xR = s2xR / (rows2 * col2 / 3 - 1); s2xG = s2xG / (rows2 * col2 / 3 - 1); s2xB = s2xB / (rows2 * col2 / 3 - 1);
                s2yR = s2yR / (rows2 * col2 / 3 - 1); s2yG = s2yG / (rows2 * col2 / 3 - 1); s2yB = s2yB / (rows2 * col2 / 3 - 1);
                sxyR = sxyR / (rows2 * col2 / 3 - 1); sxyG = sxyG / (rows2 * col2 / 3 - 1); sxyB = sxyB / (rows2 * col2 / 3 - 1);
                c1 = Math.Pow(k1 * L, 2); c2 = Math.Pow(k2 * L, 2);
                SSIMR = (2 * mxR * myR + c1) * (2 * sxyR + c2) / ((mxR * mxR + myR * myR + c1) * (s2xR + s2yR + c2));
                SSIMG = (2 * mxG * myG + c1) * (2 * sxyG + c2) / ((mxG * mxG + myG * myG + c1) * (s2xG + s2yG + c2));
                SSIMB = (2 * mxB * myB + c1) * (2 * sxyB + c2) / ((mxB * mxB + myB * myB + c1) * (s2xB + s2yB + c2));
                SSIM = (SSIMR + SSIMG + SSIMB) / 3;
            }

           // return Convert.ToString(PSNR2);

           


            //-----------------------------------------------------------------------

            DoubleMatrix x = new DoubleMatrix(rows, col, 0);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    x[i, j] = x1[i, j] + xc[j];
                    x[i, j] = Math.Round(x[i, j]);
                }
            }

            double MSE = 0;
            double PSNR = 0;

            DoubleMatrix xo = new DoubleMatrix(rows, col);

            if (cmode == false)
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < col; j++)
                    {
                        if (x[i, j] < 0) x[i, j] = 0;
                        if (x[i, j] > 255) x[i, j] = 255;
                        
                        xo[i, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols).ToArgb() & 0x000000ff);

                        myBitmap2.SetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols, Color.FromArgb(Convert.ToByte(x[i, j]), Convert.ToByte(x[i, j]), Convert.ToByte(x[i, j])));
                        myBitmap3.SetPixel(((i * cols) / (int)(Math.Sqrt(rows * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows * col)) / cols)) * cols + j % cols, Color.FromArgb
                        (Convert.ToByte(255 - Math.Abs(xo[i, j] - x[i, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i, j] - x[i, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i, j] - x[i, j]))));
                        MSE += Math.Pow((Math.Abs(xo[i, j] - x[i, j])), 2);
                    }
                MSE = MSE / (rows * col);
                PSNR = 10 * Math.Log10(255 * 255 / MSE);
            }
            else
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

                        xo[i * 3, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).R);
                        xo[i * 3 + 1, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).G);
                        xo[i * 3 + 2, j] = (myBitmap.GetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols).B);

                        myBitmap2.SetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols, Color.FromArgb(Convert.ToByte(x[i * 3, j]), Convert.ToByte(x[i * 3 + 1, j]), Convert.ToByte(x[i * 3 + 2, j])));
                        myBitmap3.SetPixel(((i * cols) / (int)(Math.Sqrt(rows / 3 * col))) * cols + j / cols, (i % ((int)(Math.Sqrt(rows / 3 * col)) / cols)) * cols + j % cols, Color.FromArgb
                        (Convert.ToByte(255 - Math.Abs(xo[i * 3, j] - x[i * 3, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i * 3 + 1, j] - x[i * 3 + 1, j])),
                         Convert.ToByte(255 - Math.Abs(xo[i * 3 + 2, j] - x[i * 3 + 2, j]))));
                        MSE += Math.Pow((Math.Abs(xo[i * 3, j] - x[i * 3, j])), 2);
                        MSE += Math.Pow((Math.Abs(xo[i * 3 + 1, j] - x[i * 3 + 1, j])), 2);
                        MSE += Math.Pow((Math.Abs(xo[i * 3 + 2, j] - x[i * 3 + 2, j])), 2);

                    }
                MSE = MSE / (rows * col);
                PSNR = 10 * Math.Log10(255 * 255 / MSE);
            }

            return Convert.ToString(PSNR) + "\n" + Convert.ToString(PSNR2);
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

            rows2 = myBitmap4.Height;
            col2 = myBitmap4.Width;

            System.Text.StringBuilder text = new System.Text.StringBuilder();


            cmode = radioButton1.Checked;
            if (cmode == false)
            {
                for (int i = 0; i < rows2; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        text.Append(Convert.ToChar(myBitmap4.GetPixel(i, j).ToArgb() & 0x000000ff));
                    }

            }
            else
            {
                for (int i = 0; i < rows2; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        text.Append(Convert.ToChar(myBitmap4.GetPixel(i, j).R));
                        text.Append(Convert.ToChar(myBitmap4.GetPixel(i, j).G));
                        text.Append(Convert.ToChar(myBitmap4.GetPixel(i, j).B));
                    }
            }
            


            
            dkp = (int)numericUpDown2.Value - 1;

            Part1(pictureBox1.Image, (int)numericUpDown1.Value, text.ToString());


            string Zvit = "";
            for (int i = kmax; i <= kmax; i++)
            {
                Zvit += "";
                for (int j = 0; j < kp; j++)
                {
                    Zvit += zzList[i][j].ToString() + ";";
                }
            }
            //textBox2.Text = Zvit;
       }


        private void Step14(string FileName)
        {
            int i, j;
            // kmax->kp
            BinaryWriter FOut = new BinaryWriter(File.Create(FileName));

            FOut.Write((byte)(kp + 1));
            FOut.Write((byte)(dkp));
            FOut.Write((byte)(col));
            for (i = 0; i < col; i++)
            {
                FOut.Write((short)xc[i]);
            }

            for (i = 0; i <= kp; i++)
            {
                for (j = 0; j < col; j++)
                {
                    FOut.Write(XBList[i][j]);
                }
            }

            for (i = 0; i <= kp; i++)
            {
                FOut.Write(DList[i]);
            }

            FOut.Write(len);

            FOut.Close();
        }



        private void Step15(string FileName)
        {
            int i, j;

            BinaryReader FIn = new BinaryReader(File.OpenRead(FileName));

            // kmax->kp

            kp = FIn.ReadByte() - 1;
            dkp = FIn.ReadByte();
            col = FIn.ReadByte();
            xc = new DoubleVector(col);
            for (i = 0; i < col; i++)
            {
                xc[i] = FIn.ReadInt16();
            }


            XBList = new List<DoubleVector>();
        
            for (i = 0; i <= kp; i++)
            {
                XBList.Add(new DoubleVector(new DoubleVector(col)));
                for (j = 0; j < col; j++)
                {
                    XBList[i][j] = FIn.ReadDouble();
                }
            }
            DList = new List<double>();
            for (i = 0; i <= kp; i++)
            {
                DList.Add(FIn.ReadDouble());
            }

            len = FIn.ReadInt32();
            FIn.Close();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                kp = Convert.ToInt32(label7.Text)- 1;
                Step14(saveFileDialog1.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Step15(openFileDialog1.FileName);
                label7.Text = (kp + 1).ToString();
                numericUpDown1.Value = (int)Math.Sqrt(kp+1);
                numericUpDown2.Value = dkp + 1;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            cmode = radioButton1.Checked;

            button6.Enabled = false;
            button9.Enabled = false;
            kp = Convert.ToInt32(label7.Text) - 1;
            dkp = (int)numericUpDown2.Value-1;

            myBitmap2 = new Bitmap(pictureBox1.Image.Width,pictureBox1.Image.Height);
            myBitmap3 = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
            
            myBitmap5 = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height);

            textBox2.Text = textBox2.Text + "\r\nPSNR: "+Part2()+"\r\n";
            pictureBox2.Image = (Image)myBitmap2;
            pictureBox3.Image = (Image)myBitmap3;
            pictureBox5.Image = (Image)myBitmap5;
            button6.Enabled = true;
            button9.Enabled = true;
            textBox2.Text = textBox2.Text + "\r\nSSIM: " + Convert.ToString(SSIM) + "\r\n";
        }


        private string Part11(Image image, int cols)
        {

            System.Text.StringBuilder text = new System.Text.StringBuilder();

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

            for (int k1=0; k1<=kp; k1++)
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
                    zzList[zList.Count - 1][r] = zList[zList.Count - 1][r] * 1;
                    zzList[zList.Count - 1][r] = Math.Round(zzList[zList.Count - 1][r]);

                    // !!!!!!!!!!!!!
                    if (r < len && k1 == dkp)
                    {
                        text.Append((((char)((int)(zzList[k1][r])))).ToString());
                    }
                    // !!!!!!!!!!!!!

                }
         

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < col; c++)
                    {
                        x1[r, c] = x1[r, c] - XB[c] * zList[zList.Count - 1][r];
                    }
                }
            }
            col2 = myBitmap4.Width;
            if (cmode == false)
                rows2 = myBitmap4.Height;
            else
                rows2 = myBitmap4.Height * 3;
            double MSE2 = 0;
            double PSNR2 = 0;
            int p = 0;
            int pixel=0, pixel0=0, pixel1=0, pixel2=0;


            double mxR = 0, mxG = 0, mxB = 0;
            double myR = 0, myG = 0, myB = 0;
            double s2xR = 0, s2xG = 0, s2xB = 0;
            double s2yR = 0, s2yG = 0, s2yB = 0;
            double sxyR = 0, sxyG = 0, sxyB = 0;
            double c1 = 0, c2 = 0;
            uint L = 255;
            double k11 = 0.01, k2 = 0.03;
            double SSIMR = 0, SSIMG = 0, SSIMB = 0;

            DoubleMatrix xo2 = new DoubleMatrix(rows2, col2);
            
            if (cmode == false)
            {
                for (int i = 0; i < rows2; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        pixel = Convert.ToInt32(text[p]);
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;

                        xo2[i, j] = myBitmap4.GetPixel(i, j).ToArgb() & 0x000000ff;
                        myBitmap5.SetPixel(i, j, Color.FromArgb(Convert.ToByte(pixel), Convert.ToByte(pixel), Convert.ToByte(pixel)));
                        myBitmap6.SetPixel(i, j, Color.FromArgb(Convert.ToByte(255 - Math.Abs(xo2[i, j] - pixel)),Convert.ToByte(255 - Math.Abs(xo2[i, j] - pixel)),Convert.ToByte(255 - Math.Abs(xo2[i, j] - pixel))));

                        MSE2 += Math.Pow((Math.Abs(xo2[i, j] - pixel)), 2);
                        p++;
                        mxR += xo2[i, j]; mxG += xo2[i, j]; mxB += xo2[i, j];
                        myR += pixel; myG += pixel; myB += pixel;
                    }
                MSE2 = MSE2 / (rows2 * col2);
                PSNR2 = 10 * Math.Log10(255 * 255 / MSE2);

                mxR = mxR / (rows2 * col2); mxG = mxG / (rows2 * col2); mxB = mxB / (rows2 * col2);
                myR = myR / (rows2 * col2); myG = myG / (rows2 * col2); myB = myB / (rows2 * col2);

                p = 0;
                for (int i = 0; i < rows2; i++)
                    for (int j = 0; j < col2; j++)
                    {

                        pixel = Convert.ToInt32(text[p]);
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        s2xR += Math.Pow(xo2[i, j] - mxR, 2); s2xG += Math.Pow(xo2[i, j] - mxG, 2); s2xB += Math.Pow(xo2[i, j] - mxB, 2);
                        s2yR += Math.Pow(pixel - myR, 2); s2yG += Math.Pow(pixel - myG, 2); s2yB += Math.Pow(pixel - myB, 2);
                        sxyR += (xo2[i, j] - mxR) * (pixel - myR); sxyG += (xo2[i, j] - mxG) * (pixel - myG); sxyB += (xo2[i, j] - mxB) * (pixel - myB);
                        p++;
                    }
                s2xR = s2xR / (rows2 * col2 - 1); s2xG = s2xG / (rows2 * col2 - 1); s2xB = s2xB / (rows2 * col2 - 1);
                s2yR = s2yR / (rows2 * col2 - 1); s2yG = s2yG / (rows2 * col2 - 1); s2yB = s2yB / (rows2 * col2 - 1);
                sxyR = sxyR / (rows2 * col2 - 1); sxyG = sxyG / (rows2 * col2 - 1); sxyB = sxyB / (rows2 * col2 - 1);
                c1 = Math.Pow(k11 * L, 2); c2 = Math.Pow(k2 * L, 2);
                SSIMR = (2 * mxR * myR + c1) * (2 * sxyR + c2) / ((mxR * mxR + myR * myR + c1) * (s2xR + s2yR + c2));
                SSIMG = (2 * mxG * myG + c1) * (2 * sxyG + c2) / ((mxG * mxG + myG * myG + c1) * (s2xG + s2yG + c2));
                SSIMB = (2 * mxB * myB + c1) * (2 * sxyB + c2) / ((mxB * mxB + myB * myB + c1) * (s2xB + s2yB + c2));
                SSIM = (SSIMR + SSIMG + SSIMB) / 3;
            }
            else
            {
                for (int i = 0; i < rows2 / 3; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        pixel0 = Convert.ToByte(text[p]);
                        p++;
                        pixel1 = Convert.ToByte(text[p]);
                        p++;
                        pixel2 = Convert.ToByte(text[p]);
                        p++;

                        xo2[i * 3, j] = myBitmap4.GetPixel(i, j).R;
                        xo2[i * 3 + 1, j] = myBitmap4.GetPixel(i, j).G;
                        xo2[i * 3 + 2, j] = myBitmap4.GetPixel(i, j).B;

                        myBitmap5.SetPixel(i, j, Color.FromArgb(pixel0, pixel1, pixel2));
                        myBitmap6.SetPixel(i, j, Color.FromArgb(Convert.ToByte(255 - Math.Abs(xo2[i * 3, j] - pixel0)), Convert.ToByte(255 - Math.Abs(xo2[i * 3 + 1, j] - pixel1)), Convert.ToByte(255 - Math.Abs(xo2[i * 3 + 2, j] - pixel2))));

                        MSE2 = MSE2 += Math.Pow((Math.Abs(xo2[i * 3, j] - pixel0)), 2);
                        MSE2 = MSE2 += Math.Pow((Math.Abs(xo2[i * 3 + 1, j] - pixel1)), 2);
                        MSE2 = MSE2 += Math.Pow((Math.Abs(xo2[i * 3 + 2, j] - pixel2)), 2);

                        mxR += xo2[i * 3, j]; mxG += xo2[i * 3 + 1, j]; mxB += xo2[i * 3 + 2, j];
                        myR += pixel0; myG += pixel1; myB += pixel2;

                    }
                MSE2 = MSE2 / (rows2 * col2);
                PSNR2 = 10 * Math.Log10(255 * 255 / MSE2);

                mxR = mxR / (rows2 * col2 / 3); mxG = mxG / (rows2 * col2 / 3); mxB = mxB / (rows2 * col2 / 3);
                myR = myR / (rows2 * col2 / 3); myG = myG / (rows2 * col2 / 3); myB = myB / (rows2 * col2 / 3);

                p = 0;

                for (int i = 0; i < rows2 / 3; i++)
                    for (int j = 0; j < col2; j++)
                    {

                        pixel0 = Convert.ToByte(text[p]);
                        p++;
                        pixel1 = Convert.ToByte(text[p]);
                        p++;
                        pixel2 = Convert.ToByte(text[p]);
                        p++;
                        s2xR += Math.Pow(xo2[i * 3, j] - mxR, 2); s2xG += Math.Pow(xo2[i * 3 + 1, j] - mxG, 2); s2xB += Math.Pow(xo2[i * 3 + 2, j] - mxB, 2);
                        s2yR += Math.Pow(pixel0 - myR, 2); s2yG += Math.Pow(pixel1 - myG, 2); s2yB += Math.Pow(pixel2 - myB, 2);
                        sxyR += (xo2[i * 3, j] - mxR) * (pixel0 - myR); sxyG += (xo2[i * 3 + 1, j] - mxG) * (pixel1 - myG); sxyB += (xo2[i * 3 + 2, j] - mxB) * (pixel2 - myB);
                    }
                s2xR = s2xR / (rows2 * col2 / 3 - 1); s2xG = s2xG / (rows2 * col2 / 3 - 1); s2xB = s2xB / (rows2 * col2 / 3 - 1);
                s2yR = s2yR / (rows2 * col2 / 3 - 1); s2yG = s2yG / (rows2 * col2 / 3 - 1); s2yB = s2yB / (rows2 * col2 / 3 - 1);
                sxyR = sxyR / (rows2 * col2 / 3 - 1); sxyG = sxyG / (rows2 * col2 / 3 - 1); sxyB = sxyB / (rows2 * col2 / 3 - 1);
                c1 = Math.Pow(k11 * L, 2); c2 = Math.Pow(k2 * L, 2);
                SSIMR = (2 * mxR * myR + c1) * (2 * sxyR + c2) / ((mxR * mxR + myR * myR + c1) * (s2xR + s2yR + c2));
                SSIMG = (2 * mxG * myG + c1) * (2 * sxyG + c2) / ((mxG * mxG + myG * myG + c1) * (s2xG + s2yG + c2));
                SSIMB = (2 * mxB * myB + c1) * (2 * sxyB + c2) / ((mxB * mxB + myB * myB + c1) * (s2xB + s2yB + c2));
                SSIM = (SSIMR + SSIMG + SSIMB) / 3;

            }


            pictureBox5.Image = (Image)myBitmap5;
            pictureBox6.Image = (Image)myBitmap6;
            return Convert.ToString(PSNR2);
           
        }


        private void button8_Click(object sender, EventArgs e)
        {
            button7.Enabled = false;
            button10.Enabled = false;
            cmode = radioButton1.Checked;
            textBox2.Text = "\r\nPSNR2: " + Part11(pictureBox1.Image, (int)numericUpDown1.Value) +"\r\n";
            textBox2.Text = textBox2.Text + "\r\nSSIM2: " + Convert.ToString(SSIM) + "\r\n";
            button7.Enabled = true;
            button10.Enabled = true;
       }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = textBox1.Text.Length.ToString();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = System.Drawing.Image.FromFile(openFileDialog.FileName);
                myBitmap = new Bitmap(openFileDialog.FileName);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int kp2=(int)numericUpDown1.Value;

            label7.Text = (kp2*kp2).ToString();
            numericUpDown2.Value = kp2*kp2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox4.Image = System.Drawing.Image.FromFile(openFileDialog.FileName);
                myBitmap4 = new Bitmap(openFileDialog.FileName);
                myBitmap5 = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height);
                myBitmap6 = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height);
            }
        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }


public static Bitmap ConvertTo24bpp(Image img) {
  var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
  using (var gr = Graphics.FromImage(bmp))
    gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
  return bmp;
}

        private void button6_Click(object sender, EventArgs e)
        {

            Bitmap convBitmap;
            Graphics g;
            // Save the image with a color depth of 32/24/8 bits per pixel.

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 1: myBitmap2.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case 2:
                        convBitmap = new Bitmap(myBitmap2.Width, myBitmap2.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap2, new Rectangle(Point.Empty, myBitmap2.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    
                    case 3:
                        convBitmap = new Bitmap(myBitmap2.Width, myBitmap2.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap2, new Rectangle(Point.Empty, myBitmap2.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            Bitmap convBitmap;
            Graphics g;
            // Save the image with a color depth of 32/24/8 bits per pixel.            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 1: myBitmap5.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case 2:
                        convBitmap = new Bitmap(myBitmap5.Width, myBitmap5.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap5, new Rectangle(Point.Empty, myBitmap5.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        convBitmap = new Bitmap(myBitmap5.Width, myBitmap5.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap5, new Rectangle(Point.Empty, myBitmap5.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap convBitmap;
            Graphics g;
            // Save the image with a color depth of 32/24/8 bits per pixel.  
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 1: myBitmap3.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case 2:
                        convBitmap = new Bitmap(myBitmap3.Width, myBitmap3.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap3, new Rectangle(Point.Empty, myBitmap3.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        convBitmap = new Bitmap(myBitmap3.Width, myBitmap3.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap3, new Rectangle(Point.Empty, myBitmap3.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            Bitmap convBitmap;
            Graphics g;
            // Save the image with a color depth of 32/24/8 bits per pixel.  
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 1: myBitmap6.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case 2:
                        convBitmap = new Bitmap(myBitmap6.Width, myBitmap6.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap6, new Rectangle(Point.Empty, myBitmap6.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        convBitmap = new Bitmap(myBitmap6.Width, myBitmap6.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(convBitmap);
                        g.DrawImageUnscaledAndClipped(myBitmap6, new Rectangle(Point.Empty, myBitmap6.Size));
                        convBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
            }
        }

    }
}
