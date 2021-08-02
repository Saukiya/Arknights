using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Data.Player {
    public class PlayerManager : Single<PlayerManager> {
        
        private List<PlayerData> list = new List<PlayerData>();
        
        private PlayerData playerData;

        public PlayerManager() {
            list.Add(Asset.Load<PlayerData>("Data/User", "Saukiya"));
            list.Add(Asset.Load<PlayerData>("Data/User", "Test"));
        }

        public PlayerData Get() {
            return playerData;
        }

        public PlayerData Login(string name,string password) {
            playerData = null;
            foreach (PlayerData data in list) {
                if (data.GetName() == name && data.GetPassword() == password) {
                    playerData = data;
                    data.ItemSort();
                    break;
                }
            }
            return playerData;
        }

        public void Register(string name,string password) {
            if (list.Find(data => data.name == name) != null) {
                throw new Exception("已存在该用户");
            }
            PlayerData playerData = ScriptableObject.CreateInstance<PlayerData>();
            playerData.Initialization(name, password);
            list.Add(playerData);
        }

        public void Exit() {
            playerData = null;
        }
    }
}