using System.Collections.Generic;
using System.Linq;

namespace ManagedData {
    public class ThemeBuilder {
        private Dictionary<(string, string), int> themeTableDictionary;

        public ThemeBuilder(List<(string, string)> themeUnitList) {
            themeTableDictionary = new Dictionary<(string, string), int>();
            foreach (var themeUnit in themeUnitList) {
                themeTableDictionary.Add(themeUnit, 0);
            }
        }

        public (string, string) BuildTheme() {
            var maxNum = themeTableDictionary.Values.Max() + 1;
            var sum = themeTableDictionary.Values.Sum(x => maxNum - x);
            var count = 0;
            var pin = UnityEngine.Random.Range(0, sum);
            foreach (var pair in themeTableDictionary) {
                var v = maxNum - pair.Value;
                if (count <= pin && pin < count + v) {
                    themeTableDictionary[pair.Key]++;
                    return ShuffleTheme(pair.Key);
                }

                count += v;
            }

            var minNum = themeTableDictionary.Values.Min();
            var minPair = themeTableDictionary.First(p => p.Value == minNum).Key;
            themeTableDictionary[minPair]++;
            return ShuffleTheme(minPair);
        }

        private (string, string) ShuffleTheme((string, string) pair) {
            return UnityEngine.Random.Range(0, 2) == 0 ? (pair.Item1, pair.Item2) : (pair.Item2, pair.Item1);
        }
    }
}