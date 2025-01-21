using Photon.Pun;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _PlayerSprite;
    [SerializeField] private GameObject _Player;
    [SerializeField] private SpriteRenderer _GunSprite;

    private PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (Time.timeScale != 0 && _view.IsMine)
        {
            Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (direction.x < _Player.transform.position.x)
            {
                _PlayerSprite.flipX = true;
                _GunSprite.flipY = true;
            }
            else
            {
                _PlayerSprite.flipX = false;
                _GunSprite.flipY = false;
            }
        }
    }
}
