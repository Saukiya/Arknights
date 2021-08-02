using System.ComponentModel;

namespace Data.Char {
    
    // 角色标签
    public enum CharTag {
        [Description("治疗")]
        ZHI_LIAO,
        [Description("支援")]
        ZHI_YUAN,
        [Description("输出")]
        SHU_CHU,
        [Description("群攻")]
        QUN_GONG,
        [Description("减速")]
        JIAN_SU,
        [Description("生存")]
        SHENG_CUN,
        [Description("防护")]
        FAGN_HU,
        [Description("削弱")]
        XUE_RUO,
        [Description("位移")]
        WEI_YI,
        [Description("控场")]
        KONG_CHANG,
        [Description("爆发")]
        BAO_FA,
        [Description("召唤")]
        ZHAO_HUAN,
        [Description("快速复活")]
        KUAI_SU_FU_HUO,
        [Description("费用回复")]
        FEI_YONG_HUI_FU,
        [Description("新手")]
        XIN_SHOU
    }
    
    // 角色职业
    public enum CharProfession {
        [Description("先锋")]
        XIAN_FENG,
        [Description("近卫")]
        JIN_WEI,
        [Description("狙击")]
        JU_JI,
        [Description("重装")]
        ZHONG_ZHUANG,
        [Description("医疗")]
        YI_LIAO,
        [Description("辅助")]
        FU_ZHU,
        [Description("术师")]
        SHU_SHI,
        [Description("特种")]
        TE_ZHONG
    }

    public enum CharPosition {
        [Description("近战位")]
        JIN_ZHAN,
        [Description("远程位")]
        YUAN_CHENG
    }

    public enum CharCamp {
        NULL,
        [Description("巴别塔")]
        BBT,
        [Description("黑钢")]
        HG,
        [Description("卡西米尔")]
        KXME,
        [Description("罗德岛")]
        LDD,
        [Description("龙门")]
        LM,
        [Description("雷姆必拓")]
        LMBT,
        [Description("拉特兰")]
        LTL,
        [Description("莱塔尼亚")]
        LTNY,
        [Description("莱茵生命")]
        LYSM,
        [Description("企鹅物流")]
        QEWL,
        [Description("深海猎人")]
        SHLR,
        [Description("维多利亚")]
        WDLY,
        [Description("乌萨斯")]
        WSS,
        [Description("谢拉格")]
        XLG,
        [Description("汐斯塔")]
        XST
    }
}