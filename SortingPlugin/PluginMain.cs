using PKHeX.Core;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SortingPlugin {
  public class SortingPlugin : IPlugin {
    public string Name => nameof(SortingPlugin);
    public int Priority => 1; // Loading order, lowest is first.
    public ISaveFileProvider SaveFileEditor { get; private set; }
    public IPKMView PKMEditor { get; private set; }

    public void Initialize(params object[] args) {
      Console.WriteLine($"Loading {Name}...");
      if (args == null)
        return;
      SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider);
      PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView);
      var menu = (ToolStrip)Array.Find(args, z => z is ToolStrip);
      LoadMenuStrip(menu);
    }

    public void NotifySaveLoaded() {
      Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
    }

    public bool TryLoadFile(string filePath) {
      Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
      return false; // no action taken
    }

    private void LoadMenuStrip(ToolStrip menuStrip) {
      var items = menuStrip.Items;
      var tools = items.Find("Menu_Tools", false)[0] as ToolStripDropDownItem;

      var ctrl = new ToolStripMenuItem("Sort Boxes By");
      tools.DropDownItems.Add(ctrl);

      ctrl.DropDownItems.Add(GetSortButton("Gen 1 Kanto", Gen1_Kanto.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 2 Johto", Gen2_Johto.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 3 Hoenn", Gen3_Hoenn.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 3 Kanto", Gen3_Kanto.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 4 Sinnoh Diamond/Pearl", Gen4_Sinnoh.GetDPSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 4 Sinnoh Platinum",  Gen4_Sinnoh.GetPtSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 4 Johto", Gen4_Johto.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 5 Unova Black/White", Gen5_Unova.GetBWSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 5 Unova Black 2/White 2", Gen5_Unova.GetB2W2SortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 6 Kalos", Gen6_Kalos.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 6 Hoenn", Gen6_Hoenn.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 7 Alola Sun/Moon", Gen7_Alola.GetFullSMSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 7 Alola Ultra Sun/Ultra Moon", Gen7_Alola.GetFullUSUMSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 7 Kanto", Gen7_Kanto.GetSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 8 Galar", Gen8_Galar.GetGalarDexSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 8 Galar Isle of Armor", Gen8_Galar.GetIoADexSortFunctions()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 8 Galar Crown Tundra", Gen8_Galar.GetCTDexSortFunction()));
      ctrl.DropDownItems.Add(GetSortButton("Gen 8 Galar Complete", Gen8_Galar.GetFullGalarDexSortFunctions()));

      var sortButton = new ToolStripMenuItem("National Pokédex");
      sortButton.Click += (s, e) => SortByNationalDex();
      ctrl.DropDownItems.Add(sortButton);
    }

    private ToolStripDropDownItem GetSortButton(string dex, Func<PKM, IComparable>[] sortFunctions) {
      var dexSortButton = new ToolStripMenuItem($"{dex} Regional Pokédex");
      dexSortButton.Click += (s, e) => SortByRegionalDex(sortFunctions);
      return dexSortButton;
    }

    private void SortByRegionalDex(Func<PKM, IComparable>[] sortFunctions) {
      var save = SaveFileEditor.SAV;
      var pokemon = save.BoxData;
      var sorted = pokemon.OrderByCustom(sortFunctions);
      save.BoxData = sorted.ToList();
      SaveFileEditor.ReloadSlots();
    }

    private void SortByNationalDex() {
      var save = SaveFileEditor.SAV;
      var pokemon = save.BoxData;
      var sorted = pokemon.OrderBySpecies();
      save.BoxData = sorted.ToList();
      SaveFileEditor.ReloadSlots();
    }

  }
}
