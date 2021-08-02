using Tools;

namespace Settings {
    public class GameSettings {
        public const string CHAR_META_PATH = @"Meta/Char";
        
        public const string MONSTER_META_PATH = @"Meta/Monster";
        
        public const string DUNGEON_META_PATH = @"Meta/Dungeon";

        public const string ITEM_META_PATH = @"Meta/Item";

        public const string UI_PREFAB_PATH = @"Prefab/UI";

        public static bool GAME_KEEP_SPEED = true;
        
        public static bool GAME_PERFORMANCE = false;
        
        public static int GAME_PROFILED_SCREEN = 0;
        
        public static bool SOUND_SOUND_EFFECT_ENABLE = true;
        
        public static int SOUND_SOUND_EFFECT_VALUE = 100;
        
        public static bool SOUND_MUSIC_ENABLE = true;
        
        public static int SOUND_MUSIC_VALUE = 100;
        
        public static bool SOUND_VOICE_ENABLE = true;
        
        public static int SOUND_VOICE_VALUE = 100;
    }
}