using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SortingPlugin {
  public class SortingPlugin : IPlugin {
    public string Name => nameof(SortingPlugin);
    public int Priority => 1; // Loading order, lowest is first.
    public ISaveFileProvider SaveFileEditor { get; private set; }
    public IPKMView PKMEditor { get; private set; }
    private object[] globalArgs;

    public void Initialize(params object[] args) {
      Console.WriteLine($"Loading {Name}...");
      if (args == null)
        return;
      globalArgs = args;
      SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider);
      PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView);
      LoadMenuStrip(GetMenuFromArgs(args));
    }

    public void NotifySaveLoaded() {
      Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
      LoadMenuStrip(GetMenuFromArgs(globalArgs));
    }

    public bool TryLoadFile(string filePath) {
      Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
      return false; // no action taken
    }

    private ToolStripDropDownItem GetMenuFromArgs(params object[] args) {
      ToolStrip menu = (ToolStrip)Array.Find(args, z => z is ToolStrip);
      ToolStripDropDownItem menuTools = menu.Items.Find("Menu_Tools", false)[0] as ToolStripDropDownItem;
      return menuTools;
    }

    private void LoadMenuStrip(ToolStripDropDownItem menuTools) {
      menuTools.DropDownItems.RemoveByKey("SortBoxesBy");
      ToolStripMenuItem sortBoxesItem = new ToolStripMenuItem("Sort Boxes By") {
        Name = "SortBoxesBy"
      };
      sortBoxesItem.Image = Properties.Resources.SortIcon;
      menuTools.DropDownItems.Add(sortBoxesItem);
      ToolStripItemCollection sortItems = sortBoxesItem.DropDownItems;

      int gen = SaveFileEditor.SAV.Generation;
      GameVersion version = SaveFileEditor.SAV.Version;
      bool isLetsGo = version == GameVersion.GP || version == GameVersion.GE;
      if (isLetsGo) {
        sortItems.Add(GetRegionalSortButton("Gen 7 Kanto", Gen7_Kanto.GetSortFunctions()));
      } else {
        bool isSwSh = version == GameVersion.SW || version == GameVersion.SH;
        bool isBDSP = version == GameVersion.BD || version == GameVersion.SP;
        bool isPLA = version == GameVersion.PLA;

        if (gen >= 1) {
          sortItems.Add(GetRegionalSortButton("Gen 1 Kanto", Gen1_Kanto.GetSortFunctions()));
        }

        if (gen >= 2) {
          sortItems.Add(GetRegionalSortButton("Gen 2 Johto", Gen2_Johto.GetSortFunctions()));
        }

        if (gen >= 3) {
          sortItems.Add(GetRegionalSortButton("Gen 3 Hoenn", Gen3_Hoenn.GetSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 3 Kanto", Gen3_Kanto.GetSortFunctions()));
        }

        if (gen >= 4) {
          sortItems.Add(GetRegionalSortButton("Gen 4 Sinnoh Diamond/Pearl", Gen4_Sinnoh.GetDPSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 4 Sinnoh Platinum", Gen4_Sinnoh.GetPtSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 4 Johto", Gen4_Johto.GetSortFunctions()));
        }

        if (gen >= 5 && !isBDSP) {
          sortItems.Add(GetRegionalSortButton("Gen 5 Unova Black/White", Gen5_Unova.GetBWSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 5 Unova Black 2/White 2", Gen5_Unova.GetB2W2SortFunctions()));
        }

        if (gen >= 6 && !isBDSP) {
          sortItems.Add(GetRegionalSortButton("Gen 6 Kalos Central", Gen6_Kalos.GetCentralDexSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 6 Kalos Costal", Gen6_Kalos.GetCostalDexSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 6 Kalos Mountain", Gen6_Kalos.GetMountainDexSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 6 Kalos", Gen6_Kalos.GetSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 6 Hoenn", Gen6_Hoenn.GetSortFunctions()));
        }

        if (gen >= 7 && !isBDSP && !isPLA) {
          sortItems.Add(GetRegionalSortButton("Gen 7 Alola Sun/Moon", Gen7_Alola.GetFullSMSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 7 Alola Ultra Sun/Ultra Moon", Gen7_Alola.GetFullUSUMSortFunctions()));
        }

        if (gen >= 8) {
          if (isSwSh) {
            sortItems.Add(GetRegionalSortButton("Gen 7 Kanto", Gen7_Kanto.GetSortFunctions()));
            sortItems.Add(GetRegionalSortButton("Gen 8 Galar", Gen8_Galar.GetGalarDexSortFunctions()));
            sortItems.Add(GetRegionalSortButton("Gen 8 Galar Isle of Armor", Gen8_Galar.GetIoADexSortFunctions()));
            sortItems.Add(GetRegionalSortButton("Gen 8 Galar Crown Tundra", Gen8_Galar.GetCTDexSortFunction()));
            sortItems.Add(GetRegionalSortButton("Gen 8 Galar Complete", Gen8_Galar.GetFullGalarDexSortFunctions()));
          } else if (isBDSP) {
            sortItems.Add(GetRegionalSortButton("Gen 8 Sinnoh", Gen8_Sinnoh.GetSortFunctions()));
          } else if (isPLA) {
            sortItems.Add(GetRegionalSortButton("Gen 8 Sinnoh", Gen8_Sinnoh.GetSortFunctions()));
            sortItems.Add(GetRegionalSortButton("Gen 8 Hisui", Gen8_Hisui.GetSortFunctions()));
          }
        }

        if(gen != 1) {
          ToolStripMenuItem nationalDexSortButton = new ToolStripMenuItem("National Pokédex");
          nationalDexSortButton.Click += (s, e) => SortByNationalDex();
          sortItems.Add(nationalDexSortButton);

          if(gen >= 7 && !isBDSP) {
            ToolStripMenuItem nationalDexWithFormSortButton = new ToolStripMenuItem("National Pokédex (Regional Forms With Generation)");
            nationalDexWithFormSortButton.Click += (s, e) => SortByFunctions(Gen_National.GetNationalDexWithRegionalFormsSortFunctions());
            sortItems.Add(nationalDexWithFormSortButton);
          }
        }
      }
    }

    private void SortByFunctions(Func<PKM, IComparable>[] sortFunctions) {
      IEnumerable<PKM> sortMethod(IEnumerable<PKM> pkms, int start) => pkms.OrderByCustom(sortFunctions);
      SaveFileEditor.SAV.SortBoxes(0, -1, sortMethod);
      SaveFileEditor.ReloadSlots();
    }

    private ToolStripDropDownItem GetRegionalSortButton(string dex, Func<PKM, IComparable>[] sortFunctions) {
      ToolStripMenuItem dexSortButton = new ToolStripMenuItem($"{dex} Regional Pokédex");
      dexSortButton.Click += (s, e) => SortByFunctions(sortFunctions);
      return dexSortButton;
    }

    private void SortByNationalDex() {
      SaveFileEditor.SAV.SortBoxes();
      SaveFileEditor.ReloadSlots();
    }

  }
}
