using System;
using _Components._Glass;
using _Configs;
using _Managers;
using _Meta._Collection._Config;
using UnityEngine;
using Zenject;

namespace _Infrastructure
{
    public class BootstrapInstaller : MonoInstaller
    {
        [Header("Managers")] 
        [SerializeField] private MAnalytics _analytics;
        [SerializeField] private MRemoteConfig           _remoteConfig;
        [SerializeField] private CoreConfig              _coreConfig;
        [SerializeField] private GlassesCollectionConfig _glassesConfig;
        [SerializeField] private BallsCollectionConfig   _ballsConfig;

        public override void InstallBindings()
        {
           BindAnalytics();
           BindRemoteConfig();
           BindConfigs();
        }
        
        private void BindAnalytics()
        {
            Container.
                Bind<MAnalytics>().
                FromInstance(_analytics)
                .AsSingle();
        }
        
        private void BindRemoteConfig()
        {
            Container.
                Bind<MRemoteConfig>().
                FromInstance(_remoteConfig)
                .AsSingle();
        }
        
        private void BindConfigs()
        {
            Container
                .Bind<CoreConfig>()
                .FromInstance(_coreConfig)
                .AsSingle();
            
            Container
                .Bind<ICollectionConfig<Glass>>()
                .FromInstance(_glassesConfig)
                .AsSingle();
            
            Container
                .Bind<ICollectionConfig<Material>>()
                .FromInstance(_ballsConfig)
                .AsSingle();
        }
    }
}