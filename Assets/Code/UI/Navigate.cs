using System;
using Klaesh.Core.Message;

namespace Klaesh.UI
{
    //public enum NavigateAction
    //{
    //    Show, Close
    //}

    public class Navigate : MessageBase
    {
        public Type WindowType { get; }
        //public NavigateAction Action { get; }

        public Navigate(object sender, Type windowType/*, NavigateAction action = NavigateAction.Show*/)
            : base(sender)
        {
            WindowType = windowType;
            //Action = action;
        }
    }
}
