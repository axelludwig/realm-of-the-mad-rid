public class Player: Entity
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        MapRuntime.Instance.Init(this);
    }
}
