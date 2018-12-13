using System;
using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using UnityEngine;

namespace Klaesh.UI
{
    public interface INavigator
    {
        Navigate CurrentNavigate { get; }

        void CloseCurrentWindow();
    }

    public class Navigator : ManagerBehaviour, INavigator
    {
        private Stack<Navigate> _history;

        public Navigate CurrentNavigate { get; private set; }

        protected override void OnAwake()
        {
            _history = new Stack<Navigate>();

            _bus.Subscribe<Navigate>(HandleNavigate);
        }

        public void CloseCurrentWindow()
        {
            if (CurrentNavigate != null && _history.Count > 0)
            {
                CloseWindow(CurrentNavigate.WindowType);
            }
        }

        private void HandleNavigate(Navigate msg)
        {
            //switch (msg.Action)
            //{
            //    case NavigateAction.Show:
            //        ShowWindow(msg);
            //        break;
            //    case NavigateAction.Close:

            //        break;
            //}
            ShowWindow(msg);
        }

        private void ShowWindow(Navigate target)
        {
            if (CurrentNavigate != null)
            {
                if (CurrentNavigate.WindowType == target.WindowType)
                {
                    Debug.Log($"[Navigator] tried to show window {target.WindowType} but it is already open!");
                    return;
                }

                // close current window and save in history
                CloseWindow(CurrentNavigate.WindowType, false);
                _history.Push(CurrentNavigate);
            }

            CurrentNavigate = target;

            // check if cached window is available
            foreach (Transform child in transform)
            {
                var viewModel = child.GetComponent(CurrentNavigate.WindowType);
                if (viewModel == null)
                    continue;

                viewModel.gameObject.SetActive(true);
                Debug.Log($"[Navigator] showing window {CurrentNavigate.WindowType}");
                return;
            }

            // no window of type loaded, so load it!
            var path = $"Windows/{CurrentNavigate.WindowType.Name}";
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
                throw new Exception($"unable to locate window with name {CurrentNavigate.WindowType.Name}");
            var window = Instantiate(prefab, transform);
            window.name = CurrentNavigate.WindowType.Name;
            window.gameObject.SetActive(true);
            Debug.Log($"[Navigator] loaded window {CurrentNavigate.WindowType}");
        }

        private void CloseWindow(Type windowType, bool showPrevious = true)
        {
            bool closed = false;

            foreach (Transform child in transform)
            {
                var viewModel = child.GetComponent(windowType);
                if (viewModel == null)
                    continue;

                closed = true;

                viewModel.gameObject.SetActive(false);
                Debug.Log($"[Navigator] closed window {windowType}");
                break;
            }

            // navigate to previous window?
            if (showPrevious && closed && _history.Count > 0)
            {
                CurrentNavigate = null;
                ShowWindow(_history.Pop());
            }
        }
    }
}
