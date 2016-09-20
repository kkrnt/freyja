using System;
using System.Management.Automation;

class Toast
{
    static void Main(string[] args)
    {
        //コマンドライン引数かパイプからテキストを受け取る
        var text = string.Join(" ", args);
        if (text.Equals(""))
        {
            using (var input = Console.In)
            {
                text = input.ReadToEnd();
            }
        }

        //コマンドライン引数をトースト通知として表示します
        var code = new string[]
        {
            "$ErrorActionPreference = \"Stop\"",
            "$notificationTitle = \"" + text.Replace("\"", "") + "\"",
            "[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > $null",
            "$template = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::ToastText02)",
            "$toastXml = [xml] $template.GetXml()",
            "$toastXml.GetElementsByTagName(\"text\").AppendChild($toastXml.CreateTextNode($notificationTitle)) > $null",
            "$xml = New-Object Windows.Data.Xml.Dom.XmlDocument",
            "$xml.LoadXml($toastXml.OuterXml)",
            "$toast = [Windows.UI.Notifications.ToastNotification]::new($xml)",
            "$toast.Tag = \"PowerShell\"",
            "$toast.Group = \"PowerShell\"",
            "$toast.ExpirationTime = [DateTimeOffset]::Now.AddMinutes(5)",
            "$notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier(\"PowerShell\")",
            "$notifier.Show($toast);"
        };

        using (var invoker = new RunspaceInvoke())
        {
            invoker.Invoke(string.Join("\n", code), new object[] { });
        }
    }
}
