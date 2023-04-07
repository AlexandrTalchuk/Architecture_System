using _Configs;
using _Enums;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Managers
{
    public class MCamera : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uiCamera;
        
        private float   _cameraDamp     = 20;
        private float   _cameraSpring   = 4000;
        private float   _cameraStrength = 3;
        private Vector3 _cameraPos      = Vector3.zero;
        private Vector3 _cameraVelocity = Vector3.zero;
        private Vector3 _offset;
        private float   _normalZoom;
        private float   _startZoom = 50;
        private MGame   _game;

        public Camera MainCamera => _mainCamera;
        public Camera UICamera => _uiCamera;

        [Inject]
        private void Construct(MGame game)
        {
            _game = game;
        }

        private void Awake()
        {
            _offset = _mainCamera.transform.position;
            _normalZoom = _mainCamera.fieldOfView;
        }

        private void Update()
        {
            if (_game.State != GameState.Playing) return;
            
            _cameraVelocity.x -= _cameraPos.x * _cameraSpring * Time.deltaTime;
            _cameraVelocity.x *= 1f - _cameraDamp * Time.deltaTime;
            
            _cameraVelocity.y -= _cameraPos.y * _cameraSpring * Time.deltaTime;
            _cameraVelocity.y *= 1f - _cameraDamp * Time.deltaTime;
            
            _cameraPos.x += _cameraVelocity.x * Time.deltaTime;
            _cameraPos.y += _cameraVelocity.y * Time.deltaTime;
            
            _mainCamera.transform.localPosition = _cameraPos + _offset;
        }

        public void ApplyForce(Vector3 vec, float strength)
        {
            _cameraVelocity.x += vec.x * _cameraStrength * strength;
            _cameraVelocity.y += vec.y * _cameraStrength * strength;
            _cameraVelocity.z += vec.z * _cameraStrength * strength;
        }
        
        public void ZoomOutAtStart ( float duration)
        {
            _mainCamera.transform.LeanMove(_offset, duration).setEase(LeanTweenType.easeInOutQuad);
            _mainCamera.DOFieldOfView(_startZoom, duration).SetEase(Ease.InOutQuad);
        }

        public void ZoomToTube(float duration)
        {
            _mainCamera.DOFieldOfView(14, duration).SetEase(Ease.InOutQuad);
            _mainCamera.transform.LeanMove(new Vector2(-1.68f, 4.5f), duration).setEase(LeanTweenType.easeInOutQuad);
        }

        public void ZoomToPlatform(float duration)
        {
            _mainCamera.DOFieldOfView(_startZoom, duration).SetEase(Ease.InOutQuad);
            _mainCamera.transform.LeanMove(new Vector2(0, -2), duration).setEase(LeanTweenType.easeInOutQuad);
        }

        public void ZoomToNormal(float duration)
        {
            _mainCamera.DOFieldOfView(_normalZoom, duration).SetEase(Ease.InOutQuad);
            _mainCamera.transform.LeanMove(_offset, duration).setEase(LeanTweenType.easeInOutQuad);
        }
    }
}