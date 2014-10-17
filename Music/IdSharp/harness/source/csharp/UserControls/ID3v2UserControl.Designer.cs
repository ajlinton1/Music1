namespace IdSharpHarness
{
    partial class ID3v2UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ID3v2UserControl));
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.imageBindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.label9 = new System.Windows.Forms.Label();
            this.txtArtist = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtImageDescription = new System.Windows.Forms.TextBox();
            this.txtAlbum = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.cmbImageType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTrackNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbID3v2 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblID3v2 = new System.Windows.Forms.Label();
            this.cmbGenre = new System.Windows.Forms.ComboBox();
            this.imageOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.imageSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.imageContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miLoadImage = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveImage = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.imageBindingNavigator)).BeginInit();
            this.imageBindingNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.imageContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(94, 9);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(310, 20);
            this.txtFilename.TabIndex = 101;
            this.txtFilename.TabStop = false;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(407, 277);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 13);
            this.label10.TabIndex = 126;
            this.label10.Text = "Description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 106;
            this.label1.Text = "Artist";
            // 
            // imageBindingNavigator
            // 
            this.imageBindingNavigator.AddNewItem = this.bindingNavigatorAddNewItem;
            this.imageBindingNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imageBindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.imageBindingNavigator.DeleteItem = this.bindingNavigatorDeleteItem;
            this.imageBindingNavigator.Dock = System.Windows.Forms.DockStyle.None;
            this.imageBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem});
            this.imageBindingNavigator.Location = new System.Drawing.Point(410, 297);
            this.imageBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.imageBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.imageBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.imageBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.imageBindingNavigator.Name = "imageBindingNavigator";
            this.imageBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.imageBindingNavigator.Size = new System.Drawing.Size(256, 25);
            this.imageBindingNavigator.TabIndex = 124;
            this.imageBindingNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            this.bindingNavigatorAddNewItem.Click += new System.EventHandler(this.bindingNavigatorAddNewItem_Click);
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(36, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            this.bindingNavigatorDeleteItem.Click += new System.EventHandler(this.bindingNavigatorDeleteItem_Click);
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 21);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(407, 250);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 125;
            this.label9.Text = "Picture Type";
            // 
            // txtArtist
            // 
            this.txtArtist.Location = new System.Drawing.Point(94, 62);
            this.txtArtist.Name = "txtArtist";
            this.txtArtist.Size = new System.Drawing.Size(310, 20);
            this.txtArtist.TabIndex = 104;
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(94, 88);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(310, 20);
            this.txtTitle.TabIndex = 105;
            // 
            // txtImageDescription
            // 
            this.txtImageDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImageDescription.Enabled = false;
            this.txtImageDescription.Location = new System.Drawing.Point(484, 274);
            this.txtImageDescription.Name = "txtImageDescription";
            this.txtImageDescription.Size = new System.Drawing.Size(210, 20);
            this.txtImageDescription.TabIndex = 117;
            this.txtImageDescription.Validated += new System.EventHandler(this.txtImageDescription_Validated);
            // 
            // txtAlbum
            // 
            this.txtAlbum.Location = new System.Drawing.Point(94, 114);
            this.txtAlbum.Name = "txtAlbum";
            this.txtAlbum.Size = new System.Drawing.Size(310, 20);
            this.txtAlbum.TabIndex = 107;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ContextMenuStrip = this.imageContextMenu;
            this.pictureBox1.Location = new System.Drawing.Point(410, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(284, 232);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 123;
            this.pictureBox1.TabStop = false;
            // 
            // txtYear
            // 
            this.txtYear.Location = new System.Drawing.Point(94, 166);
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(75, 20);
            this.txtYear.TabIndex = 114;
            // 
            // cmbImageType
            // 
            this.cmbImageType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbImageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImageType.Enabled = false;
            this.cmbImageType.FormattingEnabled = true;
            this.cmbImageType.Location = new System.Drawing.Point(484, 247);
            this.cmbImageType.Name = "cmbImageType";
            this.cmbImageType.Size = new System.Drawing.Size(210, 21);
            this.cmbImageType.TabIndex = 116;
            this.cmbImageType.SelectedIndexChanged += new System.EventHandler(this.cmbImageType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 108;
            this.label2.Text = "Title";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 109;
            this.label3.Text = "Album";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 195);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 122;
            this.label8.Text = "Track";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 110;
            this.label4.Text = "Genre";
            // 
            // txtTrackNumber
            // 
            this.txtTrackNumber.Location = new System.Drawing.Point(94, 192);
            this.txtTrackNumber.Name = "txtTrackNumber";
            this.txtTrackNumber.Size = new System.Drawing.Size(75, 20);
            this.txtTrackNumber.TabIndex = 115;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 112;
            this.label5.Text = "Year";
            // 
            // cmbID3v2
            // 
            this.cmbID3v2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbID3v2.FormattingEnabled = true;
            this.cmbID3v2.Items.AddRange(new object[] {
            "ID3v2.2",
            "ID3v2.3",
            "ID3v2.4"});
            this.cmbID3v2.Location = new System.Drawing.Point(94, 35);
            this.cmbID3v2.Name = "cmbID3v2";
            this.cmbID3v2.Size = new System.Drawing.Size(75, 21);
            this.cmbID3v2.TabIndex = 103;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 113;
            this.label6.Text = "File name";
            // 
            // lblID3v2
            // 
            this.lblID3v2.AutoSize = true;
            this.lblID3v2.Location = new System.Drawing.Point(10, 38);
            this.lblID3v2.Name = "lblID3v2";
            this.lblID3v2.Size = new System.Drawing.Size(36, 13);
            this.lblID3v2.TabIndex = 121;
            this.lblID3v2.Text = "ID3v2";
            // 
            // cmbGenre
            // 
            this.cmbGenre.FormattingEnabled = true;
            this.cmbGenre.Location = new System.Drawing.Point(94, 140);
            this.cmbGenre.Name = "cmbGenre";
            this.cmbGenre.Size = new System.Drawing.Size(146, 21);
            this.cmbGenre.TabIndex = 111;
            // 
            // imageOpenFileDialog
            // 
            this.imageOpenFileDialog.Filter = "*.jpg, *.jpeg, *.png|*.jpg;*.jpeg;*.png";
            // 
            // imageContextMenu
            // 
            this.imageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoadImage,
            this.miSaveImage});
            this.imageContextMenu.Name = "contextMenuStrip1";
            this.imageContextMenu.Size = new System.Drawing.Size(99, 48);
            this.imageContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.imageContextMenu_Opening);
            // 
            // miLoadImage
            // 
            this.miLoadImage.Image = global::IdSharpHarness.Properties.Resources.open_image16_h;
            this.miLoadImage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.miLoadImage.Name = "miLoadImage";
            this.miLoadImage.Size = new System.Drawing.Size(98, 22);
            this.miLoadImage.Text = "Load";
            this.miLoadImage.Click += new System.EventHandler(this.miLoadImage_Click);
            // 
            // miSaveImage
            // 
            this.miSaveImage.Image = global::IdSharpHarness.Properties.Resources.save16_h;
            this.miSaveImage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.miSaveImage.Name = "miSaveImage";
            this.miSaveImage.Size = new System.Drawing.Size(98, 22);
            this.miSaveImage.Text = "Save";
            this.miSaveImage.Click += new System.EventHandler(this.miSaveImage_Click);
            // 
            // ID3v2UserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.imageBindingNavigator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtImageDescription);
            this.Controls.Add(this.txtArtist);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtAlbum);
            this.Controls.Add(this.cmbImageType);
            this.Controls.Add(this.txtYear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTrackNumber);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbID3v2);
            this.Controls.Add(this.cmbGenre);
            this.Controls.Add(this.lblID3v2);
            this.Name = "ID3v2UserControl";
            this.Size = new System.Drawing.Size(704, 333);
            ((System.ComponentModel.ISupportInitialize)(this.imageBindingNavigator)).EndInit();
            this.imageBindingNavigator.ResumeLayout(false);
            this.imageBindingNavigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.imageContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingNavigator imageBindingNavigator;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtArtist;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtImageDescription;
        private System.Windows.Forms.TextBox txtAlbum;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.ComboBox cmbImageType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTrackNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbID3v2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblID3v2;
        private System.Windows.Forms.ComboBox cmbGenre;
        private System.Windows.Forms.OpenFileDialog imageOpenFileDialog;
        private System.Windows.Forms.SaveFileDialog imageSaveFileDialog;
        private System.Windows.Forms.ContextMenuStrip imageContextMenu;
        private System.Windows.Forms.ToolStripMenuItem miLoadImage;
        private System.Windows.Forms.ToolStripMenuItem miSaveImage;
    }
}
