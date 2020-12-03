using System;
using UnityEngine;

namespace ConnectData {
    [Serializable]
    public class ThemeData {
        public ThemeUnit[] Theme;
    }

    [Serializable]
    public class ThemeUnit {
        public string Theme1;
        public string Theme2;
    }
}