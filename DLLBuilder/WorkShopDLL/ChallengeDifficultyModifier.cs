

// 自定义难度修改器：根据不同的挑战模式进行难度修改
using System.Collections.Generic;

public class ChallengeDifficultyModifier : IWorkshopDifficultyModifier
{
    public List<DifficultySetting> WorkShopModifyDifficultySettings(List<DifficultySetting> difficultySettings, string challengeCode = "")
    {

        if (challengeCode == GoldenTowerControl.GetChallengeModeName())
            difficultySettings.Add(SuShiYinYuan());

        return difficultySettings;
    }

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
            Level = 2,
            DifficultyModifier = 0.2f,
            OnSelected = () =>
            {
                Controller.Save.GetDifficultySetting().SpecialLevel = true;
                Controller.Save.GetDifficultySetting().SpecialModeCode = SuShiYinYuanCode();
                Controller.Save.GetDifficultySetting().Title = "夙世因缘";
                Controller.GameData.Player.SkillList.Eloquence.setLevel(Controller.GameData.Player.SkillList.Eloquence.getLevel() + 5);
                SkillBuilder.AddSkill("奇门遁甲");
                SkillBuilder.AddSkill("移星换斗");


                Talent KeyTalent = new TalentFromEffect(new YueYinWanChuan());
                KeyTalent.IsBuff = true;
                TalentManager.AddTalent(KeyTalent);
                Controller.Save.GetDifficultySetting().EnermyEffect = new List<Effect> { new TianMoHuaShen() };
            },
            IsFinished = () => { return CustomPlayerPref.GetInt(SuShiYinYuanCode(), -1) > -1; },
            RewardModifier = 3.5f // 奖励修正：100%

        };

        return difficultySetting;
    }

    public static string SuShiYinYuanCode()
    {
        return "SUSHIYINYUAN";
    }
 
}

