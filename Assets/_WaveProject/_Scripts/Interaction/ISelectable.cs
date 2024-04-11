namespace WaveProject.Interaction
{
    public interface ISelectable
    {
        Outline Outline { get; }
        
        void Select();
        void Deselect();
    }
}