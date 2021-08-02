using System.Collections.Generic;
using Settings;
using Tools;

namespace Data.Dungeon {
    public class DungeonManager : Single<DungeonManager> {
        // 元数据
        private Dictionary<string, DungeonMeta> metaDictionary = new Dictionary<string, DungeonMeta>();
        
        // 获取元数据
        public DungeonMeta GetMeta(string id) => metaDictionary.ContainsKey(id) ? metaDictionary[id] : LoadMeta(id);
        
        // 加载LoadMeta数据
        private DungeonMeta LoadMeta(string id) {
            DungeonMeta meta = Asset.Load<DungeonMeta>(GameSettings.DUNGEON_META_PATH, id);
            meta.SetId(id);
            metaDictionary.Add(id, meta);
            return meta;
        }
    }
}