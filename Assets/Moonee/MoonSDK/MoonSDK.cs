using com.adjust.sdk;
using Firebase.Analytics;
using GameAnalyticsSDK;
using Moonee.MoonSDK.Internal;
using Moonee.MoonSDK.Internal.RateUs;
using Moonee.MoonSDK.Internal.Advertisement;
using System;
using System.Collections.Generic;
using UnityEngine;


public static class MoonSDK
{
    public const string Version = "1.1.8";
    public static string token = "";

    public static void TrackCustomEvent(string eventName, AnalyticsProvider analyticsProviders = AnalyticsProvider.Firebase)
    {
        if (analyticsProviders == AnalyticsProvider.Firebase)
            FirebaseAnalytics.LogEvent(eventName);
        else if (analyticsProviders == AnalyticsProvider.GameAnalytics)
            GameAnalytics.NewDesignEvent(eventName);
    }
    public static void TrackLevelEvents(LevelEvents eventType, int levelIndex)
    {
        string outputValue = "level" + String.Format("{0:D4}", levelIndex);

        if (eventType == LevelEvents.Start)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, outputValue);
        }
        else if (eventType == LevelEvents.Fail)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, outputValue);
        }
        else if (eventType == LevelEvents.Complete)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, outputValue);
        }
    }
    public static void TrackAdjustRevenueEvent(double price)
    {
        MoonSDKSettings settings = MoonSDKSettings.Load();

        AdjustEvent adjustEvent = new AdjustEvent(settings.adjustIAPRevenueToken);
        adjustEvent.setRevenue(price, "USD");
        Adjust.trackEvent(adjustEvent);
    }
    public static void TrackAdjustRevenueEvent(double price, string transactionID)
    {
        MoonSDKSettings settings = MoonSDKSettings.Load();

        AdjustEvent adjustEvent = new AdjustEvent(settings.adjustIAPRevenueToken);
        adjustEvent.setRevenue(price, "USD");
        adjustEvent.setTransactionId(transactionID);
        Adjust.trackEvent(adjustEvent);
    }
    public static void OpenRateUsScreen()
    {
        RateUsView rateUsViewPrefab = Resources.Load<RateUsView>("MoonSDK/Views/RateUsView");
        RateUsView rateUsView = AdvertisementManager.Instantiate(rateUsViewPrefab, Vector3.zero, Quaternion.identity);
    }
    public static string UpdateAdjustToken(Moonee.MoonSDK.Internal.MoonSDKSettings settings)
    {
        token = settings.adjustToken;
        return token;
    }
    public static string getToken()
    {
        MoonSDKSettings settings = MoonSDKSettings.Load();
        return settings.adjustToken.Replace(" ", string.Empty);
    }
    public enum AnalyticsProvider
    {
        Firebase,
        GameAnalytics
    }
    public enum LevelEvents
    {
        Start, 
        Fail,
        Complete
    }
}