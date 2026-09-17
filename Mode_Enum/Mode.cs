using System.ComponentModel.DataAnnotations;

namespace LyricMotion.Mode_Enum
{
    public enum DisplayMode
    {
        [Display(Name = "ランダム", Description = "ランダムの方向からアニメーション")]
        Random = 1,

        [Display(Name = "交互", Description = "交互の方向からアニメーション")]
        Mutual = 2,
    }

    public enum EasingSetting
    {
        [Display(Name = "簡易イージング", Description = "プリセットの種類・加減速でイージングを指定")]
        Simple = 1,

        [Display(Name = "アニメーション", Description = "アニメーションスライダー（移動方法）でイージングを指定")]
        Animation = 2,
    }
}
