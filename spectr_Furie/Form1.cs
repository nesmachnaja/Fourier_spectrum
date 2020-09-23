using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace spectr_Furie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // открытие изображения
            Bitmap image;
            OpenFileDialog open_dialog = new OpenFileDialog();
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    image = new Bitmap(open_dialog.FileName);
                    pictureBox1.Size = image.Size;
                    pictureBox1.Image = image;
                    pictureBox1.Invalidate();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void спектрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image==null)
                {
                    MessageBox.Show("Откройте изображение!");
                    return;
                }
            int N = pictureBox1.Height; // n строк изображения, m столбцов
            int M = N;
            int j, k, i;
            Bitmap image = new Bitmap(pictureBox1.Image);
            var fxy = new float[image.Height, image.Width];     //массив предварительной обработки
            var fuv = new float[image.Height, image.Width];     //массив преобразования фурье
            var fuv_re = new float[image.Height, image.Width];  //массив действительной части
            var fuv_im = new float[image.Height, image.Width];  //массив мнимой части
            var fxv_re = new float[image.Height, image.Width];
            var fxv_im = new float[image.Height, image.Width];

            //-----------забираем пиксели в массив------------------
            //-----------умножаем изображение на -1 в степени x+y---
            for (i=0; i<image.Height; i++)  //высота
                for (j=0; j<image.Width; j++)   //ширина
                {
                    // fxy[i, j] = Convert.ToInt32(image.GetPixel(j, i));     //забираем пиксели в массив
                    Color col = image.GetPixel(j, i);
                    //  int R = col.R;
                    // int G = col.G;
                    //  int B = col.B;
                    //  fxy[i, j] = R * 1000000 + G * 1000 + B;
                    fxy[i, j] = col.ToArgb();
                    fxy[i, j] = fxy[i, j] * (float)Math.Pow(-1,i+j); //    умножаем функцию на -1 в степени 3
                }
            //MessageBox.Show(fxy[100, 100].ToString());

            //------------первое число массива-строка, второе-столбец
            //------------по строкам

            float Wi = image.Width;
            float Hi = image.Height;
            for (i = 0; i < Hi; i++)  //высота
                for (j = 0; j < Wi; j++)   //ширина
                    for (k = 0; k < Wi; k++)
                    {
                        fxv_re[i, j] = fxv_re[i, j] + (float)Math.Round(fxy[i, k] * Math.Cos((2 * Math.PI * j * k) / Wi));
                        fxv_im[i, j] = fxv_im[i, j] + (float)Math.Round(fxy[i, k] * Math.Sin((2 * Math.PI * j * k) / Wi));
                    }
            //MessageBox.Show(fxv_re[100, 100].ToString()+" & "+ fxv_im[100, 100].ToString());

            //------------по столбцам

            for (j = 0; j < Wi; j++)
                for (i = 0; i < Hi; i++)
                    for (k = 0; k < Hi; k++)
                    {
                        fuv_re[i, j] = fuv_re[i, j] + fxv_re[k, j] * (float)Math.Cos((2 * Math.PI * i * k) / Hi) + fxv_im[k, j] * (float)Math.Sin((2 * Math.PI * j * k) / Hi);
                        fuv_im[i, j] = fuv_im[i, j] + fxv_re[k, j] * (float)Math.Sin((2 * Math.PI * i * k) / Hi) - fxv_im[k, j] * (float)Math.Cos((2 * Math.PI * j * k) / Hi);
                    }

            //------------выделяем действительную часть
            for (i = 0; i < Hi; i++)
                for (j = 0; j < Wi; j++)
                {
                    fuv[i, j] = (float)Math.Sqrt(Math.Pow(fuv_re[i, j],2) + Math.Pow(fuv_im[i, j],2));
                }
            //MessageBox.Show(fuv[100, 100].ToString());

            //------------вывод спектра изображения
            for (i = 0; i < Hi; i++)  
                for (j = 0; j < Wi; j++)
                {
                    image.SetPixel(j, i, Color.FromArgb((int)Math.Round(fuv[i, j]))) ;   //выводим результат
                   // image.SetPixel(j, i, Color.Beige);
                    pictureBox1.Image = image;
                }
            //MessageBox.Show(fuv[100, 100].ToString());

        }
    }
}
