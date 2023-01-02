using Avalonia.Controls;
using Avalonia.Input;
using System;
using Avalonia.Interactivity;
using Avalonia.Animation;
using System.Threading;
using ColorMC.Gui.UIBinding;
using ColorMC.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ColorMC.Core.Objs.Login;
using ColorMC.Core.Game.Auth;
using ColorMC.Core.Utils;

namespace ColorMC.Gui.UI;

public record UserObj
{ 
    public bool Use { get; set; }
    public string Name { get; set; }
    public string UUID { get; set; }
    public string Type { get; set; }
    public string Text1 { get; set; }
    public string Text2 { get; set; }

    public AuthType AuthType { get; set; }
}

public partial class UserWindow : Window
{
    private ObservableCollection<UserObj> List = new();
    private static readonly CrossFade CrossFade = new(TimeSpan.FromMilliseconds(300));
    public UserWindow()
    {
        InitializeComponent();

        this.MakeItNoChrome();
        FontFamily = Program.FontFamily;

        if (App.BackBitmap != null)
        {
            Image_Back.Source = App.BackBitmap;
        }

        DataGrid_User.Items = List;
        DataGrid_User.DoubleTapped += DataGrid_User_DoubleTapped;

        Button_A1.PointerLeave += Button_A1_PointerLeave;
        Button_A.PointerEnter += Button_A_PointerEnter;

        Button_D1.PointerLeave += Button_D1_PointerLeave;
        Button_D.PointerEnter += Button_D_PointerEnter;

        Button_S1.PointerLeave += Button_S1_PointerLeave;
        Button_S.PointerEnter += Button_S_PointerEnter;

        Button_A1.Click += Button_A1_Click;
        Button_D1.Click += Button_D1_Click;
        Button_S1.Click += Button_S1_Click;

        Button_Cancel.Click += Button_Cancel_Click;
        Button_Add.Click += Button_Add_Click;

        Expander_A.ContentTransition = new CrossFade(TimeSpan.FromMilliseconds(100));
        Expander_D.ContentTransition = new CrossFade(TimeSpan.FromMilliseconds(100));
        Expander_S.ContentTransition = new CrossFade(TimeSpan.FromMilliseconds(100));

        Closed += UserWindow_Closed;
        Opened += UserWindow_Opened;

        ComboBox_UserType.SelectionChanged += UserType_SelectionChanged;
        ComboBox_UserType.Items = UserBinding.GetUserTypes();

        Load();
    }

    private void DataGrid_User_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        var item = DataGrid_User.SelectedItem as UserObj;
        if (item == null)
        {
            return;
        }

        GuiConfigUtils.Config.LastUser = new()
        {
            Type = item.AuthType,
            UUID = item.UUID
        };

        GuiConfigUtils.Save();
        MainWindow.OnUserEdit();

