using System.Runtime.InteropServices;


Console.WriteLine("=== GUI Example ===");
IGUIFactory guiFactory = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? new WindowsFactory()
    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? new LinuxFactory()
        : new MacFactory();

new GUIApplication(guiFactory).BuildUI();

Console.WriteLine("\n=== Notification Example (production) ===");
var prodService = new OrderService(new SmtpNotificationFactory());
prodService.PlaceOrder(new Order("user@example.com", "+380501234567"));

Console.WriteLine("\n=== Notification Example (fake / test) ===");
var testService = new OrderService(new FakeNotificationFactory());
testService.PlaceOrder(new Order("user@example.com", "+380501234567"));


interface IButton   { void Render(); }
interface ICheckbox { void Toggle(); }

interface IGUIFactory
{
    IButton   CreateButton();
    ICheckbox CreateCheckbox();
}

class WinButton   : IButton   { public void Render() => Console.WriteLine("[ Win Button ]");    }
class WinCheckbox : ICheckbox { public void Toggle() => Console.WriteLine("[x] Win Checkbox");  }

class WindowsFactory : IGUIFactory
{
    public IButton   CreateButton()   => new WinButton();
    public ICheckbox CreateCheckbox() => new WinCheckbox();
}


class MacButton   : IButton   { public void Render() => Console.WriteLine("( Mac Button )");    }
class MacCheckbox : ICheckbox { public void Toggle() => Console.WriteLine("( ) Mac Checkbox");  }

class MacFactory : IGUIFactory
{
    public IButton   CreateButton()   => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
}


class LinuxButton   : IButton   { public void Render() => Console.WriteLine("[ Linux Button ]");   }
class LinuxCheckbox : ICheckbox { public void Toggle() => Console.WriteLine("[x] Linux Checkbox"); }

class LinuxFactory : IGUIFactory
{
    public IButton   CreateButton()   => new LinuxButton();
    public ICheckbox CreateCheckbox() => new LinuxCheckbox();
}


class GUIApplication
{
    private readonly IGUIFactory _factory;

    public GUIApplication(IGUIFactory factory) => _factory = factory;

    public void BuildUI()
    {
        _factory.CreateButton().Render();
        _factory.CreateCheckbox().Toggle();
    }
}


interface IEmailSender { void Send(string to,    string body); }
interface ISmsSender   { void Send(string phone, string body); }

interface INotificationFactory
{
    IEmailSender CreateEmailSender();
    ISmsSender   CreateSmsSender();
}


class SmtpEmailSender : IEmailSender
{
    public void Send(string to, string body) =>
        Console.WriteLine($"[SMTP]   → {to}: {body}");
}

class TwilioSmsSender : ISmsSender
{
    public void Send(string phone, string body) =>
        Console.WriteLine($"[Twilio] → {phone}: {body}");
}

class SmtpNotificationFactory : INotificationFactory
{
    public IEmailSender CreateEmailSender() => new SmtpEmailSender();
    public ISmsSender   CreateSmsSender()   => new TwilioSmsSender();
}


class FakeEmailSender : IEmailSender
{
    public void Send(string to, string body) =>
        Console.WriteLine($"[log:email] → {to}: {body}");
}

class FakeSmsSender : ISmsSender
{
    public void Send(string phone, string body) =>
        Console.WriteLine($"[log:sms]   → {phone}: {body}");
}

class FakeNotificationFactory : INotificationFactory
{
    public IEmailSender CreateEmailSender() => new FakeEmailSender();
    public ISmsSender   CreateSmsSender()   => new FakeSmsSender();
}

record Order(string Email, string Phone);

class OrderService
{
    private readonly INotificationFactory _factory;

    public OrderService(INotificationFactory factory) => _factory = factory;

    public void PlaceOrder(Order order)
    {
        _factory.CreateEmailSender().Send(order.Email, "Замовлення прийнято");
        _factory.CreateSmsSender()  .Send(order.Phone, "SMS підтвердження");
    }
}
