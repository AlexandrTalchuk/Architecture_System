using System;
using System.Collections;
using _Managers;
using _UI._Elements;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Infrastructure
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private ProgressBarUIElement _loadingProgressBar;

        private int _mainSceneIndex = 1;
         private MAnalytics _analytics;
        
        [Inject]
        private void Construct(MAnalytics analytics)
        {
            _analytics = analytics;
        }
        
        private void OnEnable()
        {
            Application.targetFrameRate = 60;
          //  Input.multiTouchEnabled = false;

            _loadingProgressBar.Fill(0, 0.95f, () => StartCoroutine(LoadScene(_mainSceneIndex)));
        }
        
        private IEnumerator LoadScene(int nextSceneIndex, Action onLoaded = null)
        {

            if (SceneManager.GetActiveScene().buildIndex == nextSceneIndex)
            {
                onLoaded?.Invoke();
                yield break;
            }
            yield return new WaitUntil(() => _analytics.FirebaseReady);

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextSceneIndex);

            while (!waitNextScene.isDone)
                yield return null;

            onLoaded?.Invoke();
        }
    }
}