namespace Klaesh.UI.Window
{
    public class WindowBase : ViewModelBehaviour
    {
        public void Close()
        {
            var nav = _locator.GetService<INavigator>();
            nav.CloseCurrentWindow();
        }
    }
}
