using LyricMotion.Mode_Enum;
using System.Numerics;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace LyricMotion
{
    internal partial class LyricMotionEffectProcessor : IVideoEffectProcessor
    {
        private readonly IGraphicsDevicesAndContext devices;
        readonly LyricMotionEffect item;
        ID2D1Image? input;

        ID2D1CommandList? commandList;
        public ID2D1Image Output => item.FixSize ? (commandList ?? input ?? throw new NullReferenceException(nameof(input) + " is null")) : (input ?? throw new NullReferenceException(nameof(input) + " is null"));

        public LyricMotionEffectProcessor(IGraphicsDevicesAndContext devices, LyricMotionEffect item)
        {
            this.devices = devices;
            this.item = item;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            commandList?.Dispose();
            commandList = null;

            if (input == null)
                return effectDescription.DrawDescription;

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var textIndex = effectDescription.InputIndex;
            var offset = item.Offset.GetValue(frame, length, fps);
            var mutual_offset = item.Mutual_Offset.GetValue(frame, length, fps);
            var distance = item.Distance.GetValue(frame, length, fps);

            var dx = item.Direction_X;
            var dy = item.Direction_Y;
            var bid = item.Bidirectional;

            List<(int xDir, int yDir)> directions = [];

            if (dx)
            {
                directions.Add((1, 0));
                if (bid) directions.Add((-1, 0));
            }
            if (dy)
            {
                directions.Add((0, 1));
                if (bid) directions.Add((0, -1));
            }

            if (directions.Count == 0)
                return effectDescription.DrawDescription;

            if (item.PlayTime <= 0)
                return effectDescription.DrawDescription;

            double timePos = (double)frame / fps;
            double totalTime = (double)length / fps;

            bool isEnter = item.EffectEnter && timePos <= item.PlayTime;
            bool isExit = item.EffectExit && timePos >= totalTime - item.PlayTime;

            if (isEnter && isExit)
            {
                if (timePos < totalTime / 2.0)
                    isExit = false;
                else
                    isEnter = false;
            }

            if (!isEnter && !isExit)
                return effectDescription.DrawDescription;

            int seed = ((100 + textIndex + (int)Math.Floor(offset)) * (100 + textIndex)) + textIndex;
            int rand;
            if (item.Enum_Mode == DisplayMode.Random)
            {
                rand = new Random(seed % int.MaxValue).Next(directions.Count);
            }
            else
            {
                rand = (textIndex + 1 + (int)Math.Floor(mutual_offset)) % directions.Count;
            }

            var (xDir, yDir) = directions[rand];


            long playFrames = Math.Max(1, (long)Math.Round(item.PlayTime * fps));

            double easingRate;
            if (isEnter)
            {
                if (item.EasingSetting == EasingSetting.Default)
                {
                    double rate = timePos / item.PlayTime;
                    double eased = Easing.GetValue(item.EasingType, item.EasingMode, Math.Clamp(rate, 0, 1));
                    easingRate = 1 - eased;
                }
                else
                {
                    long currentFrame = Math.Clamp(frame, 0, playFrames);
                    double progress = item.CustomEasing.GetValue(currentFrame, playFrames, fps) / 100.0;
                    easingRate = 1 - progress;
                }
            }
            else
            {
                if (item.EasingSetting == EasingSetting.Default)
                {
                    double remaining = totalTime - timePos;
                    double rate = remaining / item.PlayTime;
                    double eased = Easing.GetValue(item.EasingType, item.EasingMode, Math.Clamp(rate, 0, 1));
                    easingRate = (1 - eased) * (item.Same_Direction ? -1 : 1);
                }
                else
                {
                    long exitFrame = Math.Clamp(frame - (length - playFrames), 0, playFrames);
                    double progress = item.CustomEasing.GetValue(exitFrame, playFrames, fps) / 100.0;
                    easingRate = progress * (item.Same_Direction ? -1 : 1);
                }
            }

            double x = xDir * easingRate * distance;
            double y = yDir * easingRate * distance;

            if (item.FixSize)
            {
                var dc = devices.DeviceContext;
                var oldTarget = dc.Target;

                commandList = dc.CreateCommandList();

                dc.Target = commandList;
                dc.BeginDraw();
                dc.Clear(null);

                var bounds = dc.GetImageLocalBounds(input);

                using (var layer = dc.CreateLayer())
                {
                    dc.PushLayer(new LayerParameters1
                    {
                        ContentBounds = bounds,
                        MaskAntialiasMode = AntialiasMode.PerPrimitive,
                        Opacity = 1.0f,
                        LayerOptions = LayerOptions1.None,
                    }, layer);

                    dc.Transform = Matrix3x2.CreateTranslation((float)x, (float)y);
                    dc.DrawImage(input);
                    dc.Transform = Matrix3x2.Identity;

                    dc.PopLayer();
                }

                dc.EndDraw();
                commandList.Close();
                dc.Target = oldTarget;

                return effectDescription.DrawDescription;
            }
            else
            {
                var drawDesc = effectDescription.DrawDescription;
                return drawDesc with
                {
                    Draw = new(
                        drawDesc.Draw.X + (float)x,
                        drawDesc.Draw.Y + (float)y,
                        drawDesc.Draw.Z
                    ),

                };
            }
        }


        public void ClearInput()
        {
            commandList?.Dispose();
            commandList = null;
            input = null;
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
        }

        public void Dispose()
        {
            commandList?.Dispose();
        }
    }

}
