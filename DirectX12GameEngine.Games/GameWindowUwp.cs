﻿#if WINDOWS_UWP
using System;
using System.Drawing;
using DirectX12GameEngine.Core;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DirectX12GameEngine.Games
{
    internal class GameWindowUwp : GameWindow
    {
        private readonly ApplicationView applicationView;
        private readonly CoreWindow? coreWindow;
        private readonly SwapChainPanel? swapChainPanel;
        private readonly WindowHandle windowHandle;

        public GameWindowUwp(GameBase game) : base(game)
        {
            GameContext gameContext = game.Context;

            switch (gameContext)
            {
                case GameContextCoreWindow coreWindowContext:
                    coreWindow = coreWindowContext.Control;

                    windowHandle = new WindowHandle(AppContextType.CoreWindow, coreWindow, IntPtr.Zero);

                    coreWindow.SizeChanged += CoreWindow_SizeChanged;
                    break;
                case GameContextXaml xamlContext:
                    swapChainPanel = xamlContext.Control;

                    windowHandle = new WindowHandle(AppContextType.Xaml, swapChainPanel, IntPtr.Zero);

                    swapChainPanel.SizeChanged += SwapChainPanel_SizeChanged;
                    swapChainPanel.CompositionScaleChanged += SwapChainPanel_CompositionScaleChanged;
                    break;
                default:
                    throw new ArgumentException();
            }

            applicationView = ApplicationView.GetForCurrentView();

            if (gameContext.RequestedWidth != 0 && gameContext.RequestedHeight != 0)
            {
                applicationView.SetPreferredMinSize(new Windows.Foundation.Size(gameContext.RequestedWidth, gameContext.RequestedHeight));
                applicationView.TryResizeView(new Windows.Foundation.Size(gameContext.RequestedWidth, gameContext.RequestedHeight));
            }
        }

        public override Rectangle ClientBounds
        {
            get
            {
                if (swapChainPanel != null)
                {
                    return new Rectangle(0, 0,
                        Math.Max(1, (int)(swapChainPanel.ActualWidth * swapChainPanel.CompositionScaleX + 0.5f)),
                        Math.Max(1, (int)(swapChainPanel.ActualHeight * swapChainPanel.CompositionScaleY + 0.5f)));
                }
                else if (coreWindow != null)
                {
                    double resolutionScale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

                    return new Rectangle((int)coreWindow.Bounds.X, (int)coreWindow.Bounds.X,
                        Math.Max(1, (int)(coreWindow.Bounds.Width * resolutionScale)), Math.Max(1, (int)(coreWindow.Bounds.Height * resolutionScale)));
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public override WindowHandle NativeWindow => windowHandle;

        public override void Dispose()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        internal override void Run()
        {
            if (swapChainPanel != null)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                return;
            }
            else if (coreWindow != null)
            {
                while (true)
                {
                    coreWindow.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
                    Tick();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            Tick();
        }

        private void SwapChainPanel_CompositionScaleChanged(SwapChainPanel sender, object args)
        {
            OnSizeChanged(EventArgs.Empty);
        }

        private void SwapChainPanel_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            OnSizeChanged(EventArgs.Empty);
        }

        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            OnSizeChanged(EventArgs.Empty);
        }
    }
}
#endif
