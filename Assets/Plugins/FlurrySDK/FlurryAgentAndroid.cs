/*
 * Copyright 2018, Oath Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
namespace FlurrySDKInternal
{
    public class FlurryAgentAndroid : FlurryAgent
    {
        // Add android.permission.ACCESS_NETWORK_STATE
        public static NetworkReachability internetReachability = Application.internetReachability;

        private static readonly string ORIGIN_NAME = "unity-flurry-sdk";
        private static readonly string ORIGIN_VERSION = "1.1.0";

        private static AndroidJavaClass cls_FlurryAgent = new AndroidJavaClass("com.flurry.android.FlurryAgent");
        private static AndroidJavaClass cls_FlurryAgentConstants = new AndroidJavaClass("com.flurry.android.Constants");

        public class AgentBuilderAndroid : AgentBuilder
        {
            private AndroidJavaObject obj_FlurryAgentBuilder = new AndroidJavaObject("com.flurry.android.FlurryAgent$Builder");

            public override void Build(string apiKey)
            {
                using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        cls_FlurryAgent.CallStatic("addOrigin", ORIGIN_NAME, ORIGIN_VERSION);
                        obj_FlurryAgentBuilder.Call("build", obj_Activity, apiKey);
                    }
                }
            }

            public override void WithCrashReporting(bool crashReporting)
            {
                obj_FlurryAgentBuilder.Call<AndroidJavaObject>("withCaptureUncaughtExceptions", crashReporting);
            }

            public override void WithContinueSessionMillis(long sessionMillis)
            {
                obj_FlurryAgentBuilder.Call<AndroidJavaObject>("withContinueSessionMillis", sessionMillis);
            }

            public override void WithIncludeBackgroundSessionsInMetrics(bool includeBackgroundSessionsInMetrics)
            {
                obj_FlurryAgentBuilder.Call<AndroidJavaObject>("withIncludeBackgroundSessionsInMetrics", includeBackgroundSessionsInMetrics);
            }

            public override void WithLogEnabled(bool enableLog)
            {
                obj_FlurryAgentBuilder.Call<AndroidJavaObject>("withLogEnabled", enableLog);
            }

            public override void WithLogLevel(FlurrySDK.Flurry.LogLevel logLevel)
            {
                obj_FlurryAgentBuilder.Call<AndroidJavaObject>("withLogLevel", (int) logLevel);
            }
        }

        public override void SetAge(int age)
        {
            cls_FlurryAgent.CallStatic("setAge", age);
        }

        public override void SetGender(FlurrySDK.Flurry.Gender gender)
        {
            byte flurryGender = (gender == FlurrySDK.Flurry.Gender.Male
                                 ? cls_FlurryAgentConstants.GetStatic<byte>("MALE")
                                 : cls_FlurryAgentConstants.GetStatic<byte>("FEMALE"));
            cls_FlurryAgent.CallStatic("setGender", flurryGender);
        }

        public override void SetReportLocation(bool reportLocation)
        {
            cls_FlurryAgent.CallStatic("setReportLocation", reportLocation);
        }

        public override void SetSessionOrigin(string originName, string deepLink)
        {
            cls_FlurryAgent.CallStatic("setSessionOrigin", originName, deepLink);
        }

        public override void SetUserId(string userId)
        {
            cls_FlurryAgent.CallStatic("setUserId", userId);
        }

        public override void SetVersionName(string versionName)
        {
            cls_FlurryAgent.CallStatic("setVersionName", versionName);
        }

        public override void AddOrigin(string originName, string originVersion)
        {
            cls_FlurryAgent.CallStatic("addOrigin", originName, originVersion);
        }

        public override void AddOrigin(string originName, string originVersion, IDictionary<string, string> originParameters)
        {
            cls_FlurryAgent.CallStatic("addOrigin", originName, originVersion, ConvertMap(originParameters));
        }

        public override void AddSessionProperty(string name, string value)
        {
            cls_FlurryAgent.CallStatic("addSessionProperty", name, value);
        }

        public override int GetAgentVersion()
        {
            return cls_FlurryAgent.CallStatic<int>("getAgentVersion");
        }

        public override string GetReleaseVersion()
        {
            return cls_FlurryAgent.CallStatic<string>("getReleaseVersion");
        }

        public override string GetSessionId()
        {
            return cls_FlurryAgent.CallStatic<string>("getSessionId");
        }

        public override int LogEvent(string eventId)
        {
            AndroidJavaObject result = cls_FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventId);
            return result.Call<int>("ordinal");
        }

        public override int LogEvent(string eventId, bool timed)
        {
            AndroidJavaObject result = cls_FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventId, timed);
            return result.Call<int>("ordinal");
        }

        public override int LogEvent(string eventId, IDictionary<string, string> parameters)
        {
            AndroidJavaObject result = cls_FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventId, ConvertMap(parameters));
            return result.Call<int>("ordinal");
        }

        public override int LogEvent(string eventId, IDictionary<string, string> parameters, bool timed)
        {
            AndroidJavaObject result = cls_FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventId, ConvertMap(parameters), timed);
            return result.Call<int>("ordinal");
        }

        public override void EndTimedEvent(string eventId)
        {
            cls_FlurryAgent.CallStatic("endTimedEvent", eventId);
        }

        public override void EndTimedEvent(string eventId, IDictionary<string, string> parameters)
        {
            cls_FlurryAgent.CallStatic("endTimedEvent", eventId, ConvertMap(parameters));
        }

        public override void OnPageView()
        {
            cls_FlurryAgent.CallStatic("onPageView");
        }

        public override void OnError(string errorId, string message, string errorClass)
        {
            cls_FlurryAgent.CallStatic("onError", errorId, message, errorClass);
        }

        public override void OnError(string errorId, string message, string errorClass, IDictionary<string, string> parameters)
        {
            cls_FlurryAgent.CallStatic("onError", errorId, message, errorClass, ConvertMap(parameters));
        }

        public override void LogBreadcrumb(string crashBreadcrumb)
        {
            cls_FlurryAgent.CallStatic("logBreadcrumb", crashBreadcrumb);
        }

        public override int LogPayment(string productName, string productId, int quantity, double price,
                                       string currency, string transactionId, IDictionary<string, string> parameters)
        {
            AndroidJavaObject result = cls_FlurryAgent.CallStatic<AndroidJavaObject>("logPayment", productName, productId,
                                                                                     quantity, price, currency, transactionId,
                                                                                     ConvertMap(parameters));
            return result.Call<int>("ordinal");
        }

        public override void SetIAPReportingEnabled(bool enableIAP)
        {
            Debug.Log("setIAPReportingEnabled is not supported on Android. Please use LogPayment instead.");
        }

        private static AndroidJavaObject ConvertMap(IDictionary<string, string> dictionary)
        {
            AndroidJavaObject obj_HashMap = new AndroidJavaObject("java.util.HashMap");
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    obj_HashMap.Call<string>("put", pair.Key, pair.Value);
                }
            }

            return obj_HashMap;
        }

        public override void Dispose()
        {
            cls_FlurryAgent.Dispose();
        }

    };
}
#endif
