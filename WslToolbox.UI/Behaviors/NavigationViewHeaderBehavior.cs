using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Xaml.Interactivity;
using WslToolbox.UI.Contracts.Services;

namespace WslToolbox.UI.Behaviors;

public class NavigationViewHeaderBehavior : Behavior<NavigationView>
{
    private static NavigationViewHeaderBehavior? _current;

    public static readonly DependencyProperty DefaultHeaderProperty =
        DependencyProperty.Register(nameof(DefaultHeader), typeof(object), typeof(NavigationViewHeaderBehavior), new(null, (d, e) => _current!.UpdateHeader()));

    private static readonly DependencyProperty HeaderModeProperty =
        DependencyProperty.RegisterAttached("HeaderMode", typeof(bool), typeof(NavigationViewHeaderBehavior), defaultMetadata: new PropertyMetadata(NavigationViewHeaderMode.Always, (d, e) => _current!.UpdateHeader()));

    private static readonly DependencyProperty HeaderContextProperty =
        DependencyProperty.RegisterAttached("HeaderContext", typeof(object), typeof(NavigationViewHeaderBehavior), new(null, (d, e) => _current!.UpdateHeader()));

    private static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(NavigationViewHeaderBehavior), new(null, (d, e) => _current!.UpdateHeaderTemplate()));

    private Page? _currentPage;

    public DataTemplate? DefaultHeaderTemplate { get; set; }

    public object DefaultHeader
    {
        get => GetValue(DefaultHeaderProperty);
        set => SetValue(DefaultHeaderProperty, value);
    }

    public static NavigationViewHeaderMode GetHeaderMode(Page item)
    {
        return (NavigationViewHeaderMode) item.GetValue(HeaderModeProperty);
    }

    public static void SetHeaderMode(Page item, NavigationViewHeaderMode value)
    {
        item.SetValue(dp: HeaderModeProperty, value);
    }

    public static object GetHeaderContext(Page item)
    {
        return item.GetValue(HeaderContextProperty);
    }

    public static void SetHeaderContext(Page item, object value)
    {
        item.SetValue(HeaderContextProperty, value);
    }

    private static DataTemplate GetHeaderTemplate(DependencyObject item)
    {
        return (DataTemplate) item.GetValue(HeaderTemplateProperty);
    }

    public static void SetHeaderTemplate(Page item, DataTemplate value)
    {
        item.SetValue(HeaderTemplateProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        var navigationService = App.GetService<INavigationService>();
        navigationService.Navigated += (o, args) => OnNavigated(o, args);

        _current = this;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        var navigationService = App.GetService<INavigationService>();
        // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
        navigationService.Navigated -= (o, args) => navigationServiceOnNavigated(o, args);
    }

    private void navigationServiceOnNavigated(object o, NavigationEventArgs args)
    {
        OnNavigated(o, args);
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is not Frame frame || frame.Content is not Page page) return;
        _currentPage = page;

        UpdateHeader();
        UpdateHeaderTemplate();
    }

    private void UpdateHeader()
    {
        if (_currentPage == null) return;
        var headerMode = GetHeaderMode(_currentPage);
        if (headerMode == NavigationViewHeaderMode.Never)
        {
            AssociatedObject.Header = null;
            AssociatedObject.AlwaysShowHeader = false;
        }
        else
        {
            var headerFromPage = GetHeaderContext(_currentPage);
            AssociatedObject.Header = headerFromPage;

            if (headerMode == NavigationViewHeaderMode.Always)
            {
                AssociatedObject.AlwaysShowHeader = true;
            }
            else
            {
                AssociatedObject.AlwaysShowHeader = false;
            }
        }
    }

    private void UpdateHeaderTemplate()
    {
        if (_currentPage == null) return;
        var headerTemplate = GetHeaderTemplate(_currentPage);
        AssociatedObject.HeaderTemplate = headerTemplate ?? DefaultHeaderTemplate;
    }
}