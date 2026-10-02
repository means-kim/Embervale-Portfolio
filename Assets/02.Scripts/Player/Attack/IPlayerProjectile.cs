public interface IPlayerProjectile
{
    float MaxDistance { get; }
    void Launch(int damage, int direction);
}