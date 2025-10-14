public class Player: Entity
{
    public override void OnNetworkSpawn()
    {
        Stats = new EntityStats(this);
        base.OnNetworkSpawn();
        MapRuntime.Instance.Init(this);
    }
}
