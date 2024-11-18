using System.Drawing;
using System.Windows.Forms;

namespace SkyRendererTest
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            btGo = new Button();
            btSyntetic = new Button();
            txtCoordRA = new TextBox();
            bt2RA = new Button();
            txtCoordDEC = new TextBox();
            btSaveImage = new Button();
            chkUseMoffat = new CheckBox();
            numRA = new NumericUpDown();
            numDEC = new NumericUpDown();
            label5 = new Label();
            numImageScale = new NumericUpDown();
            chkUseColor = new CheckBox();
            numRotation = new NumericUpDown();
            label4 = new Label();
            chkRedCross = new CheckBox();
            groupBox1 = new GroupBox();
            label6 = new Label();
            label3 = new Label();
            groupBox2 = new GroupBox();
            label7 = new Label();
            label8 = new Label();
            txtImageSizeW = new TextBox();
            txtImageSizeH = new TextBox();
            btGotoOrion = new Button();
            btGotoPleiades = new Button();
            groupBox3 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRA).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDEC).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numImageScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRotation).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 14);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 0;
            label1.Text = "RA (deg):";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 43);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
            label2.TabIndex = 1;
            label2.Text = "DEC (deg):";
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(198, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 600);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
            // 
            // btGo
            // 
            btGo.Location = new Point(15, 236);
            btGo.Name = "btGo";
            btGo.Size = new Size(161, 23);
            btGo.TabIndex = 4;
            btGo.Text = "Load from CDS WebService";
            btGo.UseVisualStyleBackColor = true;
            btGo.Click += btGo_Click;
            // 
            // btSyntetic
            // 
            btSyntetic.Location = new Point(15, 207);
            btSyntetic.Name = "btSyntetic";
            btSyntetic.Size = new Size(161, 23);
            btSyntetic.TabIndex = 5;
            btSyntetic.Text = "Generate Syntetic Image";
            btSyntetic.UseVisualStyleBackColor = true;
            btSyntetic.Click += btSyntetic_Click;
            // 
            // txtCoordRA
            // 
            txtCoordRA.Location = new Point(75, 22);
            txtCoordRA.Name = "txtCoordRA";
            txtCoordRA.Size = new Size(91, 23);
            txtCoordRA.TabIndex = 7;
            txtCoordRA.Text = "5 40 51.63";
            // 
            // bt2RA
            // 
            bt2RA.Location = new Point(75, 80);
            bt2RA.Name = "bt2RA";
            bt2RA.Size = new Size(91, 23);
            bt2RA.TabIndex = 8;
            bt2RA.Text = "Apply";
            bt2RA.UseVisualStyleBackColor = true;
            bt2RA.Click += bt2RA_Click;
            // 
            // txtCoordDEC
            // 
            txtCoordDEC.Location = new Point(75, 51);
            txtCoordDEC.Name = "txtCoordDEC";
            txtCoordDEC.Size = new Size(91, 23);
            txtCoordDEC.TabIndex = 9;
            txtCoordDEC.Text = "-1 07 36.9";
            // 
            // btSaveImage
            // 
            btSaveImage.Location = new Point(17, 328);
            btSaveImage.Name = "btSaveImage";
            btSaveImage.Size = new Size(161, 23);
            btSaveImage.TabIndex = 11;
            btSaveImage.Text = "Save Image";
            btSaveImage.UseVisualStyleBackColor = true;
            btSaveImage.Click += btSaveImage_Click;
            // 
            // chkUseMoffat
            // 
            chkUseMoffat.AutoSize = true;
            chkUseMoffat.Checked = true;
            chkUseMoffat.CheckState = CheckState.Checked;
            chkUseMoffat.Location = new Point(17, 131);
            chkUseMoffat.Name = "chkUseMoffat";
            chkUseMoffat.Size = new Size(121, 19);
            chkUseMoffat.TabIndex = 6;
            chkUseMoffat.Text = "Use Moffat profile";
            chkUseMoffat.UseVisualStyleBackColor = true;
            chkUseMoffat.CheckedChanged += chkUseMoffat_CheckedChanged;
            // 
            // numRA
            // 
            numRA.DecimalPlaces = 4;
            numRA.Location = new Point(93, 12);
            numRA.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numRA.Name = "numRA";
            numRA.Size = new Size(85, 23);
            numRA.TabIndex = 0;
            numRA.Value = new decimal(new int[] { 840533, 0, 0, 262144 });
            numRA.ValueChanged += numRA_ValueChanged;
            // 
            // numDEC
            // 
            numDEC.DecimalPlaces = 4;
            numDEC.Location = new Point(93, 41);
            numDEC.Maximum = new decimal(new int[] { 90, 0, 0, 0 });
            numDEC.Minimum = new decimal(new int[] { 90, 0, 0, int.MinValue });
            numDEC.Name = "numDEC";
            numDEC.Size = new Size(85, 23);
            numDEC.TabIndex = 1;
            numDEC.Value = new decimal(new int[] { 12019, 0, 0, -2147221504 });
            numDEC.ValueChanged += numDEC_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(17, 101);
            label5.Name = "label5";
            label5.Size = new Size(73, 15);
            label5.TabIndex = 21;
            label5.Text = "arcsec/pixel:";
            // 
            // numImageScale
            // 
            numImageScale.DecimalPlaces = 2;
            numImageScale.Location = new Point(93, 99);
            numImageScale.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numImageScale.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            numImageScale.Name = "numImageScale";
            numImageScale.Size = new Size(85, 23);
            numImageScale.TabIndex = 2;
            numImageScale.Value = new decimal(new int[] { 130, 0, 0, 0 });
            numImageScale.ValueChanged += numImageScale_ValueChanged;
            // 
            // chkUseColor
            // 
            chkUseColor.AutoSize = true;
            chkUseColor.Checked = true;
            chkUseColor.CheckState = CheckState.Checked;
            chkUseColor.Location = new Point(17, 156);
            chkUseColor.Name = "chkUseColor";
            chkUseColor.Size = new Size(77, 19);
            chkUseColor.TabIndex = 22;
            chkUseColor.Text = "Use Color";
            chkUseColor.UseVisualStyleBackColor = true;
            chkUseColor.CheckedChanged += chkUseColor_CheckedChanged;
            // 
            // numRotation
            // 
            numRotation.DecimalPlaces = 2;
            numRotation.Location = new Point(93, 70);
            numRotation.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numRotation.Name = "numRotation";
            numRotation.Size = new Size(85, 23);
            numRotation.TabIndex = 25;
            numRotation.ValueChanged += numRotation_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(34, 72);
            label4.Name = "label4";
            label4.Size = new Size(52, 15);
            label4.TabIndex = 26;
            label4.Text = "rotation:";
            label4.Click += label4_Click;
            // 
            // chkRedCross
            // 
            chkRedCross.AutoSize = true;
            chkRedCross.Location = new Point(17, 181);
            chkRedCross.Name = "chkRedCross";
            chkRedCross.Size = new Size(105, 19);
            chkRedCross.TabIndex = 27;
            chkRedCross.Text = "Show red cross";
            chkRedCross.UseVisualStyleBackColor = true;
            chkRedCross.CheckedChanged += chkRedCross_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtCoordRA);
            groupBox1.Controls.Add(txtCoordDEC);
            groupBox1.Controls.Add(bt2RA);
            groupBox1.Location = new Point(10, 362);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(178, 117);
            groupBox1.TabIndex = 28;
            groupBox1.TabStop = false;
            groupBox1.Text = "coordinate converter";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 54);
            label6.Name = "label6";
            label6.Size = new Size(63, 15);
            label6.TabIndex = 11;
            label6.Text = "DEC (dms)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 25);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 10;
            label3.Text = "RA (hms)";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(txtImageSizeW);
            groupBox2.Controls.Add(txtImageSizeH);
            groupBox2.Location = new Point(10, 485);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(178, 90);
            groupBox2.TabIndex = 29;
            groupBox2.TabStop = false;
            groupBox2.Text = "Image size";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 54);
            label7.Name = "label7";
            label7.Size = new Size(43, 15);
            label7.TabIndex = 11;
            label7.Text = "Height";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 25);
            label8.Name = "label8";
            label8.Size = new Size(39, 15);
            label8.TabIndex = 10;
            label8.Text = "Width";
            // 
            // txtImageSizeW
            // 
            txtImageSizeW.Location = new Point(75, 22);
            txtImageSizeW.Name = "txtImageSizeW";
            txtImageSizeW.ReadOnly = true;
            txtImageSizeW.Size = new Size(91, 23);
            txtImageSizeW.TabIndex = 7;
            // 
            // txtImageSizeH
            // 
            txtImageSizeH.Location = new Point(75, 51);
            txtImageSizeH.Name = "txtImageSizeH";
            txtImageSizeH.ReadOnly = true;
            txtImageSizeH.Size = new Size(91, 23);
            txtImageSizeH.TabIndex = 9;
            // 
            // btGotoOrion
            // 
            btGotoOrion.Location = new Point(9, 22);
            btGotoOrion.Name = "btGotoOrion";
            btGotoOrion.Size = new Size(71, 23);
            btGotoOrion.TabIndex = 30;
            btGotoOrion.Text = "Orion";
            btGotoOrion.UseVisualStyleBackColor = true;
            btGotoOrion.Click += btGotoOrion_Click;
            // 
            // btGotoPleiades
            // 
            btGotoPleiades.Location = new Point(85, 22);
            btGotoPleiades.Name = "btGotoPleiades";
            btGotoPleiades.Size = new Size(83, 23);
            btGotoPleiades.TabIndex = 31;
            btGotoPleiades.Text = "Pleiades";
            btGotoPleiades.UseVisualStyleBackColor = true;
            btGotoPleiades.Click += btGotoPleiades_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btGotoPleiades);
            groupBox3.Controls.Add(btGotoOrion);
            groupBox3.Location = new Point(10, 265);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(178, 55);
            groupBox3.TabIndex = 32;
            groupBox3.TabStop = false;
            groupBox3.Text = "Go To";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(998, 600);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(chkRedCross);
            Controls.Add(numRotation);
            Controls.Add(label4);
            Controls.Add(chkUseColor);
            Controls.Add(numImageScale);
            Controls.Add(label5);
            Controls.Add(numDEC);
            Controls.Add(numRA);
            Controls.Add(chkUseMoffat);
            Controls.Add(btSaveImage);
            Controls.Add(btSyntetic);
            Controls.Add(btGo);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(groupBox3);
            Name = "MainForm";
            Text = "SkyRenderer";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRA).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDEC).EndInit();
            ((System.ComponentModel.ISupportInitialize)numImageScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRotation).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private PictureBox pictureBox1;
        private Button btGo;
        private Button btSyntetic;
        private TextBox txtCoordRA;
        private Button bt2RA;
        private TextBox txtCoordDEC;
        private Button btSaveImage;
        private CheckBox chkUseMoffat;
        private NumericUpDown numRA;
        private NumericUpDown numDEC;
        private Label label5;
        private NumericUpDown numImageScale;
        private CheckBox chkUseColor;
        private NumericUpDown numRotation;
        private Label label4;
        private CheckBox chkRedCross;
        private GroupBox groupBox1;
        private Label label6;
        private Label label3;
        private GroupBox groupBox2;
        private Label label7;
        private Label label8;
        private TextBox txtImageSizeW;
        private TextBox txtImageSizeH;
        private Button btGotoOrion;
        private Button btGotoPleiades;
        private GroupBox groupBox3;
    }
}
