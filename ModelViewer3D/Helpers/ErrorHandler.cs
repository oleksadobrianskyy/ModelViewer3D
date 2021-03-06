﻿using System;
using System.Windows;
using ModelViewer3D.Resources;

namespace ModelViewer3D.Helpers
{
    internal static class ErrorHandler
    {
        public static void ShowMessageBox(String message)
        {
            MessageBox.Show(message, Resource.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
