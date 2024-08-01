
namespace bluebean
{
    public class BrushModePaint : IBrushMode
    {
        BrushModeContainer m_owner;

        public BrushModePaint(BrushModeContainer property)
        {
            this.m_owner = property;
        }

        public string name
        {
            get { return "Paint"; }
        }

        public bool needsInputValue
        {
            get { return true; }
        }

        public void ApplyStamps(BrushBase brush, bool modified)
        {
            for (int i = 0; i < brush.weights.Length; ++i)
            {
                if (!m_owner.Masked(i) && brush.weights[i] > 0)
                {
                    float currentValue = m_owner.Get(i);
                    float delta = brush.weights[i] * brush.opacity * brush.speed * (m_owner.GetDefault() - currentValue);

                    m_owner.Set(i, currentValue + delta * (modified ? -1 : 1));
                }
            }
        }
    }
}