namespace RockChoir
{
    public interface IViewable
    {
        void SetView(bool viewActive);
        bool viewActive { get; }
    }
}