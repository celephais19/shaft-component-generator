namespace InventorShaftGenerator.Views
{
    public interface IView
    {
        object DataContext { get; set; }
        void Show();
        void Close();
    }
}
