using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ColorMC.Core;
using ColorMC.Core.Game;
using ColorMC.Core.Objs;
using ColorMC.Gui.UI.Controls.Main;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils.LaunchSetting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColorMC.Gui.UI.Windows;

public partial class MainWindow : Window
{
    private readonly List<GamesControl> Groups = new();
    private GamesControl DefaultGroup;
    private GameControl? Obj;
    private Dictionary<GameSettingObj, GameControl> Launchs = new();

    private LaunchState Last;

    public MainWindow()
    {
        InitializeComponent();

        Head.SetWindow(this);

        FontFamily = Program.Font;

        Icon = App.Icon;

        Rectangle1.MakeResizeDrag(this);

        ItemInfo.SetWindow(this);

        Load();
        Load1();

        CoreMain.GameLaunch = GameLunch;
        CoreMain.GameDownload = GameDownload;

        App.UserEdit += MainWindow_OnUserEdit;
        Opened += MainWindow_Opened;
        Closed += MainWindow_Closed;

        App.PicUpdate += Update;

        Update();
    }

    public async void Launch(bool debug)
    {
        Info1.Show(Localizer.Instance["MainWindow.Info3"]);
        var res = await GameBinding.Launch(Obj!.Obj, debug);
        Info1.Close();
        if (res.Item1 == false)
        {
            switch (Last)
            {
                case LaunchState.LoginFail:
                    Info.Show(Localizer.Instance["MainWindow.Error1"]);
                    break;
                case LaunchState.JavaError:
                    Info.Show(Localizer.Instance["MainWindow.Error2"]);
                    break;
                default:
                    Info.Show(res.Item2!);
                    break;
            }
        }
        else
        {
            Launchs.Add(Obj.Obj, Obj);
            Obj.SetLaunch(true);
            Info2.Show(Localizer.Instance["MainWindow.Info2"]);
        }
    }

    public void GameClose(GameSettingObj obj)
    {
        if (Launchs.Remove(obj, out var con))
        {
            Dispatcher.UIThread.Post(() =>
            {
                con.SetLaunch(false);
            });
        }
    }

    private void MainWindow_OnUserEdit()
    {
        Dispatcher.UIThread.Post(Load1);
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        App.PicUpdate -= Update;
        App.UserEdit -= MainWindow_OnUserEdit;

        App.MainWindow = null;
        CoreMain.GameLaunch = null;
        CoreMain.GameDownload = null;

        App.Close();
    }

    private void MainWindow_Opened(object? sender, EventArgs e)
    {
        ItemInfo.Expander1.MakeTran();

        MotdLoad();
    }

    public void MotdLoad()
    {
        var config = ConfigBinding.GetAllConfig();
        if (config.Item2 != null && config.Item2.ServerCustom?.Motd == true &&
            !string.IsNullOrWhiteSpace(config.Item2.ServerCustom.IP))
        {
            ServerMotdControl1.IsVisible = true;
            ServerMotdControl1.Load(config.Item2.ServerCustom.IP, config.Item2.ServerCustom.Port);
        }
        else
        {
            ServerMotdControl1.IsVisible = false;
        }
    }

    public void GameItemSelect(GameControl? obj)
    {
        Obj = obj;
        if (obj != null)
        {
            ItemInfo.SetGame(obj.Obj);
        }
        else
        {
            ItemInfo.SetGame(null);
        }
    }

    private Task<bool> GameDownload(LaunchState state, GameSettingObj obj)
    {
        return Dispatcher.UIThread.InvokeAsync(async () =>
        {
            return state switch
            {
                LaunchState.LostLib => await Info.ShowWait(Localizer.Instance["MainWindow.Lost2"]),
                LaunchState.LostLoader => await Info.ShowWait(Localizer.Instance["MainWindow.Lost3"]),
                LaunchState.LostLoginCore => await Info.ShowWait(Localizer.Instance["MainWindow.Lost4"]),
                _ => await Info.ShowWait(Localizer.Instance["MainWindow.Lost1"]),
            };
        });
    }

