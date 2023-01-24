namespace RainyDay.UI
{
    public class MessageBoxButton : ButtonElement<MessageResponse>
    {
        protected override string GetText() => Parameter.Text;
    }
}
