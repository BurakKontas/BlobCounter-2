using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlobCounter.Aspects;
using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;
using OpenCvSharp.WpfExtensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace BlobCounter
{
    public partial class Form1 : Form
    {
        private Bitmap _defaultImage;

        public Form1()
        {
            InitializeComponent();
            _defaultImage = (Bitmap)pictureBox1.Image;
        }

        [TimeFixing(100)]
        private void Stage1_Button_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)_defaultImage.Clone();

            byte[] grey = ImageProcessor.ConvertBitmapToGrayscaleArray(bitmap);

            var salatalikBulucu = new SalatalikSapBulma();

            // 185                      110
            var sapliSalatalikKomponentleri = salatalikBulucu.SapliSalataliklariBul(grey, 1900, 128, (int)threshold_1.Value, (int)threshold_2.Value);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (var component in sapliSalatalikKomponentleri)
                {
                    var merkez = AgirlikMerkeziHesapla(component, bitmap.Width);
                    foreach (var cmp in component)
                    {
                        int x = cmp % bitmap.Width;
                        int y = cmp / bitmap.Width;
                        bitmap.SetPixel(x, y, Color.Aqua);
                    }
                    SalatalikSapBulma.CemberCiz(g, merkez.x, merkez.y, 10, Color.Red);
                }
            } // 30 da 29 ms 15 de 21 ms sabit

            pictureBox1.Image = bitmap;

        }

        private (int x, int y) AgirlikMerkeziHesapla(int[] component, int width)
        {
            int toplamX = 0;
            int toplamY = 0;

            foreach (int idx in component)
            {
                int x = idx % width;
                int y = idx / width;
                toplamX += x;
                toplamY += y;
            }

            int merkezX = toplamX / component.Count();
            int merkezY = toplamY / component.Count();

            return (merkezX, merkezY);
        }

        private void Upload_Button_Click(object sender, EventArgs e)
        {
            UploadImage(pictureBox1);
            _defaultImage = (Bitmap)pictureBox1.Image;
        }

        private void UploadImage(PictureBox pictureBox, bool resizeBox = false)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var selectedFile = openFileDialog.FileName;

                pictureBox.Image = Image.FromFile(selectedFile);

                if (resizeBox) pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Resim yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
