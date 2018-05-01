using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Image_Inspector
{
    public partial class Form1 : Form
    {
        private Bitmap originalBitmap = null;
        private Bitmap previewBitmap = null;
        private Bitmap resultBitmap = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(OpenFileDialog.FileName);
                originalBitmap = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
                streamReader.Close();

                previewBitmap = originalBitmap;
                MainPictureBox.Image = previewBitmap;

                UpdateImage();

                RedTrackBar.Enabled = true;
                GreenTrackBar.Enabled = true;
                BlueTrackBar.Enabled = true;
                TrackBarChanged(null, null);
            }
        }

        private void TrackBarChanged(object sender, EventArgs e)
        {
            RedLabel.Text = "Red: " + RedTrackBar.Value;
            GreenLabel.Text = "Green: " + GreenTrackBar.Value;
            BlueLabel.Text = "Blue: " + BlueTrackBar.Value;
            UpdateImage();
        }

        private void UpdateImage()
        {
            if (previewBitmap == null)
                return;

            MainPictureBox.Image = previewBitmap.Balance((byte)RedTrackBar.Value, (byte)GreenTrackBar.Value, (byte)BlueTrackBar.Value);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (originalBitmap == null)
                return;

            resultBitmap = originalBitmap.Balance((byte)RedTrackBar.Value, (byte)GreenTrackBar.Value, (byte)BlueTrackBar.Value);

            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileExtension = Path.GetExtension(SaveFileDialog.FileName).ToUpper();

                ImageFormat imageFormat = ImageFormat.Png;

                if (fileExtension == "JPG")
                    imageFormat = ImageFormat.Jpeg;

                StreamWriter streamWriter = new StreamWriter(SaveFileDialog.FileName, false);
                resultBitmap.Save(streamWriter.BaseStream, imageFormat);
                streamWriter.Flush();
                streamWriter.Close();
                SaveFileDialog.FileName = "";

                resultBitmap = null;
            }
        }
    }
}
