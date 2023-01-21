using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using com.adjust.sdk;

namespace Moonee.MoonSDK.Internal.Analytics
{
    public class FirebaseManager : MonoBehaviour
    {
        public static System.Action OnRemoteConfigValuesReceived;
        void Start()
        {
            var settings = MoonSDKSettings.Load();

            if (settings.Firebase)
            {
                Initialize();
            }
            else Destroy(this);

            OnRemoteConfigValuesReceived += () =>
            {
                if (string.IsNullOrEmpty(settings.adjustTestEventToken) == false)
                {
                    string firebase_test = FirebaseRemoteConfig.DefaultInstance.GetValue("firebase_test").StringValue;
                    string test_name = FirebaseRemoteConfig.DefaultInstance.GetValue("test_name").StringValue;
                    string test_id = FirebaseRemoteConfig.DefaultInstance.GetValue("test_id").StringValue;
                    string test_parameter = FirebaseRemoteConfig.DefaultInstance.GetValue("test_parameter").StringValue;
                    bool is_baseline = FirebaseRemoteConfig.DefaultInstance.GetValue("is_baseline").BooleanValue;
                    double test_value = FirebaseRemoteConfig.DefaultInstance.GetValue("test_value").DoubleValue;

                    AdjustEvent adjustEvent = new AdjustEvent(settings.adjustTestEventToken);
                    adjustEvent.addCallbackParameter("firebase_test", firebase_test);
                    adjustEvent.addCallbackParameter("test_name", test_name);
                    adjustEvent.addCallbackParameter("test_id", test_id);
                    adjustEvent.addCallbackParameter("test_parameter", test_parameter);
                    adjustEvent.addCallbackParameter("is_baseline", is_baseline.ToString());
                    adjustEvent.addCallbackParameter("test_value", test_value.ToString());

                    Adjust.trackEvent(adjustEvent);

                    Debug.Log("Adjust Event Was Sent");
                };
            };
        }
        void Initialize()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    var app = Firebase.FirebaseApp.DefaultInstance;
                    var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    SetDefaultRemoteConfigValues();
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
        private void SetDefaultRemoteConfigValues()
        {
            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var settings = MoonSDKSettings.Load();

            remoteConfig.SetDefaultsAsync(new Dictionary<string, object>
            {
               {"int_grace_time", settings.int_grace_time},
               {"int_grace_level", settings.int_grace_level},
               {"cooldown_between_INTs", settings.cooldown_between_INTs},
               {"cooldown_after_RVs", settings.cooldown_after_RVs},
               {"Show_int_if_fail", false},
               {"INT_in_stage", false},
               {"firebase_test","firebase_test"},
               {"test_name","value"},
               {"test_id", "value"},
               {"test_parameter", "grace"},
               {"is_baseline", true},
               {"test_value", 120},

        }).ContinueWithOnMainThread(task =>
            {
                remoteConfig.FetchAndActivateAsync().ContinueWithOnMainThread(task =>
                {
                    RemoteConfigValues.int_grace_time = FirebaseRemoteConfig.DefaultInstance.GetValue("int_grace_time").DoubleValue;
                    RemoteConfigValues.int_grace_level = FirebaseRemoteConfig.DefaultInstance.GetValue("int_grace_level").LongValue;
                    RemoteConfigValues.cooldown_between_INTs = FirebaseRemoteConfig.DefaultInstance.GetValue("cooldown_between_INTs").DoubleValue;
                    RemoteConfigValues.cooldown_after_RVs = FirebaseRemoteConfig.DefaultInstance.GetValue("cooldown_after_RVs").DoubleValue;
                    RemoteConfigValues.Show_int_if_fail = FirebaseRemoteConfig.DefaultInstance.GetValue("Show_int_if_fail").BooleanValue;
                    RemoteConfigValues.INT_in_stage = FirebaseRemoteConfig.DefaultInstance.GetValue("INT_in_stage").BooleanValue;

                    OnRemoteConfigValuesReceived?.Invoke();
                    OnRemoteConfigValuesReceived = null;
                });
            });
        }
    }
}

