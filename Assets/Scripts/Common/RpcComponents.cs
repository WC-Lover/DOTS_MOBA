
using Unity.NetCode;

public struct MobaTeamRequest : IRpcCommand
{
    public TeamType teamType;
}

public struct PlayersRemainingToStart : IRpcCommand
{
    public int Value;
}

public struct GameStartTickRpc : IRpcCommand
{
    public NetworkTick Value;
}

