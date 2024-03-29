using PKHeX.Core;
using System;
using System.Collections.Generic;

namespace SortingPlugin {
  class SortingBase {
    
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

    protected static Func<PKM, IComparable>[] GenerateSortingFunctions(Dictionary<Species, int> dex) {
      return GenerateSortingFunctions(new Dictionary<Species, int>[] { dex });
    }

  }
}
