// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Unity.WebRTC;

namespace Sendbird.Calls
{
    internal static class EndpointTypeExtension
    {
        internal static string EnumToJsonPropertyName(this EndpointType inEndpointType)
        {
            return NewtonsoftJsonExtension.EnumToJsonPropertyName(inEndpointType);
        }

        internal static RTCRtpTransceiverDirection ToTransceiverDirection(this EndpointType inEndpointType)
        {
            switch (inEndpointType)
            {
                case EndpointType.SendReceive: return RTCRtpTransceiverDirection.SendRecv;
                case EndpointType.SendOnly:    return RTCRtpTransceiverDirection.SendOnly;
                case EndpointType.ReceiveOnly: return RTCRtpTransceiverDirection.RecvOnly;
                case EndpointType.None:        return RTCRtpTransceiverDirection.Inactive;
                default:
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"ToTransceiverDirection Invalid endpoint direction. type:{inEndpointType}");
                    break;
                }
            }

            return RTCRtpTransceiverDirection.Inactive;
        }

        internal static bool IsSendAble(this EndpointType inEndpointType)
        {
            return (inEndpointType == EndpointType.SendOnly || inEndpointType == EndpointType.SendReceive);
        }
    }
}