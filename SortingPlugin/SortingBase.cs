using PKHeX.Core;
using System;
using System.Collections.Generic;

namespace SortingPlugin {
  class SortingBase {

    protected static bool SpeciesExistsInDex(Dictionary<Species, int> dex, PKM pkm) {
      return !dex.ContainsKey((Species)pkm.Species);
    }

    protected static int GetDexNumberOrSpecies(Dictionary<Species, int> dex, PKM pkm) {
      return dex.ContainsKey((Species)pkm.Species) ? dex[(Species)pkm.Species] : pkm.Species;
    }

    protected static Func<PKM, IComparable>[] GenerateSortingFunctions(Dictionary<Species, int> dex) {
      Func<PKM, IComparable>[] sortFunctions = new Func<PKM, IComparable>[] {
        (PKM p) => SpeciesExistsInDex(dex, p),
        (PKM p) => GetDexNumberOrSpecies(dex, p),
      };
      return sortFunctions;
    }

    private static int SortBetweenDexes(Dictionary<Species, int>[] dexes, PKM pkm) {
      for (int i = 0; i < dexes.Length; i++) {
        if (dexes[i].ContainsKey((Species)pkm.Species))
          return i;
      }
      return dexes.Length;
    }

    private static int SortWithinDex(Dictionary<Species, int>[] dexes, PKM pkm) {
      for (int i = 0; i < dexes.Length; i++) {
        if (dexes[i].ContainsKey((Species)pkm.Species))
          return dexes[i][(Species) pkm.Species];
      }
      return pkm.Species;
    }

    protected static Func<PKM, IComparable>[] GenerateSortingFunctions(Dictionary<Species, int>[] dexes) {
      Func<PKM, IComparable>[] sortFunctions = new Func<PKM, IComparable>[] {
        (PKM p) => SortBetweenDexes(dexes, p),
        (PKM p) => SortWithinDex(dexes, p),
      };
      return sortFunctions;
    }

  }
}
