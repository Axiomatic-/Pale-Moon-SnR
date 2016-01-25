using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Pale_Moon_SnR {
  public partial class Form1 : Form {
    public Form1() {
      InitializeComponent();
    }

    public class GlobalVariables {
      /* Pale Moon Path */
      public static string pathToProfiles = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Moonchild Productions\Pale Moon\Profiles\"; 
      /* Firefox Path
       * public static string pathToProfiles = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles\";
       */
      public static string[] directories = Directory.GetDirectories(pathToProfiles);
      public static string targetPath;
      public static string profileName;
    }

    private void Form1_Load(object sender, EventArgs e) {
      /* Populate profiles on program load. */
      foreach (string folder in GlobalVariables.directories)
        listBox1.Items.Add(Path.GetFileName(folder));
    }

    private void button2_Click(object sender, EventArgs e) {
      listBox1.Items.Clear(); /* Clear then repopulate. */
      foreach (string folder in GlobalVariables.directories)
        listBox1.Items.Add(Path.GetFileName(folder));
    }

    private void button3_Click(object sender, EventArgs e) {
      FolderBrowserDialog pathSelector = new FolderBrowserDialog();
      pathSelector.Description = "Select a folder to where we will backup your profile:\n(Make sure to save inside a folder, we only copy the inner contents)";
      pathSelector.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      if (DialogResult.OK == pathSelector.ShowDialog()) {
        label3.Text = pathSelector.SelectedPath;
        GlobalVariables.targetPath = Convert.ToString(pathSelector.SelectedPath); /* Avoid using label3.Text, UI elements are not for that. */
      }
    }

    /* Note: https://msdn.microsoft.com/en-us/library/bb762914%28v=vs.110%29.aspx 
     * Using DirectoryCopy from there would be nice, but annoyingly there is a license issue.
     * So use a different implementation: http://stackoverflow.com/a/2527714
     * Current short commings:
     * Does not validate target path.
     */
    public class Folders {
      public string Source { get; private set; }
      public string Target { get; private set; }

      public Folders(string source, string target) {
        Source = source;
        Target = target;
      }
    }

    public static void DirectoryCopy(string source, string target) {
      var stack = new Stack<Folders>();
      stack.Push(new Folders(source, target));

      while (stack.Count > 0) {
        var folders = stack.Pop();
        Directory.CreateDirectory(folders.Target);
        foreach (var file in Directory.GetFiles(folders.Source, "*.*")) {
          File.Copy(file, Path.Combine(folders.Target, Path.GetFileName(file)));
        }

        foreach (var folder in Directory.GetDirectories(folders.Source)) {
          stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
        }
      }
    }
    
    private void button1_Click(object sender, EventArgs e) {
      /* Check for any selected items. */
      if (listBox1.SelectedIndex == -1)
        MessageBox.Show("If you do not select a profile, what am I to back up?"
                      , "Error."
                      , MessageBoxButtons.OK
                      , MessageBoxIcon.Error);
      else if (label3.Text == "None") /* Why is comparing UI elements a bad thing? */
        MessageBox.Show("Missing backup path, please set it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      else
          DirectoryCopy(String.Concat(GlobalVariables.pathToProfiles, GlobalVariables.profileName), GlobalVariables.targetPath);
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
      GlobalVariables.profileName = Convert.ToString(listBox1.SelectedItem);
    }

    private void button4_Click(object sender, EventArgs e)
    {
        Form2 optionsWindow = new Form2();
        optionsWindow.Show();
    }
  }
}
