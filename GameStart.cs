using Data.Player;
using Manager;
using Scripts.Game;
using UI;
using UnityEngine;

namespace Scripts {
    public class GameStart : MonoBehaviour {
        private void Awake() {
            GameManager.Inst();
            LuaEnvManager.Inst();
            UIManager.Inst().Show("LoginUI");
            Destroy(gameObject);
        }
    }
}