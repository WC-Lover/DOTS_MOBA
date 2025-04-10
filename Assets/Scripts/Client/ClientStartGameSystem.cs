﻿using System;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class ClientStartGameSystem : SystemBase
{
    public Action<int> OnUpdatePlayersRemainingToStart;
    public Action OnStartGameCountdown;

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (playersRemainingToStart, entity)
            in SystemAPI.Query<
                PlayersRemainingToStart>().WithAll<ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
            OnUpdatePlayersRemainingToStart?.Invoke(playersRemainingToStart.Value);
        }

        foreach (var (gameStartTick, entity)
            in SystemAPI.Query<
                GameStartTickRpc>().WithAll<ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
            OnStartGameCountdown?.Invoke();

            var gameStartEntity = ecb.CreateEntity();

            ecb.AddComponent(gameStartEntity, new GameStartTick { Value = gameStartTick.Value });
        }

        ecb.Playback(EntityManager);
    }
}