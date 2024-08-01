using UnityEngine;

namespace bluebean
{
    public class BrushModeSmooth : IBrushMode
    {
        BrushModeContainer m_owner;

        public BrushModeSmooth(BrushModeContainer owner)
        {
            this.m_owner = owner;
        }

        public string name
        {
            get { return "Smooth"; }
        }

        public bool needsInputValue
        {
            get { return false; }
        }

        public void ApplyStamps(BrushBase brush, bool modified)
        {
            var owner = (BrushModeContainer)m_owner;

            float averageValue = 0;
            float totalWeight = 0;

            for (int i = 0; i < brush.weights.Length; ++i)
            {
                if (!m_owner.Masked(i) && brush.weights[i] > 0)
                {
                    averageValue += owner.Get(i) * brush.weights[i];
                    totalWeight += brush.weights[i];
                }

            }
            averageValue /= totalWeight;

            for (int i = 0; i < brush.weights.Length; ++i)
            {
                if (!m_owner.Masked(i) && brush.weights[i] > 0)
                {
                    float currentValue = owner.Get(i);
                    float delta = brush.opacity * brush.speed * (Mathf.Lerp(currentValue,averageValue,brush.weights[i]) - currentValue);

                    owner.Set(i, currentValue + delta * (modified ? -1 : 1));
                }
            }

        }
    }
}