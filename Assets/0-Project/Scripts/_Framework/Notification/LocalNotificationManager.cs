using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//#if UNITY_ANDROID
//using Unity.Notifications.Android;
//#elif UNITY_IOS
//using Unity.Notifications.iOS;
//#endif

public class LocalNotificationManager : MonoBehaviour
{
    //Kullanmak icin Package Managerdan Mobile Notifications paketi dahil edilip Project settingsten ayarlari yapilmali
    /*
    public static LocalNotificationManager Instance;

    private enum TimeType
    {
        Seconds = 0,
        Minutes = 1,
        Hours = 2,
        Days = 3
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }

    private int ScheduledNotificationCount = 0;
    private void CreateNotification(string title, string body, int afterTimes, TimeType timeType) //Kac sn/dk/saat/gün sonra ciksin?
    {
#if UNITY_ANDROID
        string channelID = "channel_id" + (UnityEngine.Random.Range(int.MinValue, int.MaxValue)).ToString();
        var c = new AndroidNotificationChannel()
        {
            Id = channelID,
            Name = Application.productName,
            Importance = Importance.High,
            Description = Application.productName,
            EnableLights = true,
            LockScreenVisibility = LockScreenVisibility.Public,
            EnableVibration = true,
            CanShowBadge = true,
            CanBypassDnd = true
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);

        var fireTime = new DateTime();
        if (timeType == TimeType.Seconds)
        {
            fireTime = DateTime.Now.AddSeconds(afterTimes);
        }
        else if (timeType == TimeType.Minutes)
        {
            fireTime = DateTime.Now.AddMinutes(afterTimes);
        }
        else if (timeType == TimeType.Hours)
        {
            fireTime = DateTime.Now.AddHours(afterTimes);
        }
        else if (timeType == TimeType.Days)
        {
            fireTime = DateTime.Now.AddDays(afterTimes);
        }

        var notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = fireTime,
            ShouldAutoCancel = true,
            Style = NotificationStyle.None,
            UsesStopwatch = false
        };
        int notificationID = AndroidNotificationCenter.SendNotification(notification, channelID);
        string str = PlayerPrefs.GetString("LocalNotifications", "");
        str += notificationID.ToString() + ",";
        PlayerPrefs.SetString("LocalNotifications", str);
#elif UNITY_IOS
        TimeSpan timeSpan = new TimeSpan();
        if (timeType == TimeType.Seconds)
        {
            timeSpan = new TimeSpan(0, 0, 0, afterTimes);
        }
        else if (timeType == TimeType.Minutes)
        {
            timeSpan = new TimeSpan(0, 0, afterTimes, 0);
        }
        else if (timeType == TimeType.Hours)
        {
            timeSpan = new TimeSpan(0, afterTimes, 0, 0);
        }
        else if (timeType == TimeType.Days)
        {
            timeSpan = new TimeSpan(afterTimes, 0, 0, 0);
        }

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = timeSpan,
            Repeats = false
        };
        ScheduledNotificationCount += 1;
        var notification = new iOSNotification()
        {
            Identifier = "_notification_01" + (UnityEngine.Random.Range(-999999, 999999)).ToString(),
            Title = title,
            Body = body,
            Subtitle = "",
            ShowInForeground = true,

            ForegroundPresentationOption = PresentationOption.Alert,
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
            Badge = ScheduledNotificationCount
        };
        iOSNotificationCenter.ScheduleNotification(notification);
        string str = PlayerPrefs.GetString("LocalNotifications", "");
        str += notification.Identifier + ",";
        PlayerPrefs.SetString("LocalNotifications", str);
#endif


    }
    
    //public static bool ParseInt(this string str)
    //{
    //    int result = 0;
    //    return int.TryParse(str, out result);
    //}
    

    public void CancelAllNotifications()
    {
        ScheduledNotificationCount = 0;

#if UNITY_ANDROID
        List<string> ids = new List<string>();
        Debug.Log("Vita Games : " + PlayerPrefs.GetString("LocalNotifications", ""));
        if (PlayerPrefs.GetString("LocalNotifications", "").Length > 1)
        {
            ids = PlayerPrefs.GetString("LocalNotifications", "").Split(',').ToList();
            ids = ids.Where(x => Extensions.ParseInt(x) == true).ToList();
            ids.ForEach(x =>
            {
                int _i = int.Parse(x);
                AndroidNotificationCenter.CancelNotification(_i);
                AndroidNotificationCenter.CancelScheduledNotification(_i);
                AndroidNotificationCenter.CancelDisplayedNotification(_i);
            });
        }
        PlayerPrefs.SetString("LocalNotifications", "");

        AndroidNotificationCenter.CancelAllNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        foreach (var _channel in AndroidNotificationCenter.GetNotificationChannels())
        {
            AndroidNotificationCenter.DeleteNotificationChannel(_channel.Id);
        }
#elif UNITY_IOS
        List<string> ids = new List<string>();
        if (PlayerPrefs.GetString("LocalNotifications", "").Length > 1)
        {
            ids = PlayerPrefs.GetString("LocalNotifications", "").Split(',').ToList();
            ids = ids.Where(x => x.Length > 0).ToList();
            ids.ForEach(x =>
            {
                iOSNotificationCenter.RemoveDeliveredNotification(x);
                iOSNotificationCenter.RemoveScheduledNotification(x);
            });
        }
        PlayerPrefs.SetString("LocalNotifications", "");

        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        Unity.Notifications.iOS.iOSNotificationCenter.ApplicationBadge = 0;
#endif

    }

    public void OnApplicationQuit()
    {
        CancelAllNotifications();
        GiveLocalNotificationOrder();
    }
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {//Durdur
            CancelAllNotifications();
            GiveLocalNotificationOrder();
        }
        else
        {//Başlat
            CancelAllNotifications();
        }
    }

    
    private void GiveLocalNotificationOrder()
    {//Local bildirim gönderme emri ver
        CreateNotification(
            Application.productName, "Shall we continue playing? :)", 12, TimeType.Hours);

        //Kullanıcı 1 gün boyunca oyuna giriş yapmamışsa
           CreateNotification(
               Application.productName, "Do you want to play again?", 1, TimeType.Days);

        //Kullanıcı 3 gün boyunca oyuna giriş yapmamışsa
        CreateNotification(
            Application.productName, "We urgently need to get together again!", 3, TimeType.Days);

        //Kullanıcı 7 gün boyunca oyuna giriş yapmamışsa
        CreateNotification(
            Application.productName,"I miss you!", 7, TimeType.Days);

        //Kullanıcı 14 gün boyunca oyuna giriş yapmamışsa
        CreateNotification(
            Application.productName, "Hey, where are you? Shall we have some fun?", 14, TimeType.Days);

        //Kullanıcı 30 gün boyunca oyuna giriş yapmamışsa
        CreateNotification(
            Application.productName, "Don't leave me so lonely :(", 30, TimeType.Days);
    }
    */
}

/*
public static class Extensions
{
    public static bool ParseInt(this string str)
    {
        int result = 0;
        return int.TryParse(str, out result);
    }
}
*/