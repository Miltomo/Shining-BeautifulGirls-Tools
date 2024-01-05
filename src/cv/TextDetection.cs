using Tesseract;

namespace ComputerVision
{
    public static class TextDetection
    {
        public static string EnginePath { get; set; } = @".\tessdata\";

        public static string ImageDir { get; set; } = @".\image\";

        public static string ExtractTextUd(string imageName = "extract")
        {
            return ExtractText(ImageDir + $"{imageName}.png");
        }

        public static string ExtractText(string imagePath)
        {
            return new TesseractEngine(EnginePath, "chi_sim", EngineMode.LstmOnly)
                .Process(Pix.LoadFromFile(imagePath), PageSegMode.SingleLine)
                .GetText();
        }

        public static string ExtractDigits(string imagePath)
        {
            using var engine = new TesseractEngine(EnginePath, "chi_sim", EngineMode.Default);
            engine.DefaultPageSegMode = PageSegMode.SingleWord; // 限制识别字符集
            engine.SetVariable("tessedit_char_whitelist", "0123456789");

            return engine.Process(Pix.LoadFromFile(imagePath)).GetText();
        }

        public static string ExtractDigitsUd(string imageName = "extract")
        {
            return ExtractDigits(ImageDir + $"{imageName}.png");
        }
    }
}
