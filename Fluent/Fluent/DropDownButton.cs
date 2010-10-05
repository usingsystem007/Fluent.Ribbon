#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents drop down button
    /// </summary>
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class DropDownButton : MenuBase, IQuickAccessItemProvider, IRibbonControl, IDropDownControl
    {
        #region Fields

        // Thumb to resize in both directions
        Thumb resizeBothThumb;
        // Thumb to resize vertical
        Thumb resizeVerticalThumb;

        private Popup popup;

        private Border buttonBorder;

        private IInputElement focusedElement;

        private MenuPanel menuPanel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup
        {
            get { return popup; }
        }

        /// <summary>
        /// Gets a value indicating whether context menu is opened
        /// </summary>
        public bool IsContextMenuOpened { get; set; }

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonControl.SizeProperty.AddOwner(typeof(DropDownButton));

        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        #region SizeDefinition Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonControl.AttachSizeDefinition(typeof(DropDownButton));

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        public string SizeDefinition
        {
            get { return (string)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public object Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(DropDownButton));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(DropDownButton));

        #endregion

        #region LargeIcon

        /// <summary>
        /// Gets or sets button large icon
        /// </summary>
        public object LargeIcon
        {
            get { return GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(object),
            typeof(DropDownButton), new UIPropertyMetadata(null));

        #endregion

        #region HasTriangle

        /// <summary>
        /// Gets or sets whether button has triangle
        /// </summary>
        public bool HasTriangle
        {
            get { return (bool)GetValue(HasTriangleProperty); }
            set { SetValue(HasTriangleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasTriangle. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasTriangleProperty =
            DependencyProperty.Register(
                "HasTriangle", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(true));

        #endregion

        #region IsDropDownOpen

        /// <summary>
        /// Gets or sets whether popup is opened
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownButton),
            new UIPropertyMetadata(false));

        #endregion

        #region ResizeMode

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode),
            typeof(DropDownButton), new UIPropertyMetadata(ContextMenuResizeMode.None));


        #endregion

        #region MaxDropDownHeight

        /// <summary>
        /// Get or sets max height of drop down popup
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(DropDownButton), new UIPropertyMetadata(350.0));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when context menu is opened
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Occurs when context menu is closed
        /// </summary>
        public event EventHandler DropDownClosed;

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static DropDownButton()
        {
            Type type = typeof(DropDownButton);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));

            ToolTipService.Attach(type);
            PopupService.Attach(type);
            ContextMenuService.Attach(type);

            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(false));

            StyleProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(DropDownButton));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DropDownButton()
        {
            KeyboardNavigation.SetControlTabNavigation(this, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);

            ContextMenuService.Coerce(this);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is FrameworkElement);
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (buttonBorder.IsMouseOver)
            {
                if (!IsDropDownOpen)
                {
                    IsDropDownOpen = true;
                }
                else PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.MouseNotOver);
                //Keyboard.Focus(popup);
                //Keyboard.Focus(FocusManager.GetFocusScope(ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement) as IInputElement);
                Debug.WriteLine(Keyboard.FocusedElement);
                focusedElement = Keyboard.FocusedElement;
                focusedElement.LostKeyboardFocus += OnFocusedElementLostKeyboardFocus;
                focusedElement.PreviewKeyDown += OnFocusedElementPreviewKeyDown;
                e.Handled = true;
            }
        }

        private void OnFocusedElementPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (!Items.Contains(Keyboard.FocusedElement))
                    Keyboard.Focus(Items[0] as IInputElement);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (!Items.Contains(Keyboard.FocusedElement))
                    Keyboard.Focus(Items[Items.Count - 1] as IInputElement);
                e.Handled = true;
            }
        }

        private void OnFocusedElementLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            focusedElement.LostKeyboardFocus -= OnFocusedElementLostKeyboardFocus;
            focusedElement.PreviewKeyDown -= OnFocusedElementPreviewKeyDown;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (popup != null)
            {
                popup.Opened -= OnDropDownOpened;
                popup.Closed -= OnDropDownClosed;
            }

            popup = GetTemplateChild("PART_Popup") as Popup;

            if (popup != null)
            {
                popup.Opened += OnDropDownOpened;
                popup.Closed += OnDropDownClosed;

                KeyboardNavigation.SetControlTabNavigation(popup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(popup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetTabNavigation(popup, KeyboardNavigationMode.Cycle);
            }

            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta -= OnResizeVerticalDelta;
            }
            resizeVerticalThumb = GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta += OnResizeVerticalDelta;
            }

            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta -= OnResizeBothDelta;
            }
            resizeBothThumb = GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta += OnResizeBothDelta;
            }

            buttonBorder = GetTemplateChild("PART_ButtonBorder") as Border;

            menuPanel = GetTemplateChild("PART_MenuPanel") as MenuPanel;

            base.OnApplyTemplate();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
            IsDropDownOpen = true;
        }

        #endregion

        #region Private methods

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(menuPanel.Width)) menuPanel.Width = menuPanel.ActualWidth;
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Width = Math.Max(menuPanel.MinWidth, menuPanel.Width + e.HorizontalChange);
            menuPanel.Height = Math.Min(Math.Max(menuPanel.MinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Height = Math.Min(Math.Max(menuPanel.MinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        // Handles drop down opened
        void OnDropDownClosed(object sender, EventArgs e)
        {
            if (DropDownClosed != null) DropDownClosed(this, e);
            if (Mouse.Captured == this) Mouse.Capture(null);
        }

        // Handles drop down closed
        void OnDropDownOpened(object sender, EventArgs e)
        {
            if (menuPanel != null)
            {
                menuPanel.Width = double.NaN;
                menuPanel.Height = double.NaN;
            }
            if (DropDownOpened != null) DropDownOpened(this, e);
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be synchronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            DropDownButton button = new DropDownButton();
            button.Size = RibbonControlSize.Small;
            BindQuickAccessItem(button);
            button.DropDownOpened += OnQuickAccessOpened;
            return button;
        }

        // Handles quick access button drop down menu opened
        protected void OnQuickAccessOpened(object sender, EventArgs e)
        {
            DropDownButton button = (DropDownButton)sender;
            if (ItemsSource != null)
            {
                button.ItemsSource = ItemsSource;
                ItemsSource = null;
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    object item = Items[0];
                    Items.Remove(item);
                    button.Items.Add(item);
                    i--;
                }
            }
            button.DropDownClosed += OnQuickAccessMenuClosed;
        }

        // Handles quick access button drop down menu closed
        protected void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            DropDownButton button = (DropDownButton)sender;
            button.DropDownClosed -= OnQuickAccessMenuClosed;
            if (button.ItemsSource != null)
            {
                ItemsSource = button.ItemsSource;
                button.ItemsSource = null;
            }
            else
            {
                for (int i = 0; i < button.Items.Count; i++)
                {
                    object item = button.Items[0];
                    button.Items.Remove(item);
                    Items.Add(item);
                    i--;
                }
            }
        }

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected virtual void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.BindQuickAccessItem(this, element);
            RibbonControl.Bind(this, element, "ResizeMode", ResizeModeProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "HasTriangle", HasTriangleProperty, BindingMode.Default);
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)GetValue(CanAddToQuickAccessToolBarProperty); }
            set { SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(DropDownButton), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        /// <summary>
        /// Gets or sets style of element on quick access toolbar
        /// </summary>
        public Style QuickAccessElementStyle
        {
            get { return (Style)GetValue(QuickAccessElementStyleProperty); }
            set { SetValue(QuickAccessElementStyleProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for QuickAccessElementStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessElementStyleProperty = RibbonControl.QuickAccessElementStyleProperty.AddOwner(typeof(DropDownButton));

        #endregion
    }


}
