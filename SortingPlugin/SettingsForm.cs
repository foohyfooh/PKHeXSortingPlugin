using System.Windows.Forms;

namespace SortingPlugin
{
  public partial class SettingsForm : Form
  {
    public SettingsForm()
    {
      InitializeComponent();
      propertyGrid.SelectedObject = PluginSettings.Default;
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
      FormClosing += new FormClosingEventHandler(SettingsForm_FormClosing);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    }

    private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e) {
      PluginSettings.Default.Save();
      SortingPlugin.LoadMenuStrip();
    }

  }
}