    private void GameLunch(GameSettingObj obj, LaunchState state)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Last = state;
            switch (state)
            {
                case LaunchState.Login:
                    Info1.NextText(Localizer.Instance["MainWindow.Check1"]);
                    break;
                case LaunchState.Check:
                    Info1.NextText(Localizer.Instance["MainWindow.Check2"]);
                    break;
                case LaunchState.CheckVersion:
                    Info1.NextText(Localizer.Instance["MainWindow.Check3"]);
                    break;
                case LaunchState.CheckLib:
                    Info1.NextText(Localizer.Instance["MainWindow.Check4"]);
                    break;
                case LaunchState.CheckAssets:
                    Info1.NextText(Localizer.Instance["MainWindow.Check5"]);
                    break;
                case LaunchState.CheckLoader:
                    Info1.NextText(Localizer.Instance["MainWindow.Check6"]);
                    break;
                case LaunchState.CheckLoginCore:
                    Info1.NextText(Localizer.Instance["MainWindow.Check7"]);
                    break;
                case LaunchState.CheckMods:
                    Info1.NextText(Localizer.Instance["MainWindow.Check10"]);
                    break;
                case LaunchState.Download:
                    Info1.NextText(Localizer.Instance["MainWindow.Check8"]);
                    break;
                case LaunchState.JvmPrepare:
                    Info1.NextText(Localizer.Instance["MainWindow.Check9"]);
                    break;
            }
        });
    }

    private void Load1()
    {
        ItemInfo.SetUser(UserBinding.GetLastUser());
    }

    public void IsDelete()
    {
        Obj = null;
        ItemInfo.SetGame(null);
    }

    public void Load()
    {
        Dispatcher.UIThread.Post(ItemInfo.Load);

        Task.Run(async () =>
        {
            Groups.Clear();

            var config = ConfigBinding.GetAllConfig();

            if (config.Item2 != null && config.Item2.ServerCustom?.LockGame == true)
            {
                var game = GameBinding.GetGame(config.Item2.ServerCustom?.GameName);
                if (game == null)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        GameGroups.Children.Clear();

                        var item = new Grid
                        {
                            Background = Brush.Parse("#EEEEEE")
                        };
                        var item1 = new Label
                        {
                            Content = "没有启动实例，请联系服务器管理员"
                        };

                        item.Children.Add(item1);

                        GameGroups.VerticalAlignment = VerticalAlignment.Center;
                        GameGroups.HorizontalAlignment = HorizontalAlignment.Center;

                        GameGroups.Children.Add(item);
                    });
                }
                else
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        GameGroups.Children.Clear();

                        var item = new GameControl();
                        item.SetItem(game);
                        item.SetSelect(true);
                        item.DoubleTapped += Item_DoubleTapped;

                        GameItemSelect(item);

                        GameGroups.VerticalAlignment = VerticalAlignment.Center;
                        GameGroups.HorizontalAlignment = HorizontalAlignment.Center;

                        GameGroups.Children.Add(item);
                    });
                }
            }
            else
            {
                var list = GameBinding.GetGameGroups();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    GameGroups.VerticalAlignment = VerticalAlignment.Top;
                    GameGroups.HorizontalAlignment = HorizontalAlignment.Stretch;

                    DefaultGroup = new();
                });
                DefaultGroup.SetWindow(this);
                foreach (var item in list)
                {
                    if (item.Key == " ")
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            DefaultGroup.SetItems(item.Value);
                            DefaultGroup.SetName(Localizer.Instance["MainWindow.DefaultGroup"]);
                        });
                    }
                    else
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            var group = new GamesControl();
                            group.SetItems(item.Value);
                            group.SetName(item.Key);
                            group.SetWindow(this);
                            Groups.Add(group);
                        });
                    }
                }

                Dispatcher.UIThread.Post(() =>
                {
                    GameGroups.Children.Clear();
                    foreach (var item in Groups)
                    {
                        GameGroups.Children.Add(item);
                    }
                    GameGroups.Children.Add(DefaultGroup);
                });
            }
        });
    }

    private void Item_DoubleTapped(object? sender, TappedEventArgs e)
    {
        Launch(false);
    }

    public void Update()
    {
        App.Update(this, Image_Back, Grid1);
    }

    public async void EditGroup(GameSettingObj obj)
    {
        await Group.Set(obj);
        Group.Close();
        if (Group.Cancel)
        {
            return;
        }

        var res = Group.Read();
        GameBinding.MoveGameGroup(obj, res);
    }

    public async Task AddGroup()
    {
        await Info3.ShowOne(Localizer.Instance["MainWindow.Info1"], false);
        Info3.Close();
        if (Info3.Cancel)
        {
            return;
        }

        var res = Info3.Read().Item1;
        if (string.IsNullOrWhiteSpace(res))
        {
            Info1.Show(Localizer.Instance["MainWindow.Error4"]);
            return;
        }

        if (!GameBinding.AddGameGroup(res))
        {
            Info1.Show(Localizer.Instance["MainWindow.Error5"]);
            return;
        }
    }

    public async void DeleteGame(GameSettingObj obj)
    {
        var res = await Info.ShowWait(string.Format("是否要删除实例 {0}", obj.Name));
        if (!res)
            return;

        await GameBinding.DeleteGame(obj);
    }
}
