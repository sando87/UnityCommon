using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// Timeline에서 사용하는 Signal방식에서 파라미터를 전달할수 있도록 해주는 스크립트
public class SignalReceiverWithFloat : MonoBehaviour, INotificationReceiver
{
    public SignalAssetEventPair[] signalAssetEventPairs;

    [Serializable]
    public class SignalAssetEventPair
    {
        public SignalAsset signalAsset;
        public ParameterizedEvent events;

        [Serializable]
        public class ParameterizedEvent : UnityEvent<float> { }
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is SignalEmitterWithFloat floatEmitter)
        {
            var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, floatEmitter.asset));
            foreach (var m in matches)
            {
                m.events.Invoke(floatEmitter.parameter);
            }
        }
    }
}
