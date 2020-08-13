using SkiaSharp;

namespace NNControl.Synapse.Impl
{
    internal static class ScaleColorManager
    {
        private static readonly string[] RedScale = new[]
        {
            "#003594", "#003696", "#003698", "#003799", "#00389B", "#00389D", "#00399E", "#003AA0", "#003AA2",
            "#003BA3", "#003CA4", "#003CA6", "#003DA8", "#003EA9", "#003FAC", "#003FAE", "#0040B0", "#0041B1",
            "#0042B3", "#0043B5", "#0044B7", "#0045B9", "#0046BA", "#0046BC", "#0047BD", "#0048BF", "#004AC1",
            "#004BC3", "#004CC5", "#004DC7", "#004EC8", "#004FCA", "#0050CB", "#0051CC", "#0053CE", "#0054CF",
            "#0055D1", "#0056D3", "#0057D4", "#0059D5", "#005AD7", "#005BD9", "#005CDA", "#005EDC", "#005FDD",
            "#0060DE", "#0061DF", "#0062E1", "#0064E3", "#0066E5", "#0067E6", "#0068E7", "#0069E8", "#006AEA",
            "#006DED", "#006EEE", "#006FEF", "#0070F0", "#0071F1", "#0072F2", "#0073F4", "#0076F6", "#0077F8",
            "#0079FA", "#007BFC", "#007CFD", "#007DFE", "#007EFF", "#0081FF", "#0083FF", "#0085FF", "#0087FF",
            "#0089FF", "#008BFF", "#008DFF", "#0090FF", "#0092FF", "#0094FF", "#0096FF", "#0098FF", "#009AFF",
            "#009DFF", "#009FFF", "#00A1FF", "#00A3FF", "#00A5FF", "#00A7FF", "#00A9FF", "#00ACFF", "#00AEFF",
            "#00B0FF", "#00B2FF", "#00B4FF", "#00B6FF", "#00B8FF", "#00BBFF", "#00BDFF", "#00BFFF", "#00C1FF",
            "#00C3FF", "#00C5FF", "#00C7FF", "#00CAFF", "#00CCFF", "#00CEFF", "#00D0FF", "#00D2FF", "#00D4FF",
            "#00D7FF", "#00D9FF", "#00DBFF", "#00DDFF", "#00DFFF", "#00E1FF", "#00E3FF", "#00E6FF", "#00E8FF",
            "#00EAFF", "#00EBFF", "#00EDFF", "#00EFFF", "#00F1FF", "#00F4FF", "#00F6FF", "#00F8FF", "#00FAFF", "#00FCFF"
        };

        private static readonly string[] RedScale2 = new[]
        {
            "#D70000", "#DA0000", "#DC0000", "#DE0000", "#E10000", "#E30000", "#E50000", "#E80000", "#EA0000",
            "#EC0000", "#EF0000", "#F10000", "#F30000", "#F50000", "#F80000", "#FA0000", "#FC0000", "#FE0600",
            "#FF0A00", "#FF0E00", "#FF1300", "#FF1500", "#FF1800", "#FF1B00", "#FF1D00", "#FF1F00", "#FF2100",
            "#FF2400", "#FF2600", "#FF2800", "#FF2B00", "#FF2D00", "#FF2F00", "#FF3200", "#FF3400", "#FF3600",
            "#FF3800", "#FF3B00", "#FF3D00", "#FF3F00", "#FF4200", "#FF4400", "#FF4600", "#FF4900", "#FF4B00",
            "#FF4D00", "#FF5000", "#FF5200", "#FF5400", "#FF5600", "#FF5900", "#FF5B00", "#FF5D00", "#FF6000",
            "#FF6200", "#FF6400", "#FF6700", "#FF6900", "#FF6B00", "#FF6D00", "#FF7000", "#FF7200", "#FF7400",
            "#FF7700", "#FF7900", "#FF7B00", "#FF7E00", "#FF8000", "#FF8200", "#FF8500", "#FF8700", "#FF8900",
            "#FF8B00", "#FF8E00", "#FF9000", "#FF9200", "#FF9500", "#FF9700", "#FF9900", "#FF9C00", "#FF9E00",
            "#FFA002", "#FFA105", "#FFA20A", "#FFA30D", "#FFA410", "#FFA613", "#FFA715", "#FFA817", "#FFA91A",
            "#FFAA1C", "#FFAB1E", "#FFAD21", "#FFAE23", "#FFAF24", "#FFB026", "#FFB228", "#FFB32A", "#FFB42B",
            "#FFB62E", "#FFB72F", "#FFB931", "#FFBA33", "#FFBB34", "#FFBD36", "#FFBE38", "#FFC039", "#FFC13B",
            "#FFC33D", "#FFC53F", "#FFC640", "#FFC741", "#FFC843", "#FFCA45", "#FFCC47", "#FFCF4A", "#FFD04B",
            "#FFD14C", "#FFD24D", "#FFD34F", "#FFD450", "#FFD551", "#FFD752", "#FFD853", "#FFD954", "#FFDB57", "#FFDD59"
        };


        private static readonly SKColor[] _colors = new SKColor[256];

        static ScaleColorManager()
        {
            for (int i = 0; i < 127; i++)
            {
                _colors[i] = SKColor.Parse(RedScale[i]);
            }

            _colors[127] = SKColor.Parse("#FFFFFF");

            for (int i = 128; i < 255; i++)
            {
                _colors[i] = SKColor.Parse(RedScale2[254 - i]);
            }

            _colors[255] = SKColor.Parse("#000000");
        }

        public static SKColor FromScale(int scale) => _colors[scale];
    }
}