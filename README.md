# Sendbird Unity calls

<span>
<img src="https://img.shields.io/badge/Unity-2019.4+-black?logo=unity" alt="Unity 2019.4+" >
</span>


## Table of contents

1. [Introduction](#introduction)
2. [Before getting started](#before-getting-started)
3. [Getting started](#getting-started)
4. [Making your first call](#making-your-first-call)
5. [Samples](#samples)
6. [Additional Notes](#additional-notes)
7. [Appendix](#appendix)

## Introduction

**Sendbird Calls** is the latest addition to our product portfolio. It enables real-time calls between users within a Sendbird application. SDKs are provided for iOS, Android, Windows, and macOS. Using any one of these, developers can quickly integrate voice call functions into their own client apps, allowing users to web-based real-time voice group call on the Sendbird platform.

> If you need any help in resolving any issues or have questions, please visit [our community](https://community.sendbird.com)

### How it works

Sendbird Calls SDK for Unity provides a framework for creating and joining voice group calls.

### More about Sendbird Calls SDK for Unity

Find out more about Sendbird Calls for Unity on [Calls SDK for Unity doc](https://sendbird.com/docs/calls/v1/unity/getting-started/about-calls-sdk) or [API References](https://sendbird.com/docs/calls/v1/unity/ref/index.html).


## Before getting started

This section shows the prerequisites you need to check to use Sendbird Calls SDK for Unity.

### Requirements

- Unity 2019.4 or higher

### SDK dependencies

- [Unity WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html), It is installed together with the Calls SDK installation.
- [Newtonsoft Json](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@2.0/manual/index.html), It is installed together with the Calls SDK installation.

### Supported Platforms

<span>
<img src="https://img.shields.io/badge/iOS-black?logo=ios" alt="ios" >
<img src="https://img.shields.io/badge/Windows-black?logo=windows" alt="windows" >
<img src="https://img.shields.io/badge/macOS-black?logo=macos" alt="macos" >
<img src="https://img.shields.io/badge/Android-black?logo=android" alt="android" >
</span>

* Please note that there are unsupported platforms below.
  * **Windows UWP** platform is not supported.
  * Building for **iOS Simulator** is not supported.
  * Building for **Android ARMv7** is not supported.
  * **WebGL** platform is not supported.


## Getting started

This section gives you information you need to get started with Sendbird Calls SDK for Unity.

### Install Calls SDK

1. Select Window/Package Manager in the menu bar.
2. Click `+` button at the top left of the window, and select `Add package from git URL...`
3. Input the string below to the input field.<br/>
`https://github.com/sendbird/sendbird-calls-unity.git`
4. Click `Add` button, and will start install the package.<br>
   [See the Install documentation.](Documentation~/Install.md)

## Making your first call

Follow the step-by-step instructions below to authenticate and make your first call.

### Step 1: Create a Sendbird application

1. Login or Sign-up for an account on [Sendbird Dashboard](https://dashboard.sendbird.com/).
2. Create or select a call-activated application on the dashboard.
3. Keep your Sendbird application ID(`APP_ID`) in a safe place for future reference.

### Step 2: Initialize the SendbirdCall instance in a client app

To integrate and run Sendbird Calls in your application, you need to initialize it first. Initialize the `SendbirdCall` instance by using the `APP_ID` of your Sendbird application, which can be found on the [Sendbird Dashboard](https://dashboard.sendbird.com/) after creating an application. If the instance is initialized with a different `APP_ID`, all existing call-related data in a client app will be cleared and the SendbirdCall instance will be initialized again with the new `APP_ID`.

```csharp
// Initialize SendbirdCall instance to use APIs in your app.
SendbirdCall.Init(APP_ID);
```

### Step 3: Authenticate a user

To start a group call, authenticate a user to Sendbird server by using their user Id through the authenticate() method.
```csharp
 // The USER_ID below should be unique to your Sendbird application.
SbAuthenticateParams authenticateParams = new SbAuthenticateParams(USER_ID);
SendbirdCall.Authenticate(authenticateParams, (user, error) =>
{
    if( error == null ){/* The user has been authenticated successfully and is connected to Sendbird server. */}
});
```

### Step 4: Create a room

When creating your first room for a group call, you can create a room that supports up to 100 participants with audio. When the room is created, a `ROOM_ID` is generated.
```csharp
SbRoomParams roomParams = new SbRoomParams();
SendbirdCall.CreateRoom(roomParams, (room, error) =>
{
    if( error == null ){/* `room` is created with a unique identifier `room.RoomId`. */}
});
```

### Step 5: Enter a room

You can now enter a room and start your first group call. When you enter a room, a participant is created with a unique participant Id to represent the user in the room.

To enter a room, you must acquire the room instance from Sendbird server with the room Id. To fetch the most up-to-date room instance from Sendbird server, use the SendbirdCall.FetchRoomById() method. Also, you can use the SendbirdCall.GetCachedRoomById() method that returns the most recently cached room instance from Sendbird Calls SDK.
```csharp
SendbirdCall.FetchRoomById(ROOM_ID, (room, error) =>
{
    if( error == null ){/* `room` with the identifier `ROOM_ID` is fetched from Sendbird Server. */}
});

SbRoom room = SendbirdCall.GetCachedRoomById(ROOM_ID);
// Returns the most recently cached room with the identifier `ROOM_ID` from the SDK.
// If there is no such room with the given room Id, `null` is returned.
```

Once the room is retrieved, call the SbRoom.Enter() method to enter the room.
```csharp
SbRoomEnterParams roomEnterParams = new SbRoomEnterParams();
room.Enter(roomEnterParams, (error) =>
{
    if( error == null ){/* User has successfully entered `room`. */}
});
```

## Samples
1. If the SDK is not installed, you need to [install the SDK](#getting-started) first.
2. To get these samples, Push the `Import into Project` button on Package Manager.
3. Open `SampleVoiceGroupCalls.unity` scene.<br/>
   [See the Samples documentation.](Documentation~/Sample.md)

## Additional Notes

### Build on Android
To build the apk file for Android platform, you need to configure player settings below.
* Choose **IL2CPP** for **Scripting backend** in Player Settings Window.
* Set enable **ARM64** and Set disable **ARMv7** for **Target Architectures** setting in Player Settings Window.

### Build on iOS and macOS
If building app for iOS and macOS, you need to add description for MicrophoneUsageDescription on PlayerSettings.(If not add description, build is failed.)

[See the Additional Notes documentation.](Documentation~/AdditionalNotes.md)

## Appendix

### Call relay protocol

Sendbird Calls is based on WebRTC to enable real-time calls between users with P2P connections, but sometimes connectivity issues may occur for users due to network policies that won’t allow WebRTC communications through Firewalls and NATs (Network Address Translators). For this, Sendbird Calls uses two different types of protocols, **Session Traversal Utilities for NAT (STUN)** and **Traversal Using Relays around NAT (TURN)**. **STUN** and **TURN** are protocols that support establishing a connection between users.

> **Note**: See our [GitHub page](https://github.com/sendbird/guidelines-calls/tree/master/Recommendation%20on%20firewall%20configuration) to learn about the requirements and how to use the Calls SDKs behind a firewall.

---

#### How STUN and TURN works

Session Traversal Utilities for NAT (STUN) is a protocol that helps hosts to discover the presence of a NAT and the IP address, which eventually makes the connection between two endpoints. Traversal Using Relays around NAT (TURN) is a protocol that serves as a relay extension for data between two parties.

Sendbird Calls first try to make a P2P connection directly using the Calls SDK. If a user is behind a NAT/Firewall, Calls will discover the host's public IP address as a location to establish connection using STUN. In most cases, STUN server is only used during the connection setup and once the session has been established, media will flow directly between two users. If the NAT/Firewall still won't allow the two users to connect directly, TURN server will be used to make a connection to relay the media data between two users. Most of the WebRTC traffic is connected with STUN.