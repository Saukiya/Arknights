using System.Collections.Generic;
using Settings;
using Tools;

namespace Data.Monster {
    public class MonsterManager : Single<MonsterManager> {
        // 元数据
        private Dictionary<string, MonsterData> metaDictionary = new Dictionary<string, MonsterData>();
        
        public MonsterData GetMeta(string id) => metaDictionary.ContainsKey(id) ? metaDictionary[id] : LoadMeta(id);

        // 加载LoadMeta数据
        private MonsterData LoadMeta(string id) {
            MonsterData data = Asset.Load<MonsterData>(GameSettings.MONSTER_META_PATH, id);
            data.SetId(id);
            metaDictionary.Add(id, data);
            return data;
        }
    }
}