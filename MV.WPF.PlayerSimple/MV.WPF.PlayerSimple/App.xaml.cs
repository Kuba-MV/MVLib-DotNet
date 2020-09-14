/*
  Media Vault Library
  Copyright (C) 2020 Jakub Gluszkiewicz (kubabrt@gmail.com)
  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  You may use this code under license agreements which can be found at www.libmediavault.com.
  You may use this code and referenced libraries for free without any limitations and any fees under following conditions:
  - your derative work is for non-commercial purposes
  - your derative work is for commercial purposes but your annual company income is not greater than 50K $ (american dollars)
  Otherwise please contact me directly to buy license for commmercial use.
*/

using System.Windows;
using MV.DotNet.Common;

namespace MV.WPF.PlayerSimple
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Usually its good place to initialize Media Vault Library
            MV_Manager.InitializeMediaVault();

            //MV_Manager.InitializeLog("mvlog.log");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //Clean Media Lib
            MV_Manager.CloseMediaVault();
            MV_Manager.DisposeLog();

            base.OnExit(e);
        }
    }
}
