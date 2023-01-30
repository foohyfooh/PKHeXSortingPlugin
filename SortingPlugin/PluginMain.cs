using PKHeX.Core;

namespace SortingPlugin {
  public class SortingPlugin : IPlugin {
    public string Name => nameof(SortingPlugin);
    public int Priority => 1; // Loading order, lowest is first.
    public ISaveFileProvider SaveFileEditor { get; private set; }
    public IPKMView PKMEditor { get; private set; }

    // Static Copies
    private static object[]? globalArgs;
    private static ISaveFileProvider? saveFileEditor;

    public void Initialize(params object[] args) {
      Console.WriteLine($"Loading {Name}...");
      if (args == null)
        return;
      globalArgs = args;
      SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider)!;
      PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView)!;
      saveFileEditor = SaveFileEditor;
      LoadMenuStrip();
    }

    public void NotifySaveLoaded() {
      Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
      LoadMenuStrip();
    }

    public bool TryLoadFile(string filePath) {
      Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
      return false; // no action taken
    }

    public static void LoadMenuStrip() {
      ToolStrip menu = (ToolStrip)Array.Find(globalArgs, z => z is ToolStrip);
      ToolStripDropDownItem menuTools = menu.Items.Find("Menu_Tools", false)[0] as ToolStripDropDownItem;
      menuTools.DropDownItems.RemoveByKey("SortBoxesBy");
      ToolStripMenuItem sortBoxesItem = new ToolStripMenuItem("Sort Boxes By") {
        Name = "SortBoxesBy",
        Image = Properties.Resources.SortIcon
      };
      menuTools.DropDownItems.Add(sortBoxesItem);
      ToolStripItemCollection sortItems = sortBoxesItem.DropDownItems;

      int gen = saveFileEditor.SAV.Generation;
      GameVersion version = saveFileEditor.SAV.Version;
      bool isLetsGo = version == GameVersion.GP || version == GameVersion.GE;
      if (isLetsGo) {
        sortItems.Add(GetRegionalSortButton("Gen 7 Kanto", Gen7_Kanto.GetSortFunctions()));
      } else {
        bool isBDSP = version == GameVersion.BD || version == GameVersion.SP;
        bool isPLA  = version == GameVersion.PLA;

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
          if (PluginSettings.Default.ShowIndividualPokedéxes) {
            sortItems.Add(GetAreaButtons("Gen 6 Kalos Areas", new ToolStripItem[] {
              GetRegionalSortButton("Central Kalos", Gen6_Kalos.GetCentralDexSortFunctions()),
              GetRegionalSortButton("Costal Kalos", Gen6_Kalos.GetCostalDexSortFunctions()),
              GetRegionalSortButton("Mountain Kalos ", Gen6_Kalos.GetMountainDexSortFunctions())
            }));
          }
          sortItems.Add(GetRegionalSortButton("Gen 6 Kalos", Gen6_Kalos.GetSortFunctions()));
          sortItems.Add(GetRegionalSortButton("Gen 6 Hoenn", Gen6_Hoenn.GetSortFunctions()));
        }

        if (gen >= 7 && !isBDSP && !isPLA) {
          if (PluginSettings.Default.ShowIndividualPokedéxes) {
            sortItems.Add(GetAreaButtons("Gen 7 Alola Sun/Moon Islands", new ToolStripItem[] {
              GetRegionalSortButton("Melemele Island", Gen7_Alola.GetSMMelemeleSortFunctions()),
              GetRegionalSortButton("Akala Island", Gen7_Alola.GetSMAkalaSortFunctions()),
              GetRegionalSortButton("Ula'ula Island", Gen7_Alola.GetSMUlaulaSortFunctions()),
              GetRegionalSortButton("Poni Island", Gen7_Alola.GetSMPoniSortFunctions())
            }));
          }
          sortItems.Add(GetRegionalSortButton("Gen 7 Alola Sun/Moon", Gen7_Alola.GetSMSortFunctions()));
          if (PluginSettings.Default.ShowIndividualPokedéxes) {
            sortItems.Add(GetAreaButtons("Gen 7 Alola Ultra Sun/Ultra Moon Islands", new ToolStripItem[] {
              GetRegionalSortButton("Melemele Island", Gen7_Alola.GetUSUMMelemeleSortFunctions()),
              GetRegionalSortButton("Akala Island", Gen7_Alola.GetUSUMAkalaSortFunctions()),
              GetRegionalSortButton("Ula'ula Island", Gen7_Alola.GetUSUMUlaulaSortFunctions()),
              GetRegionalSortButton("Poni Island", Gen7_Alola.GetUSUMPoniSortFunctions())
            }));
          }
          sortItems.Add(GetRegionalSortButton("Gen 7 Alola Ultra Sun/Ultra Moon", Gen7_Alola.GetUSUMSortFunctions()));
        }

        if (gen >= 8) {
          bool isSwSh = version == GameVersion.SW || version == GameVersion.SH;
          if (!isBDSP && !isPLA) {
            bool isScVi = version == GameVersion.SL || version == GameVersion.VL;
            if (!isScVi) {
              sortItems.Add(GetRegionalSortButton("Gen 7 Kanto", Gen7_Kanto.GetSortFunctions()));
            }
            if (PluginSettings.Default.ShowIndividualPokedéxes) {
              sortItems.Add(GetAreaButtons("Gen 8 Galar Areas", new ToolStripItem[] {
                GetRegionalSortButton("Galar", Gen8_Galar.GetGalarDexSortFunctions()),
                GetRegionalSortButton("Isle of Armor", Gen8_Galar.GetIoADexSortFunctions()),
                GetRegionalSortButton("Crown Tundra", Gen8_Galar.GetCTDexSortFunction())
              }));
            }
            sortItems.Add(GetRegionalSortButton("Gen 8 Galar", Gen8_Galar.GetFullGalarDexSortFunctions()));
          }
          if (!isSwSh) {
            sortItems.Add(GetRegionalSortButton("Gen 8 Sinnoh", Gen8_Sinnoh.GetSortFunctions()));
            if (!isBDSP) {
              if (PluginSettings.Default.ShowIndividualPokedéxes) {
                sortItems.Add(GetAreaButtons("Gen 8 Hisui Areas", new ToolStripItem[] {
                  GetRegionalSortButton("Obsidian Fieldlands", Gen8_Hisui.GetObsidianFieldlandsSortFunctions()),
                  GetRegionalSortButton("Crimson Mirelands", Gen8_Hisui.GetCrimsonMirelandsSortFunctions()),
                  GetRegionalSortButton("Cobalt Coastlands", Gen8_Hisui.GetCobaltCoastlandsSortFunctions()),
                  GetRegionalSortButton("Coronet Highlands", Gen8_Hisui.GetCoronetHighlandsSortFunctions()),
                  GetRegionalSortButton("Alabaster Icelands", Gen8_Hisui.GetAlabasterIcelandsSortFunctions())
                }));
              }
              sortItems.Add(GetRegionalSortButton("Gen 8 Hisui", Gen8_Hisui.GetSortFunctions()));
            }
          }
        }

        if (gen >= 9) {
          sortItems.Add(GetRegionalSortButton("Gen 9 Paldea", Gen9_Paldea.GetSortFunctions()));
        }

        if(gen != 1) {
          ToolStripMenuItem nationalDexSortButton = new ToolStripMenuItem("National Pokédex");
          nationalDexSortButton.Click += (s, e) => SortByFunctions();
          sortItems.Add(nationalDexSortButton);

          if(gen >= 7 && !isBDSP) {
            ToolStripMenuItem nationalDexWithFormSortButton = new ToolStripMenuItem("National Pokédex (Regional Forms With Generation)");
            nationalDexWithFormSortButton.Click += (s, e) => SortByFunctions(Gen_National.GetNationalDexWithRegionalFormsSortFunctions());
            sortItems.Add(nationalDexWithFormSortButton);
          }
        }
      }

      ToolStripMenuItem settingsButton = new ToolStripMenuItem("Settings");
      settingsButton.Click += (s, e) => new SettingsForm().ShowDialog();
      sortItems.Add(settingsButton);
    }

    private static void SortByFunctions(Func<PKM, IComparable>[]? sortFunctions = null) {
      int beginIndex = PluginSettings.Default.SortBeginBox - 1;
      int endIndex = PluginSettings.Default.SortEndBox < 0 ? -1 : PluginSettings.Default.SortEndBox - 1;
      if (sortFunctions != null) {
        IEnumerable<PKM> sortMethod(IEnumerable<PKM> pkms, int start) => pkms.OrderByCustom(sortFunctions);
        saveFileEditor.SAV.SortBoxes(beginIndex, endIndex, sortMethod);
      } else {
        saveFileEditor.SAV.SortBoxes(beginIndex, endIndex);
      }
      saveFileEditor.ReloadSlots();
    }

    private static ToolStripItem GetRegionalSortButton(string dex, Func<PKM, IComparable>[] sortFunctions) {
      ToolStripMenuItem dexSortButton = new ToolStripMenuItem($"{dex} Regional Pokédex");
      dexSortButton.Click += (s, e) => SortByFunctions(sortFunctions);
      return dexSortButton;
    }

    private static ToolStripMenuItem GetAreaButtons(string name, ToolStripItem[] sortButtons) {
      ToolStripMenuItem areas = new ToolStripMenuItem(name);
      areas.DropDownItems.AddRange(sortButtons);
      return areas;
    }

  }
}
