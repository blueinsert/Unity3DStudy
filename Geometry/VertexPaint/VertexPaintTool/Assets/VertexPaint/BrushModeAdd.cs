namespace bluebean
{
    public class BrushModeAdd : IBrushMode
    {
        BrushModeContainer m_owner;

        public BrushModeAdd(BrushModeContainer owner)
        {
            this.m_owner = owner;
        }

        public string name
        {
            get { return "Add"; }
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
                    float delta = brush.weights[i] * brush.opacity * brush.speed * m_owner.GetDefault();

                    m_owner.Set(i, currentValue + delta * (modified ? -1 : 1));
                }
            }
        }
    }
}