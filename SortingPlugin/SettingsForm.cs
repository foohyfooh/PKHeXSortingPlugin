using System.Windows.Forms;

namespace SortingPlugin
{
  public partial class SettingsForm : Form
  {
    public SettingsForm()
    {
      InitializeComponent();
      propertyGrid.SelectedObject = PluginSettings.Default;
      this.FormClosing += new FormClosingEventHandler(SettingsForm_FormClosing);
    }

    private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e) {
      PluginSettings.Default.Save();
      SortingPlugin.LoadMenuStrip();
    }

  }
}
