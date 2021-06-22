public class SystemEvents : GameEvent
{
    public static void GPGSLogin(bool authenticated)
    {
        Events.instance.Raise(new EventGPGSLogin(authenticated));
    }
}

//Вызывается когда обновляется выбранный язык
public class EventSetLang : SystemEvents
{
}

//Вызывается когда обновляется выбранный язык
public class EventGPGSLogin : SystemEvents
{
    public bool authenticated;
    public EventGPGSLogin(bool authenticated)
    {
        this.authenticated = authenticated;
    }
}