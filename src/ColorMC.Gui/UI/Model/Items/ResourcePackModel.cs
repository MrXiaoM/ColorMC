using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Flyouts;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ColorMC.Gui.UI.Model.GameEdit;

public partial class ResourcePackModel : ObservableObject
{
    public ResourcepackDisplayObj Pack { get; }

    [ObservableProperty]
    private bool _isSelect;

    private readonly ILoadFuntion<ResourcePackModel> _top;
    private readonly IUserControl _con;

    public string Local => Pack.Local;
    public string PackFormat => Pack.PackFormat.ToString();
    public string Description => Pack.Description;
    public string Broken => Pack.Pack.Broken ?
            App.GetLanguage("GameEditWindow.Tab8.Info4") : "";

    public Bitmap Pic => Pack.Icon ?? App.GameIcon;

    public ResourcePackModel(IUserControl con, ILoadFuntion<ResourcePackModel> top, ResourcepackDisplayObj pack)
    {
        _con = con;
        _top = top;
        Pack = pack;
    }

    public void Select()
    {
        _top.SetSelect(this);
    }

    public void Flyout(Control con)
    {
        _ = new GameEditFlyout3(con, this, Pack);
    }

    public async void Delete(ResourcepackDisplayObj obj)
    {
        var window = _con.Window;
        var res = await window.OkInfo.ShowWait(
            string.Format(App.GetLanguage("GameEditWindow.Tab8.Info1"), obj.Local));
        if (!res)
        {
            return;
        }

        GameBinding.DeleteResourcepack(obj.Pack);
        window.NotifyInfo.Show(App.GetLanguage("GameEditWindow.Tab4.Info3"));
        await _top.Load();
    }
}