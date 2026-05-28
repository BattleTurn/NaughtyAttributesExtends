using UnityEngine;

namespace NaughtyAttributes.Test
{
    public class NaughtyComponent : MonoBehaviour
    {
        [ProgressBar("Health", 300, EColor.Red)]
        public int health = 250;

        [ProgressBar("Mana", 100, hexColor: "#0000FF")]
        public int mana = 25;

        [ProgressBar("Stamina", maxValueName: nameof(maxStaminaValue), colorName: nameof(GetProgressBarColor))]
        public int stamina = 150;

        public int maxStaminaValue = 200;

        private Color GetProgressBarColor()
        {
            // Full should be green, near 0 should be orange
            float t = Mathf.Clamp01((float)stamina / maxStaminaValue);
            Color orange = Color.yellow;

            if (t <= 0f) return orange;

            return Color.Lerp(orange, Color.green, t);
        }
    }

    [System.Serializable]
    public class MyClass
    {
    }

    [System.Serializable]
    public struct MyStruct
    {
    }
}
