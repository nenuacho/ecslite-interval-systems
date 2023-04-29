using System;
using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Nenuacho.EcsLite.IntervalSystems
{
    public class EcsRunSystemsWithInterval : IEcsSystems
    {
        private List<EcsSystemWithInterval> _allSystemsBeforeInitList;
        private EcsSystemWithInterval[] _allSystems;
        private readonly IEcsSystems _systems;
        private Random _random;
        private bool _inited;

        public EcsRunSystemsWithInterval(EcsWorld world)
        {
            _allSystemsBeforeInitList = new List<EcsSystemWithInterval>();
            _systems = new EcsSystems(world);
            _random = new Random();
        }

        internal EcsRunSystemsWithInterval(IEcsSystems systems)
        {
            _allSystemsBeforeInitList = new List<EcsSystemWithInterval>();
            _systems = systems;
            _random = new Random();
        }

        public EcsRunSystemsWithInterval Add(IEcsRunSystem system, float interval, bool spreadByTime = true)
        {
            _allSystemsBeforeInitList.Add(new EcsSystemWithInterval()
            {
                Interval = interval,
                System = system,
                Cooldown = GetCooldown(interval, spreadByTime)
            });

            _systems.Add(system);
            return this;
        }

        internal EcsRunSystemsWithInterval Wrap(float interval, bool spreadByTime)
        {
            foreach (var system in _systems.GetAllSystems().Where(x => x is IEcsRunSystem))
            {
                _allSystemsBeforeInitList.Add(new EcsSystemWithInterval()
                {
                    Interval = interval,
                    System = (IEcsRunSystem)system,
                    Cooldown = GetCooldown(interval, spreadByTime)
                });
            }

            return this;
        }

        private float GetCooldown(float interval, bool spreadByTime) => spreadByTime ? (float)_random.NextDouble() * interval : 0;

        public void Run(float deltaTime)
        {
// #if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
//             if (!_inited)
//             {
//                 throw new Exception("Cant run without initialization.");
//             }
// #endif


            for (int i = 0; i < _allSystems.Length; i++)
            {
                ref var sys = ref _allSystems[i];
                sys.Cooldown -= deltaTime;
                if (sys.Cooldown <= 0)
                {
                    sys.System.Run(_systems);
// #if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
//                     var worldName = EcsSystems.CheckForLeakedEntities(this);
//                     if (worldName != null)
//                     {
//                         throw new Exception($"Empty entity detected in world \"{worldName}\" after {sys.GetType().Name}.Run().");
//                     }
// #endif

                    sys.Cooldown = sys.Interval;
                }
            }
        }

        public T GetShared<T>() where T : class => _systems.GetShared<T>();

        public IEcsSystems AddWorld(EcsWorld world, string name) => _systems.AddWorld(world, name);

        public EcsWorld GetWorld(string name = null) => _systems.GetWorld(name);

        public Dictionary<string, EcsWorld> GetAllNamedWorlds() => _systems.GetAllNamedWorlds();

        public IEcsSystems Add(IEcsSystem system)
        {
            throw new MethodAccessException("Use method Add(IEcsRunSystem system, float interval) instead for this type");
        }

        public List<IEcsSystem> GetAllSystems() => _systems.GetAllSystems();

        public void Init()
        {
            _allSystems = _allSystemsBeforeInitList.ToArray();
            _allSystemsBeforeInitList = null;
            _random = null;
            _systems.Init();
            _inited = true;
        }

        public void Run()
        {
            throw new MethodAccessException("Use method Run(float deltaTime) instead for this type");
        }

        public void Destroy()
        {
            _systems.Destroy();
        }

        public EcsRunSystemsWithInterval Inject(object[] injects)
        {
            _systems.Inject(injects);
            return this;
        }

        public EcsRunSystemsWithInterval Inject()
        {
            _systems.Inject();
            return this;
        }

        private struct EcsSystemWithInterval
        {
            public float Interval;
            public float Cooldown;
            public IEcsRunSystem System;
        }
    }
}