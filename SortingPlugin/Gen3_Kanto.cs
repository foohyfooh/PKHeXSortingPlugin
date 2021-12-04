using PKHeX.Core;
using System;

namespace SortingPlugin {
  class Gen3_Kanto : SortingBase {

    public static Func<PKM, IComparable>[] GetSortFunctions() {
      return Gen1_Kanto.GetSortFunctions();
    }

  }
}
