﻿//
// Automatically generated by JSComponentGenerator.
//
using UnityEngine;

public class JSComponent_FixedUpdate_OnGUI_AnimatorIK_Move_JointBreak_Server : JSComponent
{
    int idFixedUpdate;
    int idOnGUI;
    int idOnAnimatorIK;
    int idOnAnimatorMove;
    int idOnJointBreak;
    int idOnConnectedToServer;
    int idOnDisconnectedFromServer;
    int idOnFailedToConnect;
    int idOnFailedToConnectToMasterServer;
    int idOnMasterServerEvent;
    int idOnNetworkInstantiate;
    int idOnPlayerConnected;
    int idOnPlayerDisconnected;
    int idOnSerializeNetworkView;
    int idOnServerInitialized;

    protected override void initMemberFunction()
    {
        base.initMemberFunction();
        idFixedUpdate = JSApi.getObjFunction(jsObjID, "FixedUpdate");
        idOnGUI = JSApi.getObjFunction(jsObjID, "OnGUI");
        idOnAnimatorIK = JSApi.getObjFunction(jsObjID, "OnAnimatorIK");
        idOnAnimatorMove = JSApi.getObjFunction(jsObjID, "OnAnimatorMove");
        idOnJointBreak = JSApi.getObjFunction(jsObjID, "OnJointBreak");
        idOnConnectedToServer = JSApi.getObjFunction(jsObjID, "OnConnectedToServer");
        idOnDisconnectedFromServer = JSApi.getObjFunction(jsObjID, "OnDisconnectedFromServer");
        idOnFailedToConnect = JSApi.getObjFunction(jsObjID, "OnFailedToConnect");
        idOnFailedToConnectToMasterServer = JSApi.getObjFunction(jsObjID, "OnFailedToConnectToMasterServer");
        idOnMasterServerEvent = JSApi.getObjFunction(jsObjID, "OnMasterServerEvent");
        idOnNetworkInstantiate = JSApi.getObjFunction(jsObjID, "OnNetworkInstantiate");
        idOnPlayerConnected = JSApi.getObjFunction(jsObjID, "OnPlayerConnected");
        idOnPlayerDisconnected = JSApi.getObjFunction(jsObjID, "OnPlayerDisconnected");
        idOnSerializeNetworkView = JSApi.getObjFunction(jsObjID, "OnSerializeNetworkView");
        idOnServerInitialized = JSApi.getObjFunction(jsObjID, "OnServerInitialized");
    }

    void FixedUpdate()
    {
        callIfExist(idFixedUpdate);
    }
    void OnGUI()
    {
        callIfExist(idOnGUI);
    }
    void OnAnimatorIK(int layerIndex)
    {
        callIfExist(idOnAnimatorIK, layerIndex);
    }
    void OnAnimatorMove()
    {
        callIfExist(idOnAnimatorMove);
    }
    void OnJointBreak(float breakForce)
    {
        callIfExist(idOnJointBreak, breakForce);
    }
    void OnConnectedToServer()
    {
        callIfExist(idOnConnectedToServer);
    }
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        callIfExist(idOnDisconnectedFromServer, info);
    }
    void OnFailedToConnect(NetworkConnectionError error)
    {
        callIfExist(idOnFailedToConnect, error);
    }
    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        callIfExist(idOnFailedToConnectToMasterServer, info);
    }
    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        callIfExist(idOnMasterServerEvent, msEvent);
    }
    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        callIfExist(idOnNetworkInstantiate, info);
    }
    void OnPlayerConnected(NetworkPlayer player)
    {
        callIfExist(idOnPlayerConnected, player);
    }
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        callIfExist(idOnPlayerDisconnected, player);
    }
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        callIfExist(idOnSerializeNetworkView, stream, info);
    }
    void OnServerInitialized()
    {
        callIfExist(idOnServerInitialized);
    }

}