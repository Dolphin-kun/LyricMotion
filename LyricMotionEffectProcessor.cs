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

        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + "is null");

        public LyricMotionEffectProcessor(IGraphicsDevicesAndContext devices, LyricMotionEffect item)
        {
            this.devices = devices;
            this.item = item;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var textIndex = effectDescription.InputIndex;
            var offset = item.Offset.GetValue(frame, length, fps);
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

            bool isEnter = item.EffectEnter && (double)frame / fps <= item.PlayTime;
            bool isExit = item.EffectExit && (double)frame / fps >= (double)length / fps - item.PlayTime;

            if (!isEnter && !isExit)
                return effectDescription.DrawDescription;

            int seed = ((100 + textIndex + (int)offset) * (100 + textIndex)) + textIndex;
            int rand = new Random(seed % int.MaxValue).Next(directions.Count);
            var (xDir, yDir) = directions[rand];


            double easingRate = 0;
            if (isEnter)
            {
                double rate = frame / item.PlayTime / fps;
                easingRate = Easing.GetValue(item.EasingType, item.EasingMode, Math.Clamp(rate, 0, 1));
                easingRate = 1 - easingRate;
            }
            if (isExit)
            {
                double rate = (length - frame) / item.PlayTime / fps;
                easingRate = Easing.GetValue(item.EasingType, item.EasingMode, Math.Clamp(rate, 0, 1));
                easingRate = (1 - easingRate) * (item.Same_Direction ? -1 : 1);
            }

            double x = xDir * easingRate * distance;
            double y = yDir * easingRate * distance;

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


        public void ClearInput()
        {
            input = null;
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
        }

        public void Dispose()
        {
        }
    }

}
