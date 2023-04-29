# ECSLite Interval Systems

Расширение для ecslite, которое позволяет запускать системы с заданным интервалом

# Зависимости
[ecslite](https://github.com/Leopotam/ecslite)

[ecslite-di](https://github.com/Leopotam/ecslite-di)

# Установка
## В виде unity модуля
Поддерживается установка в виде unity-модуля через git-ссылку в PackageManager или прямое редактирование `Packages/manifest.json`:
```
"com.nenuacho.ecslite.interval-systems": "https://github.com/nenuacho/ecslite-interval-systems.git",
```

# Интеграция

Расширение предоставляет 2 API для создания систем с интервалом.

### Первый способ
Создать экземпляр контейнера систем типа EcsRunSystemsWithInterval. При добавлении в него систем, следует использовать метод 
```c#
Add(IEcsRunSystem system, float interval, bool spreadByTime = true)
```
Таким образом для каждой системы в контейнере можно установить индивидуальный интервал

```c#
            _systemsWithInterval = new EcsRunSystemsWithInterval(world);
            _systemsWithInterval
                .Add(new SampleSystem1(), 5f)
                .Add(new SampleSystem2(), 5f)
                .Add(new SampleSystem3(), 1f, false)
                .Add(new SampleSystem4(), 1f, false)
                .Inject()
                .Init();
```
*interval* - система будет отрабатывать 1 раз в этот промежуток

*spreadByTime* - указывает нужно ли распределять вызовы по времени, или запускать их синхронно. На примере выше SampleSystem1 и SampleSystem2 будут запускатся раз в 5 секунд, но первый запуск будет случайным в этом промежутке, таким образом они не будут отрабатывать одновременно. Если нужно чтобы системы выполнялись одновременно, нужно передать в этот параметр false.

### Второй способ
Создать обычный контейнер EcsSystems и, после добавления систем(но до вызова Init) вызвать метод расширения 
```c#
WithInterval(this IEcsSystems systems, float interval, bool spreadByTime = true)
```
:
```c#
            var intervalSystems = new EcsSystems(world)
                .Add(new SampleSystem1())
                .Add(new SampleSystem2())
                .WithInterval(5f)
                .Inject();
                
            intervalSystems.Init();
            _systemsWithInterval = intervalSystems;

```
При этом все системы в этом контейнере будут иметь один заданный интервал


# Использование

Использование не отличается от основных контейнеров EcsSystems, за исключением того, что вместо метода *Run()* нужно использовать метод *Run(float deltaTime)*, c актуальным значением deltaTime

```c#
        private void Update()
        {
            _systemsWithInterval.Run(_timeService.DeltaTime);
        }
```

# Важно

В расширении пока не поддерживается IPostRunSystem

