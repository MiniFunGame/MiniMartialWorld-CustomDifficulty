# MiniMartialWorld - 自定义难度模组示例（含完整代码与注释）

> 本文档演示 **如何以 Workshop 形式添加自定义难度**。  
> 重点：**通过实现 `IWorkshopDifficultyModifier` 的 `ChallengeDifficultyModifier`** 将自定义难度注入到指定挑战里（例如一掷千金挑战）。  
> 同时提供 6 个难度（**真龙之血、抱朴守中、钞能废柴、武痴、无极生衍、夙世因缘**）的**完整 C# 代码**与**逐行注释**。

---

## 速览：数值字段语义

- `Level`：显示用的难度档位（示例约定：`0=普通`，`1=困难`，`2=地狱`）。  
- `DifficultyModifier`：额外难度系数。  
- `RewardModifier`：**奖励修正的增量值**，**最终奖励倍数 = 1 + RewardModifier**。  
  - 例：`RewardModifier = 0.5f` ⇒ **1 + 0.5 = 1.5 倍（150%）**  
  - 例：`RewardModifier = 1.0f` ⇒ **1 + 1.0 = 2 倍（200%）**  
  - 例：`RewardModifier = 3.0f` ⇒ **1 + 3.0 = 4 倍（400%）**

---

## 一、核心入口：ChallengeDifficultyModifier（必须）

> 在此类里**根据 challengeCode** 决定**哪些自定义难度**需要加入到当前挑战的难度列表里。  
> 下例：若当前挑战为 **一掷千金（Golden Tower）**，则添加 **夙世因缘** 难度。

```csharp
using System.Collections.Generic;

// 入口类：被游戏反射到后调用，用来“把你的自定义难度塞进目标挑战的难度列表”
public class ChallengeDifficultyModifier : IWorkshopDifficultyModifier
{
    // difficultySettings: 当前挑战已有的难度列表
    // challengeCode: 正在加载的挑战模式代码（例如一掷千金挑战的 code）
    public List<DifficultySetting> WorkShopModifyDifficultySettings(List<DifficultySetting> difficultySettings, string challengeCode = "")
    {
        // 仅在“一掷千金挑战”场景下添加我们自定义的“夙世因缘”难度
        if (challengeCode == GoldenTowerControl.GetChallengeModeName())
            difficultySettings.Add(SuShiYinYuan());

        return difficultySettings; // 返回最终用于渲染的难度清单
    }

    // 夙世因缘（放在这里是为了演示“入口类+难度定义”能写在一起，实际也可拆分到独立工具类中）
    public static DifficultySetting SuShiYinYuan()
    {
        DifficultySetting difficultySetting = new DifficultySetting
        {
            Title = "夙世因缘​",
            Description = "难度【地狱】" +
                "\n某些武学招式对你而言如呼吸般自然，他人苦练十年不及你一日所悟。然这份天赋背后，似有一段模糊的前尘纠葛深深牵连……" +
                "\n掌握招式【奇门遁甲】【移星换斗】" +
                "\n获得特性【月映万川】，【叫卖】等级+5" +
                "\n大部分敌人获得【天魔化身】",
            Level = 2,                   // 显示：地狱
            DifficultyModifier = 0.2f,   // 额外难度系数
            OnSelected = () =>
            {
                // 选中时：标记“特殊难度”、写入模式代号、标题
                Controller.Save.GetDifficultySetting().SpecialLevel = true;
                Controller.Save.GetDifficultySetting().SpecialModeCode = SuShiYinYuanCode();
                Controller.Save.GetDifficultySetting().Title = "夙世因缘";

                // 玩法：叫卖（口才）+5，并添加两门招式
                Controller.GameData.Player.SkillList.Eloquence.setLevel(Controller.GameData.Player.SkillList.Eloquence.getLevel() + 5);
                SkillBuilder.AddSkill("奇门遁甲");
                SkillBuilder.AddSkill("移星换斗");

                // 给予关键天赋（基于效果）
                Talent keyTalent = new TalentFromEffect(new YueYinWanChuan()) { IsBuff = true };
                TalentManager.AddTalent(keyTalent);

                // 敌人端：大部分敌人获得“天魔化身”效果
                Controller.Save.GetDifficultySetting().EnermyEffect = new List<Effect> { new TianMoHuaShen() };
            },
            // 是否已通关（一般用于在描述里追加“通关解锁某宝物”的提示）
            IsFinished = () => { return CustomPlayerPref.GetInt(SuShiYinYuanCode(), -1) > -1; },
            RewardModifier = 3f          // **最终奖励=1+3.0=4倍（400%）**
        };

        return difficultySetting;
    }

    public static string SuShiYinYuanCode() => "SUSHIYINYUAN";
}
```

