namespace Klaesh.UI.Window
{
    public class WindowBase : ViewModelBehaviour
    {
        protected void Close()
        {
            var nav = _locator.GetService<INavigator>();
            nav.CloseCurrentWindow();
        }
    }
}
