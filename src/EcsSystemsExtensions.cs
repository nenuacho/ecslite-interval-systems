using Leopotam.EcsLite;

namespace Nenuacho.EcsLite.IntervalSystems
{
    public static class EcsSystemsExtensions
    {
        public static EcsRunSystemsWithInterval WithInterval(this IEcsSystems systems, float interval, bool spreadByTime = true) =>
            new EcsRunSystemsWithInterval(systems).Wrap(interval, spreadByTime);
    }
}