---

## 二、其余 5 个难度：完整代码 + 注释

### 1) 真龙之血（地狱）

```csharp
public static DifficultySetting ZhenLongZhiXue()
{
    DifficultySetting difficultySetting = new DifficultySetting
    {
        Title = "真龙之血",
        Description = "难度【地狱】" +
            "\n承袭真龙血脉，任何招式在你手中皆能灵活演化，化繁为简，破敌于无形。" +
            "\n获得【龙蛇九变】【悟真篇】" +
            "\n习得招式【移星换斗】" +
            "\n定力，臂力，悟性，魅力+1" +
            "\n大部分敌人获得【真魔印】",
        Level = 2,                 // 地狱
        DifficultyModifier = 0f,   // 占位：此处未额外放大数值难度
        OnSelected = () =>
        {
            // 标记“特殊难度”并写入模式代号、标题
            Controller.Save.GetDifficultySetting().SpecialLevel = true;
            Controller.Save.GetDifficultySetting().SpecialModeCode = ZhenLongZhiXueCode();
            Controller.Save.GetDifficultySetting().Title = "真龙之血";

            // 四维提升：魅力/定力/悟性/臂力 各 +1
            AttributeManager.ModifyCharm(1, false);
            AttributeManager.ModifyConcentrate(1, false);
            AttributeManager.ModifyComprehension(1, false);
            AttributeManager.ModifyStrength(1, false);

            // 启用“悟真篇”修炼效果（调用其 Apply）+ 习得【移星换斗】
            WuZhenPianTraining wuZhenPianTraining = new WuZhenPianTraining();
            wuZhenPianTraining.Apply();
            SkillBuilder.AddSkill("移星换斗", true); 

            // 给予关键天赋：龙蛇九变（基于效果）
            Talent keyTalent = new TalentFromEffect(new LongSheJiuBian()) { IsBuff = true };
            TalentManager.AddTalent(keyTalent);

            // 敌人端：大部分敌人获得“真魔印”
            Controller.Save.GetDifficultySetting().EnermyEffect = new List<Effect> { new ZhenMoYin() };
        },
        IsFinished = () => { return CustomPlayerPref.GetInt(ZhenLongZhiXueCode(), -1) > -1; },
        RewardModifier = 3f         // **最终奖励=1+3.0=4倍（400%）**
    };

    // 首通提示（未通关时在描述里追加奖励情报）
    if (CustomPlayerPref.GetInt(ZhenLongZhiXueCode(), -1) < 0)
        difficultySetting.Description += "\n通关解锁宝物【雷泽珠】";

    return difficultySetting;
}

public static string ZhenLongZhiXueCode() => "ZHENLONGZHIXUE";
```

---

### 2) 抱朴守中（困难）

