using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkyRenderer;

namespace SkyRendererTest
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            mouseTimer = new System.Windows.Forms.Timer();
            SetupMouseTracking();

        }

        private System.Windows.Forms.Timer mouseTimer;
        private System.Drawing.Point lastPosition;
        private bool isMouseMoving = false;

        Image<Rgba32>? lastimage;
        CachedImage? H2FCache;


        private void btGo_Click(object sender, EventArgs e)
        {
            IImageService imageRenderer = new ImageServiceH2F();
            PrepareService(imageRenderer);
            if (H2FCache == null || !H2FCache.IsValid(imageRenderer))
            {
                H2FCache = new CachedImage(imageRenderer);
            }
            GetImageAndRender(H2FCache);
        }

        private void GetImageAndRender(IImageService imageRenderer)
        {
            var task = imageRenderer.GetImageAsync();
            task.ContinueWith(t =>
            {
                var image = t.Result;
                lastimage = image.Clone();
                if (chkRedCross.Checked)
                    ImageHelper.AddReticle(image);
                pictureBox1.Image = ImageHelper.ConvertToBitmap(image);
            });
        }

        private void PrepareService(IImageService imageRenderer)
        {
            imageRenderer.RightAscension = (double)numRA.Value;
            imageRenderer.Declination = (double)numDEC.Value;
            imageRenderer.ImageScale = (double)numImageScale.Value;
            imageRenderer.Width = pictureBox1.Width;
            imageRenderer.Height = pictureBox1.Height;
            imageRenderer.RotationAngle = (double)numRotation.Value;
        }

        private void btSyntetic_Click(object sender, EventArgs e)
        {
            Redraw();
        }

        private void bt2RA_Click(object sender, EventArgs e)
        {
            try
            {
                suspendRedraw = true;

                // RA
                var ra24 = DmsHms2numeric(txtCoordRA.Text);
                var ra360 = ra24 * 15;
                numRA.Value = (decimal)ra360;

                // DEC
                double dec360 = DmsHms2numeric(txtCoordDEC.Text);
                numDEC.Value = (decimal)dec360;
            }
            finally
            {
                suspendRedraw = false;
            }
            Redraw();
        }

        private static readonly char[] formatSeparators = new char[] { ' ', '\t', 'h', 'm', 's', '°', '\'', '′', '"', '″' };

        private static double DmsHms2numeric(string txt)
        {
            var parts = txt.Split(formatSeparators, StringSplitOptions.RemoveEmptyEntries);

            var ch = txt2num(parts[0]);
            var cm = parts.Length > 1 ? txt2num(parts[1]) : 0;
            var cs = parts.Length > 2 ? txt2num(parts[2]) : 0;

            var sign = Math.Sign(ch);
            ch = Math.Abs(ch);

            var numeric = sign * (ch + (cm + cs / 60) / 60);
            return numeric;
        }

        private static double txt2num(string txt)
        {
            var modTxt = txt.Replace(',', '.').Replace('−', '-');
            if (double.TryParse(modTxt, CultureInfo.InvariantCulture, out var num))
                return num;
            return 0;
        }

        private void btSaveImage_Click(object sender, EventArgs e)
        {
            if (lastimage == null)
                return;
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Jpeg|*.jpg|Bitmap|*.bmp|Png|*.png ";
                dialog.DefaultExt = "png";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    lastimage.Save(dialog.FileName);
                }
            }
        }

        bool inside_ra = false;

        private void numRA_ValueChanged(object sender, EventArgs e)
        {
            if (inside_ra)
                return;

            inside_ra = true;
            try
            {
                if (numRA.Value == 360)
                    numRA.Value = 0;
                else if (numRA.Value == 0)
                    numRA.Value = 360;
            }
            finally
            {
                inside_ra = false;
            }
            Redraw();
        }

        private void numDEC_ValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void numFOV_ValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void numImageScale_ValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void numMagLimit_ValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        bool suspendRedraw;

        private void Redraw()
        {
            if (suspendRedraw)
                return;
            var imageRenderer = new ImageServiceTycho();
            PrepareService(imageRenderer);

            imageRenderer.UseMoffatProfile = chkUseMoffat.Checked;
            imageRenderer.UseColor = chkUseColor.Checked;


            GetImageAndRender(imageRenderer);

            var rightAscension = (double)numRA.Value;
            var declination = (double)numDEC.Value;

            txtCoordRA.Text = Dec2HMS(rightAscension);
            txtCoordDEC.Text = Dec2DMS(declination);

            UpdateImageSize();
        }

        private void UpdateImageSize()
        {
            txtImageSizeW.Text = pictureBox1.Width.ToString();
            txtImageSizeH.Text = pictureBox1.Height.ToString();
        }

        private static string Dec2DMS(double d)
        {
            var sign = Math.Sign(d);
            d = Math.Abs(d);
            var h = Math.Floor(d);
            d -= h;
            d *= 60;
            var m = Math.Floor(d);
            d -= m;
            d *= 60;
            d = Math.Round(d, 2);

            h = sign * h;

            var s = $"{h}° {m}' {d}\"";
            return s;
        }

        private static string Dec2HMS(double ra)
        {
            var sign = Math.Sign(ra);
            ra = Math.Abs(ra);
            var d = ra / 15.0;
            var h = Math.Floor(d);
            d -= h;
            d *= 60;
            var m = Math.Floor(d);
            d -= m;
            d *= 60;
            d = Math.Round(d, 2);

            h = sign * h;
            var s = $"{h}h {m}m {d}s";
            return s;

        }

        private void chkUseMoffat_CheckedChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void numExpositionFactor_ValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void chkUseColor_CheckedChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        bool inside_rotation;

        private void numRotation_ValueChanged(object sender, EventArgs e)
        {
            if (inside_rotation)
                return;

            inside_rotation = true;
            try
            {
                if (numRotation.Value == 360)
                    numRotation.Value = 0;
                else if (numRotation.Value == 0)
                    numRotation.Value = 360;
            }
            finally
            {
                inside_rotation = false;
            }
            Redraw();
        }

        private void chkRedCross_CheckedChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void btGotoOrion_Click(object sender, EventArgs e)
        {
            try
            {
                // Anilam coordinates
                suspendRedraw = true;
                numRA.Value = 84.0533m;
                numDEC.Value = -1.2019m;
                numRotation.Value = 0;
                numImageScale.Value = 130;
            }
            finally { suspendRedraw = false; }

            Redraw();
        }

        private void btGotoPleiades_Click(object sender, EventArgs e)
        {
            try
            {
                // Anilam coordinates
                suspendRedraw = true;
                numRA.Value = 56.60m;
                numDEC.Value = 24.10m;
                numRotation.Value = 0;
                numImageScale.Value = 10;
            }
            finally { suspendRedraw = false; }

            Redraw();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //// PictureBox Center
            //int centerX = pictureBox1.Width / 2;
            //int centerY = pictureBox1.Height / 2;

            // mouse coordinates
            int mouseX = e.X;
            int mouseY = e.Y;

            //var scaleDeg = numImageScale.Value / 3600.0m;

            //var dx = (centerX - mouseX) * scaleDeg;
            //var dy = (centerY - mouseY) * scaleDeg;

            //numRA.Value += dx;
            //numDEC.Value += dy;

            var cc = new CoordinateConverter((double)numRA.Value, (double)numDEC.Value, pictureBox1.Width, pictureBox1.Height, (double)numImageScale.Value, (double)numRotation.Value);
            var newCoord = cc.ConvertXYToRaDec(mouseX, mouseY);

            try
            {
                suspendRedraw = true;
                numRA.Value = (decimal)newCoord.ra;
                numDEC.Value = (decimal)newCoord.dec;
            }
            finally
            {
                suspendRedraw = false;
            }
            Redraw();
        }

        private void PictureBox1_MouseWheel(object? sender, MouseEventArgs e)
        {
            var delta = e.Delta;
            var shift = (numImageScale.Value / 200) * delta;
            var newValue = numImageScale.Value + shift;
            newValue = Math.Max(newValue, numImageScale.Minimum);
            newValue = Math.Min(newValue, numImageScale.Maximum);
            numImageScale.Value = newValue;
        }


        private void SetupMouseTracking()
        {
            // Configurazione del timer
            mouseTimer.Interval = 500; // 500 millisecondi = mezzo secondo
            mouseTimer.Tick += MouseTimer_Tick;

            // Eventi della PictureBox
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseLeave += PictureBox1_MouseLeave;
        }

        private void PictureBox1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (lastPosition != e.Location)
            {
                isMouseMoving = true;
                lastPosition = e.Location;
                mouseTimer.Stop();
                mouseTimer.Start();
            }
        }

        private void PictureBox1_MouseLeave(object? sender, EventArgs e)
        {
            mouseTimer.Stop();
            isMouseMoving = false;
        }

        private void MouseTimer_Tick(object? sender, EventArgs e)
        {
            if (isMouseMoving)
            {
                // Il mouse è stato fermo per mezzo secondo
                isMouseMoving = false;
                mouseTimer.Stop();

                // Stampa le coordinate
                UpdateMouseCoordinates();
            }
        }

        private void UpdateMouseCoordinates()
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);
            var cc = new CoordinateConverter((double)numRA.Value, (double)numDEC.Value, pictureBox1.Width, pictureBox1.Height, (double)numImageScale.Value, (double)numRotation.Value);
            var coord = cc.ConvertXYToRaDec(pos.X, pos.Y);
            txtCurrentMousePosition.Text = $"{coord.ra:0.0000}, {coord.dec:0.0000}";
        }

    }
}
