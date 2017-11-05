using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace myCollections.Controls
{
    public class BorderLessWindow : Window
    {
        public enum ResizeDirection
        {
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5
        }

        protected virtual void OnReady()
        {

        }

        private ResizeGrip _formResizeGrip;
        private Rectangle _leftResizeRect;
        private Rectangle _rightResizeRect;
        private Rectangle _bottomResizeRect;
        private Rectangle _topResizeRect;
        private Rectangle _topLeftResizeRect;
        private Rectangle _topRightResizeRect;
        private Rectangle _bottomLeftResizeRect;

        private UIElement _headerPanel;
        private Button _minimiseButton;
        private Button _maximiseBtn;
        private Button _restoreBtn;
        private Button _closeBtnMax;
        private Button _closeBtn;

        private HwndSource _hwndSource;
        private double _previousHeight;

        public RelayCommand MinimiseCommand
        {
            get { return (RelayCommand)GetValue(MinimiseCommandProperty); }
            set { SetValue(MinimiseCommandProperty, value); }
        }

        public RelayCommand MaximiseCommand
        {
            get { return (RelayCommand)GetValue(MaximiseCommandProperty); }
            set { SetValue(MaximiseCommandProperty, value); }
        }

        public RelayCommand ExitCommand
        {
            get { return (RelayCommand)GetValue(ExitCommandProperty); }
            set { SetValue(ExitCommandProperty, value); }
        }

        public bool ShowClose
        {
            get { return (bool)GetValue(ShowCloseProperty); }
            set { SetValue(ShowCloseProperty, value); }
        }

        public bool DontMove
        {
            get { return (bool)GetValue(DontMoveProperty); }
            set { SetValue(DontMoveProperty, value); }
        }

        public static readonly DependencyProperty ExitCommandProperty = DependencyProperty.Register("ExitCommand", typeof(RelayCommand), typeof(BorderLessWindow), new UIPropertyMetadata(null));
        public static readonly DependencyProperty MaximiseCommandProperty = DependencyProperty.Register("MaximiseCommand", typeof(RelayCommand), typeof(BorderLessWindow), new UIPropertyMetadata(null));
        public static readonly DependencyProperty MinimiseCommandProperty = DependencyProperty.Register("MinimiseCommand", typeof(RelayCommand), typeof(BorderLessWindow), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ShowCloseProperty = DependencyProperty.Register("ShowClose", typeof(bool), typeof(BorderLessWindow), new UIPropertyMetadata(null));
        public static readonly DependencyProperty DontMoveProperty = DependencyProperty.Register("DontMove", typeof(bool), typeof(BorderLessWindow), new UIPropertyMetadata(null));

        public BorderLessWindow()
        {
            Style = Application.Current.Resources["BorderLessWindow"] as Style;

            MinimiseCommand = new RelayCommand(ExecuteMinimiseCommand);
            MaximiseCommand = new RelayCommand(ExecuteMaximiseCommand);
            ExitCommand = new RelayCommand(ExecuteExitCommand);

            SourceInitialized += new EventHandler(BorderLessWindow_SourceInitialized);
            Loaded += new RoutedEventHandler(OnLoaded);
            StateChanged += WindowStateChanged;
        }

        void BorderLessWindow_SourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            if (_hwndSource != null)
                _hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        private void ExecuteMinimiseCommand(object obj)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExecuteMaximiseCommand(object obj)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void ExecuteExitCommand(object obj)
        {
            Close();
        }

        public bool IsModal { get; set; }

        public override void OnApplyTemplate()
        {
            _formResizeGrip = (ResizeGrip)GetTemplateChild("PART_FormResizeGrip");
            _leftResizeRect = (Rectangle)GetTemplateChild("PART_LeftResizeRect");
            _rightResizeRect = (Rectangle)GetTemplateChild("PART_RightResizeRect");
            _bottomResizeRect = (Rectangle)GetTemplateChild("PART_BottomResizeRect");
            _topResizeRect = (Rectangle)GetTemplateChild("PART_TopResizeRect");
            _topLeftResizeRect = (Rectangle)GetTemplateChild("PART_TopLeftResizeRect");
            _topRightResizeRect = (Rectangle)GetTemplateChild("PART_TopRightResizeRect");
            _bottomLeftResizeRect = (Rectangle)GetTemplateChild("PART_BottomLeftResizeRect");

            _headerPanel = (UIElement)GetTemplateChild("PART_HeaderPanel");
            _minimiseButton = (Button)GetTemplateChild("PART_MinimiseButton");
            _maximiseBtn = (Button)GetTemplateChild("PART_MaximiseBtn");
            _restoreBtn = (Button)GetTemplateChild("PART_RestoreBtn");
            _closeBtnMax = (Button)GetTemplateChild("PART_CloseBtnMax");
            _closeBtn = (Button)GetTemplateChild("PART_CloseBtn");

            HandleResize(_formResizeGrip);
            HandleResize(_leftResizeRect);
            HandleResize(_rightResizeRect);
            HandleResize(_bottomResizeRect);
            HandleResize(_topResizeRect);
            HandleResize(_topLeftResizeRect);
            HandleResize(_topRightResizeRect);
            HandleResize(_bottomLeftResizeRect);

            if (IsModal == true || ResizeMode == ResizeMode.NoResize)
            {
                //if (_headerPanel != null) _headerPanel.Visibility = Visibility.Collapsed;
                if (_minimiseButton != null) _minimiseButton.Visibility = Visibility.Collapsed;
                if (_maximiseBtn != null) _maximiseBtn.Visibility = Visibility.Collapsed;
                if (_restoreBtn != null) _restoreBtn.Visibility = Visibility.Collapsed;
                if (_closeBtnMax != null) _closeBtnMax.Visibility = Visibility.Collapsed;
                //if (_closeBtn != null) _closeBtn.Visibility = Visibility.Collapsed;
            }

            if (ShowClose == false)
            {
                if (_closeBtn != null) _closeBtn.Visibility = Visibility.Collapsed;
                if (_closeBtnMax != null) _closeBtnMax.Visibility = Visibility.Collapsed;
                if (_restoreBtn != null) _restoreBtn.Visibility = Visibility.Collapsed;
                if (_maximiseBtn != null) _maximiseBtn.Visibility = Visibility.Collapsed;
                if (_minimiseButton != null) _minimiseButton.Visibility = Visibility.Collapsed;
            }

            if (DontMove == true)
            {
                if (_headerPanel != null)
                    _headerPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (_headerPanel != null)
                    _headerPanel.MouseLeftButtonDown += HeaderPanelMouseLeftButtonDown;
            }

            base.OnApplyTemplate();
        }

        private void HandleResize(UIElement element)
        {
            if (element == null)
                return;

            if (IsModal == true || ResizeMode == ResizeMode.NoResize)
                element.Visibility = Visibility.Collapsed;
            else
                element.MouseLeftButtonDown += SizingRect_MouseLeftButtonDown;
        }


        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
        }



        #region No border / Resize

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case (int)Win32.WindowMessages.WM_DWMCOMPOSITIONCHANGED:
                    TryApplyShadow();
                    break;

                case (int)Win32.WindowMessages.WM_NCACTIVATE:
                    {
                        IntPtr ptr = Win32.DefWindowProc(hwnd, msg, wParam, new IntPtr(-1));
                        handled = true;
                        return ptr;
                    }

                case (int)Win32.WindowMessages.WM_NCCALCSIZE:
                    if (wParam == new IntPtr(1))
                        handled = true;
                    break;

                case (int)Win32.WindowMessages.WM_ERASEBKGND:
                    try
                    {
                        System.Drawing.Graphics.FromHdc(wParam).Clear(System.Drawing.Color.White);
                        handled = true;
                    }
                    catch
                    {
                    }
                    break;

                case (int)Win32.WindowMessages.WM_GETMINMAXINFO:
                    NativeMethods.WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;

                case (int)Win32.WindowMessages.WM_WINDOWPOSCHANGING:
                    if (!m_shadowAttempted)
                        TryApplyShadow();
                    break;

            }
            return IntPtr.Zero;
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            NativeMethods.SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(0xf000 + direction), IntPtr.Zero);
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            if (IsModal == true || ResizeMode == ResizeMode.NoResize)
                WindowState = WindowState.Normal;

            switch (base.WindowState)
            {
                case WindowState.Normal:
                    SetWindow(0);
                    break;

                case WindowState.Minimized:
                    break;

                case WindowState.Maximized:
                    SetWindow(4);
                    break;
            }
        }

        private void SetWindow(int screenMode)
        {
            if (IsModal == true || ResizeMode == ResizeMode.NoResize)
                return;

            switch (screenMode)
            {
                case 0:
                    _maximiseBtn.Visibility = Visibility.Visible;
                    _restoreBtn.Visibility = Visibility.Collapsed;
                    _closeBtnMax.Visibility = Visibility.Collapsed;
                    _closeBtn.Visibility = Visibility.Visible;
                    _formResizeGrip.Visibility = Visibility.Visible;
                    _topResizeRect.Visibility = Visibility.Visible;
                    _leftResizeRect.Visibility = Visibility.Visible;
                    _bottomResizeRect.Visibility = Visibility.Visible;
                    _rightResizeRect.Visibility = Visibility.Visible;
                    _topLeftResizeRect.Visibility = Visibility.Visible;
                    _topRightResizeRect.Visibility = Visibility.Visible;
                    _bottomLeftResizeRect.Visibility = Visibility.Visible;
                    return;

                case 4:
                    _maximiseBtn.Visibility = Visibility.Collapsed;
                    _restoreBtn.Visibility = Visibility.Visible;
                    _closeBtnMax.Visibility = Visibility.Visible;
                    _closeBtn.Visibility = Visibility.Collapsed;
                    _formResizeGrip.Visibility = Visibility.Hidden;
                    _topResizeRect.Visibility = Visibility.Hidden;
                    _leftResizeRect.Visibility = Visibility.Hidden;
                    _bottomResizeRect.Visibility = Visibility.Hidden;
                    _rightResizeRect.Visibility = Visibility.Hidden;
                    _topLeftResizeRect.Visibility = Visibility.Hidden;
                    _topRightResizeRect.Visibility = Visibility.Hidden;
                    _bottomLeftResizeRect.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private bool m_shadowAttempted;

        public void TryApplyShadow()
        {
            if (Win32.IsDwmAvailable())
            {
                Win32.MARGINS margins;

                margins.bottomHeight = 1;
                margins.leftWidth = 0;
                margins.rightWidth = 0;
                margins.topHeight = 0;

                WindowInteropHelper helper = new WindowInteropHelper(this);
                Win32.DwmExtendFrameIntoClientArea(helper.Handle, ref margins);
            }

            if (m_shadowAttempted == false)
                OnReady();

            m_shadowAttempted = true;
        }

        private void SizingRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (base.WindowState != WindowState.Maximized)
            {
                if ((e.ClickCount >= 2) && (((sender as Rectangle) == _topResizeRect) || ((sender as Rectangle) == _bottomResizeRect)))
                {
                    _previousHeight = base.Height;
                    base.Top = UIHelper.CurrentScreen().WorkingArea.Top;
                    base.Height = UIHelper.CurrentScreen().WorkingArea.Height;
                }
                else
                {
                    if ((sender as Rectangle) == _leftResizeRect)
                    {
                        ResizeWindow(ResizeDirection.Left);
                    }
                    else if ((sender as Rectangle) == _topLeftResizeRect)
                    {
                        ResizeWindow(ResizeDirection.TopLeft);
                    }
                    else if ((sender as Rectangle) == _topResizeRect)
                    {
                        ResizeWindow(ResizeDirection.Top);
                    }
                    else if ((sender as Rectangle) == _topRightResizeRect)
                    {
                        ResizeWindow(ResizeDirection.TopRight);
                    }
                    else if ((sender as Rectangle) == _rightResizeRect)
                    {
                        ResizeWindow(ResizeDirection.Right);
                    }
                    else if ((sender as ResizeGrip) == _formResizeGrip)
                    {
                        ResizeWindow(ResizeDirection.BottomRight);
                    }
                    else if ((sender as Rectangle) == _bottomResizeRect)
                    {
                        ResizeWindow(ResizeDirection.Bottom);
                    }
                    else if ((sender as Rectangle) == _bottomLeftResizeRect)
                    {
                        ResizeWindow(ResizeDirection.BottomLeft);
                    }
                }
            }
        }

        private void HeaderPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
            if (_previousHeight > 0.0)
            {
                base.Height = _previousHeight;
            }
            _previousHeight = 0.0;

            if (e.ClickCount >= 2)
            {
                switch (base.WindowState)
                {
                    case WindowState.Normal:
                        SetWindowsState(WindowState.Maximized);
                        return;

                    case WindowState.Minimized:
                        return;

                    case WindowState.Maximized:
                        SetWindowsState(WindowState.Normal);
                        return;
                }
            }
        }

        protected virtual void SetWindowsState(WindowState state)
        {
            WindowState = state;
        }

        #endregion


    }
}