```csharp
public static DifficultySetting BaoPuShouZhong()
{
    DifficultySetting difficultySetting = new DifficultySetting
    {
        Title = "抱朴守中",
        Description = "难度【困难】" +
            "\n你深谙蓄势之道，不显锋芒于一时，愈战愈强，终有摧山撼海之威。" +
            "\n获得【韬光蓄势】【先制】"+
            "\n【垂钓】等级+3" +
            "\n大部分敌人获得【天魔化身】" +
            "\n敌人属性会随游戏进程提升",
        Level = 1,                   // 困难
        DifficultyModifier = 0.1f,   // 额外难度系数（示例占位）
        OnSelected = () =>
        {
            Controller.Save.GetDifficultySetting().SpecialLevel = true;
            Controller.Save.GetDifficultySetting().SpecialModeCode = BaoPuShouZhongCode();
            Controller.Save.GetDifficultySetting().Title = "抱朴守中";

            // “先制”类天赋：调用4次 OnAwardSelected() 以叠加其开局收益/先手效果（按你设计）
            for(int i = 0; i < 4; i++)
            {
                FirstStrikeTalent firstStrikeTalent = new FirstStrikeTalent();
                firstStrikeTalent.OnAwardSelected();
            }

            // 关键天赋：韬光蓄势（基于效果）
            Talent keyTalent = new TalentFromEffect(new TaoGuangXuShi()) { IsBuff = true };
            TalentManager.AddTalent(keyTalent);

            // 生活技：垂钓 +3
            Controller.GameData.Player.SkillList.Fish.setLevel(Controller.GameData.Player.SkillList.Fish.getLevel()+3);

            // 敌人端：大部分敌人获得“天魔化身”
            Controller.Save.GetDifficultySetting().EnermyEffect = new List<Effect> { new TianMoHuaShen() };
        },
        IsFinished = () => { return CustomPlayerPref.GetInt(BaoPuShouZhongCode(), -1) > -1; },
        RewardModifier = 1f          // **最终奖励=1+1.0=2倍（200%）**
    };

    if (CustomPlayerPref.GetInt(BaoPuShouZhongCode(), -1) < 0)
        difficultySetting.Description += "\n通关解锁宝物【墨麒麟】";

    return difficultySetting;
}

public static string BaoPuShouZhongCode() => "BAOPUSHOUZHONG";
```

---

### 3) 钞能废柴（普通）

```csharp
public static DifficultySetting ChaoNengFeiChai()
{
    DifficultySetting difficultySetting = new DifficultySetting
    {
        Title = "钞能废柴​",
        Description = "难度【普通】" +
            "\n你的老爹给你备好白银万两拜师，但你发现自己完全是武道废体，养尊处优惯了的你，能应对接下来的挑战吗？" +
            "\n初始铜钱+99999，六维-3" +
            "\n额外初始道具【铁链子】x10" +
            "\n获得【护心镜】",
        Level = 0, // 普通
        OnSelected = () =>
        {
            Controller.Save.GetDifficultySetting().SpecialLevel = true;
            Controller.Save.GetDifficultySetting().SpecialModeCode = ChaoNengFeiChaiCode();
            Controller.Save.GetDifficultySetting().Title = "钞能废柴​";

            // 经济起飞，但六维全面-3（魅/悟/定/体/臂/身）
            AttributeManager.ModifyMoney(99999, false);
            AttributeManager.ModifyCharm(-3, false);
            AttributeManager.ModifyComprehension(-3, false);
            AttributeManager.ModifyConcentrate(-3, false);
            AttributeManager.ModifyPhysique(-3, false);
            AttributeManager.ModifyStrength(-3, false);
            AttributeManager.ModifySpeed(-3, false);

            // 初始道具
            // 注意：下行代码添加的是 “铁莲子”，而描述写的是“铁链子”，请根据你游戏内真实物品键作一致性修正
            ItemManager.AddItem("铁莲子", false, 10);

            // 关键天赋：护心镜（基于效果）
            Talent keyTalent = new TalentFromEffect(new HuXinJing()) { IsBuff = true };
            TalentManager.AddTalent(keyTalent);
        },
        IsFinished = () => { return CustomPlayerPref.GetInt(ChaoNengFeiChaiCode(), -1) > -1; },
        RewardModifier = 0.5f // **最终奖励=1+0.5=1.5倍（150%）**
    };

    if (CustomPlayerPref.GetInt(ChaoNengFeiChaiCode(), -1) < 0)
        difficultySetting.Description += "\n通关解锁宝物【瑶光佩】";

    return difficultySetting;
}

public static string ChaoNengFeiChaiCode() => "CHAONENGFEICHAI";
```

