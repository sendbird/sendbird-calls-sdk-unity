// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class RoomListApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            internal Request(SbRoomListQueryParams roomListQueryParams, string nextToken, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                HttpMethod = UnityWebRequest.kHttpVerbGET;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                StringBuilder urlStringBuilder = new StringBuilder();
                urlStringBuilder.Append("v1/rooms?");
                urlStringBuilder.Append($"limit={roomListQueryParams.Limit}");

                urlStringBuilder.Append($"&type={SbRoomType.LargeRoomForAudioOnly.EnumToJsonPropertyName()}");

                if (roomListQueryParams.RoomState != null)
                    urlStringBuilder.Append($"&state={roomListQueryParams.RoomState.Value.EnumToJsonPropertyName()}");

                if (roomListQueryParams.RoomIds != null && 0 < roomListQueryParams.RoomIds.Count)
                {
                    urlStringBuilder.Append("&room_ids=");
                    for (int index = 0; index < roomListQueryParams.RoomIds.Count; index++)
                    {
                        if (0 < index)
                            urlStringBuilder.Append(',');

                        urlStringBuilder.Append(roomListQueryParams.RoomIds[index]);
                    }
                }

                if (string.IsNullOrEmpty(nextToken) == false)
                    urlStringBuilder.Append($"&next={nextToken}");

                if (roomListQueryParams.CreatedAtRange != null)
                {
                    // createdAt range (start: inclusive, end: exclusive)
                    if (roomListQueryParams.CreatedAtRange.LowerBound != null)
                        urlStringBuilder.Append($"&created_at_start_date={roomListQueryParams.CreatedAtRange.LowerBound}");

                    if (roomListQueryParams.CreatedAtRange.UpperBound != null)
                    {
                        long endDate = roomListQueryParams.CreatedAtRange.UpperBound.Value;
                        if (endDate < long.MaxValue)
                            endDate += 1;

                        urlStringBuilder.Append($"&created_at_end_date={roomListQueryParams.CreatedAtRange.UpperBound}");
                    }
                }

                if (roomListQueryParams.CurrentParticipantCountRange != null)
                {
                    // current participant count range (start: inclusive, end: inclusive)
                    if (roomListQueryParams.CurrentParticipantCountRange.LowerBound != null)
                        urlStringBuilder.Append($"&current_participant_range_gte={roomListQueryParams.CurrentParticipantCountRange.LowerBound}");

                    if (roomListQueryParams.CurrentParticipantCountRange.UpperBound != null)
                        urlStringBuilder.Append($"&current_participant_range_lte={roomListQueryParams.CurrentParticipantCountRange.UpperBound}");
                }

                if (roomListQueryParams.CreatedByUserIds != null && 0 < roomListQueryParams.CreatedByUserIds.Count)
                {
                    urlStringBuilder.Append("&created_by_user_ids=");
                    for (int index = 0; index < roomListQueryParams.CreatedByUserIds.Count; index++)
                    {
                        if (0 < index)
                            urlStringBuilder.Append(',');

                        urlStringBuilder.Append(roomListQueryParams.CreatedByUserIds[index]);
                    }
                }

                URL = urlStringBuilder.ToString();
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("next")] internal readonly string Next = null;
            [JsonProperty("rooms")] internal readonly ReadOnlyCollection<RoomCommandObject> roomCommandObjects = null;
        }
    }
}