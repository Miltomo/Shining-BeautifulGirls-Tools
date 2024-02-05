using System.Text.RegularExpressions;

namespace ComputerVision
{
    public interface IOCRResult
    {
        public string Text { get; }
        public string[] TextAsLines { get; }
        public double[] NumericLines { get; }
        public bool Equals(object? target);
        public bool Contains(object target);
        public string? FirstIn(object[] finds);
        public string[] FindIn(object[] finds);
        public string[] Like(Regex regex);
    }
}
