using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Grpc.Core;

public class ChannelController
{
    private static string serverIp = "192.168.10.38";
    private static string serverPort = "12345";

    private static Channel channel = null;
    
    public static Channel GetChannel(bool isTurnOnStatus = false)
    {
        if (isTurnOnStatus)
        {
            ShowStatePerSeconds();
        }
        
        return channel ?? (channel = new Channel($"{serverIp}:{serverPort}", ChannelCredentials.Insecure));
    }

    private static void ShowStatePerSeconds(int seconds = 1)
    {
        Observable.Interval(TimeSpan.FromSeconds(seconds)).Subscribe(_ => ShowState());
    }
    
    private static void ShowState()
    {
        switch (channel.State)
        {
            case ChannelState.Idle:
                Debug.Log("green but idle");
                break;
            case ChannelState.Connecting:
                Debug.Log("blink green");
                break;
            case ChannelState.Ready:
                Debug.Log("green");
                break;
            case ChannelState.TransientFailure:
                Debug.Log("red");
                break;
            case ChannelState.Shutdown:
                Debug.Log("dark gray");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
