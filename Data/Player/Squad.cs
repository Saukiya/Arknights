using System;
using Data.Char;
using UnityEngine;

namespace Data.Player {
    [Serializable]
    public class Squad {
        [SerializeField]
        private string name;
        [SerializeField]
        private string[] chars;

        public Squad() {
            name = "";
            chars = new string[12];
        }

        public string GetName() => name;

        public string[] GetChars() => chars;

        public void SetName(string name) => this.name = name;

        public void SetChars(string[] chars) => this.chars = chars;
    }
}