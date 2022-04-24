using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    EnemyController _controller = null;
    public EnemyController EnemyController
    {
        get
        {
            if (_controller == null)
                _controller = GetComponent<EnemyController>();
            return _controller;
        }
    }

    public virtual void DoLogic() { }
}
