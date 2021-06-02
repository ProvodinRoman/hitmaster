using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HM.Extentions {
    public static class EnumerableExtentions {
        public static int GetSafeClampedIndex(this ICollection collection, int rawIndex) {
            while (rawIndex < 0) {
                rawIndex += collection.Count;
            }

            rawIndex %= collection.Count;
            return rawIndex;
        }
    }
}