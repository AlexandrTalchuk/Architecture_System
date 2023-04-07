using _Components._Glass;
using _Factories;
using _Managers;
using _Meta._Collection;
using UnityEngine;
using Zenject;

namespace _Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        [Header("Managers")] 
        [SerializeField] private MGame         _game;
        [SerializeField] private MUI           _ui;
        [SerializeField] private MCamera       _camera;
        [SerializeField] private MAudio        _audio;
        [SerializeField] private MAds          _ads;


        public override void InstallBindings()
        {
            BindInstance(_game);
            BindInstance(_ui);
            BindInstance(_camera);
            BindInstance(_audio);
            BindInstance(_ads);


            BindAsSingleNonLazy<MData>();
            BindAsSingleNonLazy<MWorlds>();
            BindAsSingleNonLazy<MCollection<Glass>>();
            BindAsSingleNonLazy<MCollection<Material>>();

            BindAsSingle<UIFactory>();
            BindAsSingle<LevelFactory>();
        }

        private void BindAsSingle<T>()
        {
            Container
                .Bind<T>()
                .AsSingle();
        }
        
        private void BindAsSingleNonLazy<T>()
        {
            Container
                .Bind<T>()
                .AsSingle()
                .NonLazy();
        }

        private void BindInstance<T>(T instance)
        {
            Container
                .Bind<T>()
                .FromInstance(instance)
                .AsSingle();
        }
    }
}