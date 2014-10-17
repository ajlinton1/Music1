namespace id3 {

  using System;
  using System.IO;
  using System.Windows.Forms;
  using System.Drawing;

  public class ID3TagEditor : System.Windows.Forms.Form {
    // Menu Components
    private MainMenu mForm;
    private MenuItem mtFile;
    private MenuItem miLoad;
    private MenuItem miSave;
    private MenuItem miExit;
    private MenuItem mtHelp;
    private MenuItem miAbout;
    private MenuItem miSeparator;
  
    // Label Components
    private Label lblTitle;
    private Label lblArtist;
    private Label lblAlbum;
    private Label lblYear;
    private Label lblComment;
  
    // Text Box Components
    private TextBox tbTitle;
    private TextBox tbArtist;
    private TextBox tbAlbum;
    private TextBox tbYear;
    private TextBox tbComment;
  
    // MP3 Struct
    MP3 workingMP3;
  
    public ID3TagEditor() {
      // Create the components
      CreateComponents();
      // Setup the Form    
      this.Text = "MP3 Tag Editor (C# Example)";    
      this.MinimizeBox = true;
      this.MaximizeBox = false;
      this.Menu = mForm;
      this.Size = new Size(340,180);    
    }
  
  
    // method to create a Label
    private Label createLabel(string pText, int pRow) {
      Label rLabel = new Label();  
      rLabel.Text = pText;
      rLabel.Location = new Point(5,8 + (pRow * 25));
      rLabel.Size = new System.Drawing.Size (70, 20);
      rLabel.Font = new System.Drawing.Font ("Arial", 10, System.Drawing.FontStyle.Bold);
      rLabel.BackColor = System.Drawing.SystemColors.Control;
      rLabel.TextAlign = ContentAlignment.MiddleRight;
      
      return (rLabel);
    }

    // method to create a Text Box
    private TextBox createTextBox(string pText, int pRow, int pLength) {
      TextBox rTextBox = new TextBox ();
      rTextBox.Location = new Point(75,5 + (pRow * 25));
      rTextBox.ReadOnly = false;
      rTextBox.Text = pText;
      rTextBox.MaxLength = pLength;
      rTextBox.Font = new System.Drawing.Font ("Arial", 10, System.Drawing.FontStyle.Bold);
      rTextBox.Size = new System.Drawing.Size (250, 18);
      rTextBox.BackColor = System.Drawing.SystemColors.Window;
      rTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
      return (rTextBox);
    }
  
    public void CreateComponents()
    {
      // Create Labels
      lblTitle        = createLabel("Title", 0);
      lblArtist       = createLabel("Artist", 1);
      lblAlbum        = createLabel("Album", 2);
      lblYear         = createLabel("Year", 3);
      lblComment      = createLabel("Comment", 4);
  
      // Create TextBoxes
      tbTitle         = createTextBox ("", 0, 30);
      tbArtist        = createTextBox ("", 1, 30);
      tbAlbum         = createTextBox ("", 2, 30);
      tbYear          = createTextBox ("", 3, 4);
      tbComment       = createTextBox ("", 4, 28);
  
      // Add Labels
      this.Controls.Add(lblTitle);
      this.Controls.Add(lblArtist);
      this.Controls.Add(lblAlbum);
      this.Controls.Add(lblYear);
      this.Controls.Add(lblComment);
  
      // Add Text Boxes
      this.Controls.Add(tbTitle);
      this.Controls.Add(tbArtist);
      this.Controls.Add(tbAlbum);
      this.Controls.Add(tbYear);
      this.Controls.Add(tbComment);
  
      //Instantiating Main Menu
      mForm = new MainMenu();
  
      // Add top level menu items
      mtFile      = new MenuItem("&File");
      mtHelp      = new MenuItem("&Help");
      miSeparator = new MenuItem("-");
      mForm.MenuItems.Add(mtFile);
      mForm.MenuItems.Add(mtHelp);
  
      // Add the Load MP3 menu item
      miLoad  = new MenuItem("&Load MP3", new EventHandler(eventLoadMP3), Shortcut.CtrlO);
      // Add the Save MP3 menu item
      miSave  = new MenuItem("&Save MP3", new EventHandler(eventSaveMP3), Shortcut.CtrlS);
      // Add the exit menu
      miExit  = new MenuItem("&Exit", new EventHandler(eventCloseForm));
      // Add the about menu
      miAbout = new MenuItem("&About", new EventHandler(eventAboutBox));
  
      mtFile.MenuItems.Add(miLoad);
      mtFile.MenuItems.Add(miSave);
      mtFile.MenuItems.Add(miSeparator);
      mtFile.MenuItems.Add(miExit);
      mtHelp.MenuItems.Add(miAbout);
  
    }
  
  
    // Event for Loading an MP3
    protected void eventLoadMP3(object pSender, EventArgs pArgs) {
      OpenFileDialog fileDialog = new OpenFileDialog();
      fileDialog.Filter = "MP3 files files (*.mp3)|*.mp3"; 
      fileDialog.ShowDialog();
      
      string fileName = fileDialog.FileName;
  
      // If a file was selected get its ID3 Tag
      if (fileName.Length > 0) {
        FileInfo fFileInfo = new FileInfo(fileName); // Creating this FileInfo so I don't have to change my generic class
        workingMP3  = new MP3(fFileInfo.DirectoryName, fFileInfo.Name); //fFile.DirectoryName, fFile.Name);
        FileCommands.readMP3Tag (ref workingMP3);
        
        tbTitle.Text    = workingMP3.id3Title;
        tbArtist.Text   = workingMP3.id3Artist;
        tbAlbum.Text    = workingMP3.id3Album;
        tbYear.Text     = workingMP3.id3Year;
        tbComment.Text  = workingMP3.id3Comment;
      }
    }
  
    // Event for Saving an MP3
    protected void eventSaveMP3(object pSender, EventArgs pArgs) {
      if (workingMP3.id3Title == null) return;
        workingMP3.id3Title   = tbTitle.Text;
        workingMP3.id3Artist  = tbArtist.Text;
        workingMP3.id3Album   = tbAlbum.Text;
        workingMP3.id3Year    = tbYear.Text;
        workingMP3.id3Comment = tbComment.Text;
  
        FileCommands.updateMP3Tag (ref workingMP3);
  
    }
  
    // Event for About Box
    protected void eventAboutBox(object pSender, EventArgs pArgs) {
      MessageBox.Show("By Paul Lockwood (paul_lockwood@yahoo.com).", "C# Example Code", 
      MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
  
    // Event for Closing Form
    protected void eventCloseForm(object pSender, EventArgs pArgs) {
      Application.Exit();
    }
    
    // The Main method
    public static void Main() {
      Application.Run(new ID3TagEditor());
    }
  }
}

