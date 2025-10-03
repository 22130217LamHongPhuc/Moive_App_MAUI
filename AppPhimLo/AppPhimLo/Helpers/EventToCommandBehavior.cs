using System.Globalization;
using System.Windows.Input;

namespace AppPhimLo.Helpers;

public class EventToCommandBehavior : Behavior<View>
{
    public static readonly BindableProperty EventNameProperty =
        BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior), null);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior), null);

    public string EventName { get; set; }
    public ICommand Command { get; set; }

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        if (!string.IsNullOrWhiteSpace(EventName))
        {
            var eventInfo = bindable.GetType().GetEvent(EventName);
            if (eventInfo != null)
            {
                var methodInfo = typeof(EventToCommandBehavior).GetMethod(nameof(OnEvent),
                                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
                eventInfo.AddEventHandler(bindable, handler);
            }
        }
    }
    public class GreaterThanOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (int)value > 1;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // Converter: kiểm tra CurrentPage < TotalPages
    public class LessThanTotalPagesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentPage && parameter is int totalPages)
                return currentPage < totalPages;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    void OnEvent(object sender, EventArgs e)
    {
        if (Command?.CanExecute(e) == true)
            Command.Execute(e);
    }
}
