using Tools;
using UnityEngine;

namespace Scripts.Game {
    public class GameManager : MonoSingle<GameManager> {

        internal Transform monsterPrefab;
        internal Transform charPrefab;
        internal Transform charPlacePrefab;
        internal Transform atkRangeDisplay;
        internal AudioClip clip;
        internal AudioClip loop_clip;
        
        protected override void Initialization() {
            monsterPrefab = Asset.Load<Transform>("Prefab/Game", "MonsterPrefab");
            charPrefab = Asset.Load<Transform>("Prefab/Game", "CharPrefab");
            charPlacePrefab = Asset.Load<Transform>("Prefab/Game", "CharPlacePrefab");
            atkRangeDisplay = Asset.Load<Transform>("Prefab/Game", "AtkRangeDisplay");
            clip = Asset.Load<AudioClip>("Audio/Music/Game", "m_bat_indust_intro");
            loop_clip = Asset.Load<AudioClip>("Audio/Music/Game", "m_bat_indust_loop");
        }

        public void StarGame(string dungeonName) {
            
        }
    }
}