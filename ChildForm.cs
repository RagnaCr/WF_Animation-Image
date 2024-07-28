using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SP_coursework
{
    public partial class ChildForm : Form
    {
        //основні
        private Bitmap image;
        public string FilePath { get; set; }
        public bool HasUnsavedChanges { get; private set; }

        //для обертання зображення
        private VScrollBar vScrollBar;
        private int rotationAngle = 0;

        //для анімації
        private List<string> imagePaths;
        private int currentImageIndex;
        private PictureBox pictureBox;
        private Timer animationTimer;
        private Point imageCenter;

        public ChildForm()
        {
            InitializeComponent();
            InitializeVScrollBar();
            animationTimer = new Timer();
            animationTimer.Tick += AnimationTimer_Tick;

            pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pictureBox);
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Visible = false;
        }
        private void InitializeVScrollBar()
        {
            vScrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Minimum = 0,
                Maximum = 360,
                SmallChange = 1,
                LargeChange = 10
            };
            vScrollBar.Scroll += VScrollBar_Scroll;
            this.Controls.Add(vScrollBar);
        }

        //для анімації
        public void SetAnimationSettings(List<string> imagePaths, int interval)
        {
            this.imagePaths = imagePaths;
            this.animationTimer.Interval = interval;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (imagePaths != null && imagePaths.Count > 0)
            {
                currentImageIndex = (currentImageIndex + 1) % imagePaths.Count;
                using (Bitmap originalBitmap = new Bitmap(imagePaths[currentImageIndex]))
                {
                    Bitmap transparentBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format32bppArgb);
                    using (Graphics graphics = Graphics.FromImage(transparentBitmap))
                    {
                        graphics.Clear(Color.Transparent);
                        graphics.DrawImage(originalBitmap, Point.Empty);
                    }

                    pictureBox.Image = transparentBitmap;
                }
                pictureBox.Location = new Point(imageCenter.X - 30, imageCenter.Y - 30);
            }
        }

        private void ChildForm_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox.Visible = true;
            if (e.Button == MouseButtons.Left && imagePaths != null && imagePaths.Count > 0)
            {
                animationTimer.Enabled = true;
                imageCenter = e.Location;
                animationTimer.Start();
            }
            else if (e.Button == MouseButtons.Right)
            {
                animationTimer.Enabled = false;
                animationTimer.Stop();
            }
            else
            {
                pictureBox.Visible = false;
            }
        }

        //для прокрутки
        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            rotationAngle = vScrollBar.Value;
            RotateImage(rotationAngle);
        }
        public void RotateImageByDialog(int angle, string direction)
        {
            if (image == null) return;

            if (direction == "Против часової стрілки")
            {
                angle = 360 - angle;
            }

            rotationAngle = angle;
            RotateImage(rotationAngle);
        }
        private void RotateImage(int angle)
        {
            if (image == null)
                return;

            var rotatedImage = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform((float)image.Width / 2, (float)image.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)image.Width / 2, -(float)image.Height / 2);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Point(0, 0));
            }
            this.BackgroundImage = rotatedImage;
        }
        //----------------

        //основний
        public void LoadImage(string path)
        {
            if (image != null)
            {
                image.Dispose();
            }

            using (var tempImage = new Bitmap(path))
            {
                image = new Bitmap(tempImage);
            }

            FilePath = path;
            this.BackgroundImage = (Image)image.Clone();
            this.BackgroundImageLayout = ImageLayout.Stretch;
            HasUnsavedChanges = false;
        }
        public void SaveImage()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                using (var tempImage = new Bitmap(image))
                {
                    tempImage.Save(FilePath);
                }
                HasUnsavedChanges = false;
            }
            else
            {
                string path = SaveImageAs();
                if (!string.IsNullOrEmpty(path))
                {
                    FilePath = path;
                    HasUnsavedChanges = false;
                }
            }
        }
        public string SaveImageAs()
        {
            if (image == null)
            {
                MessageBox.Show("Немає зображення для збереження.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.png;*.gif;*.tiff"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog.FileName;
                using (var tempImage = new Bitmap(image))
                {
                    tempImage.Save(path);
                }
                FilePath = path;
                HasUnsavedChanges = false;
                return path;
            }

            return string.Empty;
        }
        private void ChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasUnsavedChanges)
            {
                var result = MessageBox.Show("Зберегти зміни?", "Завершення", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveImage();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
        //--------------

        //ПЕРЕДЕЛАТЬ
        public string GetImageInfo()
        {
            if (image != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(FilePath); ;
                string fullPath = Path.GetFullPath(fileName);
                string format = Path.GetExtension(FilePath).Substring(1);
                int width = image.Width;
                int height = image.Height;
                float horizontalResolution = image.HorizontalResolution;
                float verticalResolution = image.VerticalResolution;
                float physicalWidth = width / horizontalResolution * 2.54f;
                float physicalHeight = height / verticalResolution * 2.54f;
                string pixelFormat = image.PixelFormat.ToString();
                bool hasAlpha = Image.IsAlphaPixelFormat(image.PixelFormat);
                int bitsPerPixel = Image.GetPixelFormatSize(image.PixelFormat);

                StringBuilder infoBuilder = new StringBuilder();
                infoBuilder.AppendLine($"Ім'я файлу: {fileName}\n");
                infoBuilder.AppendLine($"Повний шлях до файлу: {fullPath}\n");
                infoBuilder.AppendLine($"Формат файлу: {format}\n");
                infoBuilder.AppendLine($"Розміри в пікселях: {width}x{height}\n");
                infoBuilder.AppendLine($"Вертикальна роздільна здатність: {verticalResolution} точок/см\n");
                infoBuilder.AppendLine($"Горизонтальна роздільна здатність: {horizontalResolution} точок/см\n");
                infoBuilder.AppendLine($"Фізичні розміри: {physicalWidth}x{physicalHeight} см\n");
                infoBuilder.AppendLine($"Формат пікселів: {pixelFormat}\n");
                infoBuilder.AppendLine($"Використання біта або байта прозорості: {(hasAlpha ? "Біт" : "Байт")}\n");
                infoBuilder.AppendLine($"Кількість біт на піксель: {bitsPerPixel}");

                return infoBuilder.ToString();
            }
            else
            {
                return "Зображення не завантажено.";
            }
        }
    }
}

