using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ProgressBarAttribute : DrawerAttribute
    {
        public string Name { get; private set; }
        public float MaxValue { get; set; }
        public string MaxValueName { get; private set; }
        public EColor Color { get; private set; }
        public string ColorName { get; private set; }
        public string HexColor { get; private set; }

        public ProgressBarAttribute(string name, float maxValue, EColor color = EColor.Blue)
        {
            Name = name;
            MaxValue = maxValue;
            Color = color;
        }

        public ProgressBarAttribute(string name, string maxValueName, EColor color = EColor.Blue)
        {
            Name = name;
            MaxValueName = maxValueName;
            Color = color;
        }

        public ProgressBarAttribute(float maxValue, EColor color = EColor.Blue)
            : this("", maxValue, color)
        {
        }

        public ProgressBarAttribute(string maxValueName, EColor color = EColor.Blue)
            : this("", maxValueName, color)
        {
        }

        public ProgressBarAttribute(string maxValueName, string hexColor = "", string colorName = "")
            : this("", maxValueName, hexColor, colorName)
        {
        }

        public ProgressBarAttribute(float maxValue, string hexColor = "", string colorName = "")
            : this("", maxValue, hexColor, colorName)
        {
        }

        public ProgressBarAttribute(string name, string maxValueName, string hexColor = "", string colorName = "")
        {
            Name = name;
            MaxValueName = maxValueName;
            HexColor = hexColor;
            ColorName = colorName;
            Color = EColor.Custom;
        }

        public ProgressBarAttribute(string name, float maxValue, string hexColor = "", string colorName = "")
        {
            Name = name;
            MaxValue = maxValue;
            HexColor = hexColor;
            ColorName = colorName;
            Color = EColor.Custom;
        }
    }
}
