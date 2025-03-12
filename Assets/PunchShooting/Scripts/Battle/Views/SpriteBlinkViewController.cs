using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PunchShooting.Battle.Views
{
    public class SpriteBlinkViewController : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly Color _defaultColor;
        private readonly SpriteRenderer _springsRenderer;

        public SpriteBlinkViewController(SpriteRenderer springsRenderer)
        {
            _springsRenderer = springsRenderer;
            _defaultColor = _springsRenderer.color;
        }

        public void Dispose()
        {
            _cts.Dispose();
        }

        public void Blink(Color blinkColor, float blinkSecond, int blinkCount = 1)
        {
            //CancelBlink();
            BlinkAsync(blinkColor, blinkSecond, blinkCount, _cts.Token).Forget();
        }

        public void CancelBlink()
        {
            _cts.Cancel();
            _springsRenderer.color = _defaultColor;
        }


        private async UniTask BlinkAsync(Color blinkColor, float blinkSecond, int blinkCount, CancellationToken ct)
        {
            for (var i = 0; i < blinkCount; i++)
            {
                _springsRenderer.color = _springsRenderer.color == _defaultColor ? blinkColor : _defaultColor;
                await UniTask.WaitForSeconds(blinkSecond, cancellationToken: ct);
                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }

            if (_springsRenderer != null)
            {
                _springsRenderer.color = _defaultColor;
            }
        }
    }
}
