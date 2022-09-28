using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class GameObjectHurt : UnityEvent
{
}

public class Life : MonoBehaviour
{
    [SerializeField] private int maxLife = 3;

    [SerializeField] private int _life = 3;
    public int life { get { return _life; } private set { _life = value; } }

    public bool isInvincible = false;
    public bool canDie = true;

    public GameObjectHurt OnHurt;
    public GameObjectHurt OnDeath;
    [HideInInspector] public GameObjectHurt OnHeal;

    [SerializeField] float invulTime = 1f;
    [SerializeField] float invulDt = .1f;
    Renderer[] playerRenderer;

    IEnumerator CoInvulFrames()
    {
        isInvincible = true;
        for (float i = 0; i < invulTime; i += invulDt)
        {
            foreach(Renderer partRenderer in playerRenderer)
                partRenderer.enabled = !partRenderer.enabled;
            yield return new WaitForSeconds(invulDt);
        }
        foreach (Renderer partRenderer in playerRenderer)
            partRenderer.enabled = true;
        isInvincible= false;
    }

    public void InvulFrames()
    {
        StartCoroutine(CoInvulFrames());
    }

    void Awake()
    {
        life = maxLife;
        playerRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    public void Hurt(int damage)
    {
        if (isInvincible) return;

        life -= damage;
        if (life <= 0 && canDie) OnDeath.Invoke();
        else OnHurt.Invoke();
    }

    public bool Heal(int heal)
    {
        if (heal < 0) Hurt(-heal);

        if (life >= maxLife) return false;

        life += heal;
        if (life > maxLife) life = maxLife;

        OnHeal.Invoke();

        return true;
    }
}
