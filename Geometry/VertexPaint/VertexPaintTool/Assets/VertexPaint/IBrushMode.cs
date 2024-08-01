namespace bluebean
{
    public interface IBrushMode
    {
        string name{get;}
        bool needsInputValue{ get; }
        void ApplyStamps(BrushBase brush, bool modified);
    }
}
