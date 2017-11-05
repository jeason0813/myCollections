using System.Linq;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;

namespace myCollections.Controls
{
    public static class UIHelper
    {
        public static T FindAncestor<T>(UIElement element) where T : UIElement
        {
            for (UIElement element2 = element; element2 != null; element2 = VisualTreeHelper.GetParent(element2) as UIElement)
            {
                T local = element2 as T;
                if (local != null)
                {
                    return local;
                }
            }
            return default(T);
        }

        public static Screen CurrentScreen()
        {
            int num = 0;
            Screen[] screenArray = Screen.AllScreens.OrderBy<Screen, int>(delegate(Screen s)
            {
                return s.Bounds.Left;
            })
            .ToArray<Screen>();

            foreach (Screen screen in screenArray)
            {
                if ((Screen.AllScreens.Count() == 1) || (num == (Screen.AllScreens.Count() - 1)))
                {
                    return screen;
                }
                if (System.Windows.Application.Current.MainWindow.Left >= screen.Bounds.Left)
                {
                    if ((System.Windows.Application.Current.MainWindow.WindowState == WindowState.Maximized) &&
                        (System.Windows.Application.Current.MainWindow.Left < screenArray[num + 1].Bounds.Left))
                    {
                        return screen;
                    }
                    if ((System.Windows.Application.Current.MainWindow.Left + System.Windows.Application.Current.MainWindow.Width) <= screen.Bounds.Right)
                    {
                        return screen;
                    }
                }
                num++;
            }
            return Screen.AllScreens[0];
        }


    }
}
