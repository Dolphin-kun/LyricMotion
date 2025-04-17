using LyricMotion.Mode_Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.ItemEditor.CustomVisibilityAttributes;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace LyricMotion
{
    [VideoEffect("リリックモーションしながら登場退場", ["登場退場"], ["リリックモーション", "Lyric Motion", "ririkku"],IsAviUtlSupported =false)]
    internal class LyricMotionEffect : VideoEffectBase
    {
        public override string Label
        {
            get
            {
                if (EffectEnter && !EffectExit)
                    return "リリックモーションしながら登場";
                if (!EffectEnter && EffectExit)
                    return "リリックモーションしながら退場";
                if (!EffectEnter && !EffectExit)
                    return "リリックモーションしながら";
                return "リリックモーションしながら登場退場";
            }
        }

        //登場退場
        [Display(GroupName = "登場退場", Name = "登場時", Description = "アイテムが登場する際にエフェクトを適用する")]
        [ToggleSlider]
        public bool EffectEnter
        {
            get => effectEnter;
            set => Set(ref effectEnter, value, nameof(EffectEnter), nameof(Label));
        }
        bool effectEnter = true;

        [Display(GroupName = "登場退場", Name = "退場時", Description = "アイテムが退場する際にエフェクトを適用する")]
        [ToggleSlider]
        public bool EffectExit
        {
            get => effectExit;
            set => Set(ref effectExit, value, nameof(EffectExit), nameof(Label));
        }
        bool effectExit = true;

        


        [Display(GroupName = "登場退場", Name = "効果時間", Description = "エフェクトが再生される秒数")]
        [TextBoxSlider("F2", "秒", 0, 0.5)]
        [DefaultValue(0d)]
        [Range(0, 99999d)]
        public double PlayTime { get => time; set => Set(ref time, value); }
        double time = 0.30;

        [Display(GroupName = "登場退場", Name = "距離", Description = "距離")]
        [AnimationSlider("F1", "px", -100.0, 100.0)]
        public Animation Distance { get; } = new Animation(100.0, -99999.0, 99999.0);

        [Display(GroupName = "登場退場", Name = "種類", Description = "アニメーションの種類")]
        [EnumComboBox]
        public EasingType EasingType { get => easingType; set => Set(ref easingType, value); }
        EasingType easingType = EasingType.Expo;

        [Display(GroupName = "登場退場", Name = "加減速", Description = "アニメーションの加減速")]
        [EnumComboBox]
        public EasingMode EasingMode { get => easingMode; set => Set(ref easingMode, value); }
        EasingMode easingMode = EasingMode.Out;

        [Display(GroupName = "アニメーション詳細", Name = "X方向", Description = "X方向")]
        [ToggleSlider]
        public bool Direction_X { get => x; set => Set(ref x, value); }
        bool x = true;

        [Display(GroupName = "アニメーション詳細", Name = "Y方向", Description = "Y方向")]
        [ToggleSlider]
        public bool Direction_Y { get => y; set => Set(ref y, value); }
        bool y = true;

        [Display(GroupName = "アニメーション詳細", Name = "双方向", Description = "双方向")]
        [ToggleSlider]
        public bool Bidirectional { get => bidirectional; set => Set(ref bidirectional, value); }

        bool bidirectional = true;
        [Display(GroupName = "アニメーション詳細", Name = "同一方向", Description = "同一方向")]
        [ToggleSlider]
        public bool Same_Direction { get => same_Direction; set => Set(ref same_Direction, value); }
        bool same_Direction = false;

        [Display(GroupName = "アニメーション詳細", Name = "表示方法", Description = "テキストの表示方法")]
        [EnumComboBox]
        public DisplayMode Enum_Mode { get => mode; set => Set(ref mode, value); }
        DisplayMode mode = DisplayMode.Random;

        [Display(GroupName = "アニメーション詳細", Name = "シード", Description = "シード値")]
        [AnimationSlider("F0", "", 0, 10)]
        [ShowPropertyEditorWhen(nameof(Enum_Mode), DisplayMode.Random)]
        public Animation Offset { get; } = new Animation(0, 0, 1000);

        [Display(GroupName = "アニメーション詳細", Name = "オフセット", Description = "オフセット")]
        [AnimationSlider("F0", "", 0, 10)]
        [ShowPropertyEditorWhen(nameof(Enum_Mode), DisplayMode.Mutual)]
        public Animation Mutual_Offset { get; } = new Animation(0, 0, 1000);


        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new LyricMotionEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Distance, Offset, Mutual_Offset];
    }
}