---

### 4) 武痴（困难）

```csharp
public static DifficultySetting WuChi()
{
    DifficultySetting difficultySetting = new DifficultySetting
    {
        Title = "武痴​",
        Description = "难度【困难】" +
            "\n你是个真正的武痴，只有不断的修行和挑战才能让你感到满足，但这次你的敌人好像格外强大..." +
            "\n获得【武道之心】"+
            "\n体力上限+50，每回合可参与事件数-1" +
            "\n大部分敌人获得【天魔化身】",
        Level = 1, // 困难
        OnSelected = () =>
        {
            Controller.Save.GetDifficultySetting().SpecialLevel = true;
            Controller.Save.GetDifficultySetting().SpecialModeCode = WuChiCode();
            Controller.Save.GetDifficultySetting().Title = "武痴";

            // 资源调配：体力上限+50，当前体力+50；可参与事件数-1（更硬核的行动限制）
            Controller.Save.GameData.EnergyLimit += 50;
            Controller.Save.GameData.CurrentEnergy += 50;
            Controller.Save.GameData.EventTimeModifier -= 1;

            // 关键天赋：武道之心（基于效果）
            Talent keyTalent = new TalentFromEffect(new WuDaoZhiXin()) { IsBuff = true };
            TalentManager.AddTalent(keyTalent);

            // 敌人端：天魔化身
            Controller.Save.GetDifficultySetting().EnermyEffect = new List<Effect> { new TianMoHuaShen() };
        },          
        IsFinished = () => { return CustomPlayerPref.GetInt(WuChiCode(), -1) > -1; },
        RewardModifier = 1f // **最终奖励=1+1.0=2倍（200%）**
    };

    if (CustomPlayerPref.GetInt(WuChiCode(), -1) < 0)
        difficultySetting.Description += "\n通关解锁宝物【赤霄翎】";

    return difficultySetting;
}

public static string WuChiCode() => "WUCHI";
```

---

### 5) 无极生衍（地狱）

```csharp
public static DifficultySetting WuJiShengYan()
{
    DifficultySetting difficultySetting = new DifficultySetting
    {
        Title = "无极生衍",
        Description = "难度【地狱】" +
            "\n你是技道合一的修者，从来不拘于一招一式..." +
            "\n获得【知机识变】" +
            "\n大部分敌人获得【魔气侵身】" +
            "\n敌人属性会随游戏进程进一步提升" +
            "\n受宗主看重，初始身份变为：核心弟子",
        Level = 2,                 // 地狱
        DifficultyModifier = 0.2f, // 额外难度系数（示例占位）
        OnSelected = () =>
        {
            Controller.Save.GetDifficultySetting().SpecialLevel = true;
            Controller.Save.GetDifficultySetting().SpecialModeCode = WuJiShengYanCode();
            Controller.Save.GetDifficultySetting().Title = "无极生衍";

            // 属性：悟性+1、定力+1；身份：核心弟子
            AttributeManager.ModifyComprehension(1, false);
            AttributeManager.ModifyConcentrate(1, false);
            Controller.GameData.Player.PersonalData.Identity = "核心弟子";

            // 关键天赋：知机识变（基于效果）
            Talent keyTalent = new TalentFromEffect(new ZhiJiShiBian()) { IsBuff = true };
            TalentManager.AddTalent(keyTalent);

            // 敌人端：魔气侵身
            Controller.Save.GetDifficultySetting().EnermyEffect = new List<Effect> { new MoQiQinShen() };
        },
        IsFinished = () => { return CustomPlayerPref.GetInt(WuJiShengYanCode(), -1) > -1; },
        RewardModifier = 3.5f       // **最终奖励=1+3.5=4.5倍（450%）**
    };

    if (CustomPlayerPref.GetInt(WuJiShengYanCode(), -1) < 0)
        difficultySetting.Description += "\n通关解锁宝物【黄泉盏】";

    return difficultySetting;
}

public static string WuJiShengYanCode() => "WUJISHENGYAN";
```


---

