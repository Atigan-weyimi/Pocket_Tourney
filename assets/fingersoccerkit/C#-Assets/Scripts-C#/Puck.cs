using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

// Base class for all puck in the game
public class Puck : MonoBehaviour
{
    public event Action<Puck, BallManager> HitBall;
        
    [SerializeField] private GameObject _selectionCircle;
    [SerializeField] private BallManager _debugBall;
    
    private const float PassTurnSpeedRadInSec = 1.6f;

    private bool _canShoot;
    
    public bool CanShoot
    {
        get => _canShoot;
        set
        {
            _canShoot = value;
            SetSelectionActive(value);
        }
    }

    public float Radius => transform.localScale.z * 0.5f;

    private Rigidbody _rigidbody;
    private Vector3 _targetBacketCenter;
    private GameArea _area;

    public bool IsMoving => _rigidbody.velocity.sqrMagnitude > 0.05f;
    public bool IsCathingBall { get; private set; }

    public virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Update()
    {
        if (_debugBall != null)
        {
            CatchBall(_debugBall, this.GetCancellationTokenOnDestroy()).Forget();
            _debugBall = null;
        }
    }

    public void SetSelectionActive(bool isActive)
    {
        if (_selectionCircle == null)
        {
            return;
        }
        _selectionCircle.gameObject.SetActive(isActive);
    }

    public void Init(Texture2D texture, Vector3 targetCenter, GameArea gameArea)
    {
        GetComponent<Renderer>().material.mainTexture = texture;
        _targetBacketCenter = targetCenter;
        _area = gameArea;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BallManager ball))
        {
            HitBall?.Invoke(this, ball);
        }
    }

    public async UniTask CatchBall(BallManager ball, CancellationToken cancellationToken)
    {
        IsCathingBall = true;

        await StopBall(ball, cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            IsCathingBall = false;
            return;
        }
        
        await RotateAroundBall(ball, cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            IsCathingBall = false;
            return;
        }
        
        await RotateBallAroundSelf(ball, cancellationToken);
        
        IsCathingBall = false;
    }

    private async UniTask StopBall(BallManager ball, CancellationToken cancellationToken)
    {
        ball.Attach(_rigidbody);
        ball.Velocity *= 0.5f;
        await UniTask.Yield();
        while (ball.IsMoving)
        {
            ball.Velocity *= 0.8f;
            await UniTask.Yield();
        }
        
        ball.Detach();
    }

    private async UniTask RotateAroundBall(BallManager ball, CancellationToken cancellationToken)
    {
        _rigidbody.isKinematic = true;

        var currentDelta = ball.transform.position - transform.position;
        var currentAngle = MathUtils.GetAngle(-currentDelta);
        if (currentAngle < 0f)
        {
            currentAngle = Mathf.PI * 2f + currentAngle;
        }
        
        var distance = currentDelta.magnitude;

        var targetDirection = (_targetBacketCenter - ball.transform.position).normalized;
        var targetAngle = MathUtils.GetAngle(-targetDirection);
        if (targetAngle < 0f)
        {
            targetAngle = Mathf.PI * 2f + targetAngle;
        }

        var rectSizeDelta = new Vector2(Radius, Radius);
        var resizedRect = new Rect(_area.Rect.position + rectSizeDelta, _area.Rect.size - rectSizeDelta * 2f);
        var (minAngle, maxAngle) = MathUtils.GetAllowedAnglesRange(ball.transform.position, distance, resizedRect);
        if (minAngle != 0f && maxAngle != Mathf.PI * 2f)
        {
            targetAngle = MathUtils.ClampAngle(targetAngle, minAngle, maxAngle);
            if (targetAngle < 0f)
            {
                targetAngle = Mathf.PI * 2f + targetAngle;
            }
        }

        if (Mathf.Abs(targetAngle - currentAngle) > Mathf.PI)
        {
            targetAngle -= Mathf.PI * 2f;
        }

        var turnDuration = Mathf.Abs(currentAngle - targetAngle) / PassTurnSpeedRadInSec;
        var turnTime = 0f;
        while (turnTime < turnDuration)
        {
            var angle = Mathf.Lerp(currentAngle, targetAngle, turnTime / turnDuration);
            transform.position = MathUtils.GetPositionOnCircle(ball.transform.position, distance, angle);
            await UniTask.Yield();
            if (cancellationToken.IsCancellationRequested)
            {
                _rigidbody.isKinematic = false;
                return;
            }
            turnTime += Time.deltaTime;
        }
        transform.position = MathUtils.GetPositionOnCircle(ball.transform.position, distance, targetAngle);
        
        _rigidbody.isKinematic = false;
    }

    private async UniTask RotateBallAroundSelf(BallManager ball, CancellationToken cancellationToken)
    {
        ball.IsKinematic = true;
        
        var currentAngle = MathUtils.GetAngle((ball.transform.position - transform.position).normalized);
        
        var distance = (ball.transform.position - transform.position).magnitude;
        var targetDirection = (_targetBacketCenter - transform.position).normalized;
        var targetAngle = MathUtils.GetAngle(targetDirection);

        var turnDuration = Mathf.Abs(currentAngle - targetAngle) / PassTurnSpeedRadInSec;
        var turnTime = 0f;
        while (turnTime < turnDuration)
        {
            var angle = Mathf.Lerp(currentAngle, targetAngle, turnTime / turnDuration);
            ball.transform.position = MathUtils.GetPositionOnCircle(transform.position, distance, angle);
            await UniTask.Yield();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            turnTime += Time.deltaTime;
        }
        ball.transform.position = MathUtils.GetPositionOnCircle(transform.position, distance, targetAngle);
        
        ball.IsKinematic = false;
    }
}