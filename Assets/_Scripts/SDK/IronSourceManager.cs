using UnityEngine;

namespace _Scripts.SDK
{
    public class IronSourceManager : MonoBehaviour
    {
        private void OnEnable()
        {
            //Add Init Event
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        }

        public void Start()
        {
#if UNITY_ANDROID
            var appKey = "1c1cd058d";
#elif UNITY_IPHONE
        string appKey = "";
#else
        string appKey = "unexpected_platform";
#endif


            Debug.Log("unity-script: IronSource.Agent.validateIntegration");
            IronSource.Agent.validateIntegration();

            Debug.Log("unity-script: unity version" + IronSource.unityVersion());

            // SDK init
            Debug.Log("unity-script: IronSource.Agent.init");
            IronSource.Agent.init(appKey);
        }

        #region Init callback handlers

        private void SdkInitializationCompletedEvent()
        {
            Debug.Log("unity-script: I got SdkInitializationCompletedEvent");
        }

        private void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
        {
            Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
        }

        private void RewardedVideoAdOpenedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
        }

        private void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
        {
            Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() +
                      " name = " + ssp.getRewardName());
        }

        private void RewardedVideoAdClosedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
        }

        private void RewardedVideoAdStartedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
        }

        private void RewardedVideoAdEndedEvent()
        {
            Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
        }

        private void RewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() +
                      ", description : " + error.getDescription());
        }

        private void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
        {
            Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
        }

        /************* RewardedVideo DemandOnly Delegates *************/

        private void RewardedVideoAdLoadedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdLoadedDemandOnlyEvent for instance: " + instanceId);
        }

        private void RewardedVideoAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {
            Debug.Log("unity-script: I got RewardedVideoAdLoadFailedDemandOnlyEvent for instance: " + instanceId +
                      ", code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        private void RewardedVideoAdOpenedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdOpenedDemandOnlyEvent for instance: " + instanceId);
        }

        private void RewardedVideoAdRewardedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdRewardedDemandOnlyEvent for instance: " + instanceId);
        }

        private void RewardedVideoAdClosedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdClosedDemandOnlyEvent for instance: " + instanceId);
        }

        private void RewardedVideoAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {
            Debug.Log("unity-script: I got RewardedVideoAdShowFailedDemandOnlyEvent for instance: " + instanceId +
                      ", code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        private void RewardedVideoAdClickedDemandOnlyEvent(string instanceId)
        {
            Debug.Log("unity-script: I got RewardedVideoAdClickedDemandOnlyEvent for instance: " + instanceId);
        }

        #endregion
    }
}