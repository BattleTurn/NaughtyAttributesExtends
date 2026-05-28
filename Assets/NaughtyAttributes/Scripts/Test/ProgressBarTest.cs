using UnityEngine;

namespace NaughtyAttributes.Test
{
    public class ProgressBarTest : MonoBehaviour
    {
        [Header("Constant ProgressBar")]
        [ProgressBar("Health", 100, EColor.Red)]
        public float health = 50.0f;

        [Header("Nested ProgressBar")]
        public ProgressBarNest1 nest1;

        [Header("Custom Color ProgressBar")]
        [ProgressBar("Experience", 100, colorName: nameof(GetProgressBarColor))]
        public float experience = 75.0f;

        private Color GetProgressBarColor()
        {
            // Full should be green, near 0 should be orange
            float t = Mathf.Clamp01(experience / 100f);
            Color orange = new Color(1f, 0.5f, 0f);

            if (t <= 0f) return orange;

            return Color.Lerp(orange, Color.yellow, t);
        }
    }

    [System.Serializable]
    public class ProgressBarNest1
    {
        [ProgressBar("Mana", 100, hexColor: "#3474FF")]
        public float mana = 25.0f;

        public ProgressBarNest2 nest2;
    }

    [System.Serializable]
    public class ProgressBarNest2
    {
        [ProgressBar("Stamina", nameof(maxStaminaValue), EColor.Green)]
        public int stamina = 50;
        public int maxStaminaValue = 100;
    }
}