        Info2.Show("�л��ɹ�");
        Load();
    }

    private async void Button_Add_Click(object? sender, RoutedEventArgs e)
    {
        Button_Add.IsEnabled = false;
        switch (ComboBox_UserType.SelectedIndex)
        {
            case 0:
                string name = TextBox_Input1.Text;
                if (string.IsNullOrWhiteSpace(name))
                {
                    Info.Show("�������Ҫ��Ϣ");
                    break;
                }
                var res = await UserBinding.AddUser(0, name, null);
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                }
                Info2.Show("���ӳɹ�");
                TextBox_Input1.Text = "";
                break;
            case 1:
                CoreMain.LoginOAuthCode = LoginOAuthCode;
                Info1.Show("���ڼ���");
                res = await UserBinding.AddUser(1, null);
                Info3.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                }
                Info2.Show("���ӳɹ�");
                TextBox_Input1.Text = "";
                break;
            case 2:
                var server = TextBox_Input1.Text;
                if (server.Length != 32)
                {
                    Info.Show("������UUID����");
                    break;
                }
                await Info3.Show("�˻�", "����", false);
                Info3.Close();
                var user = Info3.Read();
                if (string.IsNullOrWhiteSpace(user.Item1) || string.IsNullOrWhiteSpace(user.Item2))
                {
                    Info.Show("�������Ҫ��Ϣ");
                    break;
                }
                Info1.Show("���ڵ�¼");
                res = await UserBinding.AddUser(2, server, user.Item1, user.Item2);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show("���ӳɹ�");
                TextBox_Input1.Text = "";
                break;
            case 3:
                server = TextBox_Input1.Text;
                if (string.IsNullOrWhiteSpace(server))
                {
                    Info.Show("������UUID����");
                    break;
                }
                await Info3.Show("�˻�", "����", false);
                Info3.Close();
                user = Info3.Read();
                if (string.IsNullOrWhiteSpace(user.Item1) || string.IsNullOrWhiteSpace(user.Item2))
                {
                    Info.Show("�������Ҫ��Ϣ");
                    break;
                }
                Info1.Show("���ڵ�¼");
                res = await UserBinding.AddUser(3, server, user.Item1, user.Item2);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show("���ӳɹ�");
                TextBox_Input1.Text = "";
                break;
            case 4:
                await Info3.Show("�˻�", "����", false);
                Info3.Close();
                user = Info3.Read();
                if (string.IsNullOrWhiteSpace(user.Item1) || string.IsNullOrWhiteSpace(user.Item2))
                {
                    Info.Show("�������Ҫ��Ϣ");
                    break;
                }
                Info1.Show("���ڵ�¼");
                res = await UserBinding.AddUser(4, user.Item1, user.Item2);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show("���ӳɹ�");
                break;
            case 5:
                server = TextBox_Input1.Text;
                if (string.IsNullOrWhiteSpace(server))
                {
                    Info.Show("������UUID����");
                    break;
                }
                await Info3.Show("�˻�", "����", false);
                Info3.Close();
                user = Info3.Read();
                if (string.IsNullOrWhiteSpace(user.Item1) || string.IsNullOrWhiteSpace(user.Item2))
                {
                    Info.Show("�������Ҫ��Ϣ");
                    break;
                }
                Info1.Show("���ڵ�¼");
                res = await UserBinding.AddUser(3, server, user.Item1, user.Item2);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show("���ӳɹ�");
                TextBox_Input1.Text = "";
                break;
            default:
                Info.Show("��ѡ������");
                break;
        }
        Load();
        await CrossFade.Start(Grid_Add, null, CancellationToken.None);
        Button_Add.IsEnabled = true;
    }

    private void LoginOAuthCode(string url, string code)
    {
        Info1.Close();
        Info3.Show("����ַ" + url, "�������:" + code);
    }

    private void UserType_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        switch (ComboBox_UserType.SelectedIndex)
        {
            case 0:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = "�û���";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = false;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = false;
                TextBox_Input3.Text = "";
                break;
            case 1:
                TextBox_Input1.IsEnabled = false;
                TextBox_Input1.Watermark = "";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = false;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = false;
                TextBox_Input3.Text = "";
                break;
            case 2:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = "������UUID";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
            case 3:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = "��������ַ";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
            case 4:
                TextBox_Input1.IsEnabled = false;
                TextBox_Input1.Watermark = "";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
            case 5:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = "Ƥ��վ��ַ";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
        }
    }

    private void Button_Cancel_Click(object? sender, RoutedEventArgs e)
    {
        CrossFade.Start(Grid_Add, null, CancellationToken.None);
    }

    private void Button_D1_Click(object? sender, RoutedEventArgs e)
    {
        var item = DataGrid_User.SelectedItem as UserObj;
        if (item == null)
        {
            Info.Show("����ѡ���˻�");
            return;
        }

        if (item.Use)
        {
            GuiConfigUtils.Config.LastUser = null;
            GuiConfigUtils.Save();
        }

        UserBinding.Remove(item.UUID, item.AuthType);
        MainWindow.OnUserEdit();
        Load();
    }

    private void Button_S1_Click(object? sender, RoutedEventArgs e)
    {
        var item = DataGrid_User.SelectedItem as UserObj;
        if (item == null)
        {
            Info.Show("����ѡ���˻�");
            return;
        }

        GuiConfigUtils.Config.LastUser = new()
        {
            Type = item.AuthType,
            UUID = item.UUID
        };

        GuiConfigUtils.Save();
        MainWindow.OnUserEdit();

        Info2.Show("�л��ɹ�");
        Load();
    }

    private void Button_A1_Click(object? sender, RoutedEventArgs e)
    {
        SetType(1);
    }

    private void Button_S1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_S.IsExpanded = false;
    }

    private void Button_S_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_S.IsExpanded = true;
    }

    private void Button_D1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_D.IsExpanded = false;
    }

    private void Button_D_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_D.IsExpanded = true;
    }

    private void Button_A1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_A.IsExpanded = false;
    }

    private void Button_A_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_A.IsExpanded = true;
    }

    private void UserWindow_Opened(object? sender, EventArgs e)
    {
        DataGrid_User.MakeTran();
        Expander_A.MakePadingNull();
        Expander_D.MakePadingNull();
        Expander_S.MakePadingNull();
    }

    private void UserWindow_Closed(object? sender, EventArgs e)
    {
        App.UserWindow = null;
        CoreMain.LoginOAuthCode = null;
    }

    public void SetType(int type)
    {
        switch (type)
        {
            case 1:
                CrossFade.Start(null, Grid_Add, CancellationToken.None);
                break;
        }
    }

    private void Load()
    {
        var item1 = UserBinding.GetLastUser();
        List.Clear();
        foreach (var item in UserBinding.GetAllUser())
        {
            List.Add(new()
            {
                Name = item.Value.UserName,
                UUID = item.Key.Item1,
                AuthType = item.Key.Item2,
                Type = item.Key.Item2.GetName(),
                Text1 = item.Value.Text1,
                Text2 = item.Value.Text2,
                Use = item1 == item.Value
            });
        }
    }
